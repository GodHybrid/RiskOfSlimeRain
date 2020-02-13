using Microsoft.Xna.Framework;
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

		private static Dictionary<RORRarity, List<int>> itemRarityToItemTypes;

		//Used to build the dictionary EffectByType on RORPlayer
		private static Type[] validInterfaces;
		//Used to check if an effect procs or not
		private static Type[] interfaceCanProc;
		//Those two need caching cause they are used in ModifyTooltips dynamically for multiline tooltips with color
		private static Dictionary<Type, string> flavorText;
		private static Dictionary<Type, RORRarity> rarity;
		//Reverse assign from the item itself, that the effect then accesses
		private static Dictionary<Type, string> texture;

		public static void Load()
		{
			//Reflection shenanigans
			Type[] types = typeof(ROREffectManager).Assembly.GetTypes();
			List<Type> interfaces = new List<Type>();
			List<Type> canProcs = new List<Type>();
			Dictionary<string, string> loadedTypeNamespaceToName = new Dictionary<string, string>();
			flavorText = new Dictionary<Type, string>();
			rarity = new Dictionary<Type, RORRarity>();
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
					if (!(type.FullName.StartsWith(prefix) && type.FullName.EndsWith(suffix)))
					{
						throw new Exception($"Error loading ROREffect [{type.FullName}], it doesn't start with [{prefix}] and end with [{suffix}]");
					}
					else if (loadedTypeNamespaceToName.ContainsKey(type.Name))
					{
						throw new Exception($"Error loading ROREffect [{type.FullName}], an effect with that same name already exists in [{loadedTypeNamespaceToName[type.Name]}]! Make sure to make effect names unique");
					}

					ROREffect effect = ROREffect.CreateInstanceNoPlayer(type);
					flavorText[type] = effect.FlavorText;
					rarity[type] = effect.Rarity;
					loadedTypeNamespaceToName.Add(type.Name, type.Namespace);
				}
			}
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

		public static RORRarity GetRarity<T>() where T : ROREffect
		{
			return rarity[typeof(T)];
		}

		public static string GetTexture(Type type)
		{
			if (!texture.ContainsKey(type)) return "";
			return texture[type];
		}

		/// <summary>
		/// This is called in the items code to register it in various data structures for further reference
		/// </summary>
		public static void RegisterItem<T>(RORConsumableItem<T> rItem) where T : ROREffect
		{
			//To set the effects texture to the item texture
			texture[typeof(T)] = rItem.Texture;

			//To register this item type in an according rarity entry
			List<int> list = GetItemTypesOfRarity(rItem.Rarity);
			int type = rItem.item.type;
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
		public static void ApplyEffect<T>(RORPlayer mPlayer) where T : ROREffect
		{
			//First, check if effect exists
			ROREffect existing = GetEffectOfType<T>(mPlayer);
			if (existing != null)
			{
				//Effect exists, increase stack
				//CanStack already checked in the hook ran in CanUseItem
				existing.IncreaseStack();
			}
			else
			{
				//Effect doesn't exist, add one
				ROREffect newEffect = ROREffect.NewInstance<T>(mPlayer.player);
				newEffect.OnCreate();
				//By definition of the list order, append
				mPlayer.Effects.Add(newEffect);
				Type[] validInterfaces = GetValidInterfaces(typeof(T));
				foreach (var interf in validInterfaces)
				{
					mPlayer.EffectByType[interf].Add(newEffect);
				}
			}
		}

		/// <summary>
		/// Populates the dictionary with the right effects
		/// </summary>
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
		public static bool CanDoEffect<T>(ROREffect effect) where T : IROREffectInterface
		{
			bool can = interfaceCanProc.Contains(typeof(T));
			return can ? effect.Proccing : effect.Active;
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

		public static void ModifyHitNPC(Player player, Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			List<ROREffect> effects = GetEffectsOf<IModifyHit>(player);
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
			List<ROREffect> effects = GetEffectsOf<IModifyHit>(player);
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
			List<ROREffect> effects = GetEffectsOf<IGetWeaponCrit>(player);
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
			List<ROREffect> effects = GetEffectsOf<IModifyWeaponDamage>(player);
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

		public static readonly PlayerLayer AllInOne = new PlayerLayer("RiskOfSlimeRain", "AllInOne", PlayerLayer.MiscEffectsBack, delegate (PlayerDrawInfo drawInfo)
		{
			if (drawInfo.shadow != 0f)
			{
				return;
			}
			Player dPlayer = drawInfo.drawPlayer;

			List<ROREffect> effects = GetEffectsOf<IPlayerLayer>(dPlayer);
			List<PlayerLayerParams> allParameters = new List<PlayerLayerParams>();
			foreach (var effect in effects)
			{
				if (effect.Active)
				{
					PlayerLayerParams parameters = ((IPlayerLayer)effect).GetPlayerLayerParams(dPlayer);
					if (parameters != null)
					{
						allParameters.Add(parameters);
					}
				}
			}

			foreach (var parameters in allParameters)
			{
				Texture2D tex = parameters.Texture;
				float drawX = (int)dPlayer.Center.X - Main.screenPosition.X;
				float drawY = (int)dPlayer.Center.Y - Main.screenPosition.Y;

				Vector2 off = parameters.Offset;
				SpriteEffects spriteEffects = SpriteEffects.None;

				if (dPlayer.gravDir < 0f)
				{
					off.Y = -off.Y;
					spriteEffects = SpriteEffects.FlipVertically;
				}
				drawY += off.Y + dPlayer.gfxOffY;
				drawX += off.X;

				Color color = parameters.Color ?? Color.White;
				if (!(parameters.IgnoreAlpha ?? false))
				{
					color *= (255 - dPlayer.immuneAlpha) / 255f;
				}

				Rectangle sourceRect = parameters.GetFrame();

				DrawData data = new DrawData(tex, new Vector2(drawX, drawY), sourceRect, color, 0, sourceRect.Size() / 2, parameters.Scale ?? 1f, spriteEffects, 0)
				{
					ignorePlayerRotation = true
				};
				Main.playerDrawData.Add(data);
			}
		});

		public static void DrawPlayerLayers(Player player, List<PlayerLayer> layers)
		{
			layers.Insert(0, AllInOne);
		}

		public static float UseTimeMultiplier(Player player, Item item, ref float multiplier)
		{
			List<ROREffect> effects = GetEffectsOf<IUseTimeMultiplier>(player);
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
			List<ROREffect> effects = GetEffectsOf<IPreHurt>(player);
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
