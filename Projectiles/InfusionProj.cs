using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.ROREffects.Uncommon;
using RiskOfSlimeRain.Dusts;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Network;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// Uses ai1 for heal amount, ai0 and localAI0
	/// </summary>
	public class InfusionProj : PlayerBonusCircleProj
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Infusion Orb");
		}

		public override float SlowDownFactor => 0.97f;

		public override int StartHomingTimerMax => 30;

		public override float MaxHomingSpeed => 14f;

		public override Color Color => Color.DarkRed;

		public int HealAmount
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public override void ApplyBonus(Player target)
		{
			InfusionEffect effect = ROREffectManager.GetEffectOfType<InfusionEffect>(target);
			if (effect != null)
			{
				effect.IncreaseBonusLife(HealAmount);
				CombatTextPacket.NewText(target.getRect(), CombatText.DamagedHostileCrit, HealAmount, false, false);
			}
		}

		public override bool FindTarget(out int targetIndex)
		{
			targetIndex = Projectile.owner;
			Player player = Projectile.GetOwner();
			return player.active && !player.dead;
		}

		public override void OtherAI()
		{
			if (Main.rand.NextFloat() < Projectile.velocity.Length() / MaxHomingSpeed - 0.1f)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ColorableDustAlphaFade>(), 0, 0, 0, Color.IndianRed * 0.78f, 1.5f);
				int speed = Main.rand.Next(2, 4);
				dust.customData = new InAndOutData(inEnd: Main.rand.Next(30, 50), outEnd: 200, inSpeed: speed, outSpeed: speed, reduceScale: false);
				dust.velocity.X *= 0f;
				dust.velocity.Y = Main.rand.NextFloat(-0.04f, -0.02f);
			}
		}
	}
}
