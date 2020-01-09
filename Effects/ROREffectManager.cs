using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Attributes;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Data;
using RiskOfSlimeRain.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Extensions;
using Microsoft.Xna.Framework.Graphics;

namespace RiskOfSlimeRain.Effects
{
	/// <summary>
	/// Central point of the effect system. Stores various cached data, and is able to retreive things around effects and execute their hooks
	/// </summary>
	public static class ROREffectManager
	{
		//Used to build the dictionary EffectByType on RORPlayer
		private static Type[] validInterfaces;
		//Used to check if an effect procs or not
		private static Type[] interfaceCanProc;
		//Those two need caching cause they are used in ModifyTooltips dynamically for multiline tooltips with color
		private static Dictionary<Type, string> flavorText;
		private static Dictionary<Type, int> rarity;
		//Reverse assign from the item itself, that the effect then accesses
		private static Dictionary<Type, string> texture;

		/// <summary>
		/// Index assigned before sending the SyncSingle(Stack) packet
		/// </summary>
		public static int Index { get; set; } = 0;

		public static void Load()
		{
			//Reflection shenanigans
			Type[] types = typeof(ROREffectManager).Assembly.GetTypes();
			List<Type> interfaces = new List<Type>();
			List<Type> canProcs = new List<Type>();
			flavorText = new Dictionary<Type, string>();
			rarity = new Dictionary<Type, int>();
			texture = new Dictionary<Type, string>();
			foreach (var type in types)
			{
				if (type.IsInterface)
				{
					Type interf = type.GetInterface(typeof(IROREffectInterface).Name);
					if (interf != null)
					{
						if (!interfaces.Contains(type))
						{
							interfaces.Add(type);
							var canProc = type.GetCustomAttribute<CanProc>();
							if (canProc != null) canProcs.Add(type);
						}
					}
				}
				else if (!type.IsAbstract && type.IsSubclassOf(typeof(ROREffect)))
				{
					ROREffect effect = ROREffect.CreateInstanceNoPlayer(type);
					flavorText[type] = effect.FlavorText;
					rarity[type] = effect.Rarity;
				}
			}
			validInterfaces = interfaces.ToArray();
			interfaceCanProc = canProcs.ToArray();
		}

		public static void Unload()
		{
			validInterfaces = null;
			flavorText = null;
			rarity = null;
			texture = null;
		}

		/// <summary>
		/// Sets up the effect dictionary on the player
		/// </summary>
		public static void Init(RORPlayer mPlayer)
		{
			foreach (var interf in validInterfaces)
			{
				mPlayer.EffectByType[interf] = new List<ROREffect>();
			}
		}

		public static string GetFlavorText<T>() where T : ROREffect
		{
			return flavorText[typeof(T)];
		}

		public static int GetRarity<T>() where T : ROREffect
		{
			return rarity[typeof(T)];
		}

		public static string GetTexture(Type type)
		{
			if (!texture.ContainsKey(type)) return "";
			return texture[type];
		}

		/// <summary>
		/// This is called in the items code to set the effects texture to the item texture
		/// </summary>
		public static void SetTexture<T>(string texture) where T : ROREffect
		{
			ROREffectManager.texture[typeof(T)] = texture;
		}

		/// <summary>
		/// Adds an effect to the list (or increases the stack of an existing effect)
		/// </summary>
		public static void ApplyEffect<T>(RORPlayer mPlayer) where T : ROREffect
		{
			List<ROREffect> effects = mPlayer.Effects;
			Dictionary<Type, List<ROREffect>> effectByType = mPlayer.EffectByType;
			//first, check if effect exists
			ROREffect existing = effects.FirstOrDefault(e => e.GetType().Equals(typeof(T)));
			if (existing != null)
			{
				//effect exists, increase stack
				//CanStack already checked in the hook ran in CanUseItem
				existing.IncreaseStack();
			}
			else
			{
				//effect doesn't exist, add one
				ROREffect newEffect = ROREffect.NewInstance<T>(mPlayer.player);
				//by definition of the list order, append
				effects.Add(newEffect);
				Type[] validInterfaces = GetValidInterfaces(typeof(T));
				foreach (var interf in validInterfaces)
				{
					effectByType[interf].Add(newEffect);
				}
			}
		}

		public static void Populate(RORPlayer mPlayer)
		{
			//GeneralHelper.Print("populating " + mPlayer.player.whoAmI);
			List<ROREffect> effects = mPlayer.Effects;
			Dictionary<Type, List<ROREffect>> effectByType = mPlayer.EffectByType;
			Clear(mPlayer);
			foreach (var effect in effects)
			{
				Type[] interfaces = GetValidInterfaces(effect.GetType()); //because this is dynamic
				foreach (var interf in interfaces)
				{
					//If this interface is one in effectByType, add this effect as a value to this list
					effectByType[interf].Add(effect);
				}
				foreach (var interf in interfaces)
				{
					effectByType[interf].Sort();
				}
			}
		}

		//used in a place where the type is dynamic
		public static Type[] GetValidInterfaces(Type effectType)
		{
			Type[] interfaces = effectType.GetInterfaces();
			return interfaces.Intersect(validInterfaces).ToArray();
		}

		public static void Clear(RORPlayer mPlayer)
		{
			foreach (var value in mPlayer.EffectByType.Values)
			{
				value.Clear();
			}
		}

		public static List<ROREffect> GetEffectsOf<T>(RORPlayer mPlayer)
		{
			return mPlayer.EffectByType[typeof(T)];
		}

		/// <summary>
		/// Used to retreive an effect of type T on the player 
		/// </summary>
		public static T GetEffectOfType<T>(RORPlayer mPlayer) where T : ROREffect
		{
			return mPlayer.Effects.FirstOrDefault(e => e is T) as T;
		}

		/// <summary>
		/// Used to retreive the index of an effect on the player
		/// </summary>
		public static int GetIndexOfEffect(RORPlayer mPlayer, ROREffect effect)
		{
			return mPlayer.Effects.FindIndex(e => e == effect); //check same reference
		}

		/// <summary>
		/// Compact way to check if an effect can trigger (but slower, only used in Perform()
		/// </summary>
		public static bool CanDoEffect<T>(ROREffect effect)
		{
			bool can = interfaceCanProc.Contains(typeof(T));
			return can ? effect.Proccing : effect.Active;
		}

		#region Syncing
		public static void HandleOnEnterToServer(BinaryReader reader)
		{
			//if this is server: from OnEnterWorld
			//if this is client: from here

			byte whoAmI = reader.ReadByte();
			PopulatePlayer(whoAmI, reader);

			if (Main.netMode == NetmodeID.Server)
			{
				//forward to other players
				SendOnEnter(whoAmI, -1, whoAmI);
			}
		}

		public static void SendOnEnter(byte whoAmI, int to = -1, int from = -1)
		{
			RORPlayer mPlayer = Main.player[whoAmI].GetRORPlayer();
			ModPacket packet = RiskOfSlimeRainMod.Instance.GetPacket();
			packet.Write((int)RORMessageType.SyncEffectsOnEnterToServer);
			packet.Write((byte)whoAmI);
			packet.Write((int)mPlayer.Effects.Count);
			for (int i = 0; i < mPlayer.Effects.Count; i++)
			{
				ROREffect effect = mPlayer.Effects[i];
				effect.SendOnEnter(packet);
			}
			packet.Send(to, from);
		}

		private static void PopulatePlayer(byte whoAmI, BinaryReader reader)
		{
			Player player = Main.player[whoAmI];
			RORPlayer mPlayer = player.GetRORPlayer();
			int length = reader.ReadInt32();
			mPlayer.Effects = new List<ROREffect>();
			for (int i = 0; i < length; i++)
			{
				ROREffect effect = ROREffect.CreateInstanceFromNet(player, reader);
				mPlayer.Effects.Add(effect);
			}
			Populate(mPlayer);
		}

		public static void SendSingleEffect(RORPlayer mPlayer, ROREffect effect)
		{
			Index = GetIndexOfEffect(mPlayer, effect);
			if (Index == -1) return;
			mPlayer.SendIfLocal<ROREffectSyncSinglePacket>();
		}

		public static void SendSingleEffectStack(RORPlayer mPlayer, ROREffect effect)
		{
			Index = GetIndexOfEffect(mPlayer, effect);
			if (Index == -1) return;
			mPlayer.SendIfLocal<ROREffectSyncSingleStackPacket>();
		}
		#endregion

		#region Hooks
		/// <summary>
		/// This works for regular void, non-ref methods
		/// </summary>
		public static void Perform<T>(RORPlayer mPlayer, Action<T> action) where T : IROREffectInterface
		{
			List<ROREffect> effects = GetEffectsOf<T>(mPlayer);
			foreach (var effect in effects)
			{
				if (CanDoEffect<T>(effect) && effect is T t)
				{
					action(t);
				}
			}
		}

		public static void ModifyHitNPC(Player player, Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			List<ROREffect> effects = GetEffectsOf<IModifyHit>(player.GetRORPlayer());
			foreach (var effect in effects)
			{
				if (effect.Proccing)
				{
					((IModifyHit)effect).ModifyHitNPC(player, item, target, ref damage, ref knockback, ref crit);
				}
			}
		}

		public static void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			List<ROREffect> effects = GetEffectsOf<IModifyHit>(player.GetRORPlayer());
			foreach (var effect in effects)
			{
				if (effect.Proccing)
				{
					((IModifyHit)effect).ModifyHitNPCWithProj(player, proj, target, ref damage, ref knockback, ref crit, ref hitDirection);
				}
			}
		}

		public static void GetWeaponCrit(Player player, Item item, ref int crit)
		{
			List<ROREffect> effects = GetEffectsOf<IGetWeaponCrit>(player.GetRORPlayer());
			foreach (var effect in effects)
			{
				if (effect.Active)
				{
					((IGetWeaponCrit)effect).GetWeaponCrit(player, item, ref crit);
				}
			}
		}

		//unused
		public static void ModifyWeaponDamage(Player player, Item item, ref float add, ref float mult, ref float flat)
		{
			List<ROREffect> effects = GetEffectsOf<IModifyWeaponDamage>(player.GetRORPlayer());
			foreach (var effect in effects)
			{
				if (effect.Active)
				{
					((IModifyWeaponDamage)effect).ModifyWeaponDamage(player, item, ref add, ref mult, ref flat);
				}
			}
		}

		public static List<Effect> GetScreenShaders(Player player)
		{
			List<Effect> shaders = new List<Effect>();
			List<ROREffect> effects = GetEffectsOf<IScreenShader>(player.GetRORPlayer());
			foreach (var effect in effects)
			{
				if (effect.Active)
				{
					Effect shader = ((IScreenShader)effect).GetScreenShader(player);
					if (shader != null) shaders.Add(shader);
				}
			}
			return shaders;
		}

		public static void DrawPlayerLayers(Player player, List<PlayerLayer> layers)
		{
			List<PlayerLayer> newLayers = new List<PlayerLayer>();
			List<ROREffect> effects = GetEffectsOf<IPlayerLayer>(player.GetRORPlayer());
			foreach (var effect in effects)
			{
				if (effect.Active)
				{
					PlayerLayerParams definition = ((IPlayerLayer)effect).GetPlayerLayerParams(player);
					if (definition != null)
					{
						newLayers.Add(new PlayerLayer("RiskOfSlimeRain", effect.Name, PlayerLayer.MiscEffectsBack, delegate(PlayerDrawInfo drawInfo)
						{
							if (drawInfo.shadow != 0f)
							{
								return;
							}
							Player dPlayer = drawInfo.drawPlayer;

							Texture2D tex = definition.Texture;
							float drawX = (int)dPlayer.Center.X - Main.screenPosition.X;
							float drawY = (int)dPlayer.Center.Y - Main.screenPosition.Y;

							Vector2 off = definition.Offset;
							SpriteEffects spriteEffects = SpriteEffects.None;
							if (dPlayer.gravDir < 0f)
							{
								off.Y = -off.Y;
								spriteEffects = SpriteEffects.FlipVertically;
							}
							drawY += off.Y + dPlayer.gfxOffY;
							drawX += off.X;
							Color color = definition.Color ?? Color.White;
							if (definition.IgnoreAlpha ?? false)
							{
								color *= (255 - dPlayer.immuneAlpha) / 255f;
							}
							Rectangle sourceRect = definition.GetFrame();
							DrawData data = new DrawData(tex, new Vector2(drawX, drawY), sourceRect, color, 0, sourceRect.Size() / 2, definition.Scale ?? 1f, spriteEffects, 0);
							Main.playerDrawData.Add(data);
						}));
					}
				}
			}
			foreach (var layer in newLayers)
			{
				layers.Insert(0, layer);
			}
		}

		public static float UseTimeMultiplier(Player player, Item item, ref float multiplier)
		{
			List<ROREffect> effects = GetEffectsOf<IUseTimeMultiplier>(player.GetRORPlayer());
			foreach (var effect in effects)
			{
				if (effect.Active)
				{
					((IUseTimeMultiplier)effect).UseTimeMultiplier(player, item, ref multiplier);
				}
			}
			return multiplier;
		}

		public static bool PreHurt(Player player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			bool ret = true;
			List<ROREffect> effects = GetEffectsOf<IPreHurt>(player.GetRORPlayer());
			foreach (var effect in effects)
			{
				if (effect.Proccing)
				{
					ret &= ((IPreHurt)effect).PreHurt(player, pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);
				}
			}
			//if atleast one PreHurt returns false, it will return false
			return ret;
		}
		#endregion
	}
}
