using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Core.Warbanners;
using RiskOfSlimeRain.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	/// <summary>
	/// This effect is special because it only handles the trigger to spawning the banner. Everything else is in RORPlayer and WarbannerManager
	/// </summary>
	public class WarbannerEffect : RORCommonEffect, IOnKill
	{
		//const int Initial = 4;
		//const int Increase = 1;

		public override float Initial => 7f;

		public override float Increase => 1f;

		public int Radius => (int)Formula() * 16;

		public static int WarbannerHealAmount(Player player) => Math.Max(player.statLifeMax2 / 200, 1);

		public int KillCount { get; private set; }

		/// <summary>
		/// Flags the player having this effect to spawn a warbanner when possible
		/// </summary>
		public bool WarbannerReadyToDrop { get; set; } = false;

		public static LocalizedText UIInfoActiveText { get; private set; }
		public static LocalizedText UIInfoReadyText { get; private set; }
		public static LocalizedText UIInfoInvasionText { get; private set; }
		public static LocalizedText UIInfoConfigText { get; private set; }
		public static LocalizedText UIInfoRemovedText { get; private set; }

		public override void SetStaticDefaults()
		{
			UIInfoActiveText ??= GetLocalization("UIInfoActive");
			UIInfoReadyText ??= GetLocalization("UIInfoReady");
			UIInfoInvasionText ??= GetLocalization("UIInfoInvasion");
			UIInfoConfigText ??= GetLocalization("UIInfoConfig");
			UIInfoRemovedText ??= GetLocalization("UIInfoRemoved");
		}

		public override string UIInfo()
		{
			var text = UIInfoText.Format(Math.Max(0, WarbannerManager.KillCountForNextWarbanner - KillCount));
			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				text += UIInfoActiveText.Format(WarbannerManager.warbanners.Count);
			}
			if (WarbannerReadyToDrop)
			{
				text += "\n" + UIInfoReadyText.ToString();
			}
			if (NPCHelper.AnyInvasion())
			{
				text += "\n" + UIInfoInvasionText.ToString();
			}
			text += "\n" + UIInfoRemovedText.ToString();
			text += "\n" + UIInfoConfigText.ToString();
			return text;
		}

		public void OnKillNPCWithItem(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
		{
			IncreaseKillCountAndPrepareWarbannerSpawn(target);
		}

		public void OnKillNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			IncreaseKillCountAndPrepareWarbannerSpawn(target);
		}

		void IncreaseKillCountAndPrepareWarbannerSpawn(NPC target)
		{
			if (NPCHelper.IsWormBodyOrTail(target)) return;
			if (NPCHelper.IsBossPiece(target)) return;
			if (NPCHelper.IsChild(target, out _)) return;
			if (NPCHelper.IsSpawnedFromStatue(target)) return;
			if (NPCHelper.AnyInvasion()) return;
			if (target.type == NPCID.EaterofWorldsHead && !Main.rand.NextBool(10)) return;

			KillCount++;
			if (KillCount >= WarbannerManager.KillCountForNextWarbanner)
			{
				WarbannerReadyToDrop = true;
			}
		}

		public void ResetKillCount()
		{
			KillCount = 0;
		}

		public override void PopulateTag(TagCompound tag)
		{
			tag.Add("KillCount", KillCount);
		}

		public override void PopulateFromTag(TagCompound tag)
		{
			KillCount = tag.GetInt("KillCount");
		}

		protected override void NetSend(BinaryWriter writer)
		{
			writer.Write7BitEncodedInt(KillCount);
		}

		protected override void NetReceive(BinaryReader reader)
		{
			KillCount = reader.Read7BitEncodedInt();
		}

		public override string ToString()
		{
			return base.ToString() + "; KillCount: " + KillCount;
		}
	}
}
