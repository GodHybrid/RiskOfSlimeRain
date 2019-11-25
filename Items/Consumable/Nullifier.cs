using System;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ID;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class Nullifier : RORConsumableItem
	{
		public static List<Action<RORPlayer>> resetList = new List<Action<RORPlayer>>();

		public override void Initialize()
		{
			description = "Resets all the upgrades you ever got";
			flavorText = "Gone with the wind...";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			foreach (var reset in resetList)
			{
				reset?.Invoke(mPlayer);
			}
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			//doesn't reset
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			item.rare = ItemRarityID.Red;
			item.UseSound = new LegacySoundStyle(SoundID.Shatter, 0);
		}

		/*
		public override bool UseItem(Player player)
		{
			RORPlayer mPlayer = player.GetModPlayer<RORPlayer>();
			//player.statLifeMax2 -= mPlayer.bitterRootIncrease;
			//mPlayer.bitterRoots = 0;
			//mPlayer.bitterRootIncrease = 0;
			//mPlayer.bustlingFungi = 0;
			//mPlayer.bustlingFungusHeals = 0;
			//mPlayer.fungalRadius = 0;
			//mPlayer.fungalDefense = false;
			//mPlayer.meatNuggets = 0;
			//mPlayer.monsterTeeth = 0;
			//mPlayer.medkits = 0;
			//mPlayer.medkitTimer = -1;
			//mPlayer.mysteriousVials = 0;
			//mPlayer.sproutingEggs = 0;
			//mPlayer.sproutingEggTimer = -1;
			//mPlayer.scarfs = 0;
			//mPlayer.lensMakersGlasses = 0;
			//mPlayer.fireShields = 0;
			//mPlayer.spikestrips = 0;
			//mPlayer.tasers = 0;
			mPlayer.warbanners = 0;
			RORWorld.pos.Clear();
			RORWorld.radius.Clear();
			//mPlayer.savings = 0;
			//mPlayer.piggyBankTimer = -1;
			//mPlayer.paulsGoatHooves = 0;
			//mPlayer.snakeEyesDice = 0;
			//mPlayer.snakeEyesDiceIncrease = 0;
			//mPlayer.snakeEyesDiceReady = false;
			//mPlayer.soldiersSyringes = 0;
			//mPlayer.barbedWires = 0;
			//mPlayer.wireTimer = -1;
			//mPlayer.crowbars = 0;
			//mPlayer.gasCanisters = 0;
			//mPlayer.stompers = 0;
			//mPlayer.mortarTubes = 0;
			//mPlayer.rustyKnives = 0;
			//mPlayer.stickyBombs = 0;
			return true;
		}
		*/
	}
}
