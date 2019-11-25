using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Buffs
{
	class FungalDefenseMechanism : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Fungal Defense Mechanism");
			Description.SetDefault("The fungi protect you\nSignificant life restoration while stationary\nFriends will regain health when close to you\nAny movement or attack will cease the healing");
			Main.debuff[Type] = true;
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
			canBeCleared = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{

		}
	}
}
