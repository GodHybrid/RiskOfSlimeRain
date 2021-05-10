using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ID;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.ROREffects.Uncommon;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// This projectile, when dealing damage, won't trigger OnHitNPC by design
	/// </summary>
	public class ToxicWormProj : StickyProj
	{
		public const int tickAmount = 11;
		public const int Radius = 32;
		public const int StrikeTimerMax = 30;

		public int TimeLeftDefault => tickAmount * StrikeTimerMax;//+ StrikeTimerMax >> 1;

		/// <summary>
		/// Timer for strikes on only that NPC
		/// </summary>
		public int StrikeTimer
		{
			get => (int)projectile.localAI[0];
			set => projectile.localAI[0] = value;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Toxic Worm");
			Main.projFrames[projectile.type] = 7;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.Size = new Vector2(64, 66);
			projectile.timeLeft = TimeLeftDefault;
		}

		public override void WhileStuck(NPC npc)
		{
			if (Main.myPlayer == projectile.owner)
			{
				StrikeTimer++;
				if (StrikeTimer > StrikeTimerMax)
				{
					StrikeTimer = 0;
					Player player = projectile.GetOwner();
					player.ApplyDamageToNPC(npc, damage, 0f, 0, false);
					if (npc.life <= 0)
					{
						ToxicWormEffect tmp = ROREffectManager.GetEffectOfType<ToxicWormEffect>(player);
						//tmp.currentOut = tmp.currentOut <= 0 ? 0 : tmp.currentOut--;
						tmp.available = true;
						for (int i = 0; i < tmp.maxBounces; i++)
						{
							NPC nextnpc = Main.npc.FirstActiveOrDefault(n => n.CanBeChasedBy() && projectile.DistanceSQ(n.Center) <= (Radius + 16) * (Radius + 16));
							tmp.SpawnProjectile(player, nextnpc);
						}
						this.Kill(0);
						return;
					}
				}
				if (projectile.timeLeft <= 0)
				{
					Player player = projectile.GetOwner();
					ToxicWormEffect tmp = ROREffectManager.GetEffectOfType<ToxicWormEffect>(player);
					tmp.currentOut = tmp.currentOut <= 0 ? 0 : tmp.currentOut--;
				}
			}
		}

		public override void OtherAI()
		{
			projectile.LoopAnimation(5);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
	}
}
