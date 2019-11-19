using Terraria;
using Terraria.ModLoader;
using RiskOfSlimeRain.NPCs;

namespace RiskOfSlimeRain.Buffs
{
    class SpikestripSlowdown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Slowdown");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<RoRGlobalNPC>().slowedBySpikestrip = true;
        }
    }
}
