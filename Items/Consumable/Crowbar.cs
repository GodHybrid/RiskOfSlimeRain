namespace RiskOfSlimeRain.Items.Consumable
{
	public class Crowbar : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "Deal 50% more damage to enemies above 80% HP";
			flavorText = "Crowbar/prybar/wrecking bar allows for both prying and smashing! \nCarbon steel, so it should last for a very long time, at least until the 3rd edition arrives.";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.crowbars++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.crowbars = 0;
		}
	}
}
