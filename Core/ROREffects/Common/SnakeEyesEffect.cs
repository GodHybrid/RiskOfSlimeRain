using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class SnakeEyesEffect : RORCommonEffect, IPostHurt, IKill, IModifyWeaponCrit, IPostUpdateEquips, IPlayerLayer
	{
		//TODO In multiplayer, another player succeeding at a shrine will remove your Snake Eyes counter. Them failing will also up your Snake Eyes count. 
		//TODO this currently doesnt work like in ror, because no shrines yet
		//const int Initial = 3;
		//const int Increase = 3;

		const int maxIncrease = 6;
		byte failedAttempts = 0;
		bool ready = false;

		public override float Initial => 6f;

		public override float Increase => 3f;

		int CritIncrease => (int)(failedAttempts * Formula());

		public override LocalizedText Description => base.Description.WithFormatArgs(Initial.ToPercent(), maxIncrease);

		public override string UIInfo()
		{
			return UIInfoText.Format(CritIncrease);
		}

		public void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			failedAttempts = 0;
			ready = true;
		}

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			if (failedAttempts > 0) return new PlayerLayerParams("Textures/SnakeEyes", new Vector2(0, -66), frame: failedAttempts - 1, frameCount: 6);
			else return null;
		}

		public void PostUpdateEquips(Player player)
		{
			if (failedAttempts > 0 && player.HasBuff(BuffID.PotionSickness)) failedAttempts = 0;

			if (!ready && player.statLifeMax2 >= player.statLife) ready = true;
		}

		public void PostHurt(Player player, Player.HurtInfo info)
		{
			if (ready && failedAttempts < maxIncrease && player.statLife < player.statLifeMax2 * 0.1)
			{
				failedAttempts++;
				ready = false;
			}
		}

		public void ModifyWeaponCrit(Player player, Item item, ref float crit)
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
