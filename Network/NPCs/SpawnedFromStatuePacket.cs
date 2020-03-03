using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.NPCs;
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

		protected override bool PostReceive(BinaryReader reader, int fromWho)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				GeneralHelper.Print("received statue " + Npc);
				Npc.GetGlobalNPC<RORGlobalNPC>().spawnedFromStatue = true;
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
							if (!gNPC.spawnedFromStatue)
							{
								GeneralHelper.Print("spawned from statue got synced " + Main.npc[number]);
								gNPC.spawnedFromStatue = true;
								new SpawnedFromStatuePacket(npc).Send();
							}
						}
					}
					catch
					{

					}
				}
			}
		}
	}
}
