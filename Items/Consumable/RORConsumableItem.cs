using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public abstract class RORConsumableItem : ModItem
	{
		#region boilerplate
		//static
		public string displayName = string.Empty;
		public string description = string.Empty;

		//non-static
		public string flavorText = string.Empty;
		//TODO change this based on rarity of item?
		public Color flavorTextColor = Color.FloralWhite;

		public bool HasDisplayName => displayName != string.Empty;
		public bool HasFlavorText => flavorText != string.Empty;

		public RORConsumableItem()
		{
			Initialize(); //so the fields get updated (for example when hovering over the item)
		}

		/// <summary>
		/// Called during mod loading, use to set up variables in it
		/// </summary>
		public virtual void Initialize()
		{
			//already declared variables with default values at the top
		}

		/// <summary>
		/// Called whenever the item is used
		/// </summary>
		public abstract void ApplyEffect(RORPlayer mPlayer);

		/// <summary>
		/// Called whenever the Nullifier is used
		/// </summary>
		public abstract void ResetEffect(RORPlayer mPlayer);

		/// <summary>
		/// Called when the item is about to be used
		/// </summary>
		public virtual bool CanUse(RORPlayer mPlayer)
		{
			return true;
		}
		#endregion

		#region tml hooks
		public sealed override bool Autoload(ref string name)
		{
			if (!(this is Nullifier)) Nullifier.resetList.Add(ResetEffect);
			return true;
		}

		public sealed override void SetStaticDefaults()
		{
			if (HasDisplayName)
			{
				DisplayName.SetDefault(displayName);
			}
			Tooltip.SetDefault(description);
		}

		public sealed override bool CanUseItem(Player player)
		{
			return CanUse(player.GetModPlayer<RORPlayer>());
		}

		public sealed override bool UseItem(Player player)
		{
			ApplyEffect(player.GetModPlayer<RORPlayer>());
			return true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (HasFlavorText)
			{
				string color = (flavorTextColor * (Main.mouseTextColor / 255f)).Hex3();

				//flavorText that has \n in it has to be split into single tooltiplines
				string[] lines = flavorText.Split(new char[] { '\n' }, 2, StringSplitOptions.RemoveEmptyEntries);

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
