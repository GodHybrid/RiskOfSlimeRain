using System.IO;
using Terraria;
using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Data.NPCEffects
{
	public class NPCEffectPacket : NetworkPacket
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAll;

		//Need to "wrap" variables for the packet to see them
		public int WhoAmI
		{
			get => whoAmI;
			set => whoAmI = value;
		}

		public int Type
		{
			get => type;
			set => type = value;
		}

		public int Duration
		{
			get => duration;
			set => duration = value;
		}

		public string TypeName
		{
			get => typeName;
			set => typeName = value;
		}

		//To identify the NPC fully
		public static int whoAmI;
		public static int type;

		//Effect parameters
		public static int duration;
		public static string typeName = string.Empty;

		public static void SendPacket(NPC npc, NPCEffect effect)
		{
			SendPacket(npc.whoAmI, npc.type, effect.Time, effect.Name);
		}

		public static void SendPacket(int nPCwhoAmI, int npcType, int effectDuration, string effectTypeName)
		{
			whoAmI = nPCwhoAmI;
			type = npcType;
			duration = effectDuration;
			typeName = effectTypeName;
			new NPCEffectPacket().Send();
		}

		protected override bool PostReceive(BinaryReader reader, int fromWho)
		{
			//Do something with the received data, which is now in the variables we wrapped previously
			if (WhoAmI < 0 || WhoAmI >= Main.maxNPCs) return base.PostReceive(reader, fromWho);
			NPC npc = Main.npc[WhoAmI];
			if (npc.type != Type) return base.PostReceive(reader, fromWho);

			NPCEffectManager.ApplyNPCEffect(TypeName, npc, Duration);
			return base.PostReceive(reader, fromWho);
		}
	}
}
