using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Interfaces
{
	/// <summary>
	/// Use in conjunction with a buff on a GlobalNPC
	/// </summary>
	public interface IPostDrawNPC : IROREffectInterface
	{
		void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor);
	}
}
