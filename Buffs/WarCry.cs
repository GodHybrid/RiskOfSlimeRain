using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace RiskOfSlimeRain.Buffs
{
	class WarCry : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("War Cry");
			Description.SetDefault("AAAAAAAAAAAARRRRRRRRRRRGHHHHH!!!");
			Main.debuff[Type] = false;
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.meleeDamage *= 1.04f;
			player.minionDamage *= 1.04f;
			player.magicDamage *= 1.04f;
			player.rangedDamage *= 1.04f;
			player.moveSpeed *= 1.3f;
			player.maxRunSpeed *= 1.3f;
			player.meleeSpeed *= 1.04f;
			player.pickSpeed *= 1.04f;
			
			player.GetModPlayer<RORPlayer>().affectedWarbanner = true;
		}
	}
}
