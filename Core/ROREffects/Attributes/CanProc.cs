using System;

namespace RiskOfSlimeRain.Core.ROREffects.Attributes
{
	/// <summary>
	/// This attribute is attached to interfaces that should only run when procced. Use on interfaces of IROREffectInterface if chance is involved
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface)]
	public class CanProc : Attribute
	{

	}
}
