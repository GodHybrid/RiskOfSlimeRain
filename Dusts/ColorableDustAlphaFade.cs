using Terraria;

namespace RiskOfSlimeRain.Dusts
{
	/// <summary>
	/// When spawning it, assign dust.customData to it if you want to change the default of 5 (int). Make sure do dust.customData = new InAndOutData() if you want to customize further
	/// </summary>
	public class ColorableDustAlphaFade : ColorableDust
	{
		private InAndOutData GetData(Dust dust)
		{
			InAndOutData data = null;
			if (dust.customData is InAndOutData) data = (InAndOutData)dust.customData;
			else dust.customData = new InAndOutData();
			return data;
		}

		public override bool Update(Dust dust)
		{
			//0 is full, 255 is transparent

			InAndOutData data = GetData(dust);
			data.Init(dust);

			if (data.Direction == 1)
			{
				//fade in
				dust.alpha -= data.InSpeed;
				if (dust.alpha < data.InEnd)
				{
					dust.alpha = data.InEnd;
					data.SwitchDirection();
				}
			}
			else
			{
				//fade out
				dust.alpha += data.OutSpeed;
				if (dust.alpha > data.OutEnd)
				{
					dust.active = false;
				}
			}

			return base.Update(dust);
		}

		protected override void ReduceScale(Dust dust)
		{
			if (GetData(dust).ReduceScale) base.ReduceScale(dust);
		}
	}

	public class InAndOutData
	{
		//0 is full, 255 is transparent
		public int InEnd { get; private set; }

		public int OutEnd { get; private set; }

		//-1 for "fade to transparent", 1 for "fade to opaque"
		//when it is 1 and reaches InEnd, switch to -1, but not the other way around
		public int Direction { get; private set; }

		public int InSpeed { get; private set; }

		public int OutSpeed { get; private set; }

		public bool ReduceScale { get; private set; }

		private bool spawned = false;

		public InAndOutData(int inEnd = 0, int outEnd = 255, int direction = 1, int inSpeed = 5, int outSpeed = 5, bool reduceScale = true)
		{
			InEnd = inEnd;
			OutEnd = outEnd;
			Direction = direction;
			InSpeed = inSpeed;
			OutSpeed = outSpeed;
			ReduceScale = reduceScale;
		}

		/// <summary>
		/// Called on spawn, sets alpha to whatever edge value specified (inEnd/outEnd), ignores the Alpha from NewDust
		/// </summary>
		public void Init(Dust dust)
		{
			if (!spawned)
			{
				spawned = true;
				dust.alpha = Direction == 1 ? OutEnd : InEnd;
			}
		}

		public void SwitchDirection()
		{
			Direction *= -1;
		}
	}
}
