using RiskOfSlimeRain.Helpers;
using System.IO;
using Terraria;
using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Data.NPCEffects
{
	public class NPCEffectPacket : NetworkPacket
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAll;

		//To identify the NPC fully
		public int NPCWhoAmI { get; set; }

		public int NPCType { get; set; }

		//Effect parameters
		public int EffectTime { get; set; }

		public sbyte EffectType { get; set; }

		public static void SendPacket(NPC npc, NPCEffect effect)
		{
			SendPacket(npc.whoAmI, npc.type, effect.Time, effect.Type);
		}

		public static void SendPacket(int nWhoAmI, int nType, int eTime, sbyte eType)
		{
			new NPCEffectPacket()
			{
				NPCWhoAmI = nWhoAmI,
				NPCType = nType,
				EffectTime = eTime,
				EffectType = eType
			}.Send();
		}

		protected override bool PostReceive(BinaryReader reader, int fromWho)
		{
			//Do something with the received data, which is now in the variables we wrapped previously
			if (NPCWhoAmI < 0 || NPCWhoAmI >= Main.maxNPCs) return base.PostReceive(reader, fromWho);
			NPC npc = Main.npc[NPCWhoAmI];
			if (npc.type != NPCType) return base.PostReceive(reader, fromWho);

			NPCEffectManager.ApplyNPCEffect(EffectType, npc, EffectTime);
			return base.PostReceive(reader, fromWho);
		}
	}
}
