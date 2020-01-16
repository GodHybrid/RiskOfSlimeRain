using RiskOfSlimeRain.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Buffs
{
	public class TaserImmobility : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Tased");
			Description.SetDefault("You cannot move or attack.");
			Main.debuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<RORGlobalNPC>().tasered = true;
		}
	}
}
