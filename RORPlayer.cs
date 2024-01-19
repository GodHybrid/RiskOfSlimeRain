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
using Terraria.Audio;
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
				if (InWarbannerRange && Main.netMode != NetmodeID.Server && Player.whoAmI == Main.myPlayer)
				{
					int heal = WarbannerEffect.WarbannerHealAmount(Player);
					Player.HealMe(heal);
				}
			}
			if (InWarbannerRange)
			{
				Player.GetAttackSpeed(DamageClass.Generic) += 0.15f;
				WarbannerBuffTime--;
			}
			if (!InWarbannerRange) LastWarbannerIdentity = -1;

			WarbannerEffect effect = ROREffectManager.GetEffectOfType<WarbannerEffect>(this);
			if (!InWarbannerRange && (effect?.Active ?? false) && Main.netMode != NetmodeID.Server && Player.whoAmI == Main.myPlayer && effect.WarbannerReadyToDrop)
			{
				if (WarbannerManager.TryAddWarbanner(effect.Radius, new Vector2(Player.Center.X, Player.Top.Y)))
				{
					effect.ResetKillCount();
					effect.WarbannerReadyToDrop = false;
				}
			}
		}
		#endregion

		#region Timers
		/// <summary>
		/// Time the player hasn't been moving
		/// </summary>
		public int NoMoveTimer { get; private set; } = 0;

		/// <summary>
		/// Time the player hasn't been using items
		/// </summary>
		public int NoItemUseTimer { get; private set; } = 0;

		/// <summary>
		/// Time the player hasn't been hit by damage
		/// </summary>
		public int NoHurtTimer { get; private set; } = 0;

		/// <summary>
		/// Time the player hasn't dealt damage
		/// </summary>
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
			if (Player.velocity == Vector2.Zero) NoMoveTimer++;
			else NoMoveTimer = 0;

			if (Player.itemAnimation == 0) NoItemUseTimer++;
			else NoItemUseTimer = 0;

			NoHurtTimer++;

			NoOnHitTimer++;

			if (ProcTimer > 0) ProcTimer--;
		}

		public void ResetNoHurtTimer()
		{
			NoHurtTimer = 0;
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
			if (Main.netMode == NetmodeID.Server || Player.whoAmI != Main.myPlayer) return;

			nullifierActive = true;
			SoundEngine.PlaySound(SoundID.MenuOpen.WithVolumeScale(0.8f));
		}

		/// <summary>
		/// Actually turns effects into items. Returns true if successful
		/// </summary>
		public bool ApplyNullifier()
		{
			//Clientside only
			if (Main.netMode == NetmodeID.Server || Player.whoAmI != Main.myPlayer) return true;
			if (savings < nullifierMoney)
			{
				CombatText.NewText(Player.getRect(), CombatText.DamagedHostile, "Not enough money!");
				return false;
			}
			if (nullifierMoney <= 0)
			{
				CombatText.NewText(Player.getRect(), CombatText.DamagedHostile, "No removed items specified!");
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
					Player.QuickSpawnItem(Player.GetSource_FromThis(), item.Key, item.Value);
					count += item.Value;
				}
			}
			Player.BuyItem((int)nullifierMoney);
			SoundEngine.PlaySound(SoundID.Coins.WithVolumeScale(0.8f));
			string itemtext = "item" + (count == 1 ? "" : "s");
			CombatText.NewText(Player.getRect(), CombatText.HealLife, $"Restored {count} {itemtext} for {nullifierMoney.MoneyToString()}!");
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
			SoundEngine.PlaySound(SoundID.MenuClose.WithVolumeScale(0.8f));
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
				SoundEngine.PlaySound(SoundID.MenuTick.WithVolumeScale(0.8f));
				SetNullifierTimer();
			}
			else if (mouseLeft && nullifierApplyTimer < nullifierApplyTimerMax - 3) //3 tick margin in case of some weird frameskip stuff or whatnot
			{
				SoundEngine.PlaySound(SoundID.MenuTick.WithVolumeScale(0.8f));
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

		public int CountTotalEffects()
		{
			return Effects.Sum(e => e.UnlockedStack);
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
			ROREffectManager.Perform<IResetEffects>(this, e => e.ResetEffects(Player));
		}

		public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
		{
			if (item.damage > 0 && InWarbannerRange)
			{
				damage.Flat += 4;
			}
			ROREffectManager.ModifyWeaponDamage(Player, item, ref damage);
		}

		public override void PostUpdateRunSpeeds()
		{
			if (InWarbannerRange)
			{
				//player.moveSpeed *= 1.3f;
				//These next two actually do something (increase acceleration slightly and increase max speed + cap)
				//ONLY increasing acceleration while keeping the max speed in check is not possible afaik
				Player.accRunSpeed *= 1.3f;
				Player.runAcceleration *= 1.3f;
			}
			ROREffectManager.Perform<IPostUpdateRunSpeeds>(this, e => e.PostUpdateRunSpeeds(Player));
		}

		public override void PostUpdateEquips()
		{
			UpdateWarbanner();
			ROREffectManager.Perform<IPostUpdateEquips>(this, e => e.PostUpdateEquips(Player));
		}

		public override void UpdateLifeRegen()
		{
			ROREffectManager.Perform<IUpdateLifeRegen>(this, e => e.UpdateLifeRegen(Player));
		}

		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			ROREffectManager.Perform<IProcessTriggers>(this, e => e.ProcessTriggers(Player, triggersSet));

			mouseLeft = Main.mouseLeft && Main.mouseLeftRelease;
			mouseRight = Main.mouseRight && Main.mouseRightRelease;
		}

		public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
		{
			NoOnHitTimer = 0;

			if (!NPCHelper.IsHostile(target)) return;

			if (target.life <= 0)
			{
				ROREffectManager.Perform<IOnKill>(this, e => e.OnKillNPCWithItem(Player, item, target, hit, damageDone));
			}

			ROREffectManager.Perform<IOnHit>(this, e => e.OnHitNPCWithItem(Player, item, target, hit, damageDone));
		}

		public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
		{
			if (!NPCHelper.IsHostile(target)) return;

			ROREffectManager.ModifyHitNPC(Player, item, target, ref modifiers);
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			NoOnHitTimer = 0;

			//This stuff should be at the bottom of everything
			if (!NPCHelper.IsHostile(target)) return;

			//If this projectile shouldn't proc at all
			if (proj.ModProjectile is IExcludeOnHit) return;

			if (target.life <= 0)
			{
				ROREffectManager.Perform<IOnKill>(this, e => e.OnKillNPCWithProj(Player, proj, target, hit, damageDone));
			}

			//If this projectile is a minion or sentry, make it only proc 20% of the time
			if (proj.IsMinionOrSentryRelated && !Main.rand.NextBool(5)) return;

			ROREffectManager.Perform<IOnHit>(this, e => e.OnHitNPCWithProj(Player, proj, target, hit, damageDone));
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
		{
			//This stuff should be at the bottom of everything
			if (!NPCHelper.IsHostile(target)) return;

			//If this projectile shouldn't proc at all
			if (proj.ModProjectile is IExcludeOnHit) return;

			//If this projectile is a minion or sentry, make it only proc 20% of the time
			if (proj.IsMinionOrSentryRelated && !Main.rand.NextBool(5)) return;

			ROREffectManager.ModifyHitNPCWithProj(Player, proj, target, ref modifiers);
		}

		public override bool FreeDodge(Player.HurtInfo info)
		{
			bool ret = ROREffectManager.FreeDodge(Player, info);
			return ret;
		}

		public override bool ConsumableDodge(Player.HurtInfo info)
		{
			bool ret = ROREffectManager.ConsumableDodge(Player, info);
			return ret;
		}

		public override void ModifyHurt(ref Player.HurtModifiers modifiers)
		{
			if (ServerConfig.Instance.DifficultyScaling)
			{
				modifiers.SourceDamage += TakenDamageMultiplier() - 1f;
			}
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			ROREffectManager.Perform<IKill>(this, e => e.Kill(Player, damage, hitDirection, pvp, damageSource));
		}

		public override void PostHurt(Player.HurtInfo info)
		{
			ResetNoHurtTimer();
			ROREffectManager.Perform<IPostHurt>(this, e => e.PostHurt(Player, info));
		}

		public override void ModifyWeaponCrit(Item item, ref float crit)
		{
			ROREffectManager.ModifyWeaponCrit(Player, item, ref crit);
		}

		public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			if (Main.gameMenu) return;
			if (Player.dead || !Player.active) return;
			if (drawInfo.shadow != 0f) return;
			if (Config.HiddenVisuals(drawInfo.drawPlayer)) return;
			if (!ROREffectManager.ParentVisibilityLayer.Visible) return;

			List<Effect> shaders = ROREffectManager.GetScreenShaders(drawInfo.drawPlayer);
			foreach (var shader in shaders)
			{
				ShaderManager.ApplyToScreenOnce(Main.spriteBatch, shader);
			}
		}

		public override void OnEnterWorld()
		{
			if (Main.myPlayer == Player.whoAmI)
			{
				ROREffectManager.Populate(this);
			}
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			if (Main.netMode != NetmodeID.Server)
			{
				ROREffectManager.Populate(this);
			}
			new RORPlayerSyncPacket(this).Send(toWho, fromWho);
		}

		public override void SaveData(TagCompound tag)
		{
			List<TagCompound> effectCompounds = Effects.ConvertAll((effect) => effect.Save());
			tag.Add("version", LATEST_VERSION);
			tag.Add("effects", effectCompounds);
			tag.Add("nullifierEnabled", nullifierEnabled);
			tag.Add("warbannerRemoverDropped", warbannerRemoverDropped);
			tag.Add("burningWitnessDropped", burningWitnessDropped);
		}

		public override void LoadData(TagCompound tag)
		{
			if (tag.ContainsKey("effects"))
			{
				byte version = tag.GetByte("version");
				List<TagCompound> effectCompounds = tag.GetList<TagCompound>("effects").ToList();
				Effects.Clear();
				foreach (var compound in effectCompounds)
				{
					ROREffect effect = ROREffect.Load(Player, compound, version);
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
			warbannerRemoverDropped = false;
			burningWitnessDropped = false;
		}

		public override void ModifyScreenPosition()
		{
			MagmaWorm.UpdateScreenShake();
		}

		public override void PreUpdate()
		{
			//This is here because only here resetting the scrollwheel status works properly
			RORInterfaceLayers.Update(Player);
		}

		public override void PostUpdate()
		{
			UpdateNullifier();
			UpdateTimers();
		}
	}
}
