using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Network.Effects;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
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

		public RORRarity Rarity => ROREffectManager.GetRarity<T>();

		public static Color FlavorColor => new Color(220, 220, 220);

		public bool HasFlavorText => FlavorText != string.Empty;

		[CloneByReference]
		private ROREffect _effect;
		public ROREffect Effect => _effect ??= ROREffect.CreateInstanceNoPlayer(typeof(T));
		#endregion

		#region tml hooks
		public override LocalizedText DisplayName => Effect.DisplayName;

		public override LocalizedText Tooltip => Effect.Description;

		public static LocalizedText CantUseText { get; private set; }

		public sealed override void SetStaticDefaults()
		{
			string category = $"Items.RORConsumableItem.";
			CantUseText ??= Mod.GetLocalization($"{category}CantUse");

			ROREffectManager.RegisterItem(this);
		}

		private bool CanUse(Player player)
		{
			//If player has the effect already, check on that. If not, check on a fresh one
			ROREffect effect = ROREffectManager.GetEffectOfType<T>(player);
			if (effect == null)
			{
				effect = ROREffect.CreateInstance(player, typeof(T));
			}
			if (effect.EnforceMaxStack && effect.UnlockedStack >= effect.MaxRecommendedStack) return false;
			return effect.CanUse(player);
		}

		public sealed override bool CanUseItem(Player player)
		{
			return CanUse(player);
		}

		public sealed override bool? UseItem(Player player)
		{
			if (Main.myPlayer == player.whoAmI)
			{
				var mPlayer = player.GetRORPlayer();
				//It is important that this runs on client only, and send to server
				var effect = ROREffectManager.ApplyEffect<T>(mPlayer);

				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					new ROREffectApplyPacket(mPlayer, effect).Send();
				}
			}

			return true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (HasFlavorText)
			{
				string color = (FlavorColor * (Main.mouseTextColor / 255f)).Hex3();

				//FlavorText that has \n in it has to be split into single tooltiplines
				string[] lines = FlavorText.Split(new char[] { '\n' }, 2, StringSplitOptions.RemoveEmptyEntries);

				for (int i = 0; i < lines.Length; i++)
				{
					string tooltip = "[c/" + color + ":" + lines[i] + "]";
					tooltips.Add(new TooltipLine(Mod, Name + i.ToString(), tooltip));
				}
			}
			if (!CanUse(Main.LocalPlayer))
			{
				tooltips.Add(new TooltipLine(Mod, nameof(CantUseText), CantUseText.ToString())
				{
					OverrideColor = Color.Orange
				});
			}

			//When wanting to add more tooltips without flavorTextColor, call base.ModifyTooltips(tooltips) first
		}

		public sealed override void AddRecipes()
		{
			if (!ServerConfig.Instance.DisableRecipes)
			{
				SafeAddRecipes();
			}
		}

		public virtual void SafeAddRecipes()
		{

		}

		public override void SetDefaults()
		{
			Item.maxStack = 9999;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.width = 18;
			Item.height = 18;
			Item.useStyle = 4;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.value = Item.sellPrice(0, 2, 0, 0);
			Item.rare = (int)Rarity;
			Item.UseSound = SoundID.Item4;

			//When wanting to add more SetDefaults, call base.SetDefaults() first
		}
		#endregion
	}
}
