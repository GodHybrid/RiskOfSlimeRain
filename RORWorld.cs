using RiskOfSlimeRain.Core.ItemSpawning.NPCSpawning;
using RiskOfSlimeRain.Core.ItemSpawning.ChestSpawning;
using RiskOfSlimeRain.Core.Warbanners;
using RiskOfSlimeRain.Network.NPCs;
using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain
{
	public class RORWorld : ModWorld
	{
		//public static bool rorMode;

		public override void Initialize()
		{
			//rorMode = false;
			WarbannerManager.Init();
			ChestManager.Init();
			NPCLootManager.Init();
		}

		public override void NetSend(BinaryWriter writer)
		{
			//BitsByte flags = new BitsByte();
			//flags[0] = rorMode;
			//writer.Write((byte)flags);

			NPCLootManager.NetSend(writer, true);
			NPCLootManager.NetSend(writer, false);

			//Warbanner backend is all serverside, so clients don't need to know about that
		}

		public override void NetReceive(BinaryReader reader)
		{
			//BitsByte flags = reader.ReadByte();
			//rorMode = flags[0];

			NPCLootManager.NetReceive(reader, true);
			NPCLootManager.NetReceive(reader, false);
		}

		public override void Load(TagCompound tag)
		{
			//rorMode = tag.GetBool("rorMode");

			WarbannerManager.Load(tag);
			ChestManager.Load(tag);
			NPCLootManager.Load(tag);
		}

		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound();

			//tag.Add("rorMode", rorMode);

			WarbannerManager.Save(tag);
			ChestManager.Save(tag);
			NPCLootManager.Save(tag);
			return tag;
		}

		public override void PostWorldGen()
		{
			ChestManager.AddItemsToChests();
		}

		public override void PostUpdate()
		{
			WarbannerManager.TrySpawnWarbanners();
			SpawnedFromStatuePacket.SendSpawnedFromStatues();
		}
	}
}
