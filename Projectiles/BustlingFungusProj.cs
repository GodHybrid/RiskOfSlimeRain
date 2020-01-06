using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.Projectiles
{
	//ai0 is heal amount
	/// <summary>
	/// When spawned, heals players and town NPCs in TimerMax intervals nearby to the player they spawned on. Despawns as soon as the player moves
	/// </summary>
	public class BustlingFungusProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 8;
		}

		public override void SetDefaults()
		{
			projectile.width = 82;
			projectile.height = 22;
			projectile.friendly = false;
			projectile.hostile = false;
			projectile.penetrate = -1;
			projectile.tileCollide = true;
			projectile.timeLeft = 60;
			drawOriginOffsetY = 2;
		}

		public const int TimerMax = 60;

		public const int RadiusSQ = 30 * 30;

		public int Heal
		{
			get => (int)projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		public int Timer
		{
			get => (int)projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		public bool StoppedAnimating
		{
			get => projectile.localAI[0] == 1;
			set => projectile.localAI[0] = value ? 1f : 0f;
		}

		public bool StartDespawning
		{
			get => projectile.localAI[1] == 1;
			set => projectile.localAI[1] = value ? 1f : 0f;
		}

		public override void AI()
		{
			DoHeal();
			TryDespawn();
			Animate();
			//TODO sounds (click clICK)
		}

		private void TryDespawn()
		{
			RORPlayer mPlayer = projectile.GetOwner().GetRORPlayer();
			if (mPlayer.NoInputTimer == 0)
			{
				StartDespawning = true;
			}

			if (StartDespawning)
			{
				projectile.alpha += 5;
				if (projectile.alpha > 255) projectile.alpha = 255;
			}
			else
			{
				projectile.timeLeft = 60;
			}
		}

		private void Animate()
		{
			projectile.WaterfallAnimation(8);
		}

		private void DoHeal()
		{
			Timer++;
			Player player = projectile.GetOwner();
			if (Timer > TimerMax)
			{
				Timer = 0;
				Main.npc.WhereActive(n => n.townNPC && n.DistanceSQ(player.Center) < RadiusSQ).Do(n => n.HealMe(Heal));
				if (projectile.owner == Main.myPlayer)
				{
					if (Main.netMode == NetmodeID.SinglePlayer) player.HealMe(Heal);
					else Main.player.WhereActive(p => p.DistanceSQ(player.Center) < RadiusSQ).Do(p => p.HealMe(Heal));
				}
			}
		}
	}
}
