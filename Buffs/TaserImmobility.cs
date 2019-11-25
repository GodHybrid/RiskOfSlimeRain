using RiskOfSlimeRain.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Buffs
{
	class TaserImmobility : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Tased");
			Description.SetDefault("You cannot move or attack.");
			Main.debuff[Type] = true;
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<RoRGlobalNPC>().tasered = true;
		}
	}
}
