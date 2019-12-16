using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Effects;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Projectiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain
{
	public class RORPlayer : ModPlayer
	{
		//IMPORTANT: both structures keep the same effect object in them. modifying it in one of them also modifies it in the other

		//Only used for saving/loading/housekeeping/drawing
		//This is the thing synced to clients on world join aswell, the dict is rebuilt from that anyway
		public List<ROREffect> Effects { get; set; }

		//Actual access for performing the effect
		//Key: Interface, Value: List of effects implementing this interface
		public Dictionary<Type, List<ROREffect>> EffectByType { get; set; }

		#region Defensive Common
		#endregion
		#region Utility Common
		public int warbanners { get; set; } = 0;
		public float warbannerRadius { get; set; } = 64;
		public bool affectedWarbanner { get; set; } = false; //
		#endregion
		#region Offensive Common
		#endregion

		public override void ResetEffects()
		{
			ROREffectManager.Perform<IResetEffects>(this, e => e.ResetEffects(player));
		}

		public override void PostUpdateRunSpeeds()
		{
			ROREffectManager.Perform<IPostUpdateRunSpeeds>(this, e => e.PostUpdateRunSpeeds(player));
		}

		public override void PostUpdateEquips()
		{
			ROREffectManager.Perform<IPostUpdateEquips>(this, e => e.PostUpdateEquips(player));
		}

		public override float UseTimeMultiplier(Item item)
		{
			return ROREffectManager.UseTimeMultiplier(player, item);
			//if (affectedWarbanner && player.HasBuff(ModContent.BuffType<WarCry>())) dudChange *= 1.3f;
		}

		public override void UpdateLifeRegen()
		{
			ROREffectManager.Perform<IUpdateLifeRegen>(this, e => e.UpdateLifeRegen(player));
		}

		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{
			ROREffectManager.Perform<IOnHit>(this, e => e.OnHitNPC(player, item, target, damage, knockback, crit));

			if (warbanners > 0 && target.life <= 0) AddBanner();
		}

		public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			ROREffectManager.ModifyHitNPC(player, item, target, ref damage, ref knockback, ref crit);
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			if (warbanners > 0 && target.life <= 0) AddBanner();

			//this stuff should be at the bottom of everything

			//if this projectile shouldn't proc at all
			if (proj.modProjectile is IExcludeOnHit) return;
			//if this projectile is a minion, make it only proc 10% of the time
			if ((proj.minion || ProjectileID.Sets.MinionShot[proj.type]) && !Main.rand.NextBool(10)) return;

			ROREffectManager.Perform<IOnHit>(this, e => e.OnHitNPCWithProj(player, proj, target, damage, knockback, crit));
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			//this stuff should be at the bottom of everything

			//if this projectile shouldn't proc at all
			if (proj.modProjectile is IExcludeOnHit) return;
			//if this projectile is a minion, make it only proc 10% of the time
			if ((proj.minion || ProjectileID.Sets.MinionShot[proj.type]) && !Main.rand.NextBool(10)) return;

			ROREffectManager.ModifyHitNPCWithProj(player, proj, target, ref damage, ref knockback, ref crit, ref hitDirection);
		}

		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			bool ret = ROREffectManager.PreHurt(player, pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);
			return ret;
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			ROREffectManager.Perform<IKill>(this, e => e.Kill(player, damage, hitDirection, pvp, damageSource));
		}

		public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			ROREffectManager.Perform<IPostHurt>(this, e => e.PostHurt(player, pvp, quiet, damage, hitDirection, crit));
		}

		public override void GetWeaponCrit(Item item, ref int crit)
		{
			ROREffectManager.GetWeaponCrit(player, item, ref crit);
		}

		//TODO warbanner rework
		public void AddBanner()
		{
			if (Main.rand.Next(2 * RORWorld.radius.Count + 4) == 1)
			{
				if (RORWorld.radius.Count >= 100)
				{
					RORWorld.pos.RemoveAt(0);
					RORWorld.radius.RemoveAt(0);
				}
				byte tmpid = (byte)RORWorld.radius.Count;
				RORWorld.radius.Add(warbannerRadius * (warbanners * 0.4f + 0.6f));
				RORWorld.pos.Add(new Vector2(player.position.X, player.position.Y));
				Projectile.NewProjectile(RORWorld.pos[tmpid], new Vector2(0, 6), ModContent.ProjectileType<WarbannerBanner>(), 0, 0, Main.myPlayer, warbanners * warbannerRadius);
			}
		}

		public override void OnEnterWorld(Player player)
		{
			if (Main.netMode != NetmodeID.Server && Main.myPlayer == player.whoAmI)
			{
				//populate
				ROREffectManager.Populate(this);
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					//send to server, which then broadcasts
					ROREffectManager.SendOnEnter((byte)player.whoAmI);
				}
			}
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			//this is used when a new player joins the game. It sends its info to other players so they can update it
			//(from server to clients) (this means the server has to know the correct data of the player beforehand)
			ModPacket packet = mod.GetPacket();
			packet.Write((byte)MessageType.SyncEffectsOnEnterToClients);
			packet.Write((byte)player.whoAmI);
			packet.Write((int)Effects.Count);
			for (int i = 0; i < Effects.Count; i++)
			{
				ROREffect effect = Effects[i];
				effect.Send(packet);
			}
			packet.Send(toWho, fromWho);
		}

		public override TagCompound Save()
		{
			List<TagCompound> effectCompounds = Effects.ConvertAll((effect) => effect.Save());
			TagCompound tag = new TagCompound();
			tag.Add("effects", effectCompounds);
			return tag;
		}

		public override void Load(TagCompound tag)
		{
			if (tag.ContainsKey("effects"))
			{
				List<TagCompound> effectCompounds = tag.GetList<TagCompound>("effects").ToList();
				Effects.Clear();
				foreach (var compound in effectCompounds)
				{
					Effects.Add(ROREffect.Load(player, compound));
				}
				//sort by creation time
				Effects.Sort();
			}
		}

		public override void Initialize()
		{
			Effects = new List<ROREffect>();
			EffectByType = new Dictionary<Type, List<ROREffect>>();
			ROREffectManager.Init(this);
		}

		public override void ModifyDrawLayers(List<PlayerLayer> layers)
		{
			ROREffectManager.Perform<IModifyDrawLayers>(this, e => e.ModifyDrawLayers(layers));
		}

		public override void PreUpdate()
		{
			//this is here because only here resetting the scrollwheel status works properly
			RORInterfaceLayers.Update(player);
		}

		#region Boring commented stuff
		// In MP, other clients need accurate information about your player or else bugs happen.
		// clientClone, SyncPlayer, and SendClientChanges, ensure that information is correct.
		// We only need to do this for data that is changed by code not executed by all clients, 
		// or data that needs to be shared while joining a world.
		// For example, examplePet doesn't need to be synced because all clients know that the player is wearing the ExamplePet item in an equipment slot. 
		// The examplePet bool is set for that player on every clients computer independently (via the Buff.Update), keeping that data in sync.
		// ExampleLifeFruits, however might be out of sync. For example, when joining a server, we need to share the exampleLifeFruits variable with all other clients.
		public override void clientClone(ModPlayer clientClone)
		{
			//RORPlayer clone = clientClone as RORPlayer;
			// Here we would make a backup clone of values that are only correct on the local players Player instance.
			// Some examples would be RPG stats from a GUI, Hotkey states, and Extra Item Slots
			// clone.someLocalVariable = someLocalVariable;
		}

		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			// Here we would sync something like an RPG stat whenever the player changes it.
			// So far, ExampleMod has nothing that needs this.
			// if (clientPlayer.someLocalVariable != someLocalVariable)
			// {
			//	Send a Mod Packet with the changes.
			// }
		}

		public override void SetupStartInventory(IList<Item> items, bool mediumcoreDeath)
		{
			//Item item = new Item();
			//item.SetDefaults(ModContent.ItemType<ExampleItem>());
			//item.stack = 5;
			//items.Add(item);
		}

		public override void UpdateBiomes()
		{
			//ZoneExample = (ExampleWorld.exampleTiles > 50);
		}

		public override bool CustomBiomesMatch(Player other)
		{
			//RORPlayer modOther = other.GetModPlayer<RORPlayer>();
			return true;
			//return ZoneExample == modOther.ZoneExample;
			//If you have several Zones, you might find the &= operator or other logic operators useful:
			//bool allMatch = true;
			//allMatch &= ZoneExample == modOther.ZoneExample;
			//allMatch &= ZoneModel == modOther.ZoneModel;
			//return allMatch;
			//Here is an example just using && chained together in one statemeny

			// return ZoneExample == modOther.ZoneExample && ZoneModel == modOther.ZoneModel;
		}

		public override void CopyCustomBiomesTo(Player other)
		{
			//RORPlayer modOther = other.GetModPlayer<RORPlayer>();
			//modOther.ZoneExample = ZoneExample;
		}

		public override void SendCustomBiomes(BinaryWriter writer)
		{
			//BitsByte flags = new BitsByte();
			//flags[0] = ZoneExample;
			//writer.Write(flags);
		}

		public override void ReceiveCustomBiomes(BinaryReader reader)
		{
			//BitsByte flags = reader.ReadByte();
			//ZoneExample = flags[0];
		}

		public override void UpdateBiomeVisuals()
		{
			//bool usePurity = NPC.AnyNPCs(ModContent.NPCType<PuritySpirit>());
			//player.ManageSpecialBiomeVisuals("ExampleMod:PuritySpirit", usePurity);
			//bool useVoidMonolith = voidMonolith && !usePurity && !NPC.AnyNPCs(NPCID.MoonLordCore);
			//player.ManageSpecialBiomeVisuals("ExampleMod:MonolithVoid", useVoidMonolith, player.Center);
		}

		public override Texture2D GetMapBackgroundImage()
		{
			//if (ZoneExample)
			//{
			//	return mod.GetTexture("ExampleBiomeMapBackground");
			//}
			return null;
		}

		public static bool PlayerSolidTileCollision(Player p)
		{
			try
			{
				foreach (Point x in p.TouchedTiles)
				{
					Tile tile = Main.tile[x.X, x.Y];
					if (tile != null && tile.active() && tile.nactive())
					{
						return true;
					}
				}
			}
			catch { return false; }
			return false;
		}

		public override void UpdateVanityAccessories()
		{
			//for (int n = 13; n < 18 + player.extraAccessorySlots; n++)
			//{
			//	Item item = player.armor[n];
			//	if (item.type == mod.ItemType<Items.Armor.ExampleCostume>())
			//	{
			//		blockyHideVanity = false;
			//		blockyForceVanity = true;
			//	}
			//}
		}

		public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
		{
			// Make sure this condition is the same as the condition in the Buff to remove itself. We do this here instead of in ModItem.UpdateAccessory in case we want future upgraded items to set blockyAccessory
			//if (player.townNPCs >= 1 && blockyAccessory)
			//{
			//	player.AddBuff(mod.BuffType<Buffs.Blocky>(), 60, true);
			//}
		}

		public override void PostUpdateMiscEffects()
		{
			//if (lockTime > 0)
			//{
			//	lockTime--;
			//}
			//if (reviveTime > 0)
			//{
			//	reviveTime--;
			//}
		}

		public override void FrameEffects()
		{
			//if ((blockyPower || blockyForceVanity) && !blockyHideVanity)
			//{
			//	player.legs = mod.GetEquipSlot("BlockyLeg", EquipType.Legs);
			//	player.body = mod.GetEquipSlot("BlockyBody", EquipType.Body);
			//	player.head = mod.GetEquipSlot("BlockyHead", EquipType.Head);
			//}
			//if (nullified)
			//{
			//	Nullify();
			//}
		}

		public override void AnglerQuestReward(float quality, List<Item> rewardItems)
		{
			//if (voidMonolith)
			//{
			//	Item sticky = new Item();
			//	sticky.SetDefaults(ItemID.StickyDynamite);
			//	sticky.stack = 4;
			//	rewardItems.Add(sticky);
			//}
			//foreach (Item item in rewardItems)
			//{
			//	if (item.type == ItemID.GoldCoin)
			//	{
			//		int stack = item.stack;
			//		item.SetDefaults(ItemID.PlatinumCoin);
			//		item.stack = stack;
			//	}
			//}
		}

		public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
		{
			if (junk)
			{
				return;
			}
			//if (player.FindBuffIndex(BuffID.TwinEyesMinion) > -1 && liquidType == 0 && Main.rand.Next(3) == 0)
			//{
			//	caughtType = ModContent.ItemType<SparklingSphere>();
			//}
			//if (player.gravDir == -1f && questFish == ModContent.ItemType<ExampleQuestFish>() && Main.rand.Next(2) == 0)
			//{
			//	caughtType = ModContent.ItemType<ExampleQuestFish>();
			//}
		}

		public override void GetFishingLevel(Item fishingRod, Item bait, ref int fishingLevel)
		{
			//if (player.FindBuffIndex(ModContent.BuffType<CarMount>()) > -1)
			//{
			//	fishingLevel = (int)(fishingLevel * 1.1f);
			//}
		}

		public override void GetDyeTraderReward(List<int> dyeItemIDsPool)
		{
			//if (player.FindBuffIndex(BuffID.UFOMount) > -1)
			//{
			//	dyeItemIDsPool.Clear();
			//	dyeItemIDsPool.Add(ItemID.MartianArmorDye);
			//}
		}

		public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
		{
			//if ((blockyPower || blockyForceVanity) && !blockyHideVanity)
			//{
			//	player.headRotation = player.velocity.Y * (float)player.direction * 0.1f;
			//	player.headRotation = Utils.Clamp(player.headRotation, -0.3f, 0.3f);
			//	if (ZoneExample)
			//	{
			//		player.headRotation = (float)Main.time * 0.1f * player.direction;
			//	}
			//}
		}

		public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			//if (eFlames)
			//{
			//	if (Main.rand.Next(4) == 0 && drawInfo.shadow == 0f)
			//	{
			//		//int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, ModContent.DustType<EtherealFlame>(), player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 3f);
			//		//Main.dust[dust].noGravity = true;
			//		//Main.dust[dust].velocity *= 1.8f;
			//		//Main.dust[dust].velocity.Y -= 0.5f;
			//		//Main.playerDrawDust.Add(dust);
			//	}
			//	r *= 0.1f;
			//	g *= 0.2f;
			//	b *= 0.7f;
			//	fullBright = true;
			//}
		}

		public static readonly PlayerLayer MiscEffects = new PlayerLayer("ExampleMod", "MiscEffectsBack", PlayerLayer.MiscEffectsFront, delegate (PlayerDrawInfo drawInfo)
		{
			//if (drawInfo.shadow != 0f)
			//{
			//	return;
			//}
			//Player drawPlayer = drawInfo.drawPlayer;
			//Mod mod = ModLoader.GetMod("ExampleMod");
			//RORPlayer modPlayer = drawPlayer.GetModPlayer<RORPlayer>(mod);
			//if (modPlayer.reviveTime > 0)
			//{
			//	Texture2D texture = mod.GetTexture("NPCs/PuritySpirit/Revive");
			//	int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
			//	int drawY = (int)(drawInfo.position.Y + drawPlayer.height / 4f - 60f + modPlayer.reviveTime - Main.screenPosition.Y);
			//	DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Color.White * (modPlayer.reviveTime / 60f), 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0);
			//	Main.playerDrawData.Add(data);
			//}
		});
		#endregion
	}
}
