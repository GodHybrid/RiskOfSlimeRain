using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public class SubworldPlayer : ModPlayer
	{
		public override bool CloneNewInstances => false;

		public override void PostUpdateBuffs()
		{
			if (SubworldManager.AnyActive() ?? false)
			{
				if (SubworldManager.Current == null)
				{
					SubworldManager.Current = new SubworldMonitor();
				}
				SubworldManager.Current.Update();

				//TODO remove when release
				//player.noBuilding = true;
				//player.AddBuff(BuffID.NoBuilding, 3);
			}
			else
			{
				SubworldManager.Reset();
			}
		}

		public override void ModifyDrawLayers(List<PlayerLayer> layers)
		{
			if (Main.gameMenu) return;

			if (SubworldMonitor.HideLayers())
			{
				foreach (var layer in layers)
				{
					layer.visible = false;
				}
			}
		}

		public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			if (Main.gameMenu) return;

			SubworldMonitor.DrawTeleportSequence(Main.spriteBatch, player);
		}

		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (SubworldManager.AnyActive() ?? false)
			{
				if (damageSource.SourceOtherIndex == 0) //Fall damage
				{
					damage /= 2;
				}
			}
			return true;
		}
	}
}
