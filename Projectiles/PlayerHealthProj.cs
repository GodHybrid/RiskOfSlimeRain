using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using Terraria;

namespace RiskOfSlimeRain.Projectiles
{
	public class PlayerHealthProj : PlayerBonusCircleProj
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Player Health Orb");
		}

		public int HealAmount
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public override Color Color => Color.LimeGreen;

		public override int Radius => HealAmount / 10;

		public override void ApplyBonus(Player target)
		{
			target.HealMe(HealAmount);
		}

		public override bool FindTarget(out int targetIndex)
		{
			Player owner = Projectile.GetOwner();
			targetIndex = Projectile.owner;
			int minHealth = int.MaxValue;
			const float maxRangeSQ = 1080 * 1080 * 2;
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player player = Main.player[i];
				if (!player.active || player.dead) continue;

				int health = player.statLife;
				bool maxHealth = player.statLife == player.statLifeMax2;
				if (!maxHealth && health < minHealth && player.DistanceSQ(Projectile.Center) < maxRangeSQ)
				{
					minHealth = health;
					targetIndex = i;
				}
			}
			if (targetIndex == Projectile.owner) return owner.active;
			else return targetIndex != -1;
		}
	}
}
