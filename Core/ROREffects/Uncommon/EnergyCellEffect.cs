using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	/// <summary>
	/// Effects of this on the player are hardcoded in RORInterfaceLayers for now
	/// </summary>
	public class EnergyCellEffect : RORUncommonEffect, IUseTimeMultiplier, IPostUpdateEquips
	{
		//const float Increase = 0.1f;
		public const int shakeTimerMax = 6; //To and back
		const float virtualMargin = 0.05f;
		public int hpLossBonus = 0;

		public Vector2 shakePosOffset = new Vector2(default(Vector2).X * 40, default(Vector2).Y * 40);

		public Vector2 shakeScaleOffset = new Vector2(default(Vector2).X * 20, default(Vector2).Y * 40);

		public int shakeTimer = 0;

		public bool increment = true;

		public override float Initial => ServerConfig.Instance.OriginalStats ? 0.2f : 0.1f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 0.2f : 0.05f;

		public override int MaxRecommendedStack => ServerConfig.Instance.OriginalStats ? 4 : 6;

		public override bool EnforceMaxStack => true;

		public float attackSpeedBonus => Formula() * 0.25f * hpLossBonus;

		public override string Description => $"Increase attack speed by up to {(Initial + Increase).ToPercent()} based on lost health";

		public override string FlavorText => "Use ONLY in fusion-based vehicles and machinery.\nDo NOT upload to mainframe.";

		public override string UIInfo()
		{
			return $"Maximum attack speed bonus: {Formula().ToPercent()}\nCurrent bonus: {attackSpeedBonus.ToPercent()}";
		}

		public void UseTimeMultiplier(Player player, Item item, ref float multiplier)
		{
			if (item.damage > 0 || item.axe > 0 || item.hammer > 0 || item.pick > 0) multiplier += attackSpeedBonus; //15% is made into 10%, but it still works as 15%
		}

		public void PostUpdateEquips(Player player)
		{
			int maxHP = player.statLifeMax2;
			int currentHP = player.statLife;
			if (currentHP > maxHP / 2) hpLossBonus = 0;
			else
			{
				if (currentHP <= maxHP / 2) hpLossBonus = 1;
				if (currentHP <= maxHP / 3) hpLossBonus = 2;
				if (currentHP <= maxHP / 5) hpLossBonus = 3;
				if (currentHP <= maxHP / 10) hpLossBonus = 4;

				if (currentHP < maxHP / 2)
				{
					if (increment)
					{
						if (shakeTimer < shakeTimerMax)
						{
							shakeTimer++;
							if (shakeTimer >= shakeTimerMax) increment = false;
						}
					}
					else
					{
						if (shakeTimer > 0)
						{
							shakeTimer--;
							if (shakeTimer <= 0)
							{
								shakePosOffset = new Vector2(Main.rand.NextFloat(-1f * hpLossBonus, 1f * hpLossBonus), Main.rand.NextFloat(-1f * hpLossBonus, 1f * hpLossBonus)) * 3 / shakeTimerMax;
								shakeScaleOffset = new Vector2(Main.rand.NextFloat(-0.1f * hpLossBonus, 0.1f * hpLossBonus), Main.rand.NextFloat(-0.1f * hpLossBonus, 0.1f * hpLossBonus)) * 3 / shakeTimerMax;
								increment = true;
							}
						}
					}
				}
			}
		}
	}
}
