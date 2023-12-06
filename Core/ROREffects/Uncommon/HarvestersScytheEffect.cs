using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Helpers;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using Terraria.Localization;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class HarvestersScytheEffect : HealingPoolEffect, IOnHit, IModifyWeaponCrit, IPostUpdateEquips, IPlayerLayer
	{
		public const float critChance = 0.05f;

		public float CritChance => critChance + (ServerConfig.Instance.OriginalStats ? 0.02f * Math.Max(0, Stack - 1) : 0f);

		public override float Initial => 8;

		public override float Increase => 2;

		public override float CurrentHeal => Formula();

		public override int HitCheckMax => 1; //Minimum, max once per second

		public override LocalizedText Description => base.Description.WithFormatArgs(critChance.ToPercent(), Initial);

		public override string UIInfo()
		{
			string info = $"Heal amount: {Math.Round(CurrentHeal, 2)}";
			if (ServerConfig.Instance.OriginalStats)
			{
				info = $"Crit chance increase: {CritChance.ToPercent()}. " + info;
			}
			return info;
		}

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			return new PlayerLayerParams("Textures/HarvestersScythe", new Vector2(0, -48));
		}

		public void OnHitNPCWithItem(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
		{
			HandleAndApplyHeal(player);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			HandleAndApplyHeal(player);
		}

		public void PostUpdateEquips(Player player)
		{
			UpdateHitCheckCount(player);
		}

		public void ModifyWeaponCrit(Player player, Item item, ref float crit)
		{
			crit += (int)(CritChance * 100);
		}
	}
}
