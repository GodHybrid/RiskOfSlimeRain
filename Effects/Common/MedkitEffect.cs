using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Data;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;

namespace RiskOfSlimeRain.Effects.Common
{
	public class MedkitEffect : RORCommonEffect, IPostUpdateEquips, IPostHurt, IPlayerLayer
	{
		int timer = -1;
		const int amount = 10;
		const int maxTimer = 96;
		const int maxTimerHeal = 66;

		public override string Description => $"Heal for {amount} health {maxTimerHeal / 60d} seconds after receiving damage";

		public override string FlavorText => "Each Medkit should contain bandages, sterile dressings, soap,\nantiseptics, saline, gloves, scissors, aspirin, codeine, and an Epipen";

		public void PostUpdateEquips(Player player)
		{
			if (timer >= 0)
			{
				timer++;
				if (timer == maxTimerHeal && Main.myPlayer == player.whoAmI)
				{
					//TODO test in MP if timer even counts up for other clients
					//because the healeffect number is delayed, to sync it up with the timer
					player.HealMe(Stack * amount);
				}
				if (timer >= maxTimer)
				{
					timer = -1;
				}
			}
		}

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			timer = 0;
		}

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			if (timer >= 0)
			{
				return new PlayerLayerParams("Textures/Medkit", new Vector2(24f, -24f), ignoreAlpha: true, frame: timer / 6, frameCount: 15);
			}
			else
			{
				return null;
			}
		}
	}
}
