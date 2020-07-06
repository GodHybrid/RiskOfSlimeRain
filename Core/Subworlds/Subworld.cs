using RiskOfSlimeRain.Tiles.SubworldTiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public class Subworld
	{
		public readonly string displayName;

		public readonly int width;

		public readonly int height;

		public Subworld(string displayName, int width, int height)
		{
			this.displayName = displayName;
			this.width = width;
			this.height = height;
		}

		public virtual void LoadWorld()
		{
			Main.dayTime = true;
			Main.time = 27000;
		}

		public virtual List<GenPass> Generation()
		{
			return new List<GenPass>();
		}

		public void AddSelf(Dictionary<Type, Subworld> subworlds)
		{
			object result = SubworldManager.subworldLibrary.Call(
					"Register",
					/*Mod mod*/ RiskOfSlimeRainMod.Instance,
					/*string id*/ displayName,
					/*int width*/ width,
					/*int height*/ height,
					/*List<GenPass> tasks*/ Generation(),
					/*Action load*/ (Action)LoadWorld,
					/*Action unload*/ null,
					/*ModWorld modWorld*/ ModContent.GetInstance<RORWorld>()
					);

			if (result != null && result is string s)
			{
				subworlds.Add(GetType(), this);
			}
		}
	}

	public class FirstLevel : Subworld
	{
		public FirstLevel() : base("First Level (Basic)", 360, 600)
		{

		}


	}
}
