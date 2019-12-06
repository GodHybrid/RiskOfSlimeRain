using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public abstract class RORConsumableItem<T> : ModItem where T : ROREffect
	{
		#region stuff
		public string FlavorText => ROREffectManager.GetFlavorText<T>();

		//TODO change this based on rarity of item?
		public Color FlavorTextColor => ROREffectManager.GetFlavorColor<T>();

		public bool HasFlavorText => FlavorText != string.Empty;

		/// <summary>
		/// Called when the item is about to be used. Default return is if the effect can still stack
		/// </summary>
		public virtual bool CanUse(RORPlayer mPlayer)
		{
			ROREffect effect = ROREffect.CreateInstance(typeof(T));
			return effect.CanUse(mPlayer.player);
		}
		#endregion

		#region tml hooks
		public sealed override void SetStaticDefaults()
		{
			ROREffectManager.SetTexture<T>(Texture);

			ROREffect effect = ROREffect.CreateInstance(typeof(T));
			if (effect.Name != string.Empty)
			{
				DisplayName.SetDefault(effect.Name);
			}
			Tooltip.SetDefault(effect.Description);
		}

		public sealed override bool CanUseItem(Player player)
		{
			//if player has the effect already, check on that. If not, check on a fresh one
			ROREffect effect = player.GetModPlayer<RORPlayer>().Effects.FirstOrDefault(e => e.GetType().Equals(typeof(T)));
			if (effect == null)
			{
				effect = ROREffect.CreateInstance(typeof(T));
			}
			return effect.CanUse(player);
		}

		public sealed override bool UseItem(Player player)
		{
			ROREffectManager.ApplyEffect<T>(player.GetModPlayer<RORPlayer>());
			return true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (HasFlavorText)
			{
				string color = (FlavorTextColor * (Main.mouseTextColor / 255f)).Hex3();

				//flavorText that has \n in it has to be split into single tooltiplines
				string[] lines = FlavorText.Split(new char[] { '\n' }, 2, StringSplitOptions.RemoveEmptyEntries);

				for (int i = 0; i < lines.Length; i++)
				{
					string tooltip = "[c/" + color + ":" + lines[i] + "]";
					tooltips.Add(new TooltipLine(mod, Name + i.ToString(), tooltip));
				}
			}

			//when wanting to add more tooltips without flavorTextColor, call base.ModifyTooltips(tooltips) first
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
			item.rare = ItemRarityID.White;

			//when wanting to add more setdefaults, call base.SetDefaults() first
		}
		#endregion
	}
}
