using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.ROREffects.Common;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Effects;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Network.Effects;
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

		#region Warbanner
		private const int WarbannerTimeMax = 120;

		private int WarbannerTime { get; set; }

		//Because the actual warbanner effect is only for "spawning" the warbanner and not being in range of one
		public bool InWarbannerRange => WarbannerTime > 0;

		/// <summary>
		/// For major changes in loading/saving tags and migration. Make sure to increase it by 1 and add backwards compatibility in ROREffect.Load
		/// </summary>
		private const byte LATEST_VERSION = 1;

		public void ActivateWarbanner()
		{
			WarbannerTime = WarbannerTimeMax;
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

		private void UpdateTimers()
		{
			if (player.velocity == Vector2.Zero) NoMoveTimer++;
			else NoMoveTimer = 0;

			if (player.itemAnimation == 0) NoItemUseTimer++;
			else NoItemUseTimer = 0;

			NoHurtTimer++;

			NoOnHitTimer++;
		}
		#endregion

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
			if (InWarbannerRange)
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
			if (InWarbannerRange) WarbannerTime--;
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
		}

		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{
			NoOnHitTimer = 0;

			if (!IsHostile(target)) return;

			if (target.life <= 0)
			{
				ROREffectManager.Perform<IOnKill>(this, e => e.OnKillNPC(player, item, target, damage, knockback, crit));
			}

			ROREffectManager.Perform<IOnHit>(this, e => e.OnHitNPC(player, item, target, damage, knockback, crit));
		}

		public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			if (!IsHostile(target)) return;

			ROREffectManager.ModifyHitNPC(player, item, target, ref damage, ref knockback, ref crit);
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			NoOnHitTimer = 0;

			//This stuff should be at the bottom of everything
			if (!IsHostile(target)) return;

			//If this projectile shouldn't proc at all
			if (proj.modProjectile is IExcludeOnHit) return;

			//If this projectile is a minion, make it only proc 10% of the time
			if ((proj.minion || ProjectileID.Sets.MinionShot[proj.type]) && !Main.rand.NextBool(10)) return;

			if (target.life <= 0)
			{
				ROREffectManager.Perform<IOnKill>(this, e => e.OnKillNPCWithProj(player, proj, target, damage, knockback, crit));
			}

			ROREffectManager.Perform<IOnHit>(this, e => e.OnHitNPCWithProj(player, proj, target, damage, knockback, crit));
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			//This stuff should be at the bottom of everything
			if (!IsHostile(target)) return;

			//If this projectile shouldn't proc at all
			if (proj.modProjectile is IExcludeOnHit) return;

			//If this projectile is a minion, make it only proc 10% of the time
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
			if (InWarbannerRange) layers.Insert(0, WarbannerEffect.WarbannerLayer);
		}

		public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
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
		}

		public override void Initialize()
		{
			Effects = new List<ROREffect>();
			EffectByType = new Dictionary<Type, List<ROREffect>>();
			ROREffectManager.Init(this);
		}

		public override void PreUpdate()
		{
			//This is here because only here resetting the scrollwheel status works properly
			RORInterfaceLayers.Update(player);
		}

		public override void PostUpdate()
		{
			UpdateTimers();
		}

		private bool IsHostile(NPC npc)
		{
			return !npc.friendly && !npc.immortal && npc.lifeMax > 5 && !npc.dontTakeDamage && npc.chaseable;
		}
	}
}
