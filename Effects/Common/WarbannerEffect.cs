﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Data.Warbanners;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	/// <summary>
	/// This effect is special because it only handles the trigger to spawning the banner. Everything else is in RORPlayer and WarbannerManager
	/// </summary>
	public class WarbannerEffect : ROREffect, IOnHit
	{
		const int initial = 4;
		const int increase = 1;

		public override string Description => "Chance to drop an empowering banner when killing an enemy";

		public override string FlavorText => "Very very valuable\nDon't drop it; it's worth more than you";

		//Chance is handled in WarbannerManager
		public override string UIInfo => $"Chance on a banner: {WarbannerManager.WarbannerChance.ToPercent(3)}. Active banners: {WarbannerManager.warbanners.Count}";

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			if (target.life <= 0) PassStatsIntoWarbanner(player);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			if (target.life <= 0) PassStatsIntoWarbanner(player);
		}

		void PassStatsIntoWarbanner(Player player)
		{
			WarbannerManager.TryAddWarbanner((initial + increase * Stack) * 16, player.Center);
		}

		public static readonly PlayerLayer WarbannerLayer = new PlayerLayer("RiskOfSlimeRain", "Warbanner", PlayerLayer.MiscEffectsBack, delegate (PlayerDrawInfo drawInfo)
		{
			if (drawInfo.shadow != 0f)
			{
				return;
			}
			Player player = drawInfo.drawPlayer;

			Texture2D tex = ModContent.GetTexture("RiskOfSlimeRain/Textures/Warbanner");
			float drawX = (int)player.Center.X - Main.screenPosition.X;
			float drawY = (int)player.Top.Y + player.gfxOffY - Main.screenPosition.Y;

			drawY -= 40;
			DrawData data = new DrawData(tex, new Vector2(drawX, drawY), null, Color.White * ((255 - player.immuneAlpha) / 255f), 0, tex.Size() / 2, 1f, SpriteEffects.None, 0);
			Main.playerDrawData.Add(data);
		});
	}
}
