using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace RiskOfSlimeRain.Effects.Interfaces
{
	public interface IScreenShader : IROREffectInterface
	{
		Effect GetScreenShader(Player player);
	}
}
