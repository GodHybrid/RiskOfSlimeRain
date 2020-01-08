using System;

namespace RiskOfSlimeRain.Effects.Attributes
{
	/// <summary>
	/// This attribute is attached to methods that should only run when procced. Use on methods in ROREffects if chance is involved
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface)]
	public class CanProc : Attribute
	{

	}
}
