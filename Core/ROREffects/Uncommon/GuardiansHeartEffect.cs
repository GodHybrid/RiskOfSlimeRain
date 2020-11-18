using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using Terraria;
using Terraria.DataStructures;
using RiskOfSlimeRain.Helpers;
using System.IO;
using RiskOfSlimeRain.Network.Effects;
using Terraria.ID;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	//TODO proper visuals with the healthbar
	public class GuardiansHeartEffect : RORUncommonEffect, IPlayerLayer, IPreHurt, IPostUpdateEquips
	{
		public override float Initial => ServerConfig.Instance.OriginalStats ? 60 : 30;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 60 : 30;

		public const int time = 7;

		public int MaxShield => (int)Formula();

		public int Shield { get; private set; } = 0;

		public override string Description => $"Gain {Initial} shields. Recharges in {time} seconds out of combat";

		public override string FlavorText => "While living, the subject had advanced muscle growth, cell regeneration, higher agility...\nHis heart seems to still beat independent of the rest of the body.";
		
		public override string Name => "Guardian's Heart";

		public override string UIInfo()
		{
			return $"Shield: {Shield}/{MaxShield}";
		}

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			return Shield > 0 ?
				new PlayerLayerParams("Textures/GuardiansHeart", new Vector2(24, -24),
				color: Color.Lerp(Color.Red, Color.White, ((float)Shield)/MaxShield)) :
				null;
		}

		public bool PreHurt(Player player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (Shield > 0)
			{
				player.GetRORPlayer().ResetNoHurtTimer();
				Shield -= damage;
				CombatText.NewText(player.Hitbox, new Color(177, 215, 222), damage, dramatic: true);
				Shield = Utils.Clamp(Shield, 0, MaxShield);

				//vanilla immune times divided by 3
				player.immune = true;
				player.immuneTime = 20;
				if (player.longInvince)
				{
					player.immuneTime += 10;
				}
				for (int i = 0; i < player.hurtCooldowns.Length; i++)
				{
					player.hurtCooldowns[i] = player.immuneTime;
				}

				if (playSound)
				{
					if (Shield <= 0)
					{
						//Death from martian dude
						Main.PlaySound(SoundID.NPCKilled, (int)player.Center.X, (int)player.Center.Y, 45, volumeScale: 0.8f, pitchOffset: 0.2f);
					}
					else
					{
						//Shield hit from martian dude
						Main.PlaySound(SoundID.NPCHit, (int)player.Center.X, (int)player.Center.Y, 43, volumeScale: 0.8f, pitchOffset: 0.1f);
					}
				}

				new ROREffectSyncSinglePacket(this).Send();
				return false;
			}
			return true;
		}

		public void PostUpdateEquips(Player player)
		{
			if (Shield < MaxShield && player.GetRORPlayer().NoHurtTimer > time * 60)
			{
				Main.PlaySound(SoundID.MaxMana, player.Center);
				Shield = MaxShield;
			}
		}

		protected override void NetReceive(BinaryReader reader)
		{
			Shield = reader.ReadInt32();
		}

		protected override void NetSend(BinaryWriter writer)
		{
			writer.Write(Shield);
		}
	}
}
