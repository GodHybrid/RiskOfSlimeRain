using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Helpers;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Dusts;
using System;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class LeechingSeedEffect : HealingPoolEffect, IOnHit, IPostUpdateEquips
	{
		public override RORRarity Rarity => RORRarity.Uncommon;

		public override int HitCheckMax => 5;

		public override float CurrentHeal => Formula();

		public override float Initial => ServerConfig.Instance.OriginalStats ? 0.4f : 0.2f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 0.2f : 0.1f;

		public override string Description => $"Dealing damage heals you for {Initial} health";

		public override string FlavorText => "These flesh-infesting pods seem to burrow, balloon, and then pop in a few months. Most test patients have... died.\nHowever, before they die, they feel increased health and state of mind!";

		public override string UIInfo()
		{
			return $"Stored heal: {Math.Round(StoredHeals, 2)}. Heal amount: {Math.Round(CurrentHeal, 2)}";
		}

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			HandleAndApplyHeal(player);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			HandleAndApplyHeal(player);
		}

		public void PostUpdateEquips(Player player)
		{
			UpdateHitCheckCount(player);

			if (Config.HiddenVisuals(player)) return;

			if (Main.rand.NextBool(40))
			{
				Vector2 size = new Vector2(8);
				Dust dust = Dust.NewDustDirect(player.Center - size / 2, (int)size.X, (int)size.Y, ModContent.DustType<ColorableDustAlphaFade>(), 0, 0, 0, Color.IndianRed * 0.78f, 1.5f);
				int speed = Main.rand.Next(2, 4);
				dust.customData = new InAndOutData(inEnd: Main.rand.Next(30, 50), outEnd: 200, inSpeed: speed, outSpeed: speed, reduceScale: false);
				dust.velocity.X *= 0f;
				dust.velocity.Y = Main.rand.NextFloat(-0.04f, -0.02f);
				dust.scale += Main.rand.NextFloat(-0.2f, 0.2f);
			}
		}
	}
}
