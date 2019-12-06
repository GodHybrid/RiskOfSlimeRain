using System;
using System.Linq;
using System.Collections.Generic;
using RiskOfSlimeRain.Effects.Interfaces;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using System.IO;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects
{
	public static class ROREffectManager
	{
		//used to build the dictionary EffectByType on RORPlayer
		public static Type[] validInterfaces;
		//those two need caching cause they are used in ModifyTooltips dynamically for multiline tooltips with color
		public static Dictionary<Type, string> flavorText;
		public static Dictionary<Type, Color> flavorColor;
		//reverse assign from the item itself, that the effect then accesses
		public static Dictionary<Type, string> texture;

		public static void Load()
		{
			//Reflection shenanigans
			Type[] types = typeof(ROREffectManager).Assembly.GetTypes();
			List<Type> interfaces = new List<Type>();
			flavorText = new Dictionary<Type, string>();
			flavorColor = new Dictionary<Type, Color>();
			texture = new Dictionary<Type, string>();
			foreach (var type in types)
			{
				if (type.IsInterface)
				{
					Type interf = type.GetInterface(typeof(IROREffectInterface).Name);
					if (interf != null)
					{
						if (!interfaces.Contains(type)) interfaces.Add(type);
					}
				}
				else if(type.IsSubclassOf(typeof(ROREffect)))
				{
					ROREffect effect = ROREffect.CreateInstance(type);
					flavorText[type] = effect.FlavorText;
					flavorColor[type] = effect.FlavorColor;
				}
			}
			validInterfaces = interfaces.ToArray();
		}

		public static void Unload()
		{
			validInterfaces = null;
			flavorText = null;
			flavorColor = null;
			texture = null;
		}

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

		public static Color GetFlavorColor<T>() where T : ROREffect
		{
			return flavorColor[typeof(T)];
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

		public static void ApplyEffect<T>(RORPlayer mPlayer) where T : ROREffect
		{
			List<ROREffect> effects = GetGlobalEffectList(mPlayer);
			Dictionary<Type, List<ROREffect>> effectByType = GetEffectByType(mPlayer);
			//first, check if effect exists
			ROREffect existing = effects.FirstOrDefault(e => e.GetType().Equals(typeof(T)));
			if (existing != null)
			{
				//effect exists, increase stack
				//CanStack already checked in the hook ran in CanUseItem
				existing.Stack++;
			}
			else
			{
				//effect doesn't exist, add one
				ROREffect newEffect = ROREffect.NewInstance<T>();
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
			List<ROREffect> effects = GetGlobalEffectList(mPlayer);
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

		public static bool IsEmpty(RORPlayer mPlayer)
		{
			foreach (var effects in mPlayer.EffectByType.Values)
			{
				if (effects.Count > 0) return false;
			}
			return true;
		}

		public static List<ROREffect> GetEffectsOf<T>(RORPlayer mPlayer)
		{
			return GetEffectByType(mPlayer)[typeof(T)];
		}

		public static List<ROREffect> GetGlobalEffectList(RORPlayer mPlayer)
		{
			return mPlayer.Effects;
		}

		public static Dictionary<Type, List<ROREffect>> GetEffectByType(RORPlayer mPlayer)
		{
			if (IsEmpty(mPlayer))
			{
				Populate(mPlayer);
			}
			return mPlayer.EffectByType;
		}

		/// <summary>
		/// This works for regular void, non-ref methods
		/// </summary>
		public static void Perform<T>(RORPlayer mPlayer, Action<T> action) where T : IROREffectInterface
		{
			//if (mPlayer.player.loadStatus == 0 && typeof(T).Equals(typeof(IResetEffects))) return;
			List<ROREffect> effects = GetEffectsOf<T>(mPlayer);
			foreach (var effect in effects)
			{
				if (effect.Proccing && effect is T t) action(t);
			}
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
			RORPlayer mPlayer = Main.player[whoAmI].GetModPlayer<RORPlayer>();
			ModPacket packet = RiskOfSlimeRain.Instance.GetPacket();
			packet.Write((byte)MessageType.SyncEffectsOnEnterToServer);
			packet.Write((byte)whoAmI);
			packet.Write((int)mPlayer.Effects.Count);
			for (int i = 0; i < mPlayer.Effects.Count; i++)
			{
				ROREffect effect = mPlayer.Effects[i];
				effect.Send(packet);
			}
			packet.Send(to, from);
		}

		public static void HandleOnEnterToClients(BinaryReader reader)
		{
			//from SyncPlayer
			byte whoAmI = reader.ReadByte();
			PopulatePlayer(whoAmI, reader);
		}

		private static void PopulatePlayer(byte whoAmI, BinaryReader reader)
		{
			Player player = Main.player[whoAmI];
			RORPlayer mPlayer = player.GetModPlayer<RORPlayer>();
			int length = reader.ReadInt32();
			for (int i = 0; i < length; i++)
			{
				ROREffect effect = ROREffect.CreateInstanceFromNet(reader);
				mPlayer.Effects.Add(effect);
			}
			Populate(mPlayer);
		}
		#endregion

		#region Special Hooks
		public static void ModifyHitNPC(Player player, Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			List<ROREffect> effects = GetEffectsOf<IModifyHit>(player.GetModPlayer<RORPlayer>());
			foreach (var effect in effects)
			{
				if (effect.Proccing) ((IModifyHit)effect).ModifyHitNPC(player, item, target, ref damage, ref knockback, ref crit);
			}
		}

		public static void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			List<ROREffect> effects = GetEffectsOf<IModifyHit>(player.GetModPlayer<RORPlayer>());
			foreach (var effect in effects)
			{
				if (effect.Proccing) ((IModifyHit)effect).ModifyHitNPCWithProj(player, proj, target, ref damage, ref knockback, ref crit, ref hitDirection);
			}
		}

		public static void GetWeaponCrit(Player player, Item item, ref int crit)
		{
			List<ROREffect> effects = GetEffectsOf<IGetWeaponCrit>(player.GetModPlayer<RORPlayer>());
			foreach (var effect in effects)
			{
				if (effect.Proccing) ((IGetWeaponCrit)effect).GetWeaponCrit(player, item, ref crit);
			}
		}

		public static float UseTimeMultiplier(Player player, Item item)
		{
			float multiplier = 1f;
			List<ROREffect> effects = GetEffectsOf<IUseTimeMultiplier>(player.GetModPlayer<RORPlayer>());
			foreach (var effect in effects)
			{
				if (effect.Proccing) ((IUseTimeMultiplier)effect).UseTimeMultiplier(player, item, ref multiplier);
			}
			return multiplier;
		}

		public static bool PreHurt(Player player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			bool ret = true;
			List<ROREffect> effects = GetEffectsOf<IPreHurt>(player.GetModPlayer<RORPlayer>());
			foreach (var effect in effects)
			{
				if (effect.Proccing) ret &= ((IPreHurt)effect).PreHurt(player, pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);
			}
			//if atleast one PreHurt returns false, it will return false
			return ret;
		}
		#endregion
	}
}
