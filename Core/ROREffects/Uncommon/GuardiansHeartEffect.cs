using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using Terraria;
//using RiskOfSlimeRain.Helpers;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	//TODO Make it actually work lol
	public class GuardiansHeartEffect : RORUncommonEffect, IPlayerLayer
	{
		public override float Initial => 60;
		public override float Increase => 60;

		public const int time = 7;

		public float Shield => Formula();

		public override string Description => $"Gain {Initial} SP shields. Recharges in {time} seconds";

		public override string FlavorText => "While living, the subject had advanced muscle growth, cell regeneration, higher agility...\nHis heart seems to still beat independent of the rest of the body.";
		
		public override string Name => "Guardian's Heart";

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			return new PlayerLayerParams("Textures/GuardiansHeart", new Vector2(38, -36));
		}
	}
}
