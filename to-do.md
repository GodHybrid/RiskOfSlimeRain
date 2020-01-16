# RISK OF SLIME RAIN

Risk of Slime Rain is a mod, that aims to bring Risk of Rain, a rogue-lite platformer into the world of Terraria. This ranges from items to generation.

*[x][ ] The first checkmark shows if the item has been coded correctly and if it works as intended. The second checkmark shows if the graphical effects has been coded properly and if they work as intended.*

## Items

-Items can crafted by hand at high price or found in specific mod-related chests and lockboxes.\
-All items are consumable and provide permanent buffs or boosts to the character.\
-Items have a specific amount of various ingredients to craft with: common - 4 ingredients, uncommon - 6, rare - 8.\
-Usable items are not craftable (found in chests? Rare drop? There should definitely be a limit on how many a player can carry. Use time limitation can be a debuff akin to Potion Sickness)\
-Items have a use cap after which they become ineffective.

**All items need their sprites/effects redrawn/resized to fit Terraria's aesthetic**
**All damage scaling/percentage is based off of the currently held weapon. This is a subject to change.**

#### Common items
###### Offensive items
* [x][x]Barbed Wire - encircles the player in a small, grey, jagged circle, damaging enemies who come within. If several enemies are within range, only one of them will receive the damage.\
**Stacking effect:** +20% larger radius, +17% damage/sec.\
		* Damage scales properly. Only one enemy receives damage at a time.\
		* Wire sprite draws correctly (only first stage). Circle showing the radius draws correctly
* [x][x]Crowbar -  on hitting an enemy, deal +50% damage to enemies above 80% health.\
**Stacking effect:** +30% damage.\
		* Damage increases properly.\
		* On-hit sound and graphics work correctly.
* [x][x]Gasoline - on killing an enemy, burns the ground for 60% damage for 2 seconds and burns target.\
**Stacking effect:** +40% damage.\
		* Applies burning and damages enemies properly.
* [x][x]Headstompers - damage enemies by falling on them.\
**Stacking effect:** +30% max damage.\
		* Applies damage correctly. *DOES NOT WORK IN MULTIPLAYER.*\
		* Missing damage sounds (not needed, the on-hit default sound does the job).
* [x][x]Mortar Tube - on hitting an enemy, the player has a 9% chance to fire a mortar for 170% damage and deal explosive damage in a radius of 3 blocks.\
**Stacking effect:** +170% damage.\
		* Activates and applies damage properly.\
		* Animations work as intended.
* [x][x]Rusty Knife - on hitting an enemy, the player has a 15% chance to cause bleed debuff for 4x35% damage.\
**Stacking effect:** 15% extra chance to bleed, 100% chance at 7 stacks.\
		* Applies buff properly. The buff works as intended.\
		* Draws correctly.
* [x][x]Sticky Bomb - on hitting an enemy, the player has a 8% chance to attach a bomb, detonating for 140% damage and deal explosive damage in a radius of 1 block.\
**Stacking effect:** +40% damage.\
		* Damages the enemy and activates properly.
* [x][x]Bundle of Fireworks - fires fireworks each time a Chest or Container is opened, as well as any time you activate Shrines, Shops, Roulette Chambers, and when picking up Drones.\
**Stacking effect:** +2 Fireworks launched.\
		* Works under a different effect: 1% chance killed enemy launches 4 fireworks. Chance increases per stack (+ 0.5%).\
		* Effects work properly.

###### Defensive items
* [x][ ]Bitter Root - increases the player's maximum health by 8% and caps at 300% extra base health.\
**Stacking effect:** Further gain 8% max HP, up to 38 stacks (300% extra HP).\
		* Increases HP properly.\
		* No leaf visuals yet.
* [x][x]Bustling Fungus - heals the player for 4.5% of their health after 2 seconds out of combat and being completely immobile.\
**Stacking effect:** 4.5% more HP/s increase, increased radius of effect.\
		* Heals the player and friendly NPCs properly.\
		* Visuals and sounds work correctly.\
        * Same radius all times.
* [x][x]Meat Nugget - on hitting an enemy, the player has a 8% chance to drop 2 meat nuggets that heal for 6 health.\
**Stacking effect:** +6 HP per meat nugget.\
		* Heals the player properly.\
		* Effects work properly.
* [x][x]Medkit - restores 10 health after a short delay after being hit.\
**Stacking effect:** +10 HP restored.\
		* Heals the player properly.\
		* Sprites and sounds play out properly.
* [x][ ]Monster Tooth - on killing an enemy, heals 10 health.\
**Stacking effect:** +5 more health after kill.\
		* Heals the player properly.\
		* Healing projectile missing.
* [x][ ]Mysterious Vial - increases health regeneration by 1.2 HP/s.\
**Stacking effect:** +1.2 HP/s.\
		* Increases the regeneration rate properly.\
* [x][x]Sprouting Egg - increases the player's health regeneration by 2.4 HP/s when out of combat for 7 seconds.\
**Stacking effect:** +2.4 HP/s.\
		* Increases the regeneration rate properly.\
		* Player texture needs to be redone, dust works properly.

###### Utility items
* [x][x]Fire Shield - causes an explosion that deals 200% damage after being hit for 10% of player's maximum health in one hit.\
**Stacking effect:** Increases explosion damage by 200%, and increases knockback by 20%.\
		* Works as intended.\
* [x][x]Hermit's Scarf - gives the player a 10% chance to 'evade' the incoming damage.\
**Stacking effect:** Increases dodge chance +5%, up to a maximum of 35% at 6 stacks.\
		* Works as intended.\
		* Graphics work as intended.
* [x][x]Lens Maker's Glasses - increases the critical strike chance by 7%.\
**Stacking effect:** +7% crit chance. 100% chance to crit on 14 stacks.\
		* Works as intended.\
		* Graphics work as intended. 
* [x][x]Life Savings - generates 1 copper coin every 3 seconds.\
**Stacking effect:** +1 copper generation rate.\
		* Works as intended. (Gives money whenever player opens inventory)
* [x][ ]Paul's Goat Hoof - increases movement speed by 20%.\
**Stacking effect:** Further increases movement speed by 20%, caps at around 25.\
		* Works as intended. *Definitely needs rebalancing.*\
		* Missing effect sprites.
* [x][x]Snake Eyes - falling below 10% increases the critical chance by 6%. The effect 6 times.\
**Stacking effect:** Further increases crit chance by 3% for each time the player's in peril.\
		* Works as intended. *As the shrines don't exist yet, the activation requirements have been changed. Will need to change them once the shrines are in.*\
		* Graphics work as intended.
* [x][ ]Soldier's Syringe - Increased attack speed by 15%.\
**Stacking effect:**  further increase by 15%, up to 13 times, maxing at +195%.\
		* Works as intended.\
		* Missing shader.
* [x][x]Spikestrip - on hit drop spikestrips that slow enemies by 20%.\
**Stacking effect:** Increases the duration of spikestrips by 1 second per stack.\
		* Works as intended.\
		* Arrow on enemy draws properly.
* [x][x]Warbanner - On level up drop a banner. Raise attack/move speed by 30% and damage by 4.\
**Stacking effect:** Area of effect increased by 40% of base size.\
		* The banners spawn and increase stats properly. *As the leveling system doesn't exist yet, the activation requirements have been changed. Will need to change them once the leveling system is in.*\
		* Effect sounds unfinished. Player sprite draws correctly.
* [x][ ]Taser - 7% chance to snare enemies for 1.5 seconds.\
**Stacking effect:** Increases snare duration by 0.5 seconds.\
		* Works as intended. *Nullifies X-axis velocity. Might want to increase Y-axis velocity to make flying enemies fall?*\
		* Graphics work as intended.

#### Uncommon items **-NONE IMPLEMENTED-**
###### Offensive items
* [ ][ ]Golden Gun - Deals bonus damage per gold coins, up to 40% damage at 70 platinum coins.\
**Stacking effect:** Halves the gold required for the maximum damage bonus.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked fill-up bar sprite. *Perhaps lock it somewhere next to the map/inventory?*)
* [ ][ ]AtG Missile Mk.1 - On hitting an enemy, the player has a 10% chance of firing a missile that causes an explosion for 300% damage. These missiles cannot critically strike.\
**Stacking effect:** +10% chance to fire missile\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked texture + missiles)
* [ ][ ]Boxing Gloves - On hitting an enemy, the player has a 6% chance to knockback the enemy. The power of the knockback depends on the strength of the attack.\
**Stacking effect:** 6% multiplicative increase in knockback chance.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (On-hit enemy-locked texture.)
* [ ][ ]Chargefield Generator - On killing an enemy, generates a lightning ring, dealing 100% damage/sec to one enemy for 6 seconds. Killing additional enemies while the ring is active makes the ring larger and resets the 6 seconds.\
**Stacking effect:** +10% damage/sec.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked animation. *It's animated, maybe make it not animated? Or use dusts.*)
* [ ][ ]Predatory Instincts - +5% crit chance. On getting a critical hit on an enemy, subsequent critical strikes increase attack speed by 10% for 3 seconds. Stacks up to 30%.\
**Stacking effect:** +5% crit chance, +1% extra attack speed on crit\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked texture.)
* [ ][ ]Toxic Worm - Infects any enemy that passes over the player, dealing 50% damage per second to that enemy. The effect of this item bounces to other enemies on death.\
**Stacking effect:** +1 simultaneous infection target.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked animation + enemy-locked texture.)
* [ ][ ]Will-o'-the-Wisp - On killing an enemy, the player has a 33% chance on killing an enemy to create an explosive lava pillar for 500% damage.\
**Stacking effect:** +100% damage increase.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (On-death enemy-locked animation.)
* [ ][ ]Ukulele - On hitting an enemy, the player has a 20% chance to fire chain lighting for 4x33% damage.\
**Stacking effect:** +33% damage per bounce.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (On-hit animation.)
* [ ][ ]Frost Relic - on killing an enemy, surrounds the player with 3 icicles that deal 3x33% damage.\
**Stacking effect:** +1 additional icicle.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked animation.)

###### Defensive items
* [ ][ ]Leeching Seed - On hitting an enemy, heals the player for 2 health.\
**Stacking effect:** +1 health on hit.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked dust.)
* [ ][ ]Infusion - On killing an enemy, permanently increases player's health by 1.\
**Stacking effect:** +0.5 health on kill.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Projectiles + projectile-locked dust.)
* [ ][ ]Harvester's Scythe - On getting a critical hit on an enemy, gain additional 5% critical chance. Critical strikes heal for 12 health.\
**Stacking effect:** +5% critical chance and +2 health on critical strike.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked texture.)
* [ ][ ]Guardian's Heart - Gives the player a 60 health shield. Recharges in 7 seconds.\
**Stacking effect:** +60 Shield.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Heart texture + player-locked texture.)
* [ ][ ]Dead Man's Foot -  At low health, the player drops a mine at their feet, which detonates once an enemy passes over it, causing poison damage to the affected enemies.\
**Stacking effect:** +1 poison tick.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked dust + projectiles.)
* [ ][ ]Panic Mines - drop a landmine when the player reaches low health that deals 500% damage.\
**Stacking effect:** +1 mine dropped.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Projectiles.)
* [ ][ ]Tough Times - grants the player 14 armor. *Reduces the damage by ~12.3%.*\
**Stacking effect:** +14 armor.\
		* The item hasn't been coded yet.\
		* Effects aren't required.

###### Utility items
* [ ][ ]Hopoo Feather - Gives the player an additional jump.\
**Stacking effect:** +1 extra jump.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (On-jump animation.)
* [ ][ ]56 Leaf Clover - gives Elite mobs a 4% chance to drop items.\
**Stacking effect:** +1.5% item drop chance, up to a maximum of 100% at 67 stacks.\
		* The item hasn't been coded yet.\
		* Effects aren't required.
* [ ][ ]Arms Race - On drone action: 9% chance for drones to fire missiles and mortars.\
**Stacking effect:** +10% chance to fire missles, +170% more mortar damage.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Drone-locked textures.)
* [ ][ ]Concussion Grenade - On hitting an enemy, the player has a 6% chance to stun enemies for two seconds.\
**Stacking effect:** +6% chance to stun enemies, multiplicatively.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked dusts + enemy-locked animation.)
* [ ][ ]Energy Cell - When below 50% health: increases attack speed; maxes out at 10% health with +40% attack speed.\
**Stacking effect:** +20% increased maximum attack speed, caps at 90%.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked animation.)
* [ ][ ]Filial Imprinting - Hatch a creature who drops attack speed/health regen/move speed buffs every 20 seconds.\
**Stacking effect:** +1 hatched creature.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Minions.)
* [ ][ ]Smart Shopper - Killed enemies drop 25% more coins.\
**Stacking effect:** +25% enemy coin drops.\
		* The item hasn't been coded yet.\
		* Effects aren't required.
* [ ][ ]Rusty Jetpack - Decrease gravity by 50% and increase jump height by 10%.\
**Stacking effect:** +10% jump height.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (On-jump animation.)
* [ ][ ]Red Whip - Leaving combat for 1.5 seconds boosts your movement speed by 80%.\
**Stacking effect:** **Does not stack.**\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked dusts/animation.)
* [ ][ ]Prison Shackles - On hitting an enemy, slow enemies by 20%.\
**Stacking effect:** +0.5s slowdown duration.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Enemy-locked texture.)
* [ ][ ]Time Keeper's Secret - Falling to low health stops time for 3 seconds.\
**Stacking effect:** +1s time stop duration, up to a maximum of 10 seconds at 8 stacks.\
		* The item hasn't been coded yet.\
		* Effects aren't required.

#### Rare items **-NONE IMPLEMENTED-**
###### Offensive items
* [ ][ ]Hyper-Threader - On hitting an enemy, fire a laser, dealing 40% damage and bouncing to 2 enemies.\
**Stacking effect:** +1 extra bounce.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Projectiles.)
* [ ][ ]Heaven Cracker - Every 4th basic attack pierce through enemies for 100% damage.\
**Stacking effect:** Reduce attacks required by 1 per additional Heaven Cracker. Caps at 4.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked animation.)
* [ ][ ]Fireman's Boots - Walking leaves behind a fire trail that burns for 35% damage.\
**Stacking effect:** +20% damage.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Projectiles.)
* [ ][ ]AtG Missile Mk.2 - On hitting an enemy, with a 7% chance fire three missiles that cause explosions, dealing 300% damage.\
**Stacking effect:** +7% chance to fire missiles.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked texture + projectiles.)
* [ ][ ]Brilliant Behemoth - Makes all attacks explode for a bonus 20% damage to nearby enemies.\
**Stacking effect:** +20% damage.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (On-hit animation.)
* [ ][ ]Ceremonial Dagger - On killing an enemy, fire out 4 heat-seeking bolts that deal 100% damage.\
**Stacking effect:** +2 extra bolts.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Projectiles.)
* [ ][ ]Telescopic Sight - On hitting an enemy, 1% chance to instantly kill the enemy.\
**Stacking effect:** +0.5% proc chance up to a maximum of 3%. Stacks at 5.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (*Something?*)
* [ ][ ]Shattering Justice - On hitting an enemy, attacks reduce enemy armor by 5. Reduction stacks up to 25.\
**Stacking effect:** +0.5s debuff length.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Enemy-locked texture.)
* [ ][ ]Plasma Chain - On hitting an enemy, chance to tether onto enemies dealing 60% damage per second.\
**Stacking effect:** +1 max tether.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-enemy-locked texture/dust.)
* [ ][ ]The Ol' Lopper - On hitting an enemy, 100% critical chance on enemies below 9% health.\
**Stacking effect:** +4% health threshold, up to a cap of 35% health at 8 stacks.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (*Something?*)
* [ ][ ]Tesla Coil - Shock nearby enemies for 150% damage.\
**Stacking effect:** +50% damage.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked animations.)
* [ ][ ]Thallium - On hitting an enemy, has a 10% chance to damage by 500% of enemy damage and slow by 100% speed over 3 seconds.\
**Stacking effect:** Does not stack.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Enemy-locked texture.)
* [ ][ ]Laser Turbine - All attacks charge the generator. At full power, fire a laser for 2000% damage.\
**Stacking effect:** Speeds up the charge.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Animation.)

###### Defensive items
* [ ][ ]Dio's Best Friend - Taking fatal damage revives the player to 40% health. Invincible for 2 seconds. Disappears upon activation.\
**Stacking effect:** Does not stack.\
		* The item hasn't been coded yet.\
		* Effects aren't required.
* [ ][ ]Repulsion Armor - After 6 hits reduce and reflect incoming damage by 83% for 3 seconds.\
**Stacking effect:** Increases length of reflection by 1 second to a cap of 8 seconds.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked animation + player-locked texture.)
* [ ][ ]Interstellar Desk Plant - On killing an enemy, spawn an alien plant that heals the player for 8 health.\
**Stacking effect:** +3 health per fruit.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Projectiles.)
* [ ][ ]White Undershirt (M) - Increases armor by 3.\
**Stacking effect:** +3 armor.\
		* The item hasn't been coded yet.\
		* Effects aren't required.

###### Utility items
* [ ][ ]Old Box - Drop a jack-in-the-box at low health, fearing enemies for 2 seconds.\
**Stacking effect:** +2% health activation range.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Projectile + enemy-locked animation.)
* [ ][ ]Happiest Mask - On hit with a 15% chance cause 25% of dealt damage to all enemies on screen.\
**Stacking effect:** +15% damage.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Enemy-locked textures.)
* [ ][ ]Ancient Scepter - Upgrade your fourth ability. *Needs to be remade into something else probably, at least for not-RoR mode.*\
**Stacking effect:** Does not stack. *Might stack in the future.*\
		* The item hasn't been coded yet.\
		* Effects aren't required.
* [ ][ ]Alien Head - Decreases player's cooldowns between attacks by 25%. Caps at 60%.\
**Stacking effect:** -25% cooldown, multiplicatively.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked dusts.)
* [ ][ ]Beating Embryo - Use items have a 30% chance to deal double the effect.\
**Stacking effect:** +30% duplication chance. Caps at 4.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (*Something?*)
* [ ][ ]Rapid Mitosis - Reduce the cooldown of use items by 25%.\
**Stacking effect:** -25% cooldown, multiplicatively.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (*Something?*)
* [ ][ ]Permafrost - 12% chance to stun enemies for 1.5 seconds and slow enemies by 60% for three seconds.\
**Stacking effect:** +6% chance, multiplicatively.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (*Something?*)
* [ ][ ]Photon Jetpack - Allows the player to fly for up to 1.6 seconds nonstop; the jetpack's fuel tank quickly refills when not in use.\
**Stacking effect:** +0.8s max flight duration.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked animation + player-locked texture.)
* [ ][ ]The Hit List - Killing a randomly marked enemy permanently increases damage by 0.5.\
**Stacking effect:** +1 simultaneous markable enemy.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Enemy-locked animation + UI-locked texture.)
* [ ][ ]Wicked Ring - Critical strikes reduce cooldowns by 1 second. +6% crit chance.\
**Stacking effect:** +1s cooldown reduction on crit, +6% critical chance.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (*Something?*)
* [ ][ ]Keycard - Opens certain doors.\
**Stacking effect:** +1 keycard.\
		* The item hasn't been coded yet.\
		* Effects aren't required.

#### Boss items **-NONE IMPLEMENTED-**
###### Offensive items
* [ ][ ]Ifrit's Horn - On hitting an enemy, the player has a 8% chance to incinerate enemies for 220% damage.\
**Stacking effect:** +30% damage.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (*Something?*)
* [ ][ ]Burning Witness - On killing an enemy, grants a firetrail buff, 5% movement speed, and 1 damage for 6 seconds.\
**Stacking effect:** +5% movement speed, +1 second duration.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Projectiles.)
* [ ][ ]Legendary Spark - On hitting an enemy, the player has a 8% chance to smite enemies for 200% damage.\
**Stacking effect:** +1 spark.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Enemy-locked animations.)

###### Defensive items
* [ ][ ]Colossal Knurl - Increase health by 40, health regeneration by 1.2/second, and armor by 6.\
**Stacking effect:** +40 health, +1.2 regen, +6 armor.\
		* The item hasn't been coded yet.\
		* Effects aren't required.

###### Utility items
* [ ][ ]Imp Overlord's Tentacle - Summon an imp as a bodyguard; the imp revives every 60 seconds.\
**Stacking effect:** Increases imp's strength.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Minion.)

#### Usable items **-NONE IMPLEMENTED-**
* [ ][ ]Gigantic Amethyst - Resets all cooldowns.\
		* The item hasn't been coded yet.\
		* Effects aren't required. 
* [ ][ ]Thqwib - Releases a bloom of 30 thqwibs, detonating on impact for 200%.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Projectiles.)
* [ ][ ]Captain's Brooch - Calls down a chest nearby.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Animation.)
* [ ][ ]Carrara Marble - Places a marble gate. Teleports back to the gate by activating again.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Texture.)
* [ ][ ]Crudely Drawn Buddy - Blows up a decoy, attracting enemies for 8 seconds.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Texture.)
* [ ][ ]Disposable Missile Launcher - Fires a swarm of 12 missiles.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Projectiles.)
* [ ][ ]Drone Repair Kit - Repairs all drones to full health.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Drone-locked animations.)
* [ ][ ]Foreign Fruit - Heals the player for 50% health.\
		* The item hasn't been coded yet.\
		* Effects aren't required.
* [ ][ ]Glowing Meteorite - Meteors fall from the sky, damaging enemies and friends for 220% damage. Lasts 8 seconds.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Full-screen shader/filter + animation.)
* [ ][ ]Gold-Plated Bomb - Uses 50% of player's gold/platinum coins to create a bomb, dealing 10 damage per gold spent.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Projectile.)
* [ ][ ]Dynamite Plunger - Hitting an enemy drops dynamite. Upon use detonates for 400% damage.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Projectiles.)
* [ ][ ]Instant Minefield - Drops 7 mines at player's feet, each dealing 400% damage.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Projectiles.)
* [ ][ ]Jar of Souls - Deals 50% of dealt damage to all enemies on screen for 8 seconds.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Enemy-locked animations.)
* [ ][ ]Lost Doll - Sacrifices 25% of player's health to damage an enemy for 500% of player's maximum health.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked animation.)
* [ ][ ]Massive Leech - For 10 seconds, every hit heals the player for 10 health.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked texture.)
* [ ][ ]Nematocyst Nozzle - Shoots out 6 nematocysts that deal 200% damage.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Projectiles.)
* [ ][ ]Pillaged Gold - Makes every monster drop 1 copper/silver coin on hit for 14 seconds.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked texture.)
* [ ][ ]Prescriptions - Increases damage by 10 and attack speed by 40% for 8 seconds.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked animation/)
* [ ][ ]Rotten Brain - Throws a brain that bounces in place, damaging/slowing enemies for 200%. Lasts 6 seconds.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Projectile.)
* [ ][ ]Safeguard Lantern - Drops a lantern for 10 seconds which fears and damages enemies for 20% damage.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Projectile + enemy-locked animation.)
* [ ][ ]Sawmerang - Throws out a sawmerang for 500% damage and bleed for 4x100%.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Projectile.)
* [ ][ ]Shattered Mirror - Doubles all of player's abilities' damage and effects for 15 seconds.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked animation/texture.)
* [ ][ ]Shield Generator - Grants invincibility for 8 seconds.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked animation.)
* [ ][ ]Skeleton Key - Opens all chests in view.\
		* The item hasn't been coded yet.\
		* Effects aren't required. 
* [ ][ ]Snowglobe - Summons a snowstorm that freezes monsters at a 30% chance over 7 seconds.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Full-screen shader/filter)
* [ ][ ]The Back-Up - Creates 4 drones for 10 seconds.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Minions.)
* [ ][ ]Unstable Watch - Stops time for 7 seconds.\
		* The item hasn't been coded yet.\
		* The effects haven't been coded yet. (Player-locked animation.)

#### Other items
* [x][x]Uncovered Chest - A standard chest.
* [ ][x]Salvaged Chest - A 3x2 chest with 2 extra lines of storage slots.
* [ ][ ]Regal Chest - A 4x3 chest with x2.5 the storage size.
* [ ][ ]Insignificant Container - A 1x2 chest with half the storage size.
* [ ][ ]Shellproof Container - A 1x2 chest with half the storage size.
* [ ][ ]Common Item Lockbox - Spawns a random common item upon opening.
* [ ][ ]Uncommon Item Lockbox - Spawns a random uncommon item upon opening.
* [ ][ ]Rare Item Lockbox - Spawns a random rare item upon opening.
* [ ][ ]Small Enigma - Reduces use time of usable items by 5%, multiplicatively.

## NPC
#### Town/Wandering NPC *I dunno, gotta give them reasons to exist. They'd be perfect town protectors, but I want to make the players think about where they put the NPCs and how they interact with them.*
* [ ][ ]Acrid - 
* [ ][ ]Bandit - *Sells lockboxes?*
* [ ][ ]Chef - 
* [ ][ ]Commando - 
* [ ][ ]Enforcer - *Gives other town NPCs next to him a defense buff?*
* [ ][ ]Engineer - 
* [ ][ ]HAN-D - 
* [ ][ ]Huntress - 
* [ ][ ]Loader - 
* [ ][ ]Mercenary - *Gives out elite enemies to slay for money/item rewards?*
* [ ][ ]Miner - *Sells special explosives that work even in RoR mode?*
* [ ][ ]Sniper - *Reduces enemy spawn rate by 80% if there are no more than 4 town NPCs around?*

#### Enemy NPC
* [ ][ ]Lemurian
* [ ][ ]Rock Golem
* [ ][ ]Wisp
* [ ][ ]Greater Wisp
* [ ][ ]Sand Crab
* [ ][ ]Jellyfish
* [ ][ ]Child Spitter
* [ ][ ]Tiny Imp
* [ ][ ]Black Imp
* [ ][ ]Mushrum
* [ ][ ]Whorl
* [ ][ ]Clay Man
* [ ][ ]Bighorn Bison
* [ ][ ]Mechanical Spider
* [ ][ ]Gup
* [ ][ ]Parent
* [ ][ ]Evolved Lemurian
* [ ][ ]Temple Guard
* [ ][ ]Elder Lemurian
* [ ][ ]Archer Bug
* [ ][ ]Boarlit
* [ ][ ]Armored Boar
* [ ][ ]Young Vagrant
* [ ][ ]Archaic Wisp
* [ ][ ]Purple Imp

#### Minions/Drones
* [ ][ ]Basic Drone
* [ ][ ]Attack Drone
* [ ][ ]Healing Drone
* [ ][ ]Laser Drone
* [ ][ ]Flame Drone
* [ ][ ]Missile Drone
* [ ][ ]Advanced Healing Drone

#### Bosses
* [ ][ ]Colossus
* [ ][ ]Wandering Vagrant
* [ ][ ]Magma Worm
* [ ][ ]Ancient Wisp
* [ ][ ]Imp Overlord
* [ ][ ]Ifrit
* [ ][ ]Toxic Beast
* [ ][ ]Cremator
* [ ][ ]Scavenger
* [ ][ ]Providence
* [ ][ ]Gilded Wurms

## Shrines
* [ ][ ]Money
* [ ][ ]Absolute Health
* [ ][ ]Percentage Health
* [ ][ ]Imp
* [ ][ ]Shop
* [ ][ ]Roulette

## Artifacts
* [ ][ ]Honor
* [ ][ ]Kin
* [ ][ ]Distortion
* [ ][ ]Spite
* [ ][ ]Glass
* [ ][ ]Enigma
* [ ][ ]Sacrifice
* [ ][ ]Command
* [ ][ ]Spirit
* [ ][ ]Origin

## Biomes

-The biomes(or just structures?) are a part of the map it's based off of. Example: long lava pool with metallic platforms attached to chains - Magma Barracks.\
-Map-exclusive enemies will have a much higher spawn-rate in their correspondent biome. In every other biome the chances are 1%.\
-Map-exclusive bosses will only spawn in their correspondent biome.


* [ ][ ]Ancient Valley (*Surface-level tall bridges?*)
* [ ][ ]Boar Beach (*Series of small floating islands in snow biome?*)
* [ ][ ]Damp Caverns (*Big mushroom pillars in glowing mushroom biomes?*)
* [ ][ ]Desolate Forest (*I dunno, thicker forest with rocky hill? Could put it in corruption/crimson?*)
* [ ][ ]Dried Lake (*Two-level scaffolding with ropes at ocean/beach?*)
* [ ][ ]Hive Cluster (*Sticky pink stuff near underground jungle hives?*)
* [ ][ ]Magma Barracks (*Hell-level metallic platforms and chains, flat area for lava?*)
* [ ][ ]Sky Meadow (*Space-level long islands?*)
* [ ][ ]Sunken Tomb (*Underground water biome/ocean gets deeper and more rocky?*)
* [ ][ ]Temple of the Elders (*A temple in front of the dungeon? Or under the dungeon as a part of it.*)
* [ ][ ]Risk of Rain (*Literally no idea, maybe a special generated area in the sky above ocean, like Lihzahrd Temple?*)

## Gameplay **-Subject to change, not fully fleshed out-**
* [ ][ ]RoR mode\
		* Item to activate the mode with (cannot be deactivated)\
		* Vanilla chests become locked, can only be opened with money, do not regenerate the items\
		* Mod chests/containers become locked, can only be opened with money, regenerate the items (??% chance) during special event\
		* Disabling of armors/accesories/pets\
		* Leveling system\
		* Enemy scaling system\
		* No drills/pickaxes, explosives no longer destroy blocks (unless bought from Miner)
* [ ][ ]Random chests generation in the overworld\
		* Chest generation on meteor falling OR on specific event\
		* Opening chests with money\
		* Indestructible chests with respawnable items (locking again)
* [ ][ ]Random teleports throughout the world, connected between each other in pairs (not RoR mode exclusive)
* [ ][ ]Elite enemies and bosses