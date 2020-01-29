using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Data.Warbanners;
using RiskOfSlimeRain.Effects;
using RiskOfSlimeRain.Helpers;
using System;
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
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 5;
		}

		public override void SetDefaults()
		{
			projectile.width = 32;
			projectile.height = 50;
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

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Effect circle = ShaderManager.SetupCircleEffect(projectile.Center, Radius, Color.LightYellow * 0.78f * WarbannerManager.GetWarbannerCircleAlpha());
			if (circle != null)
			{
				ShaderManager.ApplyToScreen(spriteBatch, circle);
			}

			return true;
		}

		public int Radius
		{
			get => (int)projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		public bool SpawnedByEffect => projectile.ai[1] == 1;

		public bool StoppedAnimating
		{
			get => projectile.localAI[0] == 1;
			set => projectile.localAI[0] = value ? 1f : 0f;
		}

		public int SoundTimer
		{
			get => (int)projectile.localAI[1];
			set => projectile.localAI[1] = value;
		}

		public const int TimerMax = 60;

		public int Timer { get; set; } = 0;

		public override void AI()
		{
			projectile.timeLeft = 60;
			GiveBuff();
			Animate();
		}

		private void GiveBuff()
		{
			//Timer isn't synced but here it just loops all the time so it shouldn't matter
			Timer++;
			if (Timer > TimerMax)
			{
				Timer = 0;
				Main.player.WhereActive(p => p.DistanceSQ(projectile.Center) < Radius * Radius).Do(delegate (Player p)
				{
					if (Main.netMode != NetmodeID.Server && p.whoAmI == Main.myPlayer)
					{
						int heal = Math.Max(p.statLifeMax2 / 100, 1);
						p.HealMe(heal);
					}
					p.GetRORPlayer().ActivateWarbanner();
				});
			}
		}

		private void Animate()
		{
			projectile.WaterfallAnimation(8);

			if (SpawnedByEffect)
			{
				int start = 20;
				int end = start + 136;
				if (SoundTimer > end) return;

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
				if (SoundTimer == end) PlaySound(SoundID.Item52, 0.5f, 0f);

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
