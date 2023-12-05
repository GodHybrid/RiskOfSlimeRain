using RiskOfSlimeRain.Core.Subworlds;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Backgrounds
{
	public class FirstLevelModSceneEffect : ModSceneEffect
	{
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<FirstLevelBackground>();

		public override bool IsSceneEffectActive(Player player)
		{
			return !Main.gameMenu && (SubworldManager.IsActive(FirstLevelBasic.id) ?? false);
		}
	}

	public class FirstLevelBackground : ModSurfaceBackgroundStyle
	{
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
			return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Backgrounds/FirstLevelBackgroundMid");
		}
	}
}