﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.ROREffects.Attributes;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Items.Consumable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects
{
	/// <summary>
	/// Central point of the effect system. Stores various cached data, and is able to retreive things around effects and execute their hooks
	/// </summary>
	public static class ROREffectManager
	{
		//Makes sure that effects are only loaded from this namespace (error otherwise)
		public static readonly string prefix = "RiskOfSlimeRain.Core.ROREffects.";
		//And that they end with this
		public static readonly string suffix = "Effect";

		//Maps a rarity to a list of all associated items of that rarity
		private static Dictionary<RORRarity, List<int>> itemRarityToItemTypes;

		private static List<ROREffect> effects;
		//Used to index each effect type for mp, and check for type validity
		private static Type[] indexedEffects;
		//Used to build the dictionary EffectByType on RORPlayer
		private static Type[] validInterfaces;
		//Used to check if an effect procs or not
		private static Type[] interfaceCanProc;

		//Those two need caching cause they are used in ModifyTooltips dynamically for multiline tooltips with color
		private static Dictionary<Type, LocalizedText> displayName;
		private static Dictionary<Type, LocalizedText> description;
		private static Dictionary<Type, LocalizedText> flavorText;
		private static Dictionary<Type, RORRarity> rarity;
		//Reverse assign from the item itself, that the effect then accesses
		private static Dictionary<Type, string> texture;
		//Assign type of effect to its associated item type (no key == no item assigned)
		private static Dictionary<Type, int> itemType;

		public static void Load()
		{
			//Reflection shenanigans
			Type[] types = typeof(ROREffectManager).Assembly.GetTypes();
			List<Type> effectTypes = new List<Type>();
			List<Type> interfaces = new List<Type>();
			List<Type> canProcs = new List<Type>();
			Dictionary<string, string> loadedTypeNamespaceToName = new Dictionary<string, string>();
			effects = new List<ROREffect>();
			displayName = new Dictionary<Type, LocalizedText>();
			description = new Dictionary<Type, LocalizedText>();
			flavorText = new Dictionary<Type, LocalizedText>();
			rarity = new Dictionary<Type, RORRarity>();
			texture = new Dictionary<Type, string>();
			itemType = new Dictionary<Type, int>();

			foreach (var type in types)
			{
				if (type.IsInterface)
				{
					if (typeof(IROREffectInterface).IsAssignableFrom(type))
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
					if (!(type.FullName.StartsWith(prefix) && type.FullName.EndsWith(suffix)))
					{
						throw new Exception($"Error loading ROREffect [{type.FullName}], it doesn't start with [{prefix}] and end with [{suffix}]");
					}
					else if (loadedTypeNamespaceToName.ContainsKey(type.Name))
					{
						throw new Exception($"Error loading ROREffect [{type.FullName}], an effect with that same name already exists in [{loadedTypeNamespaceToName[type.Name]}]! Make sure to make effect names unique");
					}

					ROREffect effect = ROREffect.CreateInstanceNoPlayer(type);
					effects.Add(effect);
					displayName[type] = effect.DisplayName;
					description[type] = effect.Description;
					flavorText[type] = effect.FlavorText;
					rarity[type] = effect.Rarity;
					effectTypes.Add(type);
					loadedTypeNamespaceToName.Add(type.Name, type.Namespace);
				}
			}
			indexedEffects = effectTypes.ToArray();
			validInterfaces = interfaces.ToArray();
			interfaceCanProc = canProcs.ToArray();

			itemRarityToItemTypes = new Dictionary<RORRarity, List<int>>();

			foreach (RORRarity rarity in Enum.GetValues(typeof(RORRarity)))
			{
				itemRarityToItemTypes.Add(rarity, new List<int>());
			}
		}

		public static void Unload()
		{
			effects = null;
			validInterfaces = null;
			displayName = null;
			description = null;
			flavorText = null;
			rarity = null;
			texture = null;
			itemType = null;
		}

		internal static void PostSetupContent()
		{
			ROREffect.SetupLocalization();
			foreach (var effect in effects)
			{
				_ = effect.UIInfoText; //To register localization
				effect.SetStaticDefaults();
			}
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

		public static string GetDisplayName<T>() where T : ROREffect
		{
			return displayName[typeof(T)].ToString();
		}

		public static string GetDisplayName(ROREffect effect)
		{
			return displayName[effect.GetType()].ToString();
		}

		public static string GetDescription<T>() where T : ROREffect
		{
			return description[typeof(T)].ToString();
		}

		public static string GetDescription(ROREffect effect)
		{
			return description[effect.GetType()].ToString();
		}

		public static string GetFlavorText<T>() where T : ROREffect
		{
			return flavorText[typeof(T)].ToString();
		}

		public static RORRarity GetRarity<T>() where T : ROREffect
		{
			return rarity[typeof(T)];
		}

		public static int GetRarityValue(RORRarity rarity)
		{
			switch (rarity)
			{
				case RORRarity.Common:
					return 5 * 100 * 100;
				case RORRarity.Uncommon:
					return 10 * 100 * 100;
				case RORRarity.Boss:
					return 15 * 100 * 100;
				case RORRarity.Artifact:
					return 20 * 100 * 100;
				default:
					return 0;
			}
		}

		public static Color GetRarityColor(RORRarity rarity)
		{
			return rarity == RORRarity.Common ? Color.White : ItemRarity.GetColor((int)rarity);
		}

		public static string GetTexture(Type type)
		{
			if (!texture.ContainsKey(type)) return "RiskOfSlimeRain/Empty";
			return texture[type];
		}

		/// <summary>
		/// This is called in the items code to register it in various data structures for further reference
		/// </summary>
		public static void RegisterItem<T>(RORConsumableItem<T> rItem) where T : ROREffect
		{
			int type = rItem.Item.type;

			//To set the effects texture to the item texture
			texture[typeof(T)] = rItem.Texture;
			itemType[typeof(T)] = type;

			//To register this item type in an according rarity entry
			List<int> list = GetItemTypesOfRarity(rItem.Rarity);
			if (!list.Contains(type))
			{
				list.Add(type);
			}
		}

		/// <summary>
		/// Returns the list of item types associated with this rarity
		/// </summary>
		public static List<int> GetItemTypesOfRarity(RORRarity rarity)
		{
			if (itemRarityToItemTypes.TryGetValue(rarity, out List<int> list)) return list;
			else return new List<int>();
		}

		/// <summary>
		/// Returns all RORConsumableItem types
		/// </summary>
		public static List<int> GetAllItemTypes()
		{
			List<int> list = new List<int>();

			foreach (RORRarity rarity in Enum.GetValues(typeof(RORRarity)))
			{
				list.AddRange(GetItemTypesOfRarity(rarity));
			}
			return list;
		}

		/// <summary>
		/// Adds an effect to the list (or increases the stack of an existing effect)
		/// </summary>
		public static ROREffect ApplyEffect<T>(RORPlayer mPlayer) where T : ROREffect
		{
			return ApplyEffect(mPlayer, typeof(T));
		}

		/// <summary>
		/// Adds an effect to the list (or increases the stack of an existing effect)
		/// </summary>
		public static ROREffect ApplyEffect(RORPlayer mPlayer, int id)
		{
			Type effectType = GetEffectOfId(id);
			if (effectType == null)
			{
				//TODO Issue here is that the effects list will be desynced if nothing happens here. But This can only happen if id is invalid (tampered with from outside)
				//RiskOfSlimeRainMod.Instance.Logger.Warn("Effect could not be applied");
			}
			return ApplyEffect(mPlayer, effectType);
		}

		/// <summary>
		/// Adds an effect to the list (or increases the stack of an existing effect)
		/// </summary>
		public static ROREffect ApplyEffect(RORPlayer mPlayer, Type type)
		{
			//First, check if effect exists
			ROREffect existing = GetEffectOfType(mPlayer, type);
			if (existing != null)
			{
				//Effect exists, increase stack
				//CanStack already checked in the hook ran in CanUseItem
				existing.IncreaseStack();
				return existing;
			}
			else
			{
				//Effect doesn't exist, add one
				ROREffect newEffect = ROREffect.NewInstance(mPlayer.Player, type);
				newEffect.OnCreate();
				//By definition of the list order, append
				mPlayer.Effects.Add(newEffect);
				Type[] validInterfaces = GetValidInterfaces(type);
				foreach (var interf in validInterfaces)
				{
					mPlayer.EffectByType[interf].Add(newEffect);
				}

				return newEffect;
			}
		}

		/// <summary>
		/// Populates the dictionary with the right effects
		/// </summary>
		public static void Populate(RORPlayer mPlayer)
		{
			//GeneralHelper.Print("populating " + mPlayer.player.whoAmI);
			List<ROREffect> effects = mPlayer.Effects;
			Clear(mPlayer);
			foreach (var effect in effects)
			{
				Type[] interfaces = GetValidInterfaces(effect.GetType()); //Because this is dynamic
				foreach (var interf in interfaces)
				{
					List<ROREffect> byType = mPlayer.EffectByType[interf];
					//If this interface is one in effectByType, add this effect as a value to this list
					byType.Add(effect);
					byType.Sort();
				}
			}
		}

		//Used in a place where the type is dynamic
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

		/// <summary>
		/// Used to retreive the associated item type of an effect (-1 if item nonexistant)
		/// </summary>
		public static int GetItemTypeOfEffect(ROREffect effect)
		{
			if (itemType.TryGetValue(effect.GetType(), out int value))
			{
				return value;
			}
			return -1;
		}

		/// <summary>
		/// Used to retreive the type of an effect based on its ID
		/// </summary>
		public static Type GetEffectOfId(int id)
		{
			if (id >= 0 && id < indexedEffects.Length) return indexedEffects[id];
			else return null; //needs to be catched
		}

		/// <summary>
		/// Used to retreive ID of an effect
		/// </summary>
		public static int GetIdOfEffect<T>() where T : ROREffect
		{
			return GetIdOfEffect(typeof(T));
		}

		/// <summary>
		/// Used to retreive ID of an effect type
		/// </summary>
		public static int GetIdOfEffect(Type type)
		{
			return Array.IndexOf(indexedEffects, type);
		}

		/// <summary>
		/// Check for validity during loading
		/// </summary>
		public static bool IsValidEffectFullName(string fullName)
		{
			for (int i = 0; i < indexedEffects.Length; i++)
			{
				if (indexedEffects[i].FullName == fullName) return true;
			}
			return false;
		}

		public static List<ROREffect> GetEffectsOf<T>(Player player) where T : IROREffectInterface
		{
			return GetEffectsOf<T>(player.GetRORPlayer());
		}

		public static List<ROREffect> GetEffectsOf<T>(RORPlayer mPlayer) where T : IROREffectInterface
		{
			return mPlayer.EffectByType[typeof(T)];
		}

		/// <summary>
		/// Used to retreive an effect of type T on the player. null if not found
		/// </summary>
		public static T GetEffectOfType<T>(Player player) where T : ROREffect
		{
			return GetEffectOfType<T>(player.GetRORPlayer());
		}

		/// <summary>
		/// Used to retreive an effect of type T on the player. null if not found
		/// </summary>
		public static T GetEffectOfType<T>(RORPlayer mPlayer) where T : ROREffect
		{
			return GetEffectOfType(mPlayer, typeof(T)) as T;
		}

		/// <summary>
		/// Used to retreive an effect of a type on the player. null if not found
		/// </summary>
		public static ROREffect GetEffectOfType(RORPlayer mPlayer, Type type)
		{
			return mPlayer.Effects.FirstOrDefault(e => e.GetType().Equals(type));
		}

		/// <summary>
		/// Used to retreive the index of an effect on the player
		/// </summary>
		public static int GetIndexOfEffect(ROREffect effect)
		{
			return effect.Player.GetRORPlayer().Effects.FindIndex(e => e == effect); //Check same reference
		}

		/// <summary>
		/// Compact way to check if an effect can trigger (but slower, only used in Perform())
		/// </summary>
		public static bool CanDoEffect<T>(ROREffect effect) where T : IROREffectInterface
		{
			if (!effect.Active) return false;
			if (effect.AlwaysProc) return true;

			RORPlayer mPlayer = effect.Player.GetRORPlayer();
			if (mPlayer.CanProc())
			{
				//If ProcTimer is 0
				bool result = effect.Proc();
				if (result)
				{
					//If proced, set timer
					mPlayer.SetProcTimer();
				}
				return result;
			}
			return false;
		}

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

		public static void ModifyHitNPC(Player player, Item item, NPC target, ref NPC.HitModifiers modifiers)
		{
			List<ROREffect> effects = GetEffectsOf<IModifyHit>(player);
			foreach (var effect in effects)
			{
				if (CanDoEffect<IModifyHit>(effect))
				{
					((IModifyHit)effect).ModifyHitNPC(player, item, target, ref modifiers);
				}
			}
		}

		public static void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
		{
			List<ROREffect> effects = GetEffectsOf<IModifyHit>(player);
			foreach (var effect in effects)
			{
				if (CanDoEffect<IModifyHit>(effect))
				{
					((IModifyHit)effect).ModifyHitNPCWithProj(player, proj, target, ref modifiers);
				}
			}
		}

		public static void ModifyWeaponCrit(Player player, Item item, ref float crit)
		{
			List<ROREffect> effects = GetEffectsOf<IModifyWeaponCrit>(player);
			foreach (var effect in effects)
			{
				if (effect.Active)
				{
					((IModifyWeaponCrit)effect).ModifyWeaponCrit(player, item, ref crit);
				}
			}
		}

		public static void ModifyWeaponDamage(Player player, Item item, ref StatModifier damage)
		{
			List<ROREffect> effects = GetEffectsOf<IModifyWeaponDamage>(player);
			foreach (var effect in effects)
			{
				if (effect.Active)
				{
					((IModifyWeaponDamage)effect).ModifyWeaponDamage(player, item, ref damage);
				}
			}
		}

		public static bool FreeDodge(Player player, Player.HurtInfo info)
		{
			List<ROREffect> effects = GetEffectsOf<IFreeDodge>(player);
			foreach (var effect in effects)
			{
				if (CanDoEffect<IFreeDodge>(effect))
				{
					if (((IFreeDodge)effect).FreeDodge(player, info))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool ConsumableDodge(Player player, Player.HurtInfo info)
		{
			List<ROREffect> effects = GetEffectsOf<IConsumableDodge>(player);
			foreach (var effect in effects)
			{
				if (CanDoEffect<IConsumableDodge>(effect))
				{
					if (((IConsumableDodge)effect).ConsumableDodge(player, info))
					{
						return true;
					}
				}
			}
			return false;
		}
		#endregion

		public static List<Effect> GetScreenShaders(Player player)
		{
			List<Effect> shaders = new List<Effect>();
			List<ROREffect> effects = GetEffectsOf<IScreenShader>(player);
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

		public static PlayerDrawLayer ParentLayer => PlayerDrawLayers.FirstVanillaLayer;
		public static PlayerDrawLayer ParentVisibilityLayer => PlayerDrawLayers.ElectrifiedDebuffBack;
	}
}
