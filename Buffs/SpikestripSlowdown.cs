using RiskOfSlimeRain.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Buffs
{
	public class SpikestripSlowdown : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Slowdown");
			Main.debuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<RORGlobalNPC>().slowedBySpikestrip = true;
		}
	}
}
