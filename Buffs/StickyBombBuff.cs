using RiskOfSlimeRain.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Buffs
{
	public class StickyBombBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Sticky Bomb is on you!");
			Main.debuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<RORGlobalNPC>().stickyBomb = true;
		}
	}
}
