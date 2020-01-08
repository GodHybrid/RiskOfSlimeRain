using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects;
using RiskOfSlimeRain.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	/// <summary>
	/// Base class for all items that simply add an effect on use. Textures for the effects are assigned from here
	/// </summary>
	public abstract class RORConsumableItem<T> : ModItem where T : ROREffect
	{
		#region properties
		public string FlavorText => ROREffectManager.GetFlavorText<T>();

		public int Rarity => ROREffectManager.GetRarity<T>();

		public static Color FlavorColor => new Color(220, 220, 220);

		public bool HasFlavorText => FlavorText != string.Empty;
		#endregion

		#region tml hooks
		public sealed override void SetStaticDefaults()
		{
			ROREffectManager.SetTexture<T>(Texture);

			ROREffect effect = ROREffect.CreateInstanceNoPlayer(typeof(T));
			if (effect.Name != string.Empty)
			{
				DisplayName.SetDefault(effect.Name);
			}
			Tooltip.SetDefault(effect.Description);
		}

		public sealed override bool CanUseItem(Player player)
		{
			//if player has the effect already, check on that. If not, check on a fresh one
			ROREffect effect = ROREffectManager.GetEffectOfType<T>(player.GetRORPlayer());
			if (effect == null)
			{
				effect = ROREffect.CreateInstance(player, typeof(T));
			}
			return effect.CanUse(player);
		}

		public sealed override bool UseItem(Player player)
		{
			//It is important that this runs on clients+server, otherwise big problems happen
			ROREffectManager.ApplyEffect<T>(player.GetRORPlayer());
			return true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (HasFlavorText)
			{
				string color = (FlavorColor * (Main.mouseTextColor / 255f)).Hex3();

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
			item.rare = Rarity;

			//when wanting to add more SetDefaults, call base.SetDefaults() first
		}
		#endregion
	}
}
