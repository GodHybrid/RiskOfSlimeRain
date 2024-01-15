using RiskOfSlimeRain.Core.ROREffects;
using Terraria;

namespace RiskOfSlimeRain.Core.EntitySources
{
	public class EntitySource_FromEffect_Heal : EntitySource_FromEffect
	{
		public int Heal { init; get; }

		public EntitySource_FromEffect_Heal(Player player, ROREffect effect, int heal, string context = null) : base(player, effect, context)
		{
			Heal = heal;
		}
	}
}
