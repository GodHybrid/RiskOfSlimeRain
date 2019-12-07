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
		private string TypeName { get; set; }

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

		public virtual string Texture => ROREffectManager.GetTexture(GetType());

		public virtual int MaxStack => int.MaxValue;

		public virtual float Chance => 1f;

		/// <summary>
		/// This is the chance that the effects stuff will be activated with. Applies to all hooks uniformly. 
		/// So if you have one hook that has a chance, but one that doesn't, have Chance be 1f, and do the randomness in the hook itself.
		/// </summary>
		public bool Proccing => Active && Main.rand.NextFloat() < Chance;

		private TimeSpan _CreationTime = TimeSpan.Zero;

		private int _UnlockedStack = 1;

		/// <summary>
		/// This is used for the "Custom stacking" config
		/// </summary>
		public int UnlockedStack
		{
			get => _UnlockedStack;
			set => _UnlockedStack = Utils.Clamp(value, 0, MaxStack);
		}

		private int _Stack = 1;

		public int Stack
		{
			get => _Stack;
			set => _Stack = Utils.Clamp(value, 0, _UnlockedStack);
		}

		public bool FullStack => _Stack == _UnlockedStack;

		public bool CanStack => _UnlockedStack < MaxStack;

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
			return CanStack;
		}

		/// <summary>
		/// This returns a new effect of the given type with its creation time set accordingly
		/// </summary>
		public static ROREffect NewInstance<T>() where T : ROREffect
		{
			ROREffect effect = CreateInstance(typeof(T));
			effect.SetCreationTime();
			return effect;
		}

		/// <summary>
		/// Creates an effect of the given type
		/// </summary>
		//can't be generic cause it's used dynamically when loading from a tag
		public static ROREffect CreateInstance(Type type)
		{
			ROREffect effect = (ROREffect)Activator.CreateInstance(type);
			return effect;
		}

		/// <summary>
		/// Creates an effect of the given type (string version)
		/// </summary>
		public static ROREffect CreateInstance(string typeName)
		{
			if (typeName == string.Empty) throw new Exception("Something went wrong loading this tag, typeName is empty");
			return CreateInstance(typeof(RiskOfSlimeRain).Assembly.GetType(typeName));
		}

		/// <summary>
		/// Creates an effect with values received from multiplayer
		/// </summary>
		public static ROREffect CreateInstanceFromNet(BinaryReader reader)
		{
			string typeName = reader.ReadString();
			ROREffect effect = CreateInstance(typeName);
			//double time = reader.ReadDouble();
			int unlockedStack = reader.ReadInt32();
			int stack = reader.ReadInt32();
			effect.UnlockedStack = unlockedStack;
			effect.Stack = stack;
			effect.NetRecieve(reader);
			return effect;
		}

		public override string ToString() => $" {nameof(Stack)}: {Stack} / {UnlockedStack}, {nameof(Name)}: {Name}";

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

		public static ROREffect Load(TagCompound tag)
		{
			string typeName = tag.GetString("TypeName");
			ROREffect effect = CreateInstance(typeName);
			effect._CreationTime = TimeSpan.FromSeconds(tag.GetDouble("CreationTime"));
			effect.UnlockedStack = tag.GetInt("UnlockedStack");
			effect.Stack = tag.GetInt("Stack");
			effect.PopulateFromTag(tag);
			return effect;
		}

		public virtual void PopulateTag(TagCompound tag)
		{

		}

		public virtual void PopulateFromTag(TagCompound tag)
		{

		}

		public void Send(BinaryWriter writer)
		{
			writer.Write(TypeName);
			writer.Write(UnlockedStack);
			writer.Write(Stack);
			NetSend(writer);
		}

		public virtual void NetSend(BinaryWriter writer)
		{

		}

		public virtual void NetRecieve(BinaryReader reader)
		{

		}

		public int CompareTo(ROREffect other)
		{
			return _CreationTime.CompareTo(other._CreationTime);
		}
	}
}
