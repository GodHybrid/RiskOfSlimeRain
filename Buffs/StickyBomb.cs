using Terraria;
using Terraria.ModLoader;
using RiskOfSlimeRain.NPCs;

namespace RiskOfSlimeRain.Buffs
{
    class StickyBomb : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Sticky Bomb is on you!");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<RoRGlobalNPC>().stickyBomb = true;
        }
    }
}
