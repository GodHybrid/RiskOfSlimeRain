using RiskOfSlimeRain.Helpers;
using System.IO;
using Terraria;

namespace RiskOfSlimeRain.Network
{
	/// <summary>
	/// Sent to the client that got a Warbanner Remover dropped, simply sets the flag of that player to true
	/// </summary>
	public class WarbannerRemoverDroppedPacket : PlayerPacket
	{
		public WarbannerRemoverDroppedPacket() { }

		public WarbannerRemoverDroppedPacket(Player player) : base(player)
		{

		}

		protected override void PostSend(BinaryWriter writer, Player player)
		{

		}

		protected override void PostReceive(BinaryReader reader, int sender, Player player)
		{
			player.GetRORPlayer().warbannerRemoverDropped = true;
		}
	}
}
