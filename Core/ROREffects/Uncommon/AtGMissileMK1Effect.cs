using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class AtGMissileMK1Effect : RORUncommonEffect, IOnHit
	{
		const float initialChance = 0.1f;
		const float damageIncrease = 3f;

		public override string Description => $"On hitting an enemy - {initialChance.ToPercent()} chance of firing a missile that deals {damageIncrease.ToPercent()} damage";

		public override string FlavorText => "Lightweight and attachable to torso for free use of both hands.\nCan store up to 120 heat-seaking missiles, which is just what your men need to fight off the [REDACTED]";

		public override string Name => "AtG Missile Mk. 1";

		public override bool AlwaysProc => true;

		public override float Chance => initialChance;

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			for (int i = 0; i < Stack; i++) 
			{
				if (Proc(Chance))
				{
					SpawnProjectile(player);
					break;
				}
			}
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			for (int i = 0; i < Stack; i++)
			{
				if (Proc(Chance))
				{
					SpawnProjectile(player);
					break;
				}
			}
		}

		void SpawnProjectile(Player player)
		{
			int damage = (int)(damageIncrease * player.GetDamage());
			SoundHelper.PlaySound(SoundID.Item13.SoundId, (int)player.Center.X, (int)player.Center.Y, SoundID.Item13.Style, SoundHelper.FixVolume(2f), 0.4f);
			Vector2 velo = new Vector2(Main.rand.NextFloat(-0.25f, 0.25f), -2f);
			RandomMovementProj.NewProjectile<AtGMissileMK1Proj>(player.Center, velo, damage, 10f);
		}
	}
}
