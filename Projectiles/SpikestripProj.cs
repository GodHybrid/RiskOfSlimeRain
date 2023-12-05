using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.NPCEffects;
using RiskOfSlimeRain.Core.NPCEffects.Common;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// ai0 is used to set timeLeft
	/// </summary>
	public class SpikestripProj : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 34;
			Projectile.height = 4;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			//projectile.tileCollide = true;
			Projectile.timeLeft = 300;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.velocity = Vector2.Zero;
			return false;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			//so it sticks to platforms
			fallThrough = false;
			return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
		}

		public override bool? CanCutTiles()
		{
			return false;
		}

		public int Duration
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void AI()
		{
			if (Projectile.localAI[0] == 0f)
			{
				Projectile.localAI[0] = 1f;
				Projectile.timeLeft = Duration;
			}

			Projectile.velocity.Y += 0.5f;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC n = Main.npc[i];

				if (n.active && !n.boss && n.CanBeChasedBy() && n.Hitbox.Intersects(Projectile.Hitbox))
				{
					if (NPCHelper.IsBossPiece(n)) continue;
					if (n.type == NPCID.WallofFlesh || n.type == NPCID.WallofFleshEye) continue;
					if (NPCHelper.IsWormBodyOrTail(n)) continue;
					NPCEffectManager.ApplyNPCEffect<SpikestripNPCEffect>(n, 60);
				}
			}
		}
	}
}
