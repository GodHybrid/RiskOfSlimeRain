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
using Terraria.GameInput;
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

		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			ROREffectManager.Perform<IProcessTriggers>(this, e => e.ProcessTriggers(player, triggersSet));
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
	}
}
