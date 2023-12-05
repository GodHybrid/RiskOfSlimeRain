using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Helpers;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Network.Effects;
using RiskOfSlimeRain.Projectiles;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class InfusionEffect : HealingPoolEffect, IOnKill, IResetEffects, IPostUpdateEquips
	{
		public override RORRarity Rarity => RORRarity.Uncommon;

		public override float Initial => ServerConfig.Instance.OriginalStats ? 0.2f : 0.1f;

		public override float Increase => 0.1f; //Unused, no scaling here

		/// <summary>
		/// Amount of permanent max health increase
		/// </summary>
		public int BonusLife { get; private set; }

		private const int cap = 50;

		private int Cap => cap * Math.Max(1, Stack); //Actual scaling by stack

		public override float CurrentHeal => Initial;

		public override int HitCheckMax => 60;

		public override string Description => $"Killing an enemy increases your health permanently by {Initial}, up to {cap}";

		public override string FlavorText => "You can add whatever blood sample you want, as far as I know.\nRemember that sampling from other creatures is a great basis for experimentation!";

		public override string UIInfo()
		{
			return $"Bonus life: {BonusLife}. Cap: {Cap}. Stored heal: {Math.Round(StoredHeals, 2)}";
		}

		public void OnKillNPC(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
		{
			HandleHealAndProjectile(player, target);
		}

		public void OnKillNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			HandleHealAndProjectile(player, target);
		}

		public void ResetEffects(Player player)
		{
			player.statLifeMax2 += Math.Min(BonusLife, Cap);
		}

		public override void PopulateTag(TagCompound tag)
		{
			tag.Add("BonusLife", BonusLife);
		}

		public override void PopulateFromTag(TagCompound tag)
		{
			BonusLife = tag.GetInt("BonusLife");
		}

		protected override void NetSend(BinaryWriter writer)
		{
			writer.Write(BonusLife);
		}

		protected override void NetReceive(BinaryReader reader)
		{
			BonusLife = reader.ReadInt32();
		}

		public void PostUpdateEquips(Player player)
		{
			UpdateHitCheckCount(player);
		}

		private void HandleHealAndProjectile(Player player, NPC target)
		{
			if (BonusLife >= Cap) return;
			if (NPCHelper.IsWormBodyOrTail(target)) return;
			if (NPCHelper.IsBossPiece(target)) return; //No free max health from creepers/probes/bees
			if (NPCHelper.IsSpawnedFromStatue(target)) return;
			if (NPCHelper.AnyInvasion()) return;
			if (target.type == NPCID.EaterofWorldsHead && !Main.rand.NextBool(10)) return;

			HandleStoredHeals();

			int heal = GetHeal();
			if (heal > 0)
			{
				PlayerBonusProj.NewProjectile(GetEntitySource(player), target.Center, new Vector2(player.direction, Main.rand.NextFloat(-1f, -0.7f)) * 8, onCreate: (InfusionProj inf) =>
				{
					inf.HealAmount = heal;
				});
			}
		}

		public void IncreaseBonusLife(int heal)
		{
			BonusLife += heal;
			new ROREffectSyncSinglePacket(this).Send();
		}

		public override string ToString()
		{
			return base.ToString() + ", " + UIInfo();
		}
	}
}
