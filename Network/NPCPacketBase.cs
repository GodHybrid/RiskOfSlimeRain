using System.IO;
using Terraria;

namespace RiskOfSlimeRain.Network
{
	/// <summary>
	/// NetworkPacket that is tied to an NPC
	/// </summary>
	public abstract class NPCPacketBase/* : NetworkPacket*/
	{
		public void Send(int toWho = -1, int fromWho = -1) { }
		//To identify the NPC fully
		public int NPCWhoAmI { get; set; }

		public int NPCType { get; set; }

		protected NPC Npc => NotValidNPC ? null : Main.npc[NPCWhoAmI];

		private bool NotValidNPC => NPCWhoAmI < 0 || NPCWhoAmI >= Main.maxNPCs;

		public NPCPacketBase() { }

		public NPCPacketBase(NPC npc)
		{
			NPCWhoAmI = npc.whoAmI;
			NPCType = npc.type;
		}

		protected virtual bool NewMidReceive(BinaryReader reader, int fromWho)
		{
			return true;
		}
		/*
		protected sealed override bool MidReceive(BinaryReader reader, int fromWho)
		{
			if (NotValidNPC) return base.MidReceive(reader, fromWho);
			if (Npc.type != NPCType) return base.PostReceive(reader, fromWho);

			bool ret = base.MidReceive(reader, fromWho) && NewMidReceive(reader, fromWho);

			return ret;
		}*/
	}
}
