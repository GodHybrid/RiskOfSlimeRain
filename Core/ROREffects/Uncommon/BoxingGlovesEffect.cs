using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class BoxingGlovesEffect : RORUncommonEffect, IModifyHit
	{
		public override float Initial => 0.15f;

		public override float Increase => 0.06f;

		public override LocalizedText Description => base.Description.WithFormatArgs(Initial.ToPercent());

		public override string UIInfo()
		{
			return UIInfoText.Format(RollChance.ToPercent(2));
		}

		public override float Formula()
		{
			if (Stack > 1)
			{
				return Initial + 1f - (float)Math.Pow(1f - Increase, Stack - 1);
			}
			else
			{
				return Initial;
			}
		}

		private float RollChance => Formula();

		private void ModifyKnockback(Player player, ref NPC.HitModifiers modifiers, NPC target)
		{
			if (target.knockBackResist <= 0f)
			{
				//Prevent things that don't like getting knocked back
				return;
			}

			//Custom rolling
			if (!Proc(RollChance))
			{
				return;
			}

			//Apply more knockback the less knockBackResist target has
			float antiKBResist = 1f - Utils.Clamp(target.knockBackResist, 0f, 1f);
			modifiers.Knockback.Flat += 6f + 8f * antiKBResist;

			if (Config.HiddenVisuals(player)) return;

			for (int i = 0; i < 14; i++)
			{
				Dust dust = Dust.NewDustDirect(target.position, target.width, target.height, DustID.Smoke, 0f, 0f, 100, Color.MintCream, 1f);
				dust.velocity = new Vector2(0, -1f);
				if (Main.rand.NextBool(2))
				{
					dust.scale = 0.5f;
				}
			}
		}

		public void ModifyHitNPC(Player player, Item item, NPC target, ref NPC.HitModifiers modifiers)
		{
			ModifyKnockback(player, ref modifiers, target);
		}

		public void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
		{
			ModifyKnockback(player, ref modifiers, target);
		}
	}
}
