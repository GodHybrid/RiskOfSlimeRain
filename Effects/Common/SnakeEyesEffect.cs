using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Data;
using RiskOfSlimeRain.Effects.Interfaces;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain.Effects.Common
{
	public class SnakeEyesEffect : RORCommonEffect, IPreHurt, IKill, IGetWeaponCrit, IPostUpdateEquips, IPlayerLayer
	{
		//TODO In multiplayer, another player succeeding at a shrine will remove your Snake Eyes counter. Them failing will also up your Snake Eyes count. 
		//TODO this currently doesnt work like in ror, because no shrines yet
		const int initial = 3;
		const int increase = 3;

		const int maxIncrease = 6;
		byte failedAttempts = 0;
		bool ready = false;

		int CritIncrease => failedAttempts * (initial + Stack * increase);

		public override string Description => $"Increases crit chance by 6% for each time you're in peril, up to {maxIncrease} times. Resets upon dying or drinking a potion";

		public override string FlavorText => "You dirty----------er\nYou KNEW I had to win to pay off my debts";

		public void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			failedAttempts = 0;
			ready = true;
		}

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			if (failedAttempts > 0) return new PlayerLayerParams("Textures/SnakeEyes", new Vector2(0, -50), frame: failedAttempts - 1, frameCount: 6);
			else return null;
		}

		public void PostUpdateEquips(Player player)
		{
			if (player.HasBuff(BuffID.PotionSickness)) failedAttempts = 0;
			if (!ready && player.statLifeMax2 == player.statLife) ready = true;
		}

		public bool PreHurt(Player player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (ready && failedAttempts < maxIncrease && player.statLife - damage < player.statLifeMax2 * 0.05)
			{
				failedAttempts++;
				ready = false;
			}
			return true;
		}

		public void GetWeaponCrit(Player player, Item item, ref int crit)
		{
			crit = Math.Min(100, crit + CritIncrease);
		}

		public override void PopulateTag(TagCompound tag)
		{
			tag.Add("failedAttempts", failedAttempts);
		}

		public override void PopulateFromTag(TagCompound tag)
		{
			failedAttempts = tag.GetByte("failedAttempts");
		}

		protected override void NetSend(BinaryWriter writer)
		{
			writer.Write(failedAttempts);
		}

		protected override void NetReceive(BinaryReader reader)
		{
			failedAttempts = reader.ReadByte();
		}
	}
}
