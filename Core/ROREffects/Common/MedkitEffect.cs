using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class MedkitEffect : RORCommonEffect, IPostUpdateEquips, IPostHurt, IPlayerLayer
	{
		int timer = -1;
		const int frameCount = 15;
		//const int amount = 10;

		/// <summary>
		/// Moment at which the heal happens. The animation continues for 30 more ticks in MaxTimer
		/// </summary>
		private int MaxTimerHeal => ServerConfig.Instance.OriginalStats ? 66 : 150;

		private int MaxTimer => (int)(MaxTimerHeal * 1.5f);

		private int FrameSpeed => MaxTimer / frameCount;

		public override float Initial => 10f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 10f : 5f;

		public override string Description => $"Heal for {Initial} health {MaxTimerHeal / 60d} seconds after receiving damage";

		public override string FlavorText => "Each Medkit should contain bandages, sterile dressings, soap,\nantiseptics, saline, gloves, scissors, aspirin, codeine, and an Epipen";

		public override string UIInfo()
		{
			return $"Heal amount: {Formula()}";
		}

		public void PostUpdateEquips(Player player)
		{
			if (timer >= 0 && timer < MaxTimerHeal)
			{
				timer++;
				if (timer == MaxTimerHeal && Main.myPlayer == player.whoAmI)
				{
					SoundHelper.PlaySound(SoundID.Splash, (int)player.Center.X, (int)player.Center.Y, 1, 1f, 0.6f);
					//Because the healeffect number is delayed, to sync it up with the timer
					player.HealMe((int)Formula());
				}
				if (timer >= MaxTimer)
				{
					timer = -1;
				}
			}
			else if (timer >= MaxTimerHeal)
			{
				timer += (int)(MaxTimerHeal / frameCount);
				if (timer >= MaxTimer)
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
				return new PlayerLayerParams("Textures/Medkit", new Vector2(24f, -24f), ignoreAlpha: true, frame: timer / FrameSpeed, frameCount: frameCount);
			}
			else
			{
				return null;
			}
		}
	}
}
