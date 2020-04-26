using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.Warbanners;
using RiskOfSlimeRain.Effects;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Tinq;

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
			Main.projFrames[projectile.type] = 5;
		}

		public override void SetDefaults()
		{
			projectile.width = Width;
			projectile.height = Height;
			projectile.friendly = false;
			projectile.hostile = false;
			projectile.penetrate = -1;
			projectile.tileCollide = true;
			projectile.timeLeft = 60;
			projectile.netImportant = true;
			drawOriginOffsetY = 2;
		}

		public override void Kill(int timeLeft)
		{
			Utils.PoofOfSmoke(projectile.Center);
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (Config.Instance.HideWarbannerRadius) return;
			bool iAmLast = false;
			for (int i = Main.maxProjectiles - 1; i >= 0; i--)
			{
				Projectile p = Main.projectile[i];
				if (p.active && p.modProjectile is WarbannerProj)
				{
					if (p.whoAmI == projectile.whoAmI)
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
					if (p.active && p.modProjectile is WarbannerProj w)
					{
						Effect circle = ShaderManager.SetupCircleEffect(p.Center, w.Radius, Color.LightYellow * 0.78f * WarbannerManager.GetWarbannerCircleAlpha());
						if (circle != null)
						{
							ShaderManager.ApplyToScreenOnce(spriteBatch, circle, restore: false);
						}
					}
				}
				ShaderManager.RestoreVanillaSpriteBatchSettings(spriteBatch);
			}
		}

		public int Radius
		{
			get => (int)projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		public bool SpawnedByEffect
		{
			get => projectile.ai[1] == 1;
			set => projectile.ai[1] = value ? 1f : 0f;
		}

		public int SoundTimer
		{
			get => (int)projectile.localAI[1];
			set => projectile.localAI[1] = value;
		}

		public override void AI()
		{
			projectile.timeLeft = 60;
			GiveBuff();
			Animate();
		}

		private void GiveBuff()
		{
			Main.player.WhereActive(p => p.GetRORPlayer().CanReceiveWarbannerBuff && p.DistanceSQ(projectile.Center) < Radius * Radius).Do(delegate (Player p)
			{
				RORPlayer mPlayer = p.GetRORPlayer();
				mPlayer.ActivateWarbanner(projectile.identity);
			});
		}

		private void Animate()
		{
			projectile.WaterfallAnimation(8);

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

		private void PlaySound(LegacySoundStyle sound, float vol, float pitch)
		{
			PlaySound(sound.SoundId, sound.Style, vol, pitch);
		}

		private void PlaySound(int type, float vol, float pitch)
		{
			PlaySound(type, -1, vol, pitch);
		}

		private void PlaySound(int type, int style, float vol, float pitch)
		{
			Main.PlaySound(type, (int)projectile.Center.X, (int)projectile.Center.Y, style, vol, pitch);
		}
	}
}
