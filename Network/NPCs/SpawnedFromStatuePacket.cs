using RiskOfSlimeRain.NPCs;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using WebmilioCommons.Networking;

namespace RiskOfSlimeRain.Network.NPCs
{
	public class SpawnedFromStatuePacket : NPCPacketBase
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAllClients;

		public SpawnedFromStatuePacket() : base() { }

		public SpawnedFromStatuePacket(NPC npc) : base(npc) { }

		/// <summary>
		/// Because SpawnedFromStatue is set AFTER SyncNPC, we want to sync our stuff 1 tick later
		/// </summary>
		private static List<NPC> toSendNextTick = null;

		protected override bool PostReceive(BinaryReader reader, int fromWho)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				Npc.SpawnedFromStatue = true;
			}
			return base.PostReceive(reader, fromWho);
		}

		/// <summary>
		/// Detects if an NPC spawned from a statue, then sends this to clients
		/// </summary>
		public static void HijackSendData(int msgType, int number)
		{
			if (Main.netMode == NetmodeID.Server && msgType == MessageID.SyncNPC)
			{
				if (number >= 0 && number < Main.maxNPCs)
				{
					try
					{
						NPC npc = Main.npc[number];
						if (npc.SpawnedFromStatue)
						{
							RORGlobalNPC gNPC = npc.GetGlobalNPC<RORGlobalNPC>();
							if (!gNPC.sentSpawnedFromStatue)
							{
								gNPC.sentSpawnedFromStatue = true;
								toSendNextTick.Add(npc);
							}
						}
					}
					catch
					{

					}
				}
			}
		}

		public static void Load()
		{
			toSendNextTick = new List<NPC>();
		}

		public static void Unload()
		{
			toSendNextTick = null;
		}

		public static void SendSpawnedFromStatues()
		{
			if (Main.netMode != NetmodeID.Server) return;
			foreach (var npc in toSendNextTick)
			{
				new SpawnedFromStatuePacket(npc).Send();
			}
			toSendNextTick.Clear();
		}
	}
}
