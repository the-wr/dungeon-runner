# Dungeon Runner

A prototype of an online collectible-card-game, that's a somewhat simplified version of a popular Netrunner CCG. Made with Unity.

![Screenshot](screenshot.png?raw=true)

# How to use

This game relies on hidden information and should be run on two separate machines. However, it _can_ be run on one machine for demonstration purposes.

1. If run on one machine, start DungeonRunner.exe _three_ times at once. If run on two machines, one shound run DungeonRunner.exe _two_ times (one being used as a server), the other machine should just run it once.
2. In one of the windows push "Start Server", then minimize it.
3. In two other windows join the server. Those are the players.
4. With one client start as Runner, the other should be Overlord.
5. Take turns, have fun :)

Note: This works through Unity relay server, which is limited to 10 connections in personal version. So this prototype is somewhat limited to 3 concurrent games at once: (2 players + 1 server) x 3 = 9 connections :)

# Rules

The goal for Runner player is to defeat the Overlord (reduce their Life to 0).
The goal for Overlord player is to execute their Masterplan (reduce their Masterplan Steps to 0).

Overlord has three dungeon wings. At the core of each wing is a Dungeon Heart, that Overlord must protect. Overlord builds protective rooms in each wing. Rooms are placed for free, but they start working only when activated. Activation costs mana. Room details and activation state are not visible to Runner until they enter the room, thus making rooms "surprise traps" for the Runner. Rooms can be replaced, i.e. new room can be built on top of an old one.

Runner recruits heroes to fight their way to Dungeon Hearts. Recruiting heroes cost mana, and new heroes can't attack the same turn they were recruited. Runner's goal is to attack dungeon wings with their heroes and destroy each Dungeon Heart. Runner does not know what rooms are there, and are they activated or not, until they enter the room. Before entering each room, Runner has a choice to retreat, saving heroes from an unlikely encounter, but ending the turn.

When in a room, heroes must defeat it to advance further. Runner picks a hero to fight the room, then they deal damage (bottom-left number) to each other. If a hero takes as much damage as their life (bottom-right number), it is defeated and disappears. If a room takes as much damage as it's life, it is defeated and heroes advance further into dungeon. During a room encounter, there are no limits how many times each hero can may fight the room.

At the end of Runner's turn, Overlord loses one life point for each Dungeon Heart destroyed, and advances his Masterplan by one step for each Dungeon Heart survived. Then all rooms are healed restored (but not heroes!), mana pool is replenished and increased by 1.

Whoever reaches their goal first - wins!

(Note: Server must be restarted to play again!)

# Note

This prototype demonstrates the very basics of game mechanics. The final game is expected to have Hero and Room abilities, spells, traps, enchantments and more! :)
