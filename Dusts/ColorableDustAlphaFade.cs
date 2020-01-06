using Terraria;

namespace RiskOfSlimeRain.Dusts
{
	/// <summary>
	/// When spawning it, assign dust.customData to it if you want to change the default of 5 (int)
	/// </summary>
	public class ColorableDustAlphaFade : ColorableDust
	{
		public override bool Update(Dust dust)
		{
			int ReduceAmount = 5;
			if (dust.customData is int) ReduceAmount = (int)dust.customData;
			dust.alpha += ReduceAmount;
			if (dust.alpha > 255) dust.active = false;
			return base.Update(dust);
		}
	}
}
