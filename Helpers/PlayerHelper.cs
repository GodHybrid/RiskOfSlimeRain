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
			return player.GetDamage();
		}
	}
}
