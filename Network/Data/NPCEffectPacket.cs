using RiskOfSlimeRain.Core.NPCEffects;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Network.Data
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

		private NPCEffect Effect { get; set; }

		public NPCEffectPacket() { }

		public NPCEffectPacket(NPC npc, NPCEffect effect)
		{
			Effect = effect;
			NPCWhoAmI = npc.whoAmI;
			NPCType = npc.type;
			EffectTime = effect.Time;
			EffectType = effect.Type;
		}

		protected override bool PreSend(ModPacket modPacket, int? fromWho = null, int? toWho = null)
		{
			Effect.NetSend(modPacket);
			return base.PreSend(modPacket, fromWho, toWho);
		}

		protected override bool MidReceive(BinaryReader reader, int fromWho)
		{
			if (NPCWhoAmI < 0 || NPCWhoAmI >= Main.maxNPCs) return base.MidReceive(reader, fromWho);
			NPC npc = Main.npc[NPCWhoAmI];
			if (npc.type != NPCType) return base.PostReceive(reader, fromWho);

			Effect = NPCEffectManager.ApplyNPCEffect(EffectType, npc, EffectTime);
			try
			{
				if (Effect == null)
				{
					//Read underflow protection
					Effect = NPCEffect.CreateInstance(npc, NPCEffectManager.GetNPCEffectType(EffectType), EffectTime);
				}
				Effect?.NetReceive(reader);
			}
			catch
			{

			}
			return base.MidReceive(reader, fromWho);
		}
	}
}
