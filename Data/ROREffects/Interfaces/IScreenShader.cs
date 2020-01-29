using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace RiskOfSlimeRain.Data.ROREffects.Interfaces
{
	public interface IScreenShader : IROREffectInterface
	{
		Effect GetScreenShader(Player player);
	}
}
