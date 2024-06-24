using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.ItemSpawning.ChestSpawning;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.ROREffects.Common;
using RiskOfSlimeRain.Core.Warbanners;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Items;
using RiskOfSlimeRain.Items.Consumable;
using RiskOfSlimeRain.Network.Effects;
using RiskOfSlimeRain.NPCs.Bosses;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace RiskOfSlimeRain
{
	public class RORInterfaceLayers : ModSystem
	{
		private const int inventorySize = 47;

		private const int iconSize = 32;
		private const int iconPadding = iconSize + 6;
		private const int verticalLineHeight = 50;

		public static string ModName => ModContent.GetInstance<RiskOfSlimeRainMod>().Name;

		public static int hoverIndex = -1;
		public static float hoverAlphaMax = 0.8f;
		public static float hoverAlphaMin = 0;
		public static float hoverAlpha = 0;
		public static float hoverAlphaColorMultForOther => 1f - Math.Clamp(hoverAlpha, hoverAlphaMin, hoverAlphaMax);

		public static bool EffectsVisible { private set; get; }

		//Value is Ref(int) because it's counted down from within the iteration loop
		//When timer runs out, sync
		/// <summary>
		/// Multiplayer syncing thing used when changing stack manually from the UI. Keeps track of any changed stacks and a timer
		/// </summary>
		public static Dictionary<int, Ref<int>> TimeByIndex { get; set; }

		private const int syncTimer = 25;

		public static LocalizedText EffectUIMiscHeaderText { get; private set; }
		public static LocalizedText EffectUIMiscNullifierExplanationText { get; private set; }
		public static LocalizedText EffectUIMiscCheckConfigText { get; private set; }
		public static LocalizedText EffectUIMiscChangeStackText { get; private set; }
		public static LocalizedText EffectUIMiscSwitchToNullifierText { get; private set; }
		public static LocalizedText EffectUIMiscInfoText { get; private set; }
		public static LocalizedText EffectUIMiscTotalStacksText { get; private set; }
		public static LocalizedText EffectUIMiscDifficultyScalingText { get; private set; }
		public static LocalizedText EffectUIMiscHasChestsText { get; private set; }
		public static LocalizedText EffectUIMiscNoChestsText { get; private set; }
		public static LocalizedText EffectUIMiscNullifierSavingsText { get; private set; }
		public static LocalizedText EffectUIMiscNullifierTotalText { get; private set; }

		public override void OnModLoad()
		{
			TimeByIndex = new Dictionary<int, Ref<int>>();
			On_PlayerDrawLayers.DrawPlayer_RenderAllLayers += On_PlayerDrawLayers_DrawPlayer_RenderAllLayers;

			string category = $"UI.EffectUI.Misc.";
			EffectUIMiscHeaderText ??= Mod.GetLocalization($"{category}Header");
			EffectUIMiscNullifierExplanationText ??= Mod.GetLocalization($"{category}NullifierExplanation");
			EffectUIMiscCheckConfigText ??= Mod.GetLocalization($"{category}CheckConfig");
			EffectUIMiscChangeStackText ??= Mod.GetLocalization($"{category}ChangeStack");
			EffectUIMiscSwitchToNullifierText ??= Mod.GetLocalization($"{category}SwitchToNullifier");
			EffectUIMiscInfoText ??= Mod.GetLocalization($"{category}Info");
			EffectUIMiscTotalStacksText ??= Mod.GetLocalization($"{category}TotalStacks");
			EffectUIMiscDifficultyScalingText ??= Mod.GetLocalization($"{category}DifficultyScaling");
			EffectUIMiscHasChestsText ??= Mod.GetLocalization($"{category}HasChests");
			EffectUIMiscNoChestsText ??= Mod.GetLocalization($"{category}NoChests");
			EffectUIMiscNullifierSavingsText ??= Mod.GetLocalization($"{category}NullifierSavings");
			EffectUIMiscNullifierTotalText ??= Mod.GetLocalization($"{category}NullifierTotal");

			On_RemadeChatMonitor.DrawChat += On_RemadeChatMonitor_DrawChat;
		}

		private static void On_RemadeChatMonitor_DrawChat(On_RemadeChatMonitor.orig_DrawChat orig, RemadeChatMonitor self, bool drawingPlayerChat)
		{
			if (EffectsVisible && hoverIndex != -1)
			{
				return;
			}

			orig(self, drawingPlayerChat);
		}

		public override void OnModUnload()
		{
			TimeByIndex = null;
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			if (Main.gameMenu) return;
			//int mouseIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hotbar"));
			int mouseIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseIndex != -1)
			{
				layers.Insert(mouseIndex, new LegacyGameInterfaceLayer(
					$"{ModName}: {nameof(Effects)}",
					Effects,
					InterfaceScaleType.UI
				));
				layers.Insert(mouseIndex + 1, new LegacyGameInterfaceLayer(
					$"{ModName}: {nameof(WarbannerArrow)}",
					WarbannerArrow,
					InterfaceScaleType.Game
				));
				layers.Insert(mouseIndex + 2, new LegacyGameInterfaceLayer(
					$"{ModName}: {nameof(MagmaWormWarning)}",
					MagmaWormWarning,
					InterfaceScaleType.Game
				));
			}
		}

		/// <summary>
		/// Draws the warning arrow for the <see cref="MagmaWorm"/> emerge location
		/// </summary>
		private static readonly GameInterfaceDrawMethod MagmaWormWarning = delegate
		{
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];

				if (npc.active && npc.ModNPC is MagmaWormHead head && head.EmergeWarning)
				{
					Vector2 location = head.Location;
					if (location == Vector2.Zero) continue;
					Vector2 drawCenter = location - Main.screenPosition;
					Texture2D texture = ModContent.Request<Texture2D>("RiskOfSlimeRain/Textures/MagmaWormWarning").Value;
					Rectangle destination = Utils.CenteredRectangle(drawCenter, texture.Size());

					int offsetY = (int)(Main.GameUpdateCount / 8) % 2;
					destination.Y += offsetY * 4 - 6;

					Main.spriteBatch.Draw(texture, destination, Color.White);
				}
			}
			return true;
		};

		/// <summary>
		/// Draws the item UI for currently applied item
		/// </summary>
		private static readonly GameInterfaceDrawMethod Effects = delegate
		{
			Player player = Main.LocalPlayer;
			RORPlayer mPlayer = player.GetRORPlayer();
			if (!Main.playerInventory && Config.Instance.OnlyShowWhenOpenInventory)
			{
				if (!mPlayer.nullifierActive) return true;
			}
			List<ROREffect> effects = mPlayer.Effects;
			if (effects.Count == 0) return true;

			int xPosition = 0;
			int yPosition = 0;
			Rectangle sourceRect;
			Rectangle destRect;


			//int initialVerticalOffset = Main.screenHeight - 30;
			int initialVerticalOffset = (int)(Main.screenHeight * (1 - Config.Instance.ItemUIVerticalOffset));

			int numHorizontal = (Main.screenWidth - 3 * inventorySize) / iconPadding;
			if (numHorizontal <= 0) numHorizontal = 1;

			if (Main.playerInventory)
			{
				numHorizontal -= 3 * inventorySize / iconPadding; //Padding for accessory menu on the right (overlap with "Settings")
			}

			int lineOffset = 0;
			int numLines = effects.Count / (numHorizontal + 1);
			//Above calc sometimes can cause next row of effects to not show if theres only 1 item in them, depending on resolution
			int yStart = initialVerticalOffset - numLines * verticalLineHeight;
			ROREffect effect;
			Texture2D texture;

			//Draw the info icon on the very left
			//xPosition = 2 * inventorySize + (-1 - 0) * iconPadding;
			//yPosition = yStart + 0;
			bool drawingInfo = true;

			Vector2 priceDrawPos = Vector2.Zero;

			for (int i = 0; i < effects.Count; i++)
			{
				effect = effects[i];
				texture = ModContent.Request<Texture2D>(effect.Texture).Value;

				lineOffset = i / numHorizontal;
				//2 * INVENTORY_SIZE is the distance needed to not overlap with recipe UI
				xPosition = 2 * inventorySize + (i - lineOffset * numHorizontal) * iconPadding;

				yPosition = yStart + lineOffset * verticalLineHeight;

				if (drawingInfo)
				{
					texture = mPlayer.nullifierActive ? TextureAssets.Item[ModContent.ItemType<Nullifier>()].Value : TextureAssets.NpcHead[0].Value;
					xPosition -= iconPadding; //Go left one icon distance
				}

				sourceRect = texture.Bounds;
				int width = sourceRect.Width;
				int height = sourceRect.Height;

				//SCALING
				float scale = 1f;
				if (width > height) scale = (float)iconSize / width;
				else scale = (float)iconSize / height;

				width = (int)(width * scale);
				height = (int)(height * scale);

				destRect = Utils.CenteredRectangle(new Vector2(xPosition, yPosition), new Vector2(width, height));

				Vector2 Bottom = new Vector2(xPosition, yPosition) + Main.screenPosition;

				//destRect.Y = (int)yPosition - height;
				//if (i == 0)
				//{
				//	Utils.DrawLine(Main.spriteBatch, Bottom, new Vector2(Bottom.X + 500, Bottom.Y), Color.White, Color.White, 1);
				//}
				//Vector2 Center = destRect.Center.ToVector2() + Main.screenPosition;
				//Utils.DrawLine(Main.spriteBatch, new Vector2(Center.X - 10, Center.Y - 10), new Vector2(Center.X + 20, Center.Y + 20), Color.Green, Color.White, 4);

				Color color = Color.White * 0.8f;

				if (drawingInfo)
				{
					color = Color.White;
				}

				Rectangle mouseCheck = Utils.CenteredRectangle(new Vector2(xPosition, yPosition), new Vector2(32));
				if (mouseCheck.Contains(new Point(Main.mouseX, Main.mouseY)))
				{
					hoverIndex = i;
					destRect.Inflate(2, 2);
					color = Color.White;
					if (drawingInfo)
					{
						hoverIndex = -2;
					}
				}
				else
				{
					color *= hoverAlphaColorMultForOther;
				}

				Main.spriteBatch.Draw(texture, destRect, sourceRect, color);

				Vector2 leftCenter = new Vector2(xPosition - (width / 2), yPosition + (iconSize / 2) - 2);
				//Vector2 bottomCenter = destRect.BottomLeft();

				if (drawingInfo)
				{
					if (mPlayer.nullifierActive && mPlayer.savings > -1)
					{
						priceDrawPos = new Vector2(xPosition, yPosition);
						priceDrawPos.X -= width / 2;
						priceDrawPos.Y -= 10 + 2 * iconSize;
					}

					drawingInfo = false;
					i--;
				}
				else
				{
					string text = "x" + effect.Stack.ToString();
					if (!effect.FullStack) text += "/" + effect.UnlockedStack;
					Vector2 length = FontAssets.ItemStack.Value.MeasureString(text);

					leftCenter.Y -= length.Y / 2;
					color = Color.White;
					if (effect.Capped)
					{
						color = Color.LawnGreen;
					}
					if (hoverIndex != i)
					{
						color *= hoverAlphaColorMultForOther;
					}
					ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.ItemStack.Value, text, leftCenter, color, 0, Vector2.Zero, Vector2.One * 0.78f);

					if (mPlayer.nullifierActive)
					{
						leftCenter.Y -= length.Y;
						text = "x" + effect.NullifierStack.ToString();
						length = FontAssets.ItemStack.Value.MeasureString(text);

						color = Color.OrangeRed;
						ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.ItemStack.Value, text, leftCenter, color, 0, Vector2.Zero, Vector2.One * 0.78f);
					}
				}
			}

			if (hoverIndex > -1)
			{
				if (Main.hoverItemName != "" || player.mouseInterface || Main.mouseText) return true;
				player.mouseInterface = true;
				player.cursorItemIconEnabled = false;

				effect = effects[hoverIndex];
				string name = ROREffectManager.GetDisplayName(effect);
				string text = "\n" + ROREffectManager.GetDescription(effect);
				var uiInfo = effect.UIInfo();
				if (uiInfo != string.Empty)
				{
					text += "\n" + uiInfo;
				}
				if (effect.Capped)
				{
					text += "\n" + effect.CappedMessage;
				}
				if (effect.BlockedByBlacklist)
				{
					text += "\n" + effect.BlockedMessage;
				}

				Vector2 textPos = GetTextPosFromMouse(text);

				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, name, textPos, effect.RarityColor * (Main.mouseTextColor / 255f), 0, Vector2.Zero, Vector2.One);
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, text, textPos, Color.White * (Main.mouseTextColor / 255f), 0, Vector2.Zero, Vector2.One);
			}
			else if (hoverIndex == -2) //Misc
			{
				if (Main.hoverItemName != "" || player.mouseInterface || Main.mouseText) return true;
				player.mouseInterface = true;
				player.cursorItemIconEnabled = false;

				string modName = RiskOfSlimeRainMod.Instance.DisplayName;
				string text = EffectUIMiscHeaderText.Format(modName);

				if (mPlayer.nullifierActive)
				{
					text += "\n" + EffectUIMiscNullifierExplanationText.ToString();
				}
				else
				{
					text += "\n" + EffectUIMiscCheckConfigText.ToString();
					if (Config.Instance.CustomStacking)
					{
						text += "\n" + EffectUIMiscChangeStackText.ToString();
					}
					if (mPlayer.nullifierEnabled)
					{
						text += "\n" + EffectUIMiscSwitchToNullifierText.ToString();
					}
					text += "\n" + EffectUIMiscInfoText.Format(ROREffect.GetProcByUseTime(player).ToPercent(2));

					int activeStacks = mPlayer.CountActiveEffects();
					int totalStacks = mPlayer.CountTotalEffects();
					text += "\n" + EffectUIMiscTotalStacksText.Format(activeStacks, totalStacks);

					//text += "\nNext boss to fight for guaranteed item: " + NPCLootManager.GetDisplayNameOfEarliestNonBeatenBoss(out _); //Iffy when progression is blocked
					if (ServerConfig.Instance.DifficultyScaling)
					{
						text += "\n" + EffectUIMiscDifficultyScalingText.Format(mPlayer.TakenDamageMultiplier().ToPercent(), mPlayer.SpawnIncreaseMultiplier().ToPercent());
					}
					int chestCount = ChestManager.totalChests;
					if (chestCount > 0)
					{
						text += "\n" + EffectUIMiscHasChestsText.Format(chestCount, modName);
					}
					else
					{
						text += "\n" + EffectUIMiscNoChestsText.Format(modName);
					}
				}

				Vector2 textPos = GetTextPosFromMouse(text);

				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, text, textPos, Color.White * (Main.mouseTextColor / 255f), 0, Vector2.Zero, Vector2.One);
			}

			if (hoverIndex != -2 && mPlayer.nullifierActive && mPlayer.savings > -1)
			{
				//Don't draw it if you mouseover the info icon because of readability issues
				string text = EffectUIMiscNullifierSavingsText.Format(mPlayer.savings.MoneyToString());
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.ItemStack.Value, text, priceDrawPos, Color.White, 0, Vector2.Zero, Vector2.One * 0.78f);

				priceDrawPos.Y += iconSize;
				text = EffectUIMiscNullifierTotalText.Format(mPlayer.nullifierMoney.MoneyToString());
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.ItemStack.Value, text, priceDrawPos, Color.White, 0, Vector2.Zero, Vector2.One * 0.78f);
			}

			EffectsVisible = true;

			return true;
		};

		/// <summary>
		/// Draws a directional indicator towards the nearest warbanner
		/// </summary>
		private static readonly GameInterfaceDrawMethod WarbannerArrow = delegate
		{
			//Credit to jopojelly (adjusted from Census)
			Player player = Main.LocalPlayer;
			RORPlayer mPlayer = player.GetRORPlayer();
			if (WarbannerManager.warbanners.Count <= 0) return true;
			if (player.HeldItem.ModItem is not WarbannerRemover) return true;

			Color color = Color.White;
			Vector2 target = default(Vector2);
			Projectile proj = default(Projectile);

			int identity = mPlayer.LastWarbannerIdentity;
			if (identity > -1)
			{
				proj = WarbannerManager.FindWarbannerProj(identity);
				color = Color.Red;
			}
			else
			{
				proj = WarbannerManager.FindNearestWarbannerProj(player.Center);
			}

			if (proj != null)
			{
				target = proj.Center;
			}

			if (target == default)
			{
				Warbanner banner = WarbannerManager.FindNearestInactiveWarbanner(player.Center);
				if (banner == null) return true;

				target = banner.position;
			}

			float fade = 0;

			Vector2 playerCenter = player.Center + new Vector2(0, player.gfxOffY);
			Vector2 between = target - playerCenter;
			float length = between.Length();
			if (length > 40)
			{
				Vector2 offset = Vector2.Normalize(between) * Math.Min(70, length - 20);
				float rotation = between.ToRotation();
				Vector2 drawPosition = playerCenter - Main.screenPosition + offset;
				fade = Math.Min(1f, (length - 20) / 70) * (1 - fade);

				Texture2D arrow = ModContent.Request<Texture2D>("RiskOfSlimeRain/Textures/EnemyArrow").Value;
				Texture2D arrowWhite = ModContent.Request<Texture2D>("RiskOfSlimeRain/Textures/EnemyArrowWhite").Value;
				Main.spriteBatch.Draw(arrowWhite, drawPosition, null, Color.White * fade, rotation, arrowWhite.Size() / 2, new Vector2(1.3f), SpriteEffects.None, 0);
				Main.spriteBatch.Draw(arrow, drawPosition, null, color * fade, rotation, arrow.Size() / 2, new Vector2(1), SpriteEffects.None, 0);
			}
			return true;
		};

		private static void On_PlayerDrawLayers_DrawPlayer_RenderAllLayers(On_PlayerDrawLayers.orig_DrawPlayer_RenderAllLayers orig, ref PlayerDrawSet drawinfo)
		{
			var drawPlayer = drawinfo.drawPlayer;
			SoldiersSyringeEffect effect = ROREffectManager.GetEffectOfType<SoldiersSyringeEffect>(drawPlayer);
			if (effect == null || effect?.Active == false || Config.HiddenVisuals(drawPlayer))
			{
				orig(ref drawinfo);
				return;
			}

			//Modify Main.playerDrawData first
			List<DrawData> drawDataCopy = new List<DrawData>(drawinfo.DrawDataCache);
			List<DrawData> drawDatas = new List<DrawData>();

			for (int i = 0; i < drawDataCopy.Count; i++)
			{
				try
				{
					DrawData data = drawDataCopy[i];
					if (data.texture?.Name?.StartsWith("RiskOfSlimeRain") ?? false) continue;

					data.position += effect.shakePosOffset * effect.shakeTimer;
					data.scale += effect.shakeScaleOffset * effect.shakeTimer;
					data.color = data.color.MultiplyRGBA(Color.Yellow * 0.5f);
					data.color *= 0.05f * effect.shakeTimer;
					drawDatas.Add(data);
				}
				catch
				{

				}
			}

			drawinfo.DrawDataCache.AddRange(drawDatas);

			orig(ref drawinfo);
		}

		/// <summary>
		/// Returns mouse position based on text and screen edges
		/// </summary>
		private static Vector2 GetTextPosFromMouse(string text)
		{
			Vector2 mousePos = new Vector2(Main.mouseX, Main.mouseY);
			mousePos += new Vector2(Main.ThickMouse ? 22 : 16);

			Vector2 size = FontAssets.MouseText.Value.MeasureString(text);

			if (mousePos.X + size.X + 4f > Main.screenWidth)
			{
				mousePos.X = (int)(Main.screenWidth - size.X - 4f);
			}
			if (mousePos.Y + size.Y + 4f > Main.screenHeight)
			{
				mousePos.Y = (int)(Main.screenHeight - size.Y - 4f);
			}
			return mousePos;
		}

		public static void Update(Player player)
		{
			RORPlayer mPlayer = player.GetRORPlayer();

			if (Main.myPlayer == player.whoAmI)
			{
				float alphaSpeed = 2f / 60;

				if (hoverIndex > -1 || hoverIndex == -2)
				{
					hoverAlpha += alphaSpeed;
					hoverAlpha = Math.Min(hoverAlpha, hoverAlphaMax + 0.2f); //Additional to "stagger" fade in when switching mouseover targets
				}
				else
				{
					hoverAlpha -= alphaSpeed;
					hoverAlpha = Math.Max(hoverAlpha, 0f);
				}
			}

			if (Main.myPlayer == player.whoAmI && hoverIndex != -1)
			{
				if (hoverIndex == -2)
				{
					//On the "?" or the nullifier
					if (mPlayer.nullifierActive)
					{
						//On the nullifier
						if (mPlayer.mouseRight)
						{
							mPlayer.DeactivateNullifier();
						}
						else if (mPlayer.DoubleClick())
						{
							bool success = mPlayer.ApplyNullifier();
							if (success)
							{
								mPlayer.DeactivateNullifier();
							}
						}
					}
					else
					{
						//On the "?"
						if (mPlayer.mouseRight && mPlayer.nullifierEnabled)
						{
							mPlayer.ActivateNullifier();
						}
					}
				}
				else
				{
					//On the icons
					List<ROREffect> effects = mPlayer.Effects;
					ROREffect effect = effects[hoverIndex];
					if (mPlayer.nullifierActive)
					{
						//On the icons when nullifier is enabled
						if (mPlayer.mouseLeft || PlayerInput.ScrollWheelDelta > 0)
						{
							effect.NullifierStack++;
							PlayerInput.ScrollWheelDelta = 0;
							SoundEngine.PlaySound(SoundID.MenuTick.WithVolumeScale(0.8f));
						}
						else if (mPlayer.mouseRight || PlayerInput.ScrollWheelDelta < 0)
						{
							effect.NullifierStack--;
							PlayerInput.ScrollWheelDelta = 0;
							SoundEngine.PlaySound(SoundID.MenuTick.WithVolumeScale(0.8f));
						}
					}
					else if (Config.Instance.CustomStacking)
					{
						//On the icons when custom stacking is enabled (and nullifier disabled)

						//This stuff is here cause only here resetting scrollwheel status works properly
						int oldStack = effect.Stack;
						if (mPlayer.mouseLeft || PlayerInput.ScrollWheelDelta > 0)
						{
							effect.Stack++;
							PlayerInput.ScrollWheelDelta = 0;
							SoundEngine.PlaySound(SoundID.MenuTick.WithVolumeScale(0.8f));
						}
						else if (mPlayer.mouseRight || PlayerInput.ScrollWheelDelta < 0)
						{
							effect.Stack--;
							PlayerInput.ScrollWheelDelta = 0;
							SoundEngine.PlaySound(SoundID.MenuTick.WithVolumeScale(0.8f));
						}

						if (Main.netMode != NetmodeID.SinglePlayer && oldStack != effect.Stack)
						{
							SetChangedEffect(hoverIndex);
						}
					}
				}

				hoverIndex = -1;
			}

			if (Main.myPlayer == player.whoAmI)
			{
				if (EffectsVisible && mPlayer.nullifierActive)
				{
					mPlayer.UpdateNullifierAfterUI();
					mPlayer.savings = player.GetSavings();
				}
				else
				{
					mPlayer.UpdateNullifierAfterUI();
				}

				EffectsVisible = false;
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					SyncChangedEffects(player);
				}
			}
		}

		private static void SetChangedEffect(int index)
		{
			if (TimeByIndex.ContainsKey(index))
			{
				TimeByIndex[index].Value = syncTimer;
			}
			else
			{
				TimeByIndex.Add(index, new Ref<int>(syncTimer));
			}
		}

		private static void SyncChangedEffects(Player player)
		{
			RORPlayer mPlayer = player.GetRORPlayer();
			List<ROREffect> effects = mPlayer.Effects;
			List<int> toRemove = new List<int>();
			foreach (int index in TimeByIndex.Keys)
			{
				if (TimeByIndex[index].Value <= 0)
				{
					new ROREffectSyncSingleStackPacket(player, index).Send();
					toRemove.Add(index);
				}
			}
			for (int i = 0; i < toRemove.Count; i++)
			{
				TimeByIndex.Remove(toRemove[i]);
			}
			foreach (int index in TimeByIndex.Keys)
			{
				TimeByIndex[index].Value--;
			}
		}
	}
}
