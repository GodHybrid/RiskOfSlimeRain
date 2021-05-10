using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Tinq;
using RiskOfSlimeRain.Core.ROREffects.Uncommon;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// ai0 damage
	/// </summary>
	public class WillOTheWispProj : InstantExplosion, IExcludeOnHit
	{
		public byte addRadiusX = 50;
		public byte addRadiusY = 30;

		public int Damage
		{
			get => (int)projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.Size = new Vector2(106, 184);
			projectile.timeLeft = 26;
			drawOriginOffsetY = -projectile.height / 3;
		}

		public override void AI()
		{
			Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<WillOTheWispExplosionProj>(), 0, 0, Main.myPlayer);
			Main.PlaySound(SoundID.Trackable, (int)projectile.Center.X, (int)projectile.Center.Y, 166, 0.8f, 0.6f);
			projectile.Kill();
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void Kill(int timeLeft)
		{
			Rectangle explosionArea = projectile.Hitbox;
			explosionArea.Inflate(addRadiusX / 2, addRadiusY / 2);
			ChargefieldGeneratorEffect tmp = Core.ROREffects.ROREffectManager.GetEffectOfType<ChargefieldGeneratorEffect>(projectile.GetOwner());
			Main.npc.WhereActive(n => n.CanBeChasedBy() && n.Hitbox.Intersects(explosionArea)).Do(n =>
			{
				n.StrikeNPC(Damage, 0, 0);
				if (tmp != null && n.life <= 0) tmp.IncreaseRadiusStage();
			});
			Main.PlaySound(SoundID.DD2_ExplosiveTrapExplode?.WithVolume(0.8f), projectile.Center);
		}

	}
}
