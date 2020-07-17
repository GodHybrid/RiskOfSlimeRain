using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace RiskOfSlimeRain.Core.Subworlds
{
	/// <summary>
	/// This class is used to define the terrain of a subworld. Instances of this class exist only for lookup purposes, they just get used for SubworldLibrary to register
	/// </summary>
	public abstract partial class Subworld
	{
		public readonly string displayName;
		public readonly string subName;

		public readonly int width;

		public readonly int height;

		//The "main material"
		public readonly int terrainType;
		public readonly byte terrainPaint;
		public readonly int terrainWallType;
		public readonly byte terrainWallPaint;
		//The "floor"
		public readonly int topType;
		public readonly byte topPaint;
		public readonly int topWallType;
		public readonly byte topWallPaint;

		/// <summary>
		/// Subworlds have to have a parameterless constructor calling base on this one
		/// </summary>
		public Subworld(string displayName, string subName, int width, int height, int terrainType, byte terrainPaint, int terrainWallType, byte terrainWallPaint, int topType, byte topPaint, int topWallType, byte topWallPaint)
		{
			this.displayName = displayName;
			this.subName = subName;
			this.width = width;
			this.height = height;

			this.terrainType = terrainType;
			this.terrainPaint = terrainPaint;
			this.terrainWallType = terrainWallType;
			this.terrainWallPaint = terrainWallPaint;

			this.topType = topType;
			this.topPaint = topPaint;
			this.topWallType = topWallType;
			this.topPaint = topWallPaint;
		}

		public virtual void LoadWorld()
		{
			Main.dayTime = true;
			Main.bloodMoon = false;
			Main.time = 27000;
			for (int i = 0; i < Main.maxClouds; i++)
			{
				Main.cloud[i] = new Cloud();
			}
		}

		public virtual List<GenPass> Generation()
		{
			return new List<GenPass>();
		}

		/// <summary>
		/// Does the Mod.Call, and if successful, returns its assigned ID, otherwise <see cref="string.Empty"/>
		/// </summary>
		public string RegisterSelf()
		{
			string ret = string.Empty;
			object result = null;

			try
			{
				result = SubworldManager.subworldLibrary.Call(
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
			}
			catch
			{

			}

			if (result != null && result is string id)
			{
				return id;
			}

			return ret;
		}
	}
}
