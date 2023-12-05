using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.Subworlds;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Network.NPCs;
using System.IO;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RiskOfSlimeRain
{
	public class RiskOfSlimeRainMod : Mod
	{
		public static RiskOfSlimeRainMod Instance { get; private set; }

		public RiskOfSlimeRainMod()
		{
			Instance = this;
		}

		public override void Load()
		{
			ROREffectManager.Load();
			SpawnedFromStatuePacket.Load();
		}

		public override void PostSetupContent()
		{
			NPCHelper.Load();
			SubworldManager.Load();
			NPCHelper.LogBadModNPCs();
		}

		public override void Unload()
		{
			ROREffectManager.Unload();
			NPCHelper.Unload();
			SubworldManager.Unload();
			PlayerHelper.Unload();
			SpawnedFromStatuePacket.Unload();

			Instance = null;
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			//NetworkPacketLoader.Instance.HandlePacket(reader, whoAmI);
		}
	}
}
