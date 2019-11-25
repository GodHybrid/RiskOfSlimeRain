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
	}
}
