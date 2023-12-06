﻿using RiskOfSlimeRain.Core.ROREffects.Attributes;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Interfaces
{
	/// <summary>
	/// Use this for on-hit only, not for on-kill stuff (use IOnKill instead)
	/// </summary>
	[CanProc]
	public interface IOnHit : IROREffectInterface
	{
		//TODO 1.4.4 re-evaluate this by splitting up OnHitNPC from OnHitNPCWithItem
		void OnHitNPC(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone);

		void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone);
	}
}
