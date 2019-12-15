using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain.Effects
{
	public abstract class ROREffect : IComparable<ROREffect>
	{
		//Something you can save in a TagCompound, hence why string, not Type
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

		public virtual string Description => string.Empty;

		public virtual string FlavorText => string.Empty;

		public virtual Color FlavorColor => Color.FloralWhite;

		public virtual string UIInfo => string.Empty;

		public virtual string Texture => ROREffectManager.GetTexture(GetType());
		
		/// <summary>
		/// Set this to false if you have a chance/proc based effect. Always override Chance too
		/// </summary>
		public virtual bool AlwaysProc => true;

		public virtual int MaxRecommendedStack => int.MaxValue;

		/// <summary>
		/// The time in ticks it takes for the effect to apply again (0 for always)
		/// </summary>
		public virtual int MaxProcTimer => 0;

		/// <summary>
		/// If your effect is based on chance to take effect, also override AlwaysProc and return false
		/// </summary>
		public virtual float Chance => 1f;

		private bool Proc => Main.rand.NextFloat() < GetProcChance();

		/// <summary>
		/// This is the chance that the effects stuff will be activated with. Applies to all hooks uniformly. 
		/// So if you have one hook that has a chance, but one that doesn't, have Chance be 1f, and do the randomness in the hook itself.
		/// </summary>
		public bool Proccing => Active && (AlwaysProc || Proc);

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

		public bool Capped
		{
			get
			{
				if (AlwaysProc)
				{
					return Stack >= MaxRecommendedStack;
				}
				else
				{
					return GetProcChance() >= 1f;
				}
			}
		}

		//TODO adjust for ror mode
		public string CappedMessage
		{
			get
			{
				if (AlwaysProc)
				{
					return "You reached the recommended stack amount!";
				}
				else
				{
					return "With the currently held weapon, you reached the recommended stack amount!";
				}
			}
		}

		public bool Active => Stack > 0;

		public ROREffect()
		{
			TypeName = GetType().FullName;
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
			if (!AlwaysProc) return true;
			else return UnlockedStack < MaxRecommendedStack;
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
			ROREffect effect = (ROREffect)Activator.CreateInstance(type);
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
		/// Creates an effect of the given type (string version)
		/// </summary>
		public static ROREffect CreateInstance(Player player, string typeName)
		{
			if (typeName == string.Empty) throw new Exception("Something went wrong loading this tag, typeName is empty");
			return CreateInstance(player, typeof(RiskOfSlimeRain).Assembly.GetType(typeName));
		}

		/// <summary>
		/// Creates an effect with values received from multiplayer
		/// </summary>
		public static ROREffect CreateInstanceFromNet(Player player, BinaryReader reader)
		{
			string typeName = reader.ReadString();
			ROREffect effect = CreateInstance(player, typeName);
			//double time = reader.ReadDouble();
			effect.NetReceiveStack(reader);
			effect.NetReceive(reader);
			return effect;
		}

		public override string ToString() => $" {nameof(Stack)}: {Stack} / {UnlockedStack}, {nameof(Name)}: {Name}";

		public float GetProcChance()
		{
			//0.06 for use time 2, 1 for use time 30, 2 for use time 60
			//TODO in ror mode, don't take the useTime of the weapon but instead the base use time
			Item item = Player.HeldItem;
			if (item.damage < 1) return 0f;
			int useTime = item.useTime;
			if (item.melee && item.shoot <= 0) useTime = item.useAnimation;
			float byUseTime = 2 * useTime / 60f;
			byUseTime = Utils.Clamp(byUseTime, 0, 2);
			return byUseTime * Chance;
		}

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

		public static ROREffect Load(Player player, TagCompound tag)
		{
			string typeName = tag.GetString("TypeName");
			ROREffect effect = CreateInstance(player, typeName);
			effect._CreationTime = TimeSpan.FromSeconds(tag.GetDouble("CreationTime"));
			effect.UnlockedStack = tag.GetInt("UnlockedStack");
			effect.Stack = tag.GetInt("Stack");
			effect.PopulateFromTag(tag);
			return effect;
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

		public void Send(BinaryWriter writer)
		{
			writer.Write(TypeName);
			NetSendStack(writer);
			NetSend(writer);
		}

		public virtual void NetSend(BinaryWriter writer)
		{

		}

		public virtual void NetReceive(BinaryReader reader)
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

		public int CompareTo(ROREffect other)
		{
			return _CreationTime.CompareTo(other._CreationTime);
		}
	}
}
