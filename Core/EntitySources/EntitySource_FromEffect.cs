using RiskOfSlimeRain.Core.ROREffects;
using Terraria;
using Terraria.DataStructures;

namespace RiskOfSlimeRain.Core.EntitySources
{
	public class EntitySource_FromEffect : EntitySource_Parent
	{
		public ROREffect Effect { get; init; }
		public EntitySource_FromEffect(Player player, ROREffect effect, string context = null) : base(player, context)
		{
			Effect = effect;
		}
	}
}
