using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using Terraria;

namespace RiskOfSlimeRain.Projectiles
{
	public class PlayerHealthProj : PlayerBonusCircleProj
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Player Health Orb");
		}

		public int HealAmount
		{
			get => (int)projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		public override Color Color => Color.LimeGreen;

		public override int Radius => HealAmount / 10;

		public override void ApplyBonus(Player target)
		{
			target.HealMe(HealAmount);
		}

		public override bool FindTarget(out int targetIndex)
		{
			Player owner = projectile.GetOwner();
			targetIndex = projectile.owner;
			int minHealth = int.MaxValue;
			const float maxRangeSQ = 1080 * 1080 * 2;
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player player = Main.player[i];
				if (!player.active) continue;

				int health = player.statLife;
				if (health < minHealth && player.DistanceSQ(projectile.Center) < maxRangeSQ)
				{
					minHealth = health;
					targetIndex = i;
				}
			}
			if (targetIndex == projectile.owner) return owner.active;
			else return targetIndex != -1;
		}
	}
}
