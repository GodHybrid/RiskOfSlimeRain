﻿using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.Warbanners;
using System.IO;

namespace RiskOfSlimeRain.Network.Data
{
	public class WarbannerPacket/* : NetworkPacket*/
	{
		public void Send(int toWho = -1, int fromWho = -1) { }
		//public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToServer;

		public int Radius { get; set; }

		public float X { get; set; }

		public float Y { get; set; }

		public WarbannerPacket() { }

		public WarbannerPacket(int radius, Vector2 position)
		{
			Radius = radius;
			X = position.X;
			Y = position.Y;
		}

		//protected override bool PostReceive(BinaryReader reader, int fromWho)
		//{
		//	//Do something with the received data, which is now in the variables
		//	WarbannerManager.AddWarbanner(Radius, X, Y);
		//	return base.PostReceive(reader, fromWho);
		//}
	}
}
