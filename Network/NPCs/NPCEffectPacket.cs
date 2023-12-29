using RiskOfSlimeRain.Core.NPCEffects;
using System.IO;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Network.NPCs
{
	public class NPCEffectPacket : NPCPacket
	{
		//Effect parameters
		public readonly sbyte effectType;
		public readonly int effectTime;

		private readonly NPCEffect effect;

		public NPCEffectPacket() { }

		public NPCEffectPacket(NPC npc, NPCEffect effect) : base(npc)
		{
			this.effect = effect;
			effectTime = effect.Time;
			effectType = effect.Type;
		}

		protected override void PostSend(BinaryWriter writer, NPC npc)
		{
			writer.Write7BitEncodedInt(effectTime);
			writer.Write((sbyte)effectType);
			effect.NetSend(writer);
		}

		protected override void PostReceive(BinaryReader reader, int sender, NPC npc)
		{
			var effectTime = reader.Read7BitEncodedInt();
			var effectType = reader.ReadSByte();
			var effect = NPCEffectManager.ApplyNPCEffect(effectType, npc, effectTime);
			try
			{
				if (effect == null)
				{
					RiskOfSlimeRainMod.Instance.Logger.Warn("Effect could not be applied, following exception will cause no harm to your game and can be ignored");
					return;
				}
				effect.NetReceive(reader);
			}
			catch
			{

			}

			if (Main.netMode == NetmodeID.Server)
			{
				new NPCEffectPacket(npc, effect).Send(from: sender);
			}
		}
	}
}
