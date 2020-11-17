using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using Terraria;
//using RiskOfSlimeRain.Helpers;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	//TODO Make it actually work lol
	public class GuardiansHeartEffect : RORUncommonEffect, IPlayerLayer
	{
		public const float initial = 60;
		public const float increase = 60;
		public const int time = 7;

		public float Shield => initial + increase * Stack;

		public override string Description => $"Gain {initial} SP shields. Recharges in {time} seconds";
		public override string FlavorText => "While living, the subject had advanced muscle growth, cell regeneration, higher agility...\nHis heart seems to still beat independent of the rest of the body.";
		public override string Name => "Guardian's Heart";

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			return new PlayerLayerParams("Textures/GuardiansHeart", new Vector2(38, -36));
		}
	}
}
