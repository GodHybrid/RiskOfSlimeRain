using RiskOfSlimeRain.Core.Warbanners;
using RiskOfSlimeRain.Network.NPCs;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain
{
	public class RORWorld : ModWorld
	{
		public static int downedBossCount;

		public static bool rorMode;

		public static float DropChance => Math.Min(1f, 2f / Math.Max(1, downedBossCount));

		public override void Initialize()
		{
			downedBossCount = 0;
			rorMode = false;
			WarbannerManager.Init();
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(downedBossCount);

			BitsByte flags = new BitsByte();
			flags[0] = rorMode;
			writer.Write((byte)flags);

			//Warbanner backend is all serverside, so clients don't need to know about that
		}

		public override void NetReceive(BinaryReader reader)
		{
			downedBossCount = reader.ReadInt32();

			BitsByte flags = reader.ReadByte();
			rorMode = flags[0];
		}

		public override void Load(TagCompound tag)
		{
			downedBossCount = tag.GetInt("downedBossCount");
			rorMode = tag.GetBool("rorMode");

			WarbannerManager.Load(tag);
		}

		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound();

			tag.Add("downedBossCount", downedBossCount);
			tag.Add("rorMode", rorMode);

			WarbannerManager.Save(tag);
			return tag;
		}

		public override void PostUpdate()
		{
			WarbannerManager.TrySpawnWarbanners();
			SpawnedFromStatuePacket.SendSpawnedFromStatues();
		}
	}
}
