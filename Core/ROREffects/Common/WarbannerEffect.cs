using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.Misc;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Core.Warbanners;
using RiskOfSlimeRain.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	/// <summary>
	/// This effect is special because it only handles the trigger to spawning the banner. Everything else is in RORPlayer and WarbannerManager
	/// </summary>
	public class WarbannerEffect : RORCommonEffect, IOnKill
	{
		const int initial = 4;
		const int increase = 1;

		public int Radius => (initial + increase * Stack) * 16;

		public int KillCount { get; private set; }

		/// <summary>
		/// Flags the player having this effect to spawn a warbanner when possible
		/// </summary>
		public bool WarbannerReadyToDrop { get; set; } = false;

		public override string Description => "Drop an empowering banner after killing enough enemies";

		public override string FlavorText => "Very very valuable\nDon't drop it; it's worth more than you";

		public override string UIInfo => $"Kills required for next banner: {Math.Max(0, WarbannerManager.KillCountForNextWarbanner - KillCount)}. Active banners: {WarbannerManager.warbanners.Count}"
											+ (WarbannerReadyToDrop ? "\nNew banner ready, leave the current area of effect" : "")
											+ (NPC.BusyWithAnyInvasionOfSorts() ? "\nKill countdown is disabled while an invasion is in progress" : "");

		public void OnKillNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			IncreaseKillCountAndPrepareWarbannerSpawn(target);
		}

		public void OnKillNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			IncreaseKillCountAndPrepareWarbannerSpawn(target);
		}

		void IncreaseKillCountAndPrepareWarbannerSpawn(NPC target)
		{
			if (MiscManager.IsWormBodyOrTail(target)) return;
			if (MiscManager.IsBossPiece(target)) return;
			if (MiscManager.IsChild(target, out _)) return;
			if (MiscManager.IsSpawnedFromStatue(target)) return;
			if (NPC.BusyWithAnyInvasionOfSorts()) return;
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
			writer.Write(KillCount);
		}

		protected override void NetReceive(BinaryReader reader)
		{
			KillCount = reader.ReadInt32();
		}

		public override string ToString()
		{
			return base.ToString() + "; KillCount: " + KillCount;
		}

		public static readonly PlayerLayer WarbannerLayer = new PlayerLayer("RiskOfSlimeRain", "Warbanner", ROREffectManager.ParentLayer, delegate (PlayerDrawInfo drawInfo)
		{
			if (drawInfo.shadow != 0f)
			{
				return;
			}

			Player player = drawInfo.drawPlayer;

			Texture2D tex = ModContent.GetTexture("RiskOfSlimeRain/Textures/Warbanner");
			float drawX = (int)player.Center.X - Main.screenPosition.X;
			float drawY = (int)player.Center.Y + player.gfxOffY - Main.screenPosition.Y;

			Vector2 off = new Vector2(0, -(40 + (player.height >> 1)));
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (player.gravDir < 0f)
			{
				off.Y = -off.Y;
				spriteEffects = SpriteEffects.FlipVertically;
			}

			drawY -= player.gravDir * (40 + (player.height >> 1));
			Color color = Color.White;

			if (player.whoAmI == Main.myPlayer)
			{
				RORPlayer mPlayer = player.GetRORPlayer();
				WarbannerEffect effect = ROREffectManager.GetEffectOfType<WarbannerEffect>(mPlayer);
				if (effect?.Active ?? false)
				{
					if (effect.WarbannerReadyToDrop)
					{
						if (mPlayer.WarbannerTimer < 30)
						{
							color = Color.Transparent;
						}
					}
				}
			}

			color *= (255 - player.immuneAlpha) / 255f;

			DrawData data = new DrawData(tex, new Vector2(drawX, drawY), null, color, 0, tex.Size() / 2, 1f, spriteEffects, 0)
			{
				ignorePlayerRotation = true
			};
			Main.playerDrawData.Add(data);
		});
	}
}