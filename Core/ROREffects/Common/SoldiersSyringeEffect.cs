using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	/// <summary>
	/// Effects of this on the player are hardcoded in RORInterfaceLayers for now
	/// </summary>
	public class SoldiersSyringeEffect : RORCommonEffect, IPostUpdateEquips
	{
		//const float Increase = 0.1f;
		public const int shakeTimerMax = 6; //To and back
		const float virtualMargin = 0.05f;

		public Vector2 shakePosOffset = default(Vector2);

		public Vector2 shakeScaleOffset = default(Vector2);

		public int shakeTimer = 0;

		public bool increment = true;

		public override float Initial => ServerConfig.Instance.OriginalStats ? 0.1f : 0.05f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 0.1f : 0.05f;

		public override int MaxRecommendedStack => ServerConfig.Instance.OriginalStats ? 13 : 20;

		public override bool EnforceMaxStack => true;

		public override LocalizedText Description => base.Description.WithFormatArgs((Initial + virtualMargin).ToPercent());

		public override string UIInfo()
		{
			return $"Attack speed increase: {(Initial + virtualMargin + (Increase + virtualMargin) * Math.Max(0, Stack - 1)).ToPercent()}";
		}

		public void PostUpdateEquips(Player player)
		{
			player.GetAttackSpeed(DamageClass.Generic) += Formula(); //15% is made into 10%, but it still works as 15%

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
						shakePosOffset = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)) * 3 / shakeTimerMax;
						shakeScaleOffset = new Vector2(Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(-0.1f, 0.1f)) * 3 / shakeTimerMax;
						increment = true;
					}
				}
			}
		}
	}
}
