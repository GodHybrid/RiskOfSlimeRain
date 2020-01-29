using System;

namespace RiskOfSlimeRain.Data.ROREffects.Attributes
{
	/// <summary>
	/// This attribute is attached to methods that should only run when procced. Use on interfaces of IROREffectInterface if chance is involved
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface)]
	public class CanProc : Attribute
	{

	}
}
