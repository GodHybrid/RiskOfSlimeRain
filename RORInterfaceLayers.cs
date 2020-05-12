using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.ItemSpawning.ChestSpawning;
using RiskOfSlimeRain.Core.ItemSpawning.NPCSpawning;
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
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using WebmilioCommons.Tinq;
using static RiskOfSlimeRain.NPCs.Bosses.MagmaWorm;

namespace RiskOfSlimeRain
{
	public static class RORInterfaceLayers
	{
		private const int inventorySize = 47;

		private const int iconSize = 32;
		private const int iconPadding = iconSize + 6;
		private const int verticalLineHeight = 50;

		public static string Name => ModContent.GetInstance<RiskOfSlimeRainMod>().Name;

		public static int hoverIndex = -1;

		public static bool EffectsVisible { private set; get; }

		//Value is Ref(int) because it's counted down from within the iteration loop
		//When timer runs out, sync
		/// <summary>
		/// Multiplayer syncing thing used when changing stack manually from the UI. Keeps track of any changed stacks and a timer
		/// </summary>
		public static Dictionary<int, Ref<int>> TimeByIndex { get; set; }

		private const int syncTimer = 25;

		public static void Load()
		{
			TimeByIndex = new Dictionary<int, Ref<int>>();
			On.Terraria.Main.DrawPlayer_DrawAllLayers += Main_DrawPlayer_DrawAllLayers;
		}

		public static void Unload()
		{
			TimeByIndex = null;
		}

		public static void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			if (Main.gameMenu) return;
			//int mouseIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hotbar"));
			int mouseIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseIndex != -1)
			{
				layers.Insert(mouseIndex, new LegacyGameInterfaceLayer(
					$"{Name}: {nameof(Effects)}",
					Effects,
					InterfaceScaleType.UI
				));
				layers.Insert(mouseIndex + 1, new LegacyGameInterfaceLayer(
					$"{Name}: {nameof(WarbannerArrow)}",
					WarbannerArrow,
					InterfaceScaleType.Game
				));
				layers.Insert(mouseIndex + 2, new LegacyGameInterfaceLayer(
						$"{Name}: {nameof(MagmaWormWarning)}",
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

				if (npc.active && npc.modNPC is MagmaWormHead head && head.EmergeWarning)
				{
					Vector2 location = head.Location;
					if (location == Vector2.Zero) continue;
					Vector2 drawCenter = location - Main.screenPosition;
					Texture2D texture = ModContent.GetTexture("RiskOfSlimeRain/Textures/MagmaWormWarning");
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
				texture = ModContent.GetTexture(effect.Texture);

				lineOffset = i / numHorizontal;
				//2 * INVENTORY_SIZE is the distance needed to not overlap with recipe UI
				xPosition = 2 * inventorySize + (i - lineOffset * numHorizontal) * iconPadding;

				yPosition = yStart + lineOffset * verticalLineHeight;

				if (drawingInfo)
				{
					texture = mPlayer.nullifierActive ? Main.itemTexture[ModContent.ItemType<Nullifier>()] : Main.npcHeadTexture[0];
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

				Main.spriteBatch.Draw(texture, destRect, sourceRect, color);

				Vector2 leftCenter = new Vector2(xPosition - (width >> 1), yPosition + (iconSize >> 1) - 2);
				//Vector2 bottomCenter = destRect.BottomLeft();

				if (drawingInfo)
				{
					if (mPlayer.nullifierActive && mPlayer.savings > -1)
					{
						priceDrawPos = new Vector2(xPosition, yPosition);
						priceDrawPos.X -= width >> 1;
						priceDrawPos.Y -= 10 + 2 * iconSize;
					}

					drawingInfo = false;
					i--;
				}
				else
				{
					string text = "x" + effect.Stack.ToString();
					if (!effect.FullStack) text += "/" + effect.UnlockedStack;
					Vector2 length = Main.fontItemStack.MeasureString(text);

					leftCenter.Y -= length.Y / 2;
					color = Color.White;
					if (effect.Capped)
					{
						color = Color.LawnGreen;
					}
					ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontItemStack, text, leftCenter, color, 0, Vector2.Zero, Vector2.One * 0.78f);

					if (mPlayer.nullifierActive)
					{
						leftCenter.Y -= length.Y;
						text = "x" + effect.NullifierStack.ToString();
						length = Main.fontItemStack.MeasureString(text);

						color = Color.OrangeRed;
						ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontItemStack, text, leftCenter, color, 0, Vector2.Zero, Vector2.One * 0.78f);
					}
				}
			}

			if (hoverIndex > -1)
			{
				if (Main.hoverItemName != "" || player.mouseInterface || Main.mouseText) return true;
				player.mouseInterface = true;
				player.showItemIcon = false;

				effect = effects[hoverIndex];
				string name = effect.Name;
				string text = "\n" + effect.Description;
				if (effect.UIInfo() != string.Empty)
				{
					text += "\n" + effect.UIInfo();
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

				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, name, textPos, effect.RarityColor * (Main.mouseTextColor / 255f), 0, Vector2.Zero, Vector2.One);
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, text, textPos, Color.White * (Main.mouseTextColor / 255f), 0, Vector2.Zero, Vector2.One);
			}
			else if (hoverIndex == -2) //Misc
			{
				if (Main.hoverItemName != "" || player.mouseInterface || Main.mouseText) return true;
				player.mouseInterface = true;
				player.showItemIcon = false;

				string modName = RiskOfSlimeRainMod.Instance.DisplayName;
				string text = $"This UI shows all your currently used items from '{modName}'";

				if (mPlayer.nullifierActive)
				{
					text += "\nNullifier view currently active";
					text += "\nLeft/right click or use the mousewheel on an icon to increase/decrease removed stack count";
					text += "\nDouble left click on this icon twice to pay the price, remove the effects and return the items";
					text += "\nRight click to switch to normal UI";
				}
				else
				{
					text += "\nCheck the config of this mod to customize the UI";
					if (Config.Instance.CustomStacking)
					{
						text += "\nLeft/right click or use the mousewheel on an icon to increase/decrease stack";
					}
					if (mPlayer.nullifierEnabled)
					{
						text += "\nRight click to switch to nullifier UI";
					}
					text += "\nMisc Info:";
					text += "\nProc multiplier (based on held weapon): " + ROREffect.GetProcByUseTime(player).ToPercent(2);
					//text += "\nNext boss to fight for guaranteed item: " + NPCLootManager.GetDisplayNameOfEarliestNonBeatenBoss(out _); //Iffy when progression is blocked
					if (ServerConfig.Instance.DifficultyScaling)
					{
						text += "\nTaken damage multiplier: " + mPlayer.TakenDamageMultiplier().ToPercent();
						text += "\nSpawn increase multiplier: " + mPlayer.SpawnIncreaseMultiplier().ToPercent();
					}
					int chestCount = ChestManager.totalChests;
					if (chestCount > 0)
					{
						text += $"\nThis world originally generated with {chestCount} '{modName}' items in chests";
					}
					else
					{
						text += $"\nThis world has no '{modName}' items generated in chests. Make a new world!";
					}
				}

				Vector2 textPos = GetTextPosFromMouse(text);

				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, text, textPos, Color.White * (Main.mouseTextColor / 255f), 0, Vector2.Zero, Vector2.One);
			}

			if (hoverIndex != -2 && mPlayer.nullifierActive && mPlayer.savings > -1)
			{
				//Don't draw it if you mouseover the info icon because of readability issues
				string text = "Savings: " + mPlayer.savings.MoneyToString();
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontItemStack, text, priceDrawPos, Color.White, 0, Vector2.Zero, Vector2.One * 0.78f);

				priceDrawPos.Y += iconSize;
				text = "Total price: " + mPlayer.nullifierMoney.MoneyToString();
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontItemStack, text, priceDrawPos, Color.White, 0, Vector2.Zero, Vector2.One * 0.78f);
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
			if (!(player.HeldItem.modItem is WarbannerRemover)) return true;

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

				Texture2D arrow = ModContent.GetTexture("RiskOfSlimeRain/Textures/EnemyArrow");
				Texture2D arrowWhite = ModContent.GetTexture("RiskOfSlimeRain/Textures/EnemyArrowWhite");
				Main.spriteBatch.Draw(arrowWhite, drawPosition, null, Color.White * fade, rotation, arrowWhite.Size() / 2, new Vector2(1.3f), SpriteEffects.None, 0);
				Main.spriteBatch.Draw(arrow, drawPosition, null, color * fade, rotation, arrow.Size() / 2, new Vector2(1), SpriteEffects.None, 0);
			}
			return true;
		};

		private static void Main_DrawPlayer_DrawAllLayers(On.Terraria.Main.orig_DrawPlayer_DrawAllLayers orig, Main self, Player drawPlayer, int projectileDrawPosition, int cHead)
		{
			SoldiersSyringeEffect effect = ROREffectManager.GetEffectOfType<SoldiersSyringeEffect>(drawPlayer);
			if (effect == null || effect?.Active == false || Config.HiddenVisuals(drawPlayer))
			{
				orig(self, drawPlayer, projectileDrawPosition, cHead);
				return;
			}

			//Modify Main.playerDrawData first
			List<DrawData> drawDataCopy = new List<DrawData>(Main.playerDrawData);
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

			Main.playerDrawData.AddRange(drawDatas);

			orig(self, drawPlayer, projectileDrawPosition, cHead);
		}

		/// <summary>
		/// Returns mouse position based on text and screen edges
		/// </summary>
		private static Vector2 GetTextPosFromMouse(string text)
		{
			Vector2 mousePos = new Vector2(Main.mouseX, Main.mouseY);
			mousePos += new Vector2(Main.ThickMouse ? 22 : 16);

			Vector2 size = Main.fontMouseText.MeasureString(text);

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
							Main.PlaySound(SoundID.MenuTick, volumeScale: 0.8f);
						}
						else if (mPlayer.mouseRight || PlayerInput.ScrollWheelDelta < 0)
						{
							effect.NullifierStack--;
							PlayerInput.ScrollWheelDelta = 0;
							Main.PlaySound(SoundID.MenuTick, volumeScale: 0.8f);
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
							Main.PlaySound(SoundID.MenuTick, volumeScale: 0.8f);
						}
						else if (mPlayer.mouseRight || PlayerInput.ScrollWheelDelta < 0)
						{
							effect.Stack--;
							PlayerInput.ScrollWheelDelta = 0;
							Main.PlaySound(SoundID.MenuTick, volumeScale: 0.8f);
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
					new ROREffectSyncSingleStackPacket(effects[index]).Send();
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
