using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.ROREffects.Common;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// When spawned, heals players and town NPCs in TimerMax intervals nearby to the player they spawned on. Despawns as soon as the player moves. ai0 is heal amount
	/// </summary>
	public class BustlingFungusProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 8;
		}

		public override void SetDefaults()
		{
			Projectile.width = Width;
			Projectile.height = Height;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 60;
			Projectile.netImportant = true;
			DrawOriginOffsetY = 4;
		}

		public const int TimerMax = 60;

		public const int Width = 82;

		public const int Height = 24;

		public const int RadiusSQ = 30 * 30;

		public int Heal
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public int Timer
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public bool StoppedAnimating
		{
			get => Projectile.localAI[0] == 1;
			set => Projectile.localAI[0] = value ? 1f : 0f;
		}

		public bool StartDespawning
		{
			get => Projectile.localAI[1] == 1;
			set => Projectile.localAI[1] = value ? 1f : 0f;
		}

		public bool lastAnimating = true;

		public int effectReallyGoneTimer = 0;

		public const int effectReallyGoneTimerMax = 60;

		public override void AI()
		{
			DoHeal();
			TryDespawn();
			AnimateAndSound();
		}

		private void TryDespawn()
		{
			RORPlayer mPlayer = Projectile.GetOwner().GetRORPlayer();
			if (!StartDespawning)
			{
				BustlingFungusEffect effect = ROREffectManager.GetEffectOfType<BustlingFungusEffect>(mPlayer);
				if (effect == null || effect?.Active == false)
				{
					if (Main.netMode == NetmodeID.SinglePlayer)
					{
						StartDespawning = true;
					}
					else
					{
						//In MP, the projectile spawns from the client, but if the client just changed the stack from 0 to 1, it takes a while for the information to arrive. 
						//Not waiting will cause the projectile to despawn immediately
						effectReallyGoneTimer++;
						if (effectReallyGoneTimer > effectReallyGoneTimerMax)
						{
							StartDespawning = true;
						}
					}
				}
				if (mPlayer.NoInputTimer == 0)
				{
					StartDespawning = true;
				}
			}

			if (StartDespawning)
			{
				Projectile.alpha += 5;
				if (Projectile.alpha > 255) Projectile.alpha = 255;
			}
			else
			{
				Projectile.timeLeft = 60;
			}
		}

		private void AnimateAndSound()
		{
			bool animating = Projectile.WaterfallAnimation(6);

			if (Main.myPlayer == Projectile.owner)
			{
				if (animating != lastAnimating)
				{
					Projectile.netUpdate = true;
				}
				lastAnimating = animating;
			}

			if (Projectile.frame == 0 && Projectile.frameCounter == 1)
			{
				//Plant growing
				SoundEngine.PlaySound(SoundID.Item51.WithVolumeScale(SoundHelper.FixVolume(1.1f)).WithPitchOffset(0.6f), Projectile.Center);
			}

			if ((Projectile.frame == 1 || Projectile.frame == 4) && Projectile.frameCounter == 1)
			{
				//Generic weapon swing
				SoundEngine.PlaySound(SoundID.Item19.WithVolumeScale(0.63f).WithPitchOffset(-3.5f), Projectile.Center);
			}
		}

		private void DoHeal()
		{
			Timer++;
			if (Timer > TimerMax && !StartDespawning)
			{
				Timer = 0;

				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC n = Main.npc[i];

					if (n.active && n.townNPC && n.DistanceSQ(Projectile.Center) < RadiusSQ)
					{
						n.HealMe(Heal);
					}
				}

				if (Projectile.owner == Main.myPlayer)
				{
					if (Main.netMode == NetmodeID.SinglePlayer)
					{
						Player player = Projectile.GetOwner();
						if (player.DistanceSQ(Projectile.Center) < RadiusSQ)
						{
							player.HealMe(Heal);
						}
					}
					else
					{
						for (int i = 0; i < Main.maxPlayers; i++)
						{
							Player p = Main.player[i];

							if (p.active && !p.dead && p.DistanceSQ(Projectile.Center) < RadiusSQ)
							{
								p.HealMe(Heal);
							}
						}
					}
				}
			}
		}
	}
}
