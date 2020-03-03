using RiskOfSlimeRain.Core.NPCEffects;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using WebmilioCommons.Networking;

namespace RiskOfSlimeRain.Network.Data
{
	public class NPCEffectPacket : NPCPacketBase
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAll;

		//Effect parameters
		public int EffectTime { get; set; }

		public sbyte EffectType { get; set; }

		private NPCEffect Effect { get; set; }

		public NPCEffectPacket() : base() { }

		public NPCEffectPacket(NPC npc, NPCEffect effect) : base(npc)
		{
			Effect = effect;
			EffectTime = effect.Time;
			EffectType = effect.Type;
		}

		protected override bool PreSend(ModPacket modPacket, int? fromWho = null, int? toWho = null)
		{
			if (Effect == null)
			{
				RiskOfSlimeRainMod.Instance.Logger.Warn("NPCEffect that is about to be sent is null, expect exceptions that won't impact the game");
			}
			Effect?.NetSend(modPacket);
			return base.PreSend(modPacket, fromWho, toWho);
		}

		protected override bool NewMidReceive(BinaryReader reader, int fromWho)
		{
			if (Npc == null)
			{
				RiskOfSlimeRainMod.Instance.Logger.Warn("NPC received is null, expect exceptions that won't impact the game");
				base.NewMidReceive(reader, fromWho); //Whatever happened, something broke, fuck read underflow anyways
			}
			Effect = NPCEffectManager.ApplyNPCEffect(EffectType, Npc, EffectTime);
			try
			{
				if (Effect == null)
				{
					//Read underflow protection
					Effect = NPCEffect.CreateInstance(Npc, NPCEffectManager.GetNPCEffectType(EffectType), EffectTime);
				}
				Effect?.NetReceive(reader);
			}
			catch
			{

			}
			return base.NewMidReceive(reader, fromWho);
		}
	}
}
