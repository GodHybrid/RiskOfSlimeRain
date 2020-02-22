using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Network.Effects;
using RiskOfSlimeRain.Projectiles;
using System.IO;
using Terraria;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class InfusionEffect : RORUncommonEffect, IOnKill, IResetEffects
	{
		public const float initial = 0.5f;
		public const float increase = 0.5f;

		public float BonusLife { get; private set; }

		public float CurrentIncrease => initial + Stack * increase;

		public override string Description => "Killing an enemy increases your health permanently by 1";

		public override string FlavorText => "You can add whatever blood sample you want, as far as I know.\nRemember that sampling from other creatures is a great basis for experimentation!";

		public void OnKillNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player, target);
		}

		public void OnKillNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player, target);
		}

		public void ResetEffects(Player player)
		{
			player.statLifeMax2 += (int)BonusLife;
		}

		public override void PopulateTag(TagCompound tag)
		{
			tag.Add("BonusLife", BonusLife);
		}

		public override void PopulateFromTag(TagCompound tag)
		{
			BonusLife = tag.GetFloat("BonusLife");
		}

		protected override void NetSend(BinaryWriter writer)
		{
			writer.Write(BonusLife);
		}

		protected override void NetReceive(BinaryReader reader)
		{
			BonusLife = reader.ReadSingle();
		}

		private void SpawnProjectile(Player player, NPC target)
		{
			//The projectile reads from the effect of the owner how much health it gives him
			PlayerBonusProj.NewProjectile<InfusionProj>(target.Center, new Vector2(player.direction, -1) * 8);
		}

		public void IncreaseBonusLife()
		{
			BonusLife += CurrentIncrease;
			new ROREffectSyncSinglePacket(this).Send();
		}

		public override string ToString()
		{
			return base.ToString() + " bonus: " + BonusLife;
		}
	}
}
