﻿using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class GoldenGunEffect : RORUncommonEffect, IPlayerLayer, IModifyWeaponDamage, IPostUpdateEquips
	{
		public override float Initial => 0.4f;

		public override float Increase => 0.1f;

		public const int defMaxMoney = 1 * Item.platinum;

		private const int frameCount = 9;

		public int MaxMoney => (int)(defMaxMoney * (1 + 0.1f * Math.Max(0, Stack - 1)));

		//Calculated once per second
		public int Money { get; private set; } = -1;

		public int moneyCountTimer = 0;

		private const int moneyCountTimerMax = 60;

		public float Ratio => Math.Min(1f, (float)Money / MaxMoney);

		public float DamageIncrease => Formula() * Ratio;

		public override string Description => $"Deals bonus damage scaling by money in your inventory, up to {Initial.ToPercent()} damage at {defMaxMoney / Item.platinum} platinum";

		public override string FlavorText => "Was this supposed to... intimidate me? I do like its look, however; perhaps I'll set it above my fireplace.";

		public override string UIInfo()
		{
			return $"Damage: {DamageIncrease.ToPercent()}. Max: {Math.Round((float)MaxMoney / Item.platinum, 1)} Platinum";
		}

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			return new PlayerLayerParams("Textures/GoldenGun", new Vector2(-24, -32),
			frame: (int)(Ratio * (frameCount - 1)), frameCount: frameCount);
		}

		public void PostUpdateEquips(Player player)
		{
			moneyCountTimer++;
			if (Money == -1 || moneyCountTimer % moneyCountTimerMax == 0)
			{
				Money = (int)Utils.CoinsCount(out _, player.inventory, new int[]
					 {
						58, //Mouse item
						57, //Ammo slots
						56,
						55,
						54
					 });
			}
		}

		public void ModifyWeaponDamage(Player player, Item item, ref float add, ref float mult, ref float flat)
		{
			add += DamageIncrease;
		}
	}
}
