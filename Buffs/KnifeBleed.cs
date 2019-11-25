using RiskOfSlimeRain.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Buffs
{
	public class KnifeBleed : ModBuff
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
