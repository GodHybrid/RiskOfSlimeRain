using RiskOfSlimeRain.Helpers;
using System.IO;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Network
{
	public class PlayerHealPacket : PlayerPacket
	{
		public readonly int healAmount;
		public readonly byte healerWhoAmI;

		public PlayerHealPacket() { }

		public PlayerHealPacket(Player player, int healAmount, Player healer) : base(player)
		{
			this.healAmount = healAmount;
			healerWhoAmI = (byte)healer.whoAmI;
		}

		protected override void PostSend(BinaryWriter writer, Player player)
		{
			writer.Write7BitEncodedInt(healAmount);
			writer.Write((byte)healerWhoAmI);
		}

		protected override void PostReceive(BinaryReader reader, int sender, Player player)
		{
			int heal = reader.Read7BitEncodedInt();
			byte healer = reader.ReadByte();

			//2 common scenarios:
			/*1. local to other
			 * - local player heals self
			 * - healer sends to server
			 * - server receives, sends to clients (except healer)
			 * - clients receive and heal
			 */
			/*2. local to other
			 * - local player heals other
			 * - healer sends to server
			 * - server receives, sends to clients (except healer)
			 * - clients receive and heal
			 */

			player.HealMe(heal, noBroadcast: Main.netMode == NetmodeID.MultiplayerClient, Main.player[healer]);
		}
	}
}
