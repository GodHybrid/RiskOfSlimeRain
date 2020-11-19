using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain.Core.ROREffects.Helpers
{
	/// <summary>
	/// Helper class to make effects that have healing included easier. Call <see cref="HandleStoredHeals"/>, <see cref="GetHeal"/> and <see cref="UpdateHitCheckCount"/> where required
	/// </summary>
	public abstract class HealingPoolEffect : ROREffect
	{
		/// <summary>
		/// The given heal amount added to StoredHeals
		/// </summary>
		public abstract float CurrentHeal { get; }

		/// <summary>
		/// The amount of additions to StoredHeals per second
		/// </summary>
		public abstract int HitCheckMax { get; }

		public float StoredHeals = 0;

		private int hitCheckTimer = 0;

		private int hitCheckCount = 0;

		protected int HitCheckTimerReduce => 60 / HitCheckMax;

		/// <summary>
		/// Call this when addition to the stored heal should take place
		/// </summary>
		protected void HandleStoredHeals()
		{
			if (hitCheckCount < HitCheckMax)
			{
				hitCheckCount++;
				StoredHeals += CurrentHeal;
			}
		}

		/// <summary>
		/// Call this to retreive from the heal pool. 0 if not enough 
		/// </summary>
		protected int GetHeal()
		{
			if (StoredHeals > 1)
			{
				int healAmount = (int)StoredHeals;
				StoredHeals -= healAmount;
				return healAmount;
			}
			return 0;
		}

		/// <summary>
		/// Does <see cref="HandleStoredHeals"/> and <see cref="GetHeal"/> combined
		/// </summary>
		protected void HandleAndApplyHeal(Player player)
		{
			HandleStoredHeals();
			int heal = GetHeal();
			if (heal > 0)
			{
				player.HealMe(heal);
			}
		}

		/// <summary>
		/// Call this in an update method
		/// </summary>
		public void UpdateHitCheckCount(Player player)
		{
			if (Main.myPlayer != player.whoAmI) return;

			if (hitCheckCount <= 0) return;

			hitCheckTimer++;
			if (hitCheckTimer % HitCheckTimerReduce == 0)
			{
				hitCheckCount--;
			}
		}

		public override void PopulateTag(TagCompound tag)
		{
			tag.Add("StoredHeals", StoredHeals);
		}

		public override void PopulateFromTag(TagCompound tag)
		{
			StoredHeals = tag.GetFloat("StoredHeals");
		}
	}
}
