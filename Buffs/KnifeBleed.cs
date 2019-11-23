using Terraria;
using Terraria.ModLoader;
using RiskOfSlimeRain.NPCs;

namespace RiskOfSlimeRain.Buffs
{
	class KnifeBleed : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Bleeding");
			Main.debuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<RoRGlobalNPC>().bleeding = true;
		}
	}
}
