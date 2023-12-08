using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.Warbanners;
using RiskOfSlimeRain.Effects;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	//ai0 contains the radius
	//ai1 contains if the banner is spawned the first time to play the sound
	public class WarbannerProj : ModProjectile
	{
		public const int Width = 32;

		public const int Height = 50;

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 5;
		}

		public override void SetDefaults()
		{
			Projectile.width = Width;
			Projectile.height = Height;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 60;
			Projectile.netImportant = true;
			DrawOriginOffsetY = 2;
		}

		public override void OnKill(int timeLeft)
		{
			Utils.PoofOfSmoke(Projectile.Center);
		}

		public override void PostDraw(Color lightColor)
		{
			if (Config.Instance.HideWarbannerRadius) return;
			bool iAmLast = false;
			for (int i = Main.maxProjectiles - 1; i >= 0; i--)
			{
				Projectile p = Main.projectile[i];
				if (p.active && p.ModProjectile is WarbannerProj)
				{
					if (p.whoAmI == Projectile.whoAmI)
					{
						iAmLast = true;
					}
					break;
				}
			}

			if (iAmLast)
			{
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile p = Main.projectile[i];
					if (p.active && p.ModProjectile is WarbannerProj w)
					{
						Effect circle = ShaderManager.SetupCircleEffect(p.Center, w.Radius, Color.LightYellow * 0.78f * WarbannerManager.GetWarbannerCircleAlpha());
						if (circle != null)
						{
							ShaderManager.ApplyToScreenOnce(Main.spriteBatch, circle, restore: false);
						}
					}
				}
				ShaderManager.RestoreVanillaSpriteBatchSettings(Main.spriteBatch);
			}
		}

		public int Radius
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public bool SpawnedByEffect
		{
			get => Projectile.ai[1] == 1;
			set => Projectile.ai[1] = value ? 1f : 0f;
		}

		public int SoundTimer
		{
			get => (int)Projectile.localAI[1];
			set => Projectile.localAI[1] = value;
		}

		public override void AI()
		{
			Projectile.timeLeft = 60;
			GiveBuff();
			Animate();
		}

		private void GiveBuff()
		{
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player p = Main.player[i];

				if (p.active && !p.dead)
				{
					RORPlayer mPlayer = p.GetRORPlayer();
					if (mPlayer.CanReceiveWarbannerBuff && p.DistanceSQ(Projectile.Center) < Radius * Radius)
					{
						mPlayer.ActivateWarbanner(Projectile.identity);
					}
				}
			}
		}

		private void Animate()
		{
			Projectile.WaterfallAnimation(8);

			if (SpawnedByEffect)
			{
				int start = 20;
				int end = start + 136;

				if (SoundTimer == start) PlaySound(SoundID.Item71, 0.7f, 0f);
				//if (SoundTimer == start + 5) PlaySound(SoundID.Tink, 0.7f, 0f);
				if (SoundTimer == start + 5) PlaySound(SoundID.Item52, 0.5f, 0f);

				if (SoundTimer == start + 37) PlaySound(SoundID.Item71, 0.7f, 0f);
				//if (SoundTimer == start + 42) PlaySound(SoundID.Tink, 0.7f, 0f);
				if (SoundTimer == start + 42) PlaySound(SoundID.Item52, 0.5f, 0f);

				if (SoundTimer == start + 77) PlaySound(SoundID.Item71, 0.7f, 0f);
				//if (SoundTimer == start + 82) PlaySound(SoundID.Tink, 0.7f, 0f);
				if (SoundTimer == start + 82) PlaySound(SoundID.Item52, 0.5f, 0f);

				if (SoundTimer == start + 96) PlaySound(SoundID.Item71, 0.7f, 0f);
				//if (SoundTimer == start + 101) PlaySound(SoundID.Tink, 0.7f, 0f);
				if (SoundTimer == start + 101) PlaySound(SoundID.Item52, 0.5f, 0f);

				if (SoundTimer == start + 131) PlaySound(SoundID.Item71, 0.7f, 0f);
				//if (SoundTimer == start + 136) PlaySound(SoundID.Tink, 0.7f, 0f);
				if (SoundTimer >= end)
				{
					SpawnedByEffect = false;
					PlaySound(SoundID.Item52, 0.5f, 0f);
				}

				SoundTimer++;
			}
		}

		private void PlaySound(SoundStyle sound, float vol, float pitch)
		{
			SoundEngine.PlaySound(sound.WithVolumeScale(vol).WithPitchOffset(pitch), Projectile.Center);
		}
	}
}
