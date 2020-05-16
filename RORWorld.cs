using RiskOfSlimeRain.Core.ItemSpawning.ChestSpawning;
using RiskOfSlimeRain.Core.ItemSpawning.NPCSpawning;
using RiskOfSlimeRain.Core.Warbanners;
using RiskOfSlimeRain.Network.NPCs;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain
{
	public class RORWorld : ModWorld
	{
		//public static bool rorMode;
		public static bool downedMagmaWorm = false;

		public override void Initialize()
		{
			//rorMode = false;
			downedMagmaWorm = false;
			WarbannerManager.Init();
			ChestManager.Init();
			NPCLootManager.Init();
		}

		public override void NetSend(BinaryWriter writer)
		{
			BitsByte flags = new BitsByte();
			flags[0] = downedMagmaWorm;
			writer.Write(flags);

			NPCLootManager.NetSend(writer, true);
			NPCLootManager.NetSend(writer, false);

			//Warbanner backend is all serverside, so clients don't need to know about that
		}

		public override void NetReceive(BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			downedMagmaWorm = flags[0];

			NPCLootManager.NetReceive(reader, true);
			NPCLootManager.NetReceive(reader, false);
		}

		public override void Load(TagCompound tag)
		{
			downedMagmaWorm = tag.GetBool("downedMagmaWorm");

			WarbannerManager.Load(tag);
			ChestManager.Load(tag);
			NPCLootManager.Load(tag);
		}

		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound();

			tag.Add("downedMagmaWorm", downedMagmaWorm);

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
