using Terraria;

namespace RiskOfSlimeRain.Helpers
{
	public static class PlayerHelper
	{
		/// <summary>
		/// Returns the damage of the players held item. Respects ROR-Mode
		/// </summary>
		public static int GetDamage(this Player player)
		{
			//TODO include ror mode check here
			return player.GetWeaponDamage(player.HeldItem);
		}

		public static RORPlayer GetRORPlayer(this Player player)
		{
			return player.GetModPlayer<RORPlayer>();
		}
	}
}
