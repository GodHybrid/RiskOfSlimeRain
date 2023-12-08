using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using RiskOfSlimeRain.Items.Consumable.Boss;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.NPCs.Bosses
{
	//Special health bar because it does not have one by default because no head icon
	public class MagmaWormBossBar : ModBossBar
	{
		private int oldType = -1;

		public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
		{
			return ModContent.Request<Texture2D>(ModContent.GetInstance<BurningWitness>().Texture);
		}

		public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
		{
			if (info.npcIndexToAimAt > -1)
			{
				var npc = Main.npc[info.npcIndexToAimAt];
				if (oldType == -1)
				{
					oldType = npc.type;
				}

				if (oldType > -1 && npc.type != oldType)
				{
					//NPC.Transform on bosses causes the bar to be in a broken state, this prevents it
					return false;
				}
			}

			return base.ModifyInfo(ref info, ref life, ref lifeMax, ref shield, ref shieldMax);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
		{
			drawParams.IconScale = 0.45f;

			return base.PreDraw(spriteBatch, npc, ref drawParams);
		}
	}
}
