using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class ToxicWormEffect : RORUncommonEffect, IOnHitByNPC, IOnHit, IPostUpdateEquips, IPlayerLayer
	{
		public bool available = true;
		private int frameTimer = -1;
		static private int frameCount = 7;
		static private int frameSpeed = 5;
		static private int maxFrameTimer = frameCount * frameSpeed;

		public int currentOut = 0;

		static public int rechargeTimer = 390;

		public override bool AlwaysProc => true;

		public override float Initial => 1;

		public override float Increase => 1;

		public float Dmg => ServerConfig.Instance.OriginalStats ? 0.5f : 1f;

		public int maxBounces => (int)Formula();

		public override string Description => $"Infect enemies, dealing {Dmg.ToPercent()}% damage/sec. Bounces to other enemies on death.";

		public override string FlavorText => "Seems to exude poison from its glands, so much that the 'worm' can't even contain it in its mouth.\nMay be a strain of stomach parasites, but obviously much larger and deadlier";

		public override string Name => "Toxic Worm";

		public bool Enabled => throw new NotImplementedException();

		public int UpdateOrder => throw new NotImplementedException();

		public override string UIInfo()
		{
			return $"Simultaneous post-host-death targets: {Formula()}";
		}

		public void OnHitByNPC(NPC npc, int damage, bool crit)
		{
			if (available)
			{
				available = false;
				SpawnProjectile(Player, npc);
			}
		}

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			if (available)
			{
				available = false; 
				SpawnProjectile(player, target);
			}
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			return;
		}

		public void SpawnProjectile(Player player, NPC target)
		{
			int damage = (int)(Dmg * player.GetDamage());
			if (target != null)
			{
				StickyProj.NewProjectile<ToxicWormProj>(target, damage: damage);
				currentOut++;
			}
		}

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			if (available)
			{
				frameTimer = frameTimer >= maxFrameTimer ? frameTimer = 0 : ++frameTimer;
				return new PlayerLayerParams("Textures/ToxicWorm", Vector2.Zero, ignoreAlpha: true, scale: 1.5f, frame: frameTimer / frameSpeed, frameCount: frameCount);
			}
			else return null;
		}

		public void PostUpdateEquips(Player player)
		{
			if (currentOut <= 0)
			{
				available = true;
				rechargeTimer = 390;
			}
			else
			{
				if (rechargeTimer <= 0)
				{
					rechargeTimer = 390;
					available = true;
				}
				rechargeTimer--;
			}
		}
	}
}
