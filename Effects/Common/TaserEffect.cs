using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Buffs;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class TaserEffect : RORCommonEffect, IOnHit
	{
		const int initial = 10;
		const int increase = 5;

		public override string Description => $"{Chance.ToPercent()} chance to snare enemies for {(initial + increase) / 10} seconds";

		public override string FlavorText => "You say you can fix 'em?\nThese tasers are very very faulty";

		public override bool AlwaysProc => false;

		public override float Chance => 0.07f;

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			//TODO visuals
			AddBuff(target);
		}

		//TODO draw manually for now
		public static void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
		{
			if (npc.GetGlobalNPC<RORGlobalNPC>().tasered)
			{
				Vector2 drawCenter = new Vector2(npc.Center.X, npc.Top.Y + npc.gfxOffY - 20) - Main.screenPosition;
				Texture2D texture = ModContent.GetTexture("RiskOfSlimeRain/Textures/Tasered");
				Rectangle destination = Utils.CenteredRectangle(drawCenter, texture.Size());
				destination.Inflate(10, 0);
				spriteBatch.Draw(texture, destination, drawColor);
			}
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			AddBuff(target);
		}

		void AddBuff(NPC target)
		{
			target.AddBuff(ModContent.BuffType<TaserImmobility>(), (initial + increase * Stack) * 6);
		}
	}
}
