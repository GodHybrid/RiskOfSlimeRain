using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

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

				player.noBuilding = true;
				player.AddBuff(BuffID.NoBuilding, 3);
			}
			else
			{
				SubworldManager.Reset();
			}
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
