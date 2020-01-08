using RiskOfSlimeRain.Effects.Interfaces;
using System.IO;
using Terraria;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain.Effects.Uncommon
{
	//TODO, not implemented yet, the item to receive this effect is uncommented
	public class InfusionEffect : RORUncommonEffect, IOnHit, IResetEffects
	{
		public float bonusLife = 0f;

		public override string Description => "Killing an enemy increases your health permanently by 1";

		public override string FlavorText => "You can add whatever blood sample you want, as far as I know.\nRemember that sampling from other creatures is a great basis for experimentation!";

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player, target);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player, target);
		}

		public void ResetEffects(Player player)
		{
			player.statLifeMax2 += (int)bonusLife;
		}

		public override void PopulateTag(TagCompound tag)
		{
			tag.Add("bonusLife", bonusLife);
		}

		public override void PopulateFromTag(TagCompound tag)
		{
			bonusLife = tag.GetFloat("bonusLife");
		}

		protected override void NetSend(BinaryWriter writer)
		{
			writer.Write(bonusLife);
		}

		protected override void NetReceive(BinaryReader reader)
		{
			bonusLife = reader.ReadSingle();
		}

		void SpawnProjectile(Player player, NPC target)
		{
			if (target.life <= 0)
			{
				//Spawn projectile here via Projectile.NewProjectile, and pass the life increase * Stack as ai0 or ai1
			}
		}
	}
}
