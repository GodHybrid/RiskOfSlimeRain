using Terraria;
using Terraria.DataStructures;

namespace RiskOfSlimeRain.Core.EntitySources
{
	public class EntitySource_Parent_Duration : EntitySource_Parent
	{
		public int Duration { init; get; }

		public EntitySource_Parent_Duration(Entity entity, int duration, string context = null) : base(entity, context)
		{
			Duration = duration;
		}
	}
}
