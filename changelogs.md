# Changelogs

## 0.3.2

* New sprites!
* Balance!
    * Added a new config (labeled "Server Config") containing various balance options
        * "Original Stats": If enabled, you keep using the same stats for items as in previous updates. Disabled by default
        * "Difficulty Scaling": If enabled, the game becomes harder the more powerful you are. More infos on the "?" UI
        * You can find all the balance changes [here](https://github.com/GodHybrid/RiskOfSlimeRain/blob/master/OriginalStatsvsBalancedStats.md#original-stats-vs-balanced-stats)
        * Please give feedback about the new balance on our homepage!
* Item aquisition rework!
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
