using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Effects
{
	/// <summary>
	/// The base class of all effects. Will be saved on the player once applied
	/// </summary>
	//IComparable because we use it in a list to sort
	//INetworkSerializable because we manually have to sync an entire effect sometimes (from webmiliocommons)
	//Effects are created via the ROREffect.CreateInstance method
	public abstract class ROREffect : IComparable<ROREffect>, INetworkSerializable
	{
		//Something you can save in a TagCompound, hence why string, not Type
		/// <summary>
		/// Shortened identifier name, unique
		/// </summary>
		public string TypeName { get; private set; }

		public Player Player { get; private set; }

		/// <summary>
		/// This will be the name of the item responsible for the effect to apply
		/// </summary>
		public virtual string Name
		{
			get
			{
				string name = GetType().Name;
				//if it ends with "Effect", split the name up without the effect. example:
				//"BitterRootEffect" -> "Bitter Root"
				if (name.EndsWith("Effect")) name = GetType().Name.Replace("Effect", "");
				return Regex.Replace(name, "([A-Z])", " $1").Trim();
			}
		}

		//Cached, so not dynamic
		public virtual string Description => string.Empty;

		//Cached, so not dynamic
		public virtual string FlavorText => string.Empty;

		//Cached, so not dynamic
		public virtual int Rarity => ItemRarityID.White;

		//Cause it returns the already pulsating mouse color if rarity is white
		public Color RarityColor => Rarity == ItemRarityID.White ? Color.White : ItemRarity.GetColor(Rarity);

		//Dynamic
		/// <summary>
		/// Use to display additional info when mouseovered in the UI
		/// </summary>
		public virtual string UIInfo => string.Empty;

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
		/// If your effect is based on chance to take effect, also override AlwaysProc and return false
		/// </summary>
		public virtual float Chance => 1f;

		/// <summary>
		/// Check this when a proc-type method will be ran. (Has a chance and CanProc attribute on its method)
		/// </summary>
		public bool Proccing => Active && (AlwaysProc || Proc(Chance));

		public bool Active => Stack > 0;

		/// <summary>
		/// To sort the effect when loading them
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

		public int Stack
		{
			get => _Stack;
			set => _Stack = Utils.Clamp(value, 0, _UnlockedStack);
		}

		public bool FullStack => _Stack == _UnlockedStack;

		/// <summary>
		/// Only returns true for things that reached their cap, that don't already do their desired effect
		/// </summary>
		public bool Capped
		{
			get
			{
				if (!AlwaysProc)
				{
					if (Main.LocalPlayer.HeldItem.damage < 1)
					{
						return Stack >= MaxRecommendedStack;
					}
					else
					{
						return GetProcByUseTime() * Chance >= 1f;
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
				if (Main.LocalPlayer.HeldItem.damage < 1)
				{
					return "You reached the recommended stack amount!";
				}
				else
				{
					return "With the currently held weapon, you reached the recommended stack amount!";
				}
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

		public void IncreaseStack()
		{
			UnlockedStack++;
			Stack++;
		}

		/// <summary>
		/// Override this if you have player-based conditions rather than current stack
		/// </summary>
		public virtual bool CanUse(Player player)
		{
			//if (!AlwaysProc) return true;
			//else return UnlockedStack < MaxRecommendedStack;
			return true;
		}

		/// <summary>
		/// Called when the effect is applied for the first time on the player
		/// </summary>
		public void OnCreate()
		{
			//TODO ror mode something?
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
			if (typeName == string.Empty) throw new Exception("Something went wrong loading this tag, typeName is empty");
			string fullName = ROREffectManager.prefix + typeName + ROREffectManager.suffix;
			return CreateInstance(player, typeof(ROREffect).Assembly.GetType(fullName));
		}

		/// <summary>
		/// Creates an effect of the given type (string version, full name, deprecated)
		/// </summary>
		public static ROREffect CreateInstanceFullName(Player player, string fullName)
		{
			if (fullName == string.Empty) throw new Exception("Something went wrong loading this tag, typeName is empty");
			return CreateInstance(player, typeof(ROREffect).Assembly.GetType(fullName));
		}

		public float GetProcByUseTime()
		{
			//0.06 for use time 2, 1 for use time 30, 2 for use time 60
			//TODO in ror mode, don't take the useTime of the weapon but instead the base use time
			//TODO arkhalis/channel weapons
			Item item = Player.HeldItem;
			if (item.damage < 1) return 1f;
			int useTime = item.useTime;
			//fix for melee weapons
			if (item.melee && item.shoot <= 0) useTime = item.useAnimation;
			float byUseTime = 2 * useTime / 60f;
			byUseTime = Utils.Clamp(byUseTime, 0, 2);
			return byUseTime;
		}

		/// <summary>
		/// Takes the proc by use time into account
		/// </summary>
		public bool Proc(float chance) => Main.rand.NextFloat() < GetProcByUseTime() * chance;

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
			if (version == 2)
			{

			}
			if (version == 1)
			{
				string typeName = tag.GetString("TypeName");
				ROREffect effect = CreateInstance(player, typeName);
				effect._CreationTime = TimeSpan.FromSeconds(tag.GetDouble("CreationTime"));
				effect.UnlockedStack = tag.GetInt("UnlockedStack");
				effect.Stack = tag.GetInt("Stack");
				effect.PopulateFromTag(tag);
				return effect;
			}
			//Oldest version
			else
			{
				string typeName = tag.GetString("TypeName");
				ROREffect effect = CreateInstanceFullName(player, typeName);
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
			string typeName = reader.ReadString();
			ROREffect effect = CreateInstance(player, typeName);
			effect.Receive(reader);
			return effect;
		}

		/// <summary>
		/// Same as Send, but also sends the TypeName, to reconstruct in CreateInstanceFromNet
		/// </summary>
		public void SendOnEnter(BinaryWriter writer)
		{
			writer.Write(TypeName);
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

		protected virtual void NetSend(BinaryWriter writer)
		{

		}

		protected virtual void NetReceive(BinaryReader reader)
		{

		}

		public void NetSendStack(BinaryWriter writer)
		{
			writer.Write(UnlockedStack);
			writer.Write(Stack);
		}

		public void NetReceiveStack(BinaryReader reader)
		{
			UnlockedStack = reader.ReadInt32();
			Stack = reader.ReadInt32();
		}
		#endregion

		#region object overrides and interfaces
		public override string ToString() => $" {nameof(Stack)}: {Stack} / {UnlockedStack}, {nameof(Name)}: {Name}";

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

		public void Send(NetworkPacket networkPacket, ModPacket modPacket)
		{
			Send(modPacket);
		}

		public void Receive(NetworkPacket networkPacket, BinaryReader reader)
		{
			Receive(reader);
		}
		#endregion
	}
}
