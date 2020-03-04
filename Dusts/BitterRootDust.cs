using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Dusts
{
	public class BitterRootData
	{
		public const int frameCount = 4;
		public const int frameSpeed = 5;
		public const int height = 20;
		public const int width = 10;

		public int AlphaReduceRate { get; private set; }

		public int Frame { get; set; }

		public int FrameCounter { get; set; }

		public Dust Dust { get; private set; }

		public BitterRootData(Dust dust, int alphaReduceRate = 5, int frame = -1)
		{
			Dust = dust;
			AlphaReduceRate = alphaReduceRate;
			FrameCounter = 0;
			Frame = frame == -1 ? Main.rand.Next(frameCount) : frame;
		}

		public void Animate()
		{
			FrameCounter++;
			if (FrameCounter >= frameSpeed)
			{
				FrameCounter = 0;
				Frame++;
				if (Frame >= frameCount)
				{
					Frame = 0;
				}
			}

			Dust.frame = new Rectangle(0, Frame * height, width, height);
		}
	}

	/// <summary>
	/// Use custom NewDust method to spawn
	/// </summary>
	public class BitterRootDust : ModDust
	{
		public static Dust NewDust(Player player, int alphaReduceRate = 5)
		{
			Dust dust = Dust.NewDustDirect(player.Center, 0, 0, ModContent.DustType<BitterRootDust>(), Alpha: 100, newColor: Color.White);
			dust.velocity = new Vector2(-1, 0).RotatedBy(Main.rand.NextDouble() * MathHelper.Pi); //Only upwards, rotation clock wise
			dust.velocity *= 0.3f + Main.rand.NextFloat(1f);
			dust.position += 16 * dust.velocity;
			dust.position.Y -= 10;
			dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			dust.customData = new BitterRootData(dust, alphaReduceRate, -1);
			BitterRootData data = (BitterRootData)dust.customData;
			data.Animate();
			return dust;
		}

		private BitterRootData GetData(Dust dust)
		{
			BitterRootData data = null;
			if (dust.customData is BitterRootData) data = (BitterRootData)dust.customData;
			else dust.customData = new BitterRootData(dust);
			return data;
		}

		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.noLight = true;
		}

		public override bool Update(Dust dust)
		{
			BitterRootData data = GetData(dust);
			dust.position += dust.velocity;
			data.Animate();

			dust.alpha += data.AlphaReduceRate;
			if (dust.alpha > 255)
			{
				dust.active = false;
			}
			return false;
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return dust.color.MultiplyRGBA(lightColor) * ((255 - dust.alpha) / 255f);
		}
	}
}
