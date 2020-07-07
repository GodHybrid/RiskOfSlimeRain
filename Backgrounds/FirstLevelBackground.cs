using RiskOfSlimeRain.Core.Subworlds;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Backgrounds
{
	public class FirstLevelBackground : ModSurfaceBgStyle
	{
		public override bool ChooseBgStyle()
		{
			return !Main.gameMenu && (SubworldManager.AnyActive() ?? false);
		}

		// Use this to keep far Backgrounds like the mountains
		public override void ModifyFarFades(float[] fades, float transitionSpeed)
		{
			for (int i = 0; i < fades.Length; i++)
			{
				if (i == Slot)
				{
					fades[i] += transitionSpeed;
					if (fades[i] > 1f)
					{
						fades[i] = 1f;
					}
				}
				else
				{
					fades[i] -= transitionSpeed;
					if (fades[i] < 0f)
					{
						fades[i] = 0f;
					}
				}
			}
		}

		public override int ChooseMiddleTexture()
		{
			return mod.GetBackgroundSlot("Backgrounds/FirstLevelBackgroundMid");
		}
	}
}