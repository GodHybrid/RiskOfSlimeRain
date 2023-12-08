using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using RiskOfSlimeRain.Core.EntitySources;
using Terraria.Localization;

namespace RiskOfSlimeRain.Core.ROREffects
{
	/// <summary>
	/// The base class of all effects. Will be saved on the player once applied
	/// </summary>
	//IComparable because we use it in a list to sort
	//Effects are created via the ROREffect.CreateInstance method
	public abstract class ROREffect : IComparable<ROREffect>, ICloneable
	{
		public static readonly string LocalizationCategory = "ROREffects";

		//Something you can save in a TagCompound, hence why string, not Type
		/// <summary>
		/// Shortened identifier name, unique
		/// </summary>
		public string TypeName { get; private set; }

		/// <summary>
		/// The ID of this effect for mp purposes
		/// </summary>
		public int Id => ROREffectManager.GetIdOfEffect(GetType());

		public Player Player { get; private set; }

		private string MakeDefaultDisplayName()
		{
			string name = GetType().Name;
			//if it ends with "Effect", split the name up without the effect. example:
			//"BitterRootEffect" -> "Bitter Root"
			const string effectName = "Effect";
			if (name.EndsWith(effectName)) name = name.Substring(0, name.Length - effectName.Length);
			return Regex.Replace(name, "([A-Z])", " $1").Trim();
		}

		public LocalizedText GetLocalization(string name, Func<string> makeDefaultValue = null) => RiskOfSlimeRainMod.Instance.GetLocalization($"{LocalizationCategory}.{TypeName}.{name}", makeDefaultValue);

		//TypeName contains the rarity separated via "." already so it's handy here for organization
		//Cached, so not dynamic
		/// <summary>
		/// This will be the name of the item responsible for the effect to apply
		/// </summary>
		public virtual LocalizedText DisplayName => GetLocalization("DisplayName", MakeDefaultDisplayName);

		//Cached, so not dynamic
		public virtual LocalizedText Description => GetLocalization("Description", () => string.Empty);

		//Cached, so not dynamic
		public virtual LocalizedText FlavorText => GetLocalization("FlavorText", () => string.Empty);

		public LocalizedText UIInfoText => GetLocalization("UIInfo", () => string.Empty);

		//Cached, so not dynamic
		public virtual RORRarity Rarity => RORRarity.Common;

		//Cause it returns the already pulsating mouse color if rarity is white
		public Color RarityColor => ROREffectManager.GetRarityColor(Rarity);

		public int RarityValue => ROREffectManager.GetRarityValue(Rarity);

		//Cached, so not dynamic
		public virtual string Texture => ROREffectManager.GetTexture(GetType());

		/// <summary>
		/// Set this to false if you have a chance/proc based effect. Always override Chance too.
		/// If AlwaysProc is false, it will check for the weapons use time in combination with Chance.
		/// In the UI, if AlwaysProc is true, it will only highlight it as "recommended limit" if max stack is reached
		/// </summary>
		public virtual bool AlwaysProc => true;

		public virtual int MaxRecommendedStack => int.MaxValue;

		/// <summary>
		/// Variable denoting the initial "value" of the effect when it has 1 stack
		/// </summary>
		public virtual float Initial => 0f;

		/// <summary>
		/// Increase of "value" based on stack. Added to Initial, starting at 2 stacks
		/// </summary>
		public virtual float Increase => 0f;

		/// <summary>
		/// Initial + increase * (stack - 1). Example with 2 stacks (aka one increase): 0.6f = 0.4f + 0.2f * (2 - 1)
		/// </summary>
		public virtual float Formula()
		{
			//0 stack is treated as 1
			return Initial + Increase * Math.Max(0, Stack - 1);
		}

		/// <summary>
		/// If your effect is based on chance to take effect, also override AlwaysProc and return false
		/// </summary>
		public virtual float Chance => 1f;

		public bool Active
		{
			get
			{
				if (Stack == 0) return false;
				return Stack > 0 && !BlockedByBlacklist;
			}
		}

		public bool BlockedByBlacklist
		{
			get
			{
				int item = ItemType;
				if (item > -1)
				{
					return ServerConfig.Instance.Blacklist.Contains(new ItemDefinition(item));
				}
				return false;
			}
		}

		/// <summary>
		/// To sort the effects when loading them
		/// </summary>
		private TimeSpan _CreationTime = TimeSpan.Zero;

		private int _UnlockedStack = 1;

		/// <summary>
		/// This is used for the "Custom stacking" config
		/// </summary>
		public int UnlockedStack
		{
			get => _UnlockedStack;
			private set => _UnlockedStack = Utils.Clamp(value, 0, int.MaxValue);
		}

		private int _Stack = 1;

		/// <summary>
		/// Set this to true if the recommended stack should be enforced instead (blocks item use beyond that)
		/// </summary>
		public virtual bool EnforceMaxStack => false;

		public int Stack
		{
			get => _Stack;
			set => _Stack = Utils.Clamp(value, 0, EnforceMaxStack ? Math.Min(_UnlockedStack, MaxRecommendedStack) : _UnlockedStack);
		}

		public bool FullStack => _Stack == _UnlockedStack;

		private int _NullifierStack = 0;

		public int NullifierStack
		{
			get => _NullifierStack;
			set => _NullifierStack = Utils.Clamp(value, 0, _UnlockedStack);
		}

		public int NullifierCost => NullifierStack * RarityValue; //TODO ror-mode check for reduced price?

		public bool CanBeNullified => NullifierStack > 0;

		public int ItemType => ROREffectManager.GetItemTypeOfEffect(this);

		/// <summary>
		/// Only returns true for things that reached their cap, that don't already do their desired effect
		/// </summary>
		public bool Capped
		{
			get
			{
				if (EnforceMaxStack)
				{
					return Stack >= MaxRecommendedStack;
				}
				else if (!AlwaysProc)
				{
					if (Main.LocalPlayer.HeldItem.damage < 1)
					{
						return Stack >= MaxRecommendedStack;
					}
					else
					{
						return GetProcByUseTime(Player) * Chance >= 1f;
					}
				}
				else
				{
					return false;
				}
			}
		}

		public string CappedMessage
		{
			get
			{
				if (Main.LocalPlayer.HeldItem.damage < 1 || EnforceMaxStack)
				{
					return ReachedRecommendedText.ToString();
				}
				else
				{
					return ReachedRecommendedWithWeaponText.ToString();
				}
			}
		}

		public string BlockedMessage
		{
			get
			{
				string msg = BlockedByConfigText.ToString();
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					msg += BlockedByConfigMPExtraText.ToString();
				}
				return msg;
			}
		}

		public ROREffect()
		{
			string fullName = GetType().FullName;
			int length = fullName.Length;
			if (fullName.EndsWith(ROREffectManager.suffix))
			{
				//Cut off suffix
				string name;
				name = fullName.Substring(0, length - ROREffectManager.suffix.Length);
				if (name.StartsWith(ROREffectManager.prefix))
				{
					//Cut off prefix
					TypeName = name.Substring(ROREffectManager.prefix.Length, name.Length - ROREffectManager.prefix.Length);
				}
			}
			else
			{
				throw new Exception("wtf");
			}
		}

		public virtual void SetStaticDefaults()
		{

		}

		/// <summary>
		/// Use to display additional info when mouseovered in the UI
		/// </summary>
		public virtual string UIInfo() => string.Empty;

		public void IncreaseStack()
		{
			UnlockedStack++;
			Stack++;
		}

		/// <summary>
		/// Updates the stack counts after using the nullifier, returns true if the unlocked stack reaches 0
		/// </summary>
		public bool UpdateStackAfterNullifier()
		{
			UnlockedStack -= NullifierStack;
			Stack -= NullifierStack;
			return UnlockedStack <= 0;
		}

		/// <summary>
		/// Override this if you have player-based conditions rather than current stack
		/// </summary>
		public virtual bool CanUse(Player player)
		{
			return true;
		}

		/// <summary>
		/// Called when the effect is applied for the first time on the player
		/// </summary>
		public void OnCreate()
		{
			//TODO ror mode something?
		}

		public IEntitySource GetEntitySource(Player player, string? context = null)
		{
			return new EntitySource_FromEffect(player, this, context);
		}

		public static LocalizedText ReachedRecommendedText { get; private set; }
		public static LocalizedText ReachedRecommendedWithWeaponText { get; private set; }
		public static LocalizedText BlockedByConfigText { get; private set; }
		public static LocalizedText BlockedByConfigMPExtraText { get; private set; }

		internal static void SetupLocalization()
		{
			var mod = RiskOfSlimeRainMod.Instance;
			string category = $"{LocalizationCategory}.CommonUI.";
			ReachedRecommendedText ??= mod.GetLocalization($"{category}ReachedRecommended");
			ReachedRecommendedWithWeaponText ??= mod.GetLocalization($"{category}ReachedRecommendedWithWeapon");
			BlockedByConfigText ??= mod.GetLocalization($"{category}BlockedByConfig");
			BlockedByConfigMPExtraText ??= mod.GetLocalization($"{category}BlockedByConfigMPExtra");
		}

		/// <summary>
		/// This returns a new effect of the given type with its creation time set accordingly
		/// </summary>
		public static ROREffect NewInstance<T>(Player player) where T : ROREffect
		{
			ROREffect effect = CreateInstance(player, typeof(T));
			effect.SetCreationTime();
			return effect;
		}

		/// <summary>
		/// Creates an effect of the given type
		/// </summary>
		//can't be generic cause it's used dynamically when loading from a tag
		public static ROREffect CreateInstance(Player player, Type type)
		{
			ROREffect effect = CreateInstanceNoPlayer(type);
			effect.SetupPlayer(player);
			return effect;
		}

		/// <summary>
		/// used for probing stats
		/// </summary>
		public static ROREffect CreateInstanceNoPlayer(Type type)
		{
			ROREffect effect = (ROREffect)Activator.CreateInstance(type);
			return effect;
		}

		/// <summary>
		/// Creates an effect of the given type (string version, suffix+prefix)
		/// </summary>
		public static ROREffect CreateInstance(Player player, string typeName)
		{
			string fullName = ROREffectManager.prefix + typeName + ROREffectManager.suffix;
			if (typeName == string.Empty || !ROREffectManager.IsValidEffectFullName(fullName))
			{
				RiskOfSlimeRainMod.Instance.Logger.Warn($"Something went wrong loading this type: ({typeName})");
				return null;
			}
			return CreateInstance(player, typeof(ROREffect).Assembly.GetType(fullName));
		}

		/// <summary>
		/// Creates an effect of the given type (string version, full name, deprecated)
		/// </summary>
		public static ROREffect CreateInstanceFullName(Player player, string fullName)
		{
			if (!ROREffectManager.IsValidEffectFullName(fullName))
			{
				RiskOfSlimeRainMod.Instance.Logger.Warn($"Something went wrong loading this type: ({fullName})");
				return null;
			}
			return CreateInstance(player, typeof(ROREffect).Assembly.GetType(fullName));
		}

		public static float GetProcByUseTime(Player player)
		{
			//0.06 for use time 2, 1 for use time 30, 2 for use time 60
			//TODO in ror mode, don't take the useTime of the weapon but instead the base use time
			//TODO arkhalis/channel weapons
			Item item = player.HeldItem;
			if (item.damage < 1 || item.CountsAsClass(DamageClass.Summon)) return 1f;
			int useTime = item.useTime;
			//fix for melee weapons
			if (item.CountsAsClass(DamageClass.Melee) && item.shoot <= 0) useTime = item.useAnimation;
			float byUseTime = 2 * useTime / 60f;
			byUseTime = Utils.Clamp(byUseTime, 0, 2);
			return byUseTime;
		}

		/// <summary>
		/// Takes the proc by use time into account
		/// </summary>
		public bool Proc(float chance) => Main.rand.NextFloat() < GetProcByUseTime(Player) * chance;

		/// <summary>
		/// Takes the proc by use time into account, uses Chance
		/// </summary>
		public bool Proc() => Proc(Chance);

		private void SetupPlayer(Player player)
		{
			Player = player;
		}

		private void SetCreationTime()
		{
			if (_CreationTime == TimeSpan.Zero && Main.ActivePlayerFileData != null)
			{
				_CreationTime = Main.ActivePlayerFileData.GetPlayTime();
			}
		}

		/// <summary>
		/// Override this if the effect holds variables. Currently unused
		/// </summary>
		protected virtual void CloneValues(ROREffect newEffect)
		{

		}

		#region saving/loading
		public TagCompound Save()
		{
			TagCompound tag = new TagCompound {
				{"TypeName", TypeName },
				{"CreationTime", _CreationTime.TotalSeconds },
				{"UnlockedStack", UnlockedStack },
				{"Stack", Stack }
			};
			PopulateTag(tag);
			return tag;
		}

		public static ROREffect Load(Player player, TagCompound tag, byte version)
		{
			//Newer versions here
			//if (version == 2)
			//{

			//}
			if (version == 1)
			{
				string typeName = tag.GetString("TypeName");
				ROREffect effect = CreateInstance(player, typeName);
				if (effect == null) return null;
				effect._CreationTime = TimeSpan.FromSeconds(tag.GetDouble("CreationTime"));
				effect.UnlockedStack = tag.GetInt("UnlockedStack");
				effect.Stack = tag.GetInt("Stack");
				if (effect.EnforceMaxStack && effect.Stack > effect.MaxRecommendedStack)
				{
					//This is to prevent the player from increasing the stack count in a mode with a higher limit, and then switch it to another mode
					effect.Stack = effect.MaxRecommendedStack;
				}
				effect.PopulateFromTag(tag);
				return effect;
			}
			//Oldest version
			else
			{
				//The one with Effects namespace + full name
				string typeName = tag.GetString("TypeName");
				typeName = typeName.Replace("RiskOfSlimeRain.Effects.", ROREffectManager.prefix);
				ROREffect effect = CreateInstanceFullName(player, typeName);
				if (effect == null) return null;
				effect._CreationTime = TimeSpan.FromSeconds(tag.GetDouble("CreationTime"));
				effect.UnlockedStack = tag.GetInt("UnlockedStack");
				effect.Stack = tag.GetInt("Stack");
				effect.PopulateFromTag(tag);
				return effect;
			}
		}

		/// <summary>
		/// "Save"
		/// </summary>
		public virtual void PopulateTag(TagCompound tag)
		{

		}

		/// <summary>
		/// "Load"
		/// </summary>
		public virtual void PopulateFromTag(TagCompound tag)
		{

		}
		#endregion

		#region networking
		/// <summary>
		/// Creates an effect with values received from multiplayer
		/// </summary>
		public static ROREffect CreateInstanceFromNet(Player player, BinaryReader reader)
		{
			byte id = reader.ReadByte();
			Type effectType = ROREffectManager.GetEffectOfId(id);
			if (effectType == null) return null;
			ROREffect effect = CreateInstance(player, effectType);
			effect.Receive(reader);
			return effect;
		}

		/// <summary>
		/// Same as Send, but also sends the id, to reconstruct in CreateInstanceFromNet
		/// </summary>
		public void SendOnEnter(BinaryWriter writer)
		{
			writer.Write((byte)Id);
			Send(writer);
		}

		public void Send(BinaryWriter writer)
		{
			NetSendStack(writer);
			NetSend(writer);
		}

		public void Receive(BinaryReader reader)
		{
			NetReceiveStack(reader);
			NetReceive(reader);
		}

		/// <summary>
		/// Override this if the effect holds variables
		/// </summary>
		protected virtual void NetSend(BinaryWriter writer)
		{

		}

		/// <summary>
		/// Override this if the effect holds variables
		/// </summary>
		protected virtual void NetReceive(BinaryReader reader)
		{

		}

		public void NetSendStack(BinaryWriter writer)
		{
			writer.Write7BitEncodedInt(UnlockedStack);
			writer.Write7BitEncodedInt(Stack);
		}

		public void NetReceiveStack(BinaryReader reader)
		{
			UnlockedStack = reader.Read7BitEncodedInt();
			Stack = reader.Read7BitEncodedInt();
		}
		#endregion

		#region object overrides and interfaces
		public override string ToString() => $" {nameof(Stack)}: {Stack} / {UnlockedStack}, {nameof(DisplayName)}: {DisplayName}";

		public override bool Equals(object obj)
		{
			if (obj is ROREffect other)
			{
				return TypeName == other.TypeName && Stack == other.Stack && UnlockedStack == other.UnlockedStack;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return new { TypeName, Stack, UnlockedStack }.GetHashCode();
		}

		public int CompareTo(ROREffect other)
		{
			return _CreationTime.CompareTo(other._CreationTime);
		}

		public object Clone()
		{
			ROREffect effect = CreateInstance(Player, TypeName);
			effect._CreationTime = _CreationTime;
			effect.UnlockedStack = UnlockedStack;
			effect.Stack = Stack;
			CloneValues(effect);
			return effect;
		}
		#endregion
	}
}
