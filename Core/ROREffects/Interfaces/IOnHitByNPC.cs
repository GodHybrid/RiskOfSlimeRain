using RiskOfSlimeRain.Core.ROREffects.Attributes;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Interfaces
{
	/// <summary>
	/// Use this for unique interactions on receiving damage. Typically you are to use IPreHurt/IPostHurt for receiving damage, or IOnHit - for dealing damage.
	/// </summary>
	[CanProc]
	public interface IOnHitByNPC : IROREffectInterface
	{
		void OnHitByNPC(NPC npc, int damage, bool crit);
	}
}
