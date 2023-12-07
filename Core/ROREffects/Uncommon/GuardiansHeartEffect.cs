using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Network.Effects;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	//TODO proper visuals with the healthbar
	public class GuardiansHeartEffect : RORUncommonEffect, IPlayerLayer, IConsumableDodge, IPostUpdateEquips
	{
		public override float Initial => ServerConfig.Instance.OriginalStats ? 60 : 30;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 60 : 30;

		public const int time = 7;

		public int MaxShield => (int)Formula();

		public int Shield { get; private set; } = 0;

		public override LocalizedText Description => base.Description.WithFormatArgs(Initial, time);

		public override string UIInfo()
		{
			return UIInfoText.Format(Shield, MaxShield);
		}

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			if (Shield > 0) return new PlayerLayerParams("Textures/GuardiansHeart", new Vector2(24, -24),
				color: Color.Lerp(Color.Red, Color.White, ((float)Shield) / MaxShield));
			else return null;
		}

		public bool ConsumableDodge(Player player, Player.HurtInfo info)
		{
			if (Shield <= 0)
			{
				return false;
			}
			player.GetRORPlayer().ResetNoHurtTimer();
			Shield -= info.Damage;
			CombatText.NewText(player.Hitbox, new Color(177, 215, 222), info.Damage, dramatic: true);
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

			if (!info.SoundDisabled)
			{
				if (Shield <= 0)
				{
					//Death from martian dude
					SoundEngine.PlaySound(SoundID.NPCDeath45.WithVolumeScale(0.8f).WithPitchOffset(0.2f), player.Center);
				}
				else
				{
					//Shield hit from martian dude
					SoundEngine.PlaySound(SoundID.NPCHit43.WithVolumeScale(0.8f).WithPitchOffset(0.1f), player.Center);
				}
			}

			new ROREffectSyncSinglePacket(this).Send();
			return true;
		}

		public void PostUpdateEquips(Player player)
		{
			if (Main.myPlayer == player.whoAmI && Shield < MaxShield && player.GetRORPlayer().NoHurtTimer > time * 60)
			{
				SoundEngine.PlaySound(SoundID.MaxMana, player.Center);
				Shield = MaxShield;
				new ROREffectSyncSinglePacket(this).Send();
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
