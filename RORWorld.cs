using RiskOfSlimeRain.Core.Warbanners;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain
{
	public class RORWorld : ModWorld
	{
		public static int downedBossCount;

		public override void Initialize()
		{
			downedBossCount = 0;
			WarbannerManager.Init();
		}

		public override void Load(TagCompound tag)
		{
			downedBossCount = tag.GetInt("downedBossCount");
			WarbannerManager.Load(tag);
		}

		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound();

			tag.Add("downedBossCount", downedBossCount);

			WarbannerManager.Save(tag);
			return tag;
		}

		public override void PostUpdate()
		{
			WarbannerManager.TrySpawnWarbanners();
		}
	}
}
