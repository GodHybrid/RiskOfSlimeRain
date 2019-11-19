using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameInput;

namespace RiskOfSlimeRain
{
    // ModPlayer classes provide a way to attach data to Players and act on that data. RORPlayer has a lot of functionality related to 
    // several effects and items in ExampleMod. See SimpleModPlayer for a very simple example of how ModPlayer classes work.
    public class RORPlayer : ModPlayer
    {
        private const int diceIncreaseMax = 5;

        #region Defensive Common
        public int bitterRoots = 0;
        public int bitterRootIncrease = 0;
        public int bustlingFungi = 0;
        public int bustlingFungusHeals = 0;
        public bool fungalDefense = false;//
        public bool eFungalDefense = false;//
        public int noMoveTimer = 0;
        public float fungalRadius = 64;//
        public int meatNuggets = 0;
        public int monsterTeeth = 0;
        public int medkits = 0;
        public int medkitTimer = -1;//
        public int mysteriousVials = 0;
        public int sproutingEggs = 0;
        public int sproutingEggTimer = -1;//
        #endregion
        #region Utility Common
        public int fireShields = 0;
        public int scarfs = 0;
        public int lensMakersGlasses = 0;
        public int savings = 0;
        public int piggyBankTimer = -1;//
        public int paulsGoatHooves = 0;
        public int snakeEyesDice = 0;
        public int snakeEyesDiceIncrease = 0;//
        public bool snakeEyesDiceReady = false;//
        public int soldiersSyringes = 0;
        public int spikestrips = 0;
        public int tasers = 0;
        public int warbanners = 0;
        public float warbannerRadius = 64;
        public bool affectedWarbanner = false; //
        #endregion
        #region Offensive Common
        public int barbedWires = 0;
        public int wireTimer = -1;
        public int wireRadius = 80;
        public int crowbars = 0;
        public int gasCanisters = 0;
        public int stompers = 0;
        public int mortarTubes = 0;
        #endregion

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void ResetEffects()
        {
            
            player.statLifeMax2 += bitterRootIncrease;
            player.lifeRegen += (int)(mysteriousVials * 1.2f);
            player.maxRunSpeed += player.maxRunSpeed * 0.2f * paulsGoatHooves;
            player.moveSpeed += player.moveSpeed * 0.2f * paulsGoatHooves;
            if (stompers > 0) player.maxFallSpeed += 6f;
            if (snakeEyesDiceIncrease > 0)
            {
                int sedIncrease = snakeEyesDiceIncrease * (snakeEyesDice * 3 + 3);
                player.meleeCrit += sedIncrease;
                player.rangedCrit += sedIncrease;
                player.magicCrit += sedIncrease;
                player.thrownCrit += sedIncrease;
            }
            if (((player.controlRight && player.velocity.X < 0) || (player.controlLeft && player.velocity.X > 0)) && paulsGoatHooves > 5) player.velocity.X = 0;
            if (sproutingEggTimer == -1) player.lifeRegen += (int)(sproutingEggs * 2.4f);
            if (player.HasBuff(mod.BuffType("Slowdown"))) player.velocity.X *= 0.8f;
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (scarfs > 0 && Main.rand.Next(100) < (scarfs * 5 + 5))
            {
                player.immune = true;
                player.immuneTime = 40;
                return false;
            }
            if (player.velocity.Y > 10f && Math.Abs(player.velocity.X) < 15f && stompers > 0 && damageSource.SourceNPCIndex > -1 && !player.immune) 
            {
                Main.npc[damageSource.SourceNPCIndex].StrikeNPC((int)(player.GetWeaponDamage(player.HeldItem) * ((5.07f + (0.3f * (stompers - 1))) * player.velocity.Y / 16)), 2f, 0, false);
                player.immune = true;
                player.immuneTime = 40;
                return false;
            }
            if (snakeEyesDice > 0 && snakeEyesDiceIncrease < 5 && player.statLife - damage < player.statLifeMax2 * 0.05 && snakeEyesDiceReady)
            {
                snakeEyesDiceIncrease++;
                snakeEyesDiceReady = false;
            }
            return true;
        }

        public override float UseTimeMultiplier(Item item)
        {
            float dudChange = 1f;
            if ((item.damage > 0 || item.axe > 0 || item.hammer > 0) && soldiersSyringes > 0) dudChange += soldiersSyringes * 0.1f; //15% is made into 10%, but it still works as 15%
            //if (affectedWarbanner && player.HasBuff(mod.BuffType("WarCry"))) dudChange *= 1.3f;
            return dudChange;
        }

        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            medkitTimer = 0;
            sproutingEggTimer = 420;
            if (fireShields > 0 && damage >= player.statLifeMax2 / 10) 
            {
                Projectile.NewProjectile(player.position, new Vector2(0, 0), mod.ProjectileType("FireShieldExplosion"), 200 * fireShields, 20 + fireShields, Main.myPlayer);
            }
            if (spikestrips > 0)
            {
                Projectile.NewProjectile(player.position, new Vector2(0, 0), mod.ProjectileType("SpikestripStrip"), 0, 0, Main.myPlayer);
                Projectile.NewProjectile(player.position, new Vector2(2, 0), mod.ProjectileType("SpikestripStrip"), 0, 0, Main.myPlayer);
                Projectile.NewProjectile(player.position, new Vector2(-2, 0), mod.ProjectileType("SpikestripStrip"), 0, 0, Main.myPlayer);
            }
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            sproutingEggTimer = 420;
            if (meatNuggets > 0 && Main.rand.Next(100) < 8) 
            {
                Projectile.NewProjectile(target.position, new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 1)), mod.ProjectileType("MeatNugget"), 0, 0);
                Projectile.NewProjectile(target.position, new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 1)), mod.ProjectileType("MeatNugget"), 0, 0);
            }
            if (monsterTeeth > 0 && target.life <= 0) 
            {
                player.HealEffect(monsterTeeth * 5 + 5);
                player.statLife += Math.Min(monsterTeeth * 5 + 5, MissingHP());
            }
            if (tasers > 0 && Main.rand.Next(100) < 7) target.AddBuff(mod.BuffType("TaserImmobility"), 60 + 30 * tasers);
            if (warbanners > 0 && target.life <= 0) AddBanner();
            if (crowbars > 0 && target.life < target.lifeMax * 0.8f) damage += (int)(damage * (0.2 + 0.3 * crowbars));
            if (gasCanisters > 0 && target.life <= 0) Projectile.NewProjectile(target.position, new Vector2(0, 1), mod.ProjectileType("GasBallFire"), 0, 0);
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (!crit && Main.rand.Next(100) < (lensMakersGlasses * 7)) crit = true;
            if (crowbars > 0 && target.life >= target.lifeMax * 0.8f) damage += (int)(damage * (0.2 + 0.3 * crowbars));
        }
        
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            sproutingEggTimer = 420;
            if (meatNuggets > 0 && Main.rand.Next(100) < 8)
            {
                Projectile.NewProjectile(target.position, new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 2)), mod.ProjectileType("MeatNugget"), 0, 0);
                Projectile.NewProjectile(target.position, new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 2)), mod.ProjectileType("MeatNugget"), 0, 0);
            }
            if (monsterTeeth > 0 && target.life <= 0) 
            {
                player.HealEffect(monsterTeeth * 5 + 5);
                player.statLife += Math.Min(monsterTeeth * 5 + 5, MissingHP());
            }
            if (warbanners > 0 && target.life <= 0) AddBanner();
            if (crowbars > 0 && target.life < target.lifeMax * 0.8f) damage += (int)(damage * (0.2 + 0.3 * crowbars));
            if (tasers > 0 && Main.rand.Next(100) < 7) target.AddBuff(mod.BuffType("TaserImmobility"), 60 + 30 * tasers);

            if (gasCanisters > 0 && target.life <= 0)
            {
                Projectile.NewProjectile(target.position, new Vector2(0, 1), mod.ProjectileType("GasBallFire"), 0, 0, Main.myPlayer, player.HeldItem.damage, gasCanisters);
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!crit && Main.rand.Next(100) < (lensMakersGlasses * 7)) crit = true;
            if (crowbars > 0 && target.life > target.lifeMax * 0.8f) damage += (int)(damage * (0.2 + 0.3 * crowbars));
        }

        private int MissingHP()
        {
            return player.statLifeMax2 - player.statLife;
        }

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
                Projectile.NewProjectile(RORWorld.pos[tmpid], new Vector2(0, 6), mod.ProjectileType("WarbannerBanner"), 0, 0, Main.myPlayer, warbanners * warbannerRadius);
            }
        }

        // In MP, other clients need accurate information about your player or else bugs happen.
        // clientClone, SyncPlayer, and SendClientChanges, ensure that information is correct.
        // We only need to do this for data that is changed by code not executed by all clients, 
        // or data that needs to be shared while joining a world.
        // For example, examplePet doesn't need to be synced because all clients know that the player is wearing the ExamplePet item in an equipment slot. 
        // The examplePet bool is set for that player on every clients computer independently (via the Buff.Update), keeping that data in sync.
        // ExampleLifeFruits, however might be out of sync. For example, when joining a server, we need to share the exampleLifeFruits variable with all other clients.
        public override void clientClone(ModPlayer clientClone)
        {
            RORPlayer clone = clientClone as RORPlayer;
            // Here we would make a backup clone of values that are only correct on the local players Player instance.
            // Some examples would be RPG stats from a GUI, Hotkey states, and Extra Item Slots
            // clone.someLocalVariable = someLocalVariable;
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = mod.GetPacket();
            packet.Write((byte)player.whoAmI);
            packet.Write(bitterRootIncrease);
            packet.Send(toWho, fromWho);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            // Here we would sync something like an RPG stat whenever the player changes it.
            // So far, ExampleMod has nothing that needs this.
            // if (clientPlayer.someLocalVariable != someLocalVariable)
            // {
            //    Send a Mod Packet with the changes.
            // }
        }

        public override void UpdateDead()
        {
            eFungalDefense = false;
            snakeEyesDiceIncrease = 0;
            snakeEyesDiceReady = true;
        }

        public override TagCompound Save()
        {
			return new TagCompound {
                #region Bitter Root
                {"bitterRoots", bitterRoots },
                {"bitterRootIncrease", bitterRootIncrease },
                #endregion
                #region Bustling Fungus
                {"bustlingFungi", bustlingFungi},
                {"bustlingFungusHeals", bustlingFungusHeals },
                {"fungalDefense", fungalDefense },
                {"fungalRadius", fungalRadius },
                #endregion
                {"meatNuggets", meatNuggets },
                {"monsterTeeth", monsterTeeth },
                {"soldiersSyringes", soldiersSyringes }
            };
			//note that C# 6.0 supports indexer initializers
			//return new TagCompound {
			//	["score"] = score
			//};
		}

		public override void Load(TagCompound tag)
		{
            #region Bitter Root
            bitterRoots = tag.GetInt("bitterRoots");
            bitterRootIncrease = tag.GetInt("bitterRootIncrease");
            #endregion
            #region Bustling Fungus
            bustlingFungi = tag.GetInt("bustlingFungi");
            bustlingFungusHeals = tag.GetInt("bustlingFungusHeals");
            fungalDefense = tag.GetBool("fungalDefense");
            fungalRadius = tag.GetFloat("fungalRadius");
            #endregion
            meatNuggets = tag.GetInt("meatNuggets");
            monsterTeeth = tag.GetInt("monsterTeeth");
            soldiersSyringes = tag.GetInt("soldiersSyringes");
        }

        public override void LoadLegacy(BinaryReader reader)
		{
			int loadVersion = reader.ReadInt32();
		}

		public override void SetupStartInventory(IList<Item> items)
		{
			//Item item = new Item();
			//item.SetDefaults(mod.ItemType("ExampleItem"));
			//item.stack = 5;
			//items.Add(item);
		}

		public override void UpdateBiomes()
		{
			//ZoneExample = (ExampleWorld.exampleTiles > 50);
		}

        public override bool CustomBiomesMatch(Player other)
        {
            RORPlayer modOther = other.GetModPlayer<RORPlayer>();
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
            RORPlayer modOther = other.GetModPlayer<RORPlayer>();
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
            //bool usePurity = NPC.AnyNPCs(mod.NPCType("PuritySpirit"));
            //player.ManageSpecialBiomeVisuals("ExampleMod:PuritySpirit", usePurity);
            //bool useVoidMonolith = voidMonolith && !usePurity && !NPC.AnyNPCs(NPCID.MoonLordCore);
            //player.ManageSpecialBiomeVisuals("ExampleMod:MonolithVoid", useVoidMonolith, player.Center);
        }

        public override Texture2D GetMapBackgroundImage()
        {
            //if (ZoneExample)
            //{
            //    return mod.GetTexture("ExampleBiomeMapBackground");
            //}
            return null;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {

        }

        public override void PreUpdateBuffs()
		{
            if (sproutingEggTimer >= 0) sproutingEggTimer--;
            #region Medkit
            if (medkitTimer >= 0 && medkits > 0) 
            {
                medkitTimer++;
                if (medkitTimer >= 66)
                {
                    player.HealEffect(medkits * 10);
                    player.statLife += Math.Min(medkits * 10, MissingHP());
                    medkitTimer = -1;
                }
            }
            #endregion
            #region Piggy Bank
            if (piggyBankTimer == 0)
            {
                piggyBankTimer = 180/savings;
                player.QuickSpawnItem(ItemID.CopperCoin, 1);
            }
            piggyBankTimer--;
            #endregion
        }

        public override void PostUpdateBuffs()
        {
            if (player.HasBuff(BuffID.PotionSickness)) snakeEyesDiceIncrease = 0;
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

        public override void PostUpdateEquips()
        {
            int totalFungusHeal = (int)(player.statLifeMax2 * 0.045f * bustlingFungusHeals);
            if (player.statLifeMax2 == player.statLife) snakeEyesDiceReady = true;
            if (fungalDefense && Equals(player.velocity, Vector2.Zero) && player.itemAnimation <= 0/*PlayerSolidTileCollision(player)*/)
            {
                noMoveTimer++;
                if (Main.myPlayer == player.whoAmI && noMoveTimer % 120 == 0 && noMoveTimer > 120)
                {
                    player.AddBuff(mod.BuffType("FungalDefenseMechanism"), 120); //The buff is only applied after 2 seconds has passed
                    foreach (NPC n in Main.npc)
                    {
                        if (n.townNPC && Vector2.Distance(player.position, n.position) < fungalRadius)
                        {
                            n.HealEffect((int)(totalFungusHeal), true);
                            n.life += Math.Min(totalFungusHeal, n.lifeMax - n.life);
                        }
                    }
                    if (Main.player.Length > 1)
                    {
                        foreach (Player n in Main.player)
                        {
                            if (Vector2.Distance(player.position, n.position) < fungalRadius)
                            {
                                player.HealEffect((int)(totalFungusHeal), true);
                                player.statLife += (int)(totalFungusHeal);
                            }
                        }
                    }
                }
            }
            else
            {
                noMoveTimer = 0;
                player.ClearBuff(mod.BuffType("FungalDefenseMechanism"));
            }
            if (barbedWires > 0 && wireTimer % 60 == 0)
            {
                for (int m = 0; m < 200; m++)
                {
                    NPC enemy = Main.npc[m];
                    if (enemy.CanBeChasedBy() && Vector2.Distance(player.Center, enemy.Center) <= wireRadius * barbedWires)
                    {
                        enemy.StrikeNPC((int)((0.5f + (0.2f * (barbedWires-1))) * player.GetWeaponDamage(player.HeldItem)), 0f, 0, false);
                        break;
                    }
                }
            }
            if (wireTimer > 0) wireTimer--;
            else wireTimer = 59;
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

        public override void PostUpdateMiscEffects()
        {
            //if (lockTime > 0)
            //{
            //    lockTime--;
            //}
            //if (reviveTime > 0)
            //{
            //    reviveTime--;
            //}
        }

        public override void FrameEffects()
        {
            //if ((blockyPower || blockyForceVanity) && !blockyHideVanity)
            //{
            //    player.legs = mod.GetEquipSlot("BlockyLeg", EquipType.Legs);
            //    player.body = mod.GetEquipSlot("BlockyBody", EquipType.Body);
            //    player.head = mod.GetEquipSlot("BlockyHead", EquipType.Head);
            //}
            //if (nullified)
            //{
            //    Nullify();
            //}
        }

        public override void AnglerQuestReward(float quality, List<Item> rewardItems)
        {
            //if (voidMonolith)
            //{
            //    Item sticky = new Item();
            //    sticky.SetDefaults(ItemID.StickyDynamite);
            //    sticky.stack = 4;
            //    rewardItems.Add(sticky);
            //}
            //foreach (Item item in rewardItems)
            //{
            //    if (item.type == ItemID.GoldCoin)
            //    {
            //        int stack = item.stack;
            //        item.SetDefaults(ItemID.PlatinumCoin);
            //        item.stack = stack;
            //    }
            //}
        }

        public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
        {
            if (junk)
            {
                return;
            }
            if (player.FindBuffIndex(BuffID.TwinEyesMinion) > -1 && liquidType == 0 && Main.rand.Next(3) == 0)
            {
                caughtType = mod.ItemType("SparklingSphere");
            }
            if (player.gravDir == -1f && questFish == mod.ItemType("ExampleQuestFish") && Main.rand.Next(2) == 0)
            {
                caughtType = mod.ItemType("ExampleQuestFish");
            }
        }

        public override void GetFishingLevel(Item fishingRod, Item bait, ref int fishingLevel)
        {
            if (player.FindBuffIndex(mod.BuffType("CarMount")) > -1)
            {
                fishingLevel = (int)(fishingLevel * 1.1f);
            }
        }

        public override void GetDyeTraderReward(List<int> dyeItemIDsPool)
        {
            //if (player.FindBuffIndex(BuffID.UFOMount) > -1)
            //{
            //    dyeItemIDsPool.Clear();
            //    dyeItemIDsPool.Add(ItemID.MartianArmorDye);
            //}
        }

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            //if ((blockyPower || blockyForceVanity) && !blockyHideVanity)
            //{
            //    player.headRotation = player.velocity.Y * (float)player.direction * 0.1f;
            //    player.headRotation = Utils.Clamp(player.headRotation, -0.3f, 0.3f);
            //    if (ZoneExample)
            //    {
            //        player.headRotation = (float)Main.time * 0.1f * player.direction;
            //    }
            //}
        }

        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            //if (eFlames)
            //{
            //    if (Main.rand.Next(4) == 0 && drawInfo.shadow == 0f)
            //    {
            //        int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, mod.DustType("EtherealFlame"), player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 3f);
            //        Main.dust[dust].noGravity = true;
            //        Main.dust[dust].velocity *= 1.8f;
            //        Main.dust[dust].velocity.Y -= 0.5f;
            //        Main.playerDrawDust.Add(dust);
            //    }
            //    r *= 0.1f;
            //    g *= 0.2f;
            //    b *= 0.7f;
            //    fullBright = true;
            //}
        }

        public static readonly PlayerLayer MiscEffects = new PlayerLayer("ExampleMod", "MiscEffectsBack", PlayerLayer.MiscEffectsFront, delegate (PlayerDrawInfo drawInfo)
        {
            //if (drawInfo.shadow != 0f)
            //{
            //    return;
            //}
            //Player drawPlayer = drawInfo.drawPlayer;
            //Mod mod = ModLoader.GetMod("ExampleMod");
            //RORPlayer modPlayer = drawPlayer.GetModPlayer<RORPlayer>(mod);
            //if (modPlayer.reviveTime > 0)
            //{
            //    Texture2D texture = mod.GetTexture("NPCs/PuritySpirit/Revive");
            //    int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
            //    int drawY = (int)(drawInfo.position.Y + drawPlayer.height / 4f - 60f + modPlayer.reviveTime - Main.screenPosition.Y);
            //    DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Color.White * (modPlayer.reviveTime / 60f), 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0);
            //    Main.playerDrawData.Add(data);
            //}
        });

        public static readonly PlayerLayer MiscEffectsBack = new PlayerLayer("RiskOfSlimeRain", "ItemEffects", PlayerLayer.MiscEffectsBack, delegate (PlayerDrawInfo drawInfo)
            {
                if (drawInfo.shadow != 0f)
                {
                    return;
                }
                Player drawPlayer = drawInfo.drawPlayer;
                Mod mod = ModLoader.GetMod("RiskOfSlimeRain");
                RORPlayer modPlayer = drawPlayer.GetModPlayer<RORPlayer>();

                if (modPlayer.barbedWires > 0)
                {
                    float scale = 3f;
                    Texture2D tex = ModContent.GetTexture("RiskOfSlimeRain/Projectiles/Textures/BarbedWireTexturePic");
                    int drawX = (int)(drawPlayer.Center.X - tex.Width * 0.5f * scale - Main.screenPosition.X);
                    int drawY = (int)(drawPlayer.Center.Y - tex.Width * 0.5f * scale - Main.screenPosition.Y);
                    DrawData data = new DrawData(tex, new Vector2(drawX, drawY), null, Color.White * 0.6f, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
                    Main.playerDrawData.Add(data);
                    //scale = modPlayer.wireRadius * modPlayer.barbedWires;
                    //drawX = (int)(drawPlayer.Center.X - tex.Width * 0.5f * scale - Main.screenPosition.X);
                    //drawY = (int)(drawPlayer.Center.Y - tex.Width * 0.5f * scale - Main.screenPosition.Y);
                    //data = new DrawData(tex, new Vector2(drawX, drawY), null, Color.White * 0.2f, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
                    //Main.playerDrawData.Add(data);
                }
            });
        
        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            MiscEffectsBack.visible = true;
            layers.Insert(0, MiscEffectsBack);
            //MiscEffects.visible = true;
            //layers.Add(MiscEffects);
        }
    }
}
