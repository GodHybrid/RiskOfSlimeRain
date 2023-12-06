# How to add an effect

The system won't be explained, but rather the steps you need to do to add a new effect to the mod.
For the explanation, go [here](LINK aaa). The example will assume the effect is called "Infusion". Follow the naming conventions otherwise.

## Step 1: Empty effect
First, go to the `Effects` folder and choose the rarity the effect is gonna be based on.
Then create a new file called `InfusionEffect.cs` and a new class template like this:

```csharp
using RiskOfSlimeRain.Core.ROREffects.Interfaces;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class InfusionEffect : RORUncommonEffect
	{

	}
}
```

Take note of the `Uncommon` in the namespace and the extended class (since we are making an uncommon rarity effect).
If such a folder/namespace/class doesn't exist yet, make one.

Now, go to Items/Consumable and choose the rarity the effect is gonna be based on.
Then create a new file called `Infusion.cs` and a new class template like this:

```csharp
using RiskOfSlimeRain.Core.ROREffects.Uncommon;

namespace RiskOfSlimeRain.Items.Consumable.Uncommon
{
	public class Infusion : RORConsumableItem<InfusionEffect>
	{
		//Add recipe here if you want
	}
}
```

As always, if you get red underlines, add the using by mouseovering and pressing Alt+Enter.
In this case, `using RiskOfSlimeRain.Core.ROREffects.Uncommon;`
Finally, add a texture in that same place for the item called `Infusion.png`, and you are done.
But the effect doesn't do anything at all, this is going to be the next step!

## Step 2: Making it do things

Go back to the effect class, and write up the description format arguments:
```csharp
public class InfusionEffect : RORUncommonEffect
{
	public override LocalizedText Description => base.Description.WithFormatArgs(1);

}
```

To add behavior to the effect, click with the mouse at the end of the line with the class definition and start typing in `, I`, this should bring up a list of interfaces that are named after ModPlayer hooks. In our case, we want IOnHit, which will use the OnHitNPC and OnHitNPCWithProj hooks.
After you typed that in, it should be underlined in red. Mouseover it, and click "show potential fixes", and then "implement interface". This will end you up with this:

```csharp
public class InfusionEffect : RORUncommonEffect, IOnHit
{
	public override LocalizedText Description => base.Description.WithFormatArgs() "Killing an enemy increases your health permanently by 1";

	public override LocalizedText FlavorText => base.FlavorText.WithFormatArgs() "You can add whatever blood sample you want, as far as I know.\nRemember that sampling from other creatures is a great basis for experimentation!";

	public void OnHitNPC(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
	{
		throw new System.NotImplementedException();
	}

	public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
	{
		throw new System.NotImplementedException();
	}
}
```

Because of the nature of our effect, we are going to have to spawn a projectile when the NPC is killed. We make a helper method to facilitate that.

```csharp
public class InfusionEffect : RORUncommonEffect, IOnHit
{
	public override LocalizedText Description => base.Description.WithFormatArgs() "Killing an enemy increases your health permanently by 1";

	public override LocalizedText FlavorText => base.FlavorText.WithFormatArgs() "You can add whatever blood sample you want, as far as I know.\nRemember that sampling from other creatures is a great basis for experimentation!";

	public void OnHitNPC(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
	{
		SpawnProjectile(player, target);
	}

	public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
	{
		SpawnProjectile(player, target);
	}

	void SpawnProjectile(Player player, NPC target)
	{
		if (target.life <= 0)
		{
			//Spawn projectile here via Projectile.NewProjectile, and pass the life increase * Stack as ai0 or ai1
		}
	}
}
```

Now comes the advanced part specific to this effect, saving the gathered life and applying it. For the applying, simply use IResetEffects.
For the saving, we need a float (not int) variable that stores the life (float because the stack effect only increases + 0.5).
Adding those two things, the class is now pretty big (only changes shown):

```csharp
public float bonusLife = 0f;

public void ResetEffects(Player player)
{
	player.statLifeMax2 += (int)bonusLife;
}

public override void PopulateTag(TagCompound tag)
{
	tag.Add("bonusLife", bonusLife);
}

public override void PopulateFromTag(TagCompound tag)
{
	bonusLife = tag.GetFloat("bonusLife");
}

protected override void NetSend(BinaryWriter writer)
{
	writer.Write(bonusLife);
}

protected override void NetReceive(BinaryReader reader)
{
	bonusLife = reader.ReadSingle();
}
```

This is now all we have done on the effects side of things. The rest is handled by the projectile it spawns.
Said projectile will retreive an instance of this effect from the player and increase bonusLife based on the stack it got passed to via ai0 or ai1.

# No suitable interface exists
If the hook you need is not listed in the beginning of Step 2, you need to add this "hook" manually the first time.
As an example, we want to make a MeleeEffects hook.

## Step 1: ModPlayer
Go into `RORPlayer.cs` and override MeleeEffects.
```csharp
public override void MeleeEffects(Item item, Rectangle hitbox)
{
	base.MeleeEffects(item, hitbox);
}
```
_Disclaimer: the following method only works for `void` methods with no `ref` arguments. For others, look in `Effects\ROREffectManager.cs`._

Then go into the `Effects\Interfaces` folder and add a new file called `IMeleeEffects.cs`. This should be the code in it:

```csharp
using Microsoft.Xna.Framework;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Interfaces
{
	public interface IMeleeEffects : IROREffectInterface
	{
		void MeleeEffects(Player player, Item item, Rectangle hitbox);
	}
}
```

Now go back into `RORPlayer.cs` and change the code inside the hook to this:
```csharp
public override void MeleeEffects(Item item, Rectangle hitbox)
{
	ROREffectManager.Perform<IMeleeEffects>(this, e => e.MeleeEffects(player, item, hitbox));
}
```

You can now use this interface in any new effects you make.

# Other
If you want a chance based effect, look into overriding `AlwaysProc` and `Chance`. That chance will apply the same way to all methods that proc (from Interfaces with the `CanProc` attribute).
So mostly use them for the OnHit stuff.