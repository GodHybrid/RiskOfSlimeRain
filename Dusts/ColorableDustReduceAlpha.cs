using Terraria;

namespace RiskOfSlimeRain.Dusts
{
	public class ColorableDustReduceAlpha : ColorableDust
	{
		public virtual int ReduceAmount => 10;

		public override bool Update(Dust dust)
		{
			dust.alpha += ReduceAmount;
			if (dust.alpha > 255) dust.active = false;
			return base.Update(dust);
		}
	}
}
