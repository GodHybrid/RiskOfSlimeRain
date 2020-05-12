﻿using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public class DebugItem : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.ReaverShark;

		public override void SetDefaults()
		{
			item.maxStack = 1;
			item.width = 34;
			item.height = 38;
			item.rare = 5;
			item.useStyle = 4;
			item.useTime = 30;
			item.useAnimation = 30;
			item.UseSound = SoundID.Item1;
		}

		private void P(object s) => Main.NewText(s.ToString());

		public override bool UseItem(Player player)
		{
			Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
			P($"T: {tile.type}, Fx: {tile.frameX}, Fy: {tile.frameY}");
			P($"S: {tile.slope()}, LT: {tile.liquidType()}, L%: {tile.liquid}");
			P($"C: {tile.color()}");
			P($"A: {tile.active()}, IA: {tile.inActive()}, NA: {tile.nactive()}");
			return true;
		}
	}
}
