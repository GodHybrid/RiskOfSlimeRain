using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.ROREffects.Common;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Core.Warbanners;
using RiskOfSlimeRain.Effects;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Network.Effects;
using RiskOfSlimeRain.NPCs.Bosses;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain
{
	public class RORPlayer : ModPlayer
	{
		//IMPORTANT: both structures keep the same effect objects in them. modifying it in one of them also modifies it in the other

		//Only used for saving/loading/housekeeping/drawing
		//This is the thing synced to clients on world join aswell, the dict is rebuilt from that anyway
		public List<ROREffect> Effects { get; set; }

		//Actual access for performing the effect
		//Key: Interface, Value: List of effects implementing this interface
		public Dictionary<Type, List<ROREffect>> EffectByType { get; set; }

		/// <summary>
		/// Because we want DrawEffects to only be called once (without being affected by after images)
		/// </summary>
		public bool drawEffectsCalledOnce = false;

		public bool burningWitnessDropped = false;

		#region Warbanner
		public bool warbannerRemoverDropped = false;

		public const int WarbannerTimerMax = 60;

		public int WarbannerTimer { get; set; } = 0;

		private const int WarbannerBuffTimeMax = 120;

		private int WarbannerBuffTime { get; set; } = 0;

		/// <summary>
		/// The projectile identity that last called ActivateWarbanner ("nearest")
		/// </summary>
		public int LastWarbannerIdentity { get; private set; } = -1;

		//Because the actual warbanner effect is only for "spawning" the warbanner and not being in range of one
		public bool InWarbannerRange => WarbannerBuffTime > 0;

		/// <summary>
		/// For major changes in loading/saving tags and migration. Make sure to increase it by 1 and add backwards compatibility in ROREffect.Load
		/// </summary>
		private const byte LATEST_VERSION = 1;

		public void ActivateWarbanner(int identity)
		{
			LastWarbannerIdentity = identity;
			WarbannerBuffTime = WarbannerBuffTimeMax;
		}

		public bool CanReceiveWarbannerBuff => WarbannerTimer == WarbannerTimerMax;

		private void UpdateWarbanner()
		{
			WarbannerTimer--;
			if (WarbannerTimer < 0)
			{
				WarbannerTimer = WarbannerTimerMax;
				if (InWarbannerRange && Main.netMode != NetmodeID.Server && player.whoAmI == Main.myPlayer)
				{
					int heal = WarbannerEffect.WarbannerHealAmount(player);
					player.HealMe(heal);
				}
			}
			if (InWarbannerRange) WarbannerBuffTime--;
			if (!InWarbannerRange) LastWarbannerIdentity = -1;

			WarbannerEffect effect = ROREffectManager.GetEffectOfType<WarbannerEffect>(this);
			if ((effect?.Active ?? false) && Main.netMode != NetmodeID.Server && player.whoAmI == Main.myPlayer)
			{
				if (effect.WarbannerReadyToDrop && !InWarbannerRange)
				{
					bool success = WarbannerManager.TryAddWarbanner(effect.Radius, new Vector2(player.Center.X, player.Top.Y));
					if (success)
					{
						effect.ResetKillCount();
						effect.WarbannerReadyToDrop = false;
					}
				}
			}
		}
		#endregion

		#region Timers
		public int NoMoveTimer { get; private set; } = 0;

		public int NoItemUseTimer { get; private set; } = 0;

		public int NoHurtTimer { get; private set; } = 0;

		public int NoOnHitTimer { get; private set; } = 0;

		/// <summary>
		/// Time the player hasn't moved and used items
		/// </summary>
		public int NoInputTimer => Math.Min(NoMoveTimer, NoItemUseTimer);

		/// <summary>
		/// Time the player hasn't been in combat
		/// </summary>
		public int NoCombatTimer => Math.Min(NoHurtTimer, NoOnHitTimer);

		/// <summary>
		/// Used to prevent alot of rapid procs on things that are chance based
		/// </summary>
		private int ProcTimer { get; set; } = 0;

		private const int ProcTimerMax = 3;

		/// <summary>
		/// Sets ProcTimer to ProcTimerMax
		/// </summary>
		public void SetProcTimer() => ProcTimer = ProcTimerMax;

		/// <summary>
		/// Checks if ProcTimer is 0
		/// </summary>
		public bool CanProc() => ProcTimer == 0;

		private void UpdateTimers()
		{
			if (player.velocity == Vector2.Zero) NoMoveTimer++;
			else NoMoveTimer = 0;

			if (player.itemAnimation == 0) NoItemUseTimer++;
			else NoItemUseTimer = 0;

			NoHurtTimer++;

			NoOnHitTimer++;

			if (ProcTimer > 0) ProcTimer--;
		}
		#endregion

		#region Nullifier
		public bool nullifierEnabled = false;

		public bool nullifierActive = false;

		public long nullifierMoney = 0;

		public long savings = -1;

		private const int nullifierApplyTimerMax = 15;

		private int nullifierApplyTimer = 0;

		/// <summary>
		/// Sets up the nullifier process
		/// </summary>
		public void ActivateNullifier()
		{
			if (Main.netMode == NetmodeID.Server || player.whoAmI != Main.myPlayer) return;

			nullifierActive = true;
			Main.PlaySound(SoundID.MenuOpen, volumeScale: 0.8f);
		}

		/// <summary>
		/// Actually turns effects into items. Returns true if successful
		/// </summary>
		public bool ApplyNullifier()
		{
			//Clientside only
			if (Main.netMode == NetmodeID.Server || player.whoAmI != Main.myPlayer) return true;
			if (savings < nullifierMoney)
			{
				CombatText.NewText(player.getRect(), CombatText.DamagedHostile, "Not enough money!");
				return false;
			}
			if (nullifierMoney <= 0)
			{
				CombatText.NewText(player.getRect(), CombatText.DamagedHostile, "No removed items specified!");
				return false;
			}

			//Reassign Effects list to new effects
			List<ROREffect> newEffects = new List<ROREffect>();
			Dictionary<int, int> newItemsToStack = new Dictionary<int, int>();
			foreach (ROREffect effect in Effects)
			{
				if (effect.CanBeNullified)
				{
					bool removed = effect.UpdateStackAfterNullifier();
					if (!removed)
					{
						newEffects.Add(effect);
					}
					newItemsToStack.Add(effect.ItemType, effect.NullifierStack);
				}
				else
				{
					newEffects.Add(effect);
				}
			}
			Effects = newEffects;
			ROREffectManager.Populate(this);
			new RORPlayerSyncToAllPacket(this).Send();

			//Spawn items
			int count = 0;
			foreach (KeyValuePair<int, int> item in newItemsToStack)
			{
				if (item.Key > 0)
				{
					player.QuickSpawnItem(item.Key, item.Value);
					count += item.Value;
				}
			}
			player.BuyItem((int)nullifierMoney);
			Main.PlaySound(SoundID.Coins, volumeScale: 0.8f);
			string itemtext = "item" + (count == 1 ? "" : "s");
			CombatText.NewText(player.getRect(), CombatText.HealLife, $"Restored {count} {itemtext} for {nullifierMoney.MoneyToString()}!");
			return true;
		}

		/// <summary>
		/// Resets nullifier variables, doesn't do anything
		/// </summary>
		public void DeactivateNullifier()
		{
			foreach (ROREffect effect in Effects)
			{
				effect.NullifierStack = 0;
			}
			nullifierActive = false;
			nullifierApplyTimer = 0;
			nullifierMoney = 0;
			savings = -1;
			Main.PlaySound(SoundID.MenuClose, volumeScale: 0.8f);
		}

		public void UpdateNullifier()
		{
			foreach (ROREffect effect in Effects)
			{
				if (effect.CanBeNullified)
				{
					nullifierMoney += effect.NullifierCost;
				}
			}
			if (nullifierApplyTimer > 0)
			{
				nullifierApplyTimer--;
			}
		}

		public void UpdateNullifierAfterUI()
		{
			nullifierMoney = 0;
			savings = -1;
		}

		public void SetNullifierTimer()
		{
			nullifierApplyTimer = nullifierApplyTimerMax;
		}

		public bool DoubleClick()
		{
			if (mouseLeft && nullifierApplyTimer == 0)
			{
				Main.PlaySound(SoundID.MenuTick, volumeScale: 0.8f);
				SetNullifierTimer();
			}
			else if (mouseLeft && nullifierApplyTimer < nullifierApplyTimerMax - 3) //3 tick margin in case of some weird frameskip stuff or whatnot
			{
				Main.PlaySound(SoundID.MenuTick, volumeScale: 0.8f);
				//After a doubleclick 
				return true;
			}
			return false;
		}
		#endregion

		public bool mouseLeft = false;

		public bool mouseRight = false;

		public int CountActiveEffects()
		{
			return Effects.Sum(e => e.Stack);
		}

		public float TakenDamageMultiplier()
		{
			return 1 + (CountActiveEffects() * ServerConfig.TakenDamageMultiplier);
		}

		public float SpawnIncreaseMultiplier()
		{
			return 1 + (CountActiveEffects() * ServerConfig.SpawnRateMultiplier);
		}

		public override void ResetEffects()
		{
			if (Main.gameMenu)
			{
				//Because the game loads player data and then calls ResetEffects (to show proper health/mana and stuff like that), we need to acknowledge that.
				//Usually we populate in OnEnterWorld
				ROREffectManager.Populate(this);
			}
			else
			{
				PlayerHelper.SetLocalRORPlayer(this);
			}
			ROREffectManager.Perform<IResetEffects>(this, e => e.ResetEffects(player));
		}

		public override void ModifyWeaponDamage(Item item, ref float add, ref float mult, ref float flat)
		{
			//ROREffectManager.ModifyWeaponDamage(player, item, ref add, ref mult, ref flat);
			if (item.damage > 0 && InWarbannerRange)
			{
				flat += 4;
			}
		}

		public override void PostUpdateRunSpeeds()
		{
			if (InWarbannerRange)
			{
				//player.moveSpeed *= 1.3f;
				//These next two actually do something (increase acceleration slightly and increase max speed + cap)
				//ONLY increasing acceleration while keeping the max speed in check is not possible afaik
				player.accRunSpeed *= 1.3f;
				player.runAcceleration *= 1.3f;
			}
			ROREffectManager.Perform<IPostUpdateRunSpeeds>(this, e => e.PostUpdateRunSpeeds(player));
		}

		public override void PostUpdateEquips()
		{
			UpdateWarbanner();
			ROREffectManager.Perform<IPostUpdateEquips>(this, e => e.PostUpdateEquips(player));
		}

		public override float UseTimeMultiplier(Item item)
		{
			float mult = 1f;
			ROREffectManager.UseTimeMultiplier(player, item, ref mult);
			if (InWarbannerRange) mult += 0.15f; //Seems to mimic roughly 30% dps increase 
			return mult;
		}

		public override void UpdateLifeRegen()
		{
			ROREffectManager.Perform<IUpdateLifeRegen>(this, e => e.UpdateLifeRegen(player));
		}

		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			ROREffectManager.Perform<IProcessTriggers>(this, e => e.ProcessTriggers(player, triggersSet));

			mouseLeft = Main.mouseLeft && Main.mouseLeftRelease;
			mouseRight = Main.mouseRight && Main.mouseRightRelease;
		}

		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{
			NoOnHitTimer = 0;

			if (!NPCHelper.IsHostile(target)) return;

			if (target.life <= 0)
			{
				ROREffectManager.Perform<IOnKill>(this, e => e.OnKillNPC(player, item, target, damage, knockback, crit));
			}

			ROREffectManager.Perform<IOnHit>(this, e => e.OnHitNPC(player, item, target, damage, knockback, crit));
		}

		public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			if (!NPCHelper.IsHostile(target)) return;

			ROREffectManager.ModifyHitNPC(player, item, target, ref damage, ref knockback, ref crit);
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			NoOnHitTimer = 0;

			//This stuff should be at the bottom of everything
			if (!NPCHelper.IsHostile(target)) return;

			//If this projectile shouldn't proc at all
			if (proj.modProjectile is IExcludeOnHit) return;

			if (target.life <= 0)
			{
				ROREffectManager.Perform<IOnKill>(this, e => e.OnKillNPCWithProj(player, proj, target, damage, knockback, crit));
			}

			//If this projectile is a minion or sentry, make it only proc 10% of the time
			if ((proj.minion || ProjectileID.Sets.MinionShot[proj.type] ||
				proj.sentry || ProjectileID.Sets.SentryShot[proj.type])
				&& !Main.rand.NextBool(10)) return;

			ROREffectManager.Perform<IOnHit>(this, e => e.OnHitNPCWithProj(player, proj, target, damage, knockback, crit));
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			//This stuff should be at the bottom of everything
			if (!NPCHelper.IsHostile(target)) return;

			//If this projectile shouldn't proc at all
			if (proj.modProjectile is IExcludeOnHit) return;

			//If this projectile is a minion or sentry, make it only proc 10% of the time
			if ((proj.minion || ProjectileID.Sets.MinionShot[proj.type] ||
				proj.sentry || ProjectileID.Sets.SentryShot[proj.type])
				&& !Main.rand.NextBool(10)) return;

			ROREffectManager.ModifyHitNPCWithProj(player, proj, target, ref damage, ref knockback, ref crit, ref hitDirection);
		}

		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (ServerConfig.Instance.DifficultyScaling)
			{
				damage = (int)(damage * TakenDamageMultiplier());
			}
			bool ret = ROREffectManager.PreHurt(player, pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);
			return ret;
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			ROREffectManager.Perform<IKill>(this, e => e.Kill(player, damage, hitDirection, pvp, damageSource));
		}

		public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			NoHurtTimer = 0;
			ROREffectManager.Perform<IPostHurt>(this, e => e.PostHurt(player, pvp, quiet, damage, hitDirection, crit));
		}

		public override void GetWeaponCrit(Item item, ref int crit)
		{
			ROREffectManager.GetWeaponCrit(player, item, ref crit);
		}

		public override void ModifyDrawLayers(List<PlayerLayer> layers)
		{
			if (Main.gameMenu) return;
			if (Config.HiddenVisuals(player)) return;

			ROREffectManager.DrawPlayerLayers(layers);
			if (InWarbannerRange) layers.Insert(layers.Count > 1 ? 1 : 0, WarbannerEffect.WarbannerLayer);
		}

		public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			if (!drawEffectsCalledOnce)
			{
				drawEffectsCalledOnce = true;
			}
			else
			{
				return;
			}
			if (Main.gameMenu) return;
			if (Config.HiddenVisuals(player)) return;
			if (!ROREffectManager.ParentLayer.visible) return;

			List<Effect> shaders = ROREffectManager.GetScreenShaders(player);
			foreach (var shader in shaders)
			{
				ShaderManager.ApplyToScreenOnce(Main.spriteBatch, shader);
			}
		}

		public override void OnEnterWorld(Player player)
		{
			if (Main.myPlayer == player.whoAmI)
			{
				ROREffectManager.Populate(this);
			}
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			if (Main.netMode == NetmodeID.Server)
			{
				new RORPlayerSyncToAllClientsPacket(this).Send(fromWho, toWho);
			}
			else
			{
				ROREffectManager.Populate(this);
				new RORPlayerSyncToServerPacket(this).Send(fromWho, toWho);
			}
		}

		public override TagCompound Save()
		{
			List<TagCompound> effectCompounds = Effects.ConvertAll((effect) => effect.Save());
			TagCompound tag = new TagCompound
			{
				{ "version", LATEST_VERSION },
				{ "effects", effectCompounds },
				{ "nullifierEnabled", nullifierEnabled },
				{ "warbannerRemoverDropped", warbannerRemoverDropped },
				{ "burningWitnessDropped", burningWitnessDropped },
			};
			return tag;
		}

		public override void Load(TagCompound tag)
		{
			if (tag.ContainsKey("effects"))
			{
				byte version = tag.GetByte("version");
				List<TagCompound> effectCompounds = tag.GetList<TagCompound>("effects").ToList();
				Effects.Clear();
				foreach (var compound in effectCompounds)
				{
					ROREffect effect = ROREffect.Load(player, compound, version);
					if (effect != null) Effects.Add(effect);
				}
				//Sort by creation time
				Effects.Sort();
			}

			nullifierEnabled = tag.GetBool("nullifierEnabled");
			warbannerRemoverDropped = tag.GetBool("warbannerRemoverDropped");
			burningWitnessDropped = tag.GetBool("burningWitnessDropped");
		}

		public override void Initialize()
		{
			Effects = new List<ROREffect>();
			EffectByType = new Dictionary<Type, List<ROREffect>>();
			ROREffectManager.Init(this);

			nullifierEnabled = false;
		}

		public override void ModifyScreenPosition()
		{
			MagmaWorm.UpdateScreenShake();
		}

		public override void PreUpdate()
		{
			if (drawEffectsCalledOnce)
			{
				drawEffectsCalledOnce = false;
			}
			//This is here because only here resetting the scrollwheel status works properly
			RORInterfaceLayers.Update(player);
		}

		public override void PostUpdate()
		{
			UpdateNullifier();
			UpdateTimers();
		}
	}
}
