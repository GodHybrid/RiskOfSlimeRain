# Changelogs

## 0.5
* Items
	* ALL damage items now have a minimal damage threshold that increases as player progresses. Players are no longer required to hold a weapon for the effects to deal any damage
	* Some damage items now penetrate armor, making them not useless against high armor enemies
	* Barbed Wire now immediately triggers if an enemy enters the radius
	* Bundle Of Fireworks now spawns rockets every 50 kills, triple rockets every boss kill. The chance to fire rockets remains
	* Fire Shield now correctly displays current stack stats of the item:
		* Original stats: 200% damage -> 400% damage
		* Balanced stats: 200% damage -> 600% damage; 10 knockback -> 15 knockback
	* Medkit had its animation drastically sped up after the heal was already applied
	* Panic Mines now activate at both low HP and after taking heavy damage
		* Balanced stats: Maximum quantity of mines is limited to 8 at 8 stacks, further stacks increase the damage instead
	* Spikestrips now spawn on set X locations: covers less area if hit airborne, but is much more useful if hit while grounded
	* Taser (Balanced stats): Chance 7% -> 10%
* Misc
	* (With Boss Checklist Mod enabled) Modded bosses now only drop Common items instead of Uncommon. They will now continue dropping after you defeat 8 (or 11 in hardmode) different modded bosses
	* When hovering over items in the bottom UI, it will now hide the chat and other items
	* UI now shows total item count
	* Increased taken damage multiplier slightly if Difficulty Scaling is enabled
* Fixes
	* Fixed Bustling Mushroom healing 1 HP less than intended
	* Fixed Harvester's Scythe not counting as an Uncommon
	* Fixed Paul's Goat Hoof UI info
	* Fixed Rusty Knife having less duration than intended
	* Fixed "chests with RoSR items in it" UI tooltip not working in multiplayer


## 0.4.0.2
* Added config setting to disable recipes for consumable power-up items

## 0.4.0.1
* Buffed minion/sentry proc rate by 100% (from 10% to 20%)
* Fixed Magma Worm's fireballs making a sound and bouncing too many times
* Fixed certain player bound textures showing up when they shouldn't
* Fixed Barbed Wire tooltip
* Fixed rocket projectiles not accelerating when no targets exist

## 0.4
* 10 new items of Uncommon rarity, dropped from bosses and found in post-Plantera dungeon chests
    * AtG Missile Mk. 1
    * Boxing Gloves
    * Dead Man's Foot
    * Golden Gun
    * Guardian's Heart
    * Harvester's Scythe
    * Infusion
    * Leeching Seed
    * Panic Mines
    * Tough Times

* Paul's Goat Hoof now increases max speed in addition to acceleration
* Lens-Maker's Glasses don't make a sound anymore
* Various small bugfixes
    * Added missing 'Burning Witness' player sprites
    * Fixed projectiles that fall to the ground hovering just above it

## 0.3.3.3
* Fixed only one effect triggering, preventing subsequent ones from going off

## 0.3.3.2
* Fixed minions/sentries not always proccing "on kill" effects

## 0.3.3.1

* Fixed Barbed Wire tooltip
* Fixed Magma Worm not being immune to lava
* Fixed Warbanner giving free damage for things with no damage 

## 0.3.3

* Magma Worm!
    * Recommended to fight in Pre-Hardmode, drops his unique boss item "Burning Witness" on first kill
    * Summoned via "Spicy Honey Donut"
* Fixed Barbed Wire and Headstompers softlocking the player when no item was selected

## 0.3.2

* New sprites!
* Balance!
    * Added a new config (labeled "Server Config") containing various balance options
        * "Original Stats": If enabled, you keep using the same stats for items as in previous updates. Disabled by default
        * "Difficulty Scaling": If enabled, the game becomes harder the more powerful you are. More infos on the "?" UI
        * You can find all the balance changes [here](https://github.com/GodHybrid/RiskOfSlimeRain/blob/master/OriginalStatsvsBalancedStats.md#original-stats-vs-balanced-stats)
        * Please give feedback about the new balance on our homepage!
* Item acquisition rework!
    * Items generate in chests all over the world
    * You are guaranteed one drop from each vanilla boss (with exceptions)
        * If you have "Boss Checklist" mod enabled, the amount of items you can get from bosses potentially doubles
            * Limit: 12 bosses in pre-Hardmode, 16 in Hardmode
    * You can still craft all items
* Item blacklist!
    * Filter out undesired items on server side. Selected items will have no effect on players until deselected again. Useful for filtering items you deem problematic
* Added more mouseover text for most items
* Fixed Bustling Fungus and Warbanner spawning in multiplayer when not standing on ground
* Fixed Hermit's Scarf spamming "dodged"
* Fixed unintended behavior of some weapons bypassing enemies' invincibility frames, thus proccing effects with very low delay.
* Increased Barbed Wire and Warbanner default radius slightly
* Warbanners will now drop down on world reload if the tiles below them don't exist
* Meat Nugget now only drops 1 nugget instead of 2, but heals double the amount
* Tweaked easy-to-get recipes for some items

## 0.3.1.8

* Fixed items not spawning in multiplayer

## 0.3.1.7

* Fixed Warbanner Remover and Nullifier never appearing together in the Traveling Merchants shop
* (Updated to latest library version)

## 0.3.1.6

* Follow up fix to Soldier's Syringe

## 0.3.1.5

* Fixed crash related to shaders when loading the mod on some systems
* Fixed crash related to Soldier's Syringe with some unfortunate mod combinations
* Reworked Warbanner Remover hightlighting: Points to nearest warbanner in the world

## 0.3.1.4

* Fixed warbanner counting a finished slime rain event as an invasion
* Fixed Recipe Browser mod showing this mods' items in its bestiary irregularly
* Fixed some tooltips

## 0.3.1.3

* Fixed some effects giving negative heal when Leveled Mod is present
* Simultaneous Gasoline's fires caused by killed NPCs are limited to 5 at max
* Minor UI adjustments

## 0.3.1.2

* Add visuals for Paul's Goat Hoof and Bitter Root
* Nerf Paul's Goat Hoof stack increase
* Spikestrip slow won't apply on bosses now
* Blue slimes won't count towards warbanner kill count while King Slime is alive
* Some items that have a max stack amount will not be able to be used once the limit is reached

## 0.3.1.1

* Multiple warbanners will not give multiple heals now
* If a warbanner is about to spawn, it will only do so if you aren't already inside the range of one
* Warbanner killcount won't decrease if an invasion/event is in progress
* Warbanner killcount won't decrease and Monster Tooth won't spawn healing orbs when a statue-spawned NPC is killed

## 0.3.1

* Fixes
    * Various unintended interactions with enemies are fixed/toned down
        * Monster Tooth, Spikestrip, Taser
    * Reworked how warbanners work:
        * Instead of a chance, it's via kill count, guaranteed banner after exceeding that count
        * Count increases the more banners exist in the world
    * UI will show proper boss item drop chance in multiplayer

* Additions
    * Painting (Colossus), sold by Painter NPC
    * Nullifier
        * Drops when you kill the Wall of Flesh and have atleast one item from this mod activated
        * Allows you to get back your items for a price
        * Consumable, enables a new option in the UI (see the "?")
    * Warbanner Remover
        * Drops when you kill Skeletron and atleast one banner is active in the world
        * Allows you to remove the nearest warbanner you stand in range of. Will reset your current kill count
        * Not consumable
    * The latter two items, once unlocked, will also sell from the traveling merchant with a 25% chance

## 0.3.0.4

* Added a "?" icon on the left of the UI, giving you additional information on proc chance and boss drops

* Added "hide visuals" config settings

## 0.3.0.3

* Changed the way items drop from bosses: Now, **if** an item drops, every participating player will get a random item of that same rarity

## 0.3.0.1 + 2

* Hotfixes

## 0.3

* Initial release, 25 common items
