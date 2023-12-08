using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Network.Effects
{
	/// <summary>
	/// Sends the Effects list, and other various variables
	/// </summary>
	public class RORPlayerSyncPacket : PlayerPacket
	{
		public record struct Data(int EffectCount, bool NullifierEnabled, bool WarbannerRemoverDropped, bool BurningWitnessDropped)
		{
			public static Data Receive(BinaryReader reader, RORPlayer mPlayer)
			{
				int count = reader.Read7BitEncodedInt();

				BitsByte flags = reader.ReadByte();
				bool nullifierEnabled = flags[0];
				bool warbannerRemoverDropped = flags[1];
				bool burningWitnessDropped = flags[2];

				mPlayer.nullifierEnabled = nullifierEnabled;
				mPlayer.warbannerRemoverDropped = warbannerRemoverDropped;
				mPlayer.burningWitnessDropped = burningWitnessDropped;

				mPlayer.Effects = new();
				for (int i = 0; i < count; i++)
				{
					ROREffect effect = ROREffect.CreateInstanceFromNet(mPlayer.Player, reader);
					if (effect == null)
					{
						RiskOfSlimeRainMod.Instance.Logger.Warn("No effect of the provided type exists, following exception will cause no harm to your game and can be ignored");
						break;
					}
					mPlayer.Effects.Add(effect);
				}
				ROREffectManager.Populate(mPlayer);

				return new Data(count, nullifierEnabled, warbannerRemoverDropped, burningWitnessDropped);
			}

			public void Send(BinaryWriter writer, RORPlayer mPlayer)
			{
				writer.Write7BitEncodedInt(EffectCount);

				BitsByte flags = new BitsByte();
				flags[0] = NullifierEnabled;
				flags[1] = WarbannerRemoverDropped;
				flags[2] = BurningWitnessDropped;
				writer.Write((byte)flags);

				for (int i = 0; i < EffectCount; i++)
				{
					ROREffect effect = mPlayer.Effects[i];
					effect.SendOnEnter(writer);
				}
			}
		}

		public readonly Data data;

		public virtual bool Broadcast => false;

		public RORPlayerSyncPacket() { }

		public RORPlayerSyncPacket(RORPlayer mPlayer) : this(mPlayer.Player, new Data(mPlayer.Effects.Count, mPlayer.nullifierEnabled, mPlayer.warbannerRemoverDropped, mPlayer.burningWitnessDropped))
		{

		}

		public RORPlayerSyncPacket(Player player, Data data) : base(player)
		{
			this.data = data;
		}

		protected override void PostSend(BinaryWriter writer, Player player)
		{
			//GeneralHelper.Print(GetType().Name + " " + (DateTime.Now.Ticks % 1000) + " sending " + player.name + " " + data.EffectCount + " effects");
			data.Send(writer, player.GetRORPlayer());
		}

		protected override void PostReceive(BinaryReader reader, int sender, Player player)
		{
			var data = Data.Receive(reader, player.GetRORPlayer());
			//GeneralHelper.Print(GetType().Name + " " + (DateTime.Now.Ticks % 1000) + " receiving " + player.name + " " + data.EffectCount + " effects");

			if (Broadcast && Main.netMode == NetmodeID.Server)
			{
				new RORPlayerSyncPacket(player, data).Send(from: sender);
			}
		}
	}
}
