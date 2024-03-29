﻿using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using Terraria.Localization;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	//It doesn't use IGetWeaponCrit because we want crit rate to be dynamic based on item use time (which requires proc chance)
	//additional crit chance won't be shown on the tooltip
	public class LensmakersGlassesEffect : RORCommonEffect, IModifyHit, IPlayerLayer
	{
		public override float Initial => ServerConfig.Instance.OriginalStats ? 0.07f : 0.04f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 0.07f : 0.04f;

		public override int MaxRecommendedStack => ServerConfig.Instance.OriginalStats ? 14 : 25;

		public override LocalizedText Description => base.Description.WithFormatArgs(Initial.ToPercent());

		public override string UIInfo()
		{
			return UIInfoText.Format(Math.Min(Formula(), 1f).ToPercent());
		}

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			return new PlayerLayerParams("Textures/LensMakersGlasses", new Vector2(0, -50));
		}

		public void ModifyHitNPC(Player player, Item item, NPC target, ref NPC.HitModifiers modifiers)
		{
			if (!Proc(Formula())) return;
			modifiers.SetCrit();
		}

		public void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
		{
			if (!Proc(Formula())) return;
			modifiers.SetCrit();
		}
	}
}
