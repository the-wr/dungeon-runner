# Dungeon Runner

A prototype of an online collectible-card-game, that's a somewhat simplified version of a popular Netrunner CCG. Made with Unity.

![Screenshot](screenshot.png?raw=true)

How to use:

This game relies on hidden information and should be run on two separate machines. However, it _can_ be run on one machine for demonstration purposes.

1. If run on one machine, start DungeonRunner.exe _three_ times at once. If run on two machines, one shound run DungeonRunner.exe _two_ times (one being used as a server), the other machine should just run it once.
2. In one of the windows push "Start Server", then minimize it.
3. In two other windows join the server. Those are the players.
4. With one client start as Runner, the other sould be Overlord.
5. Take turns, have fun :)

Note: This works through Unity relay server, so clients can be run on different machines.

# Rules

The goal for Runner player is to defeat the Overlord (reduce their Life to 0).
The goal for Overlord player is to execute their Masterplan (reduce their Masterplan Steps to 0).

Overlord has three dungeon wings. At the core of each wing is a Dungeon Heart, that Overlord must protect. Overlord builds protective rooms in each wing. Rooms are placed for free, but they start working only when activated. Activation costs mana.

Runner recruits heroes to fight their way to Dungeon Hearts. Recruiting heroes cost mana, and new heroes can't attack the same turn they were recruited. Runner's goal is to attack dungeon wings with their heroes and destroy each Dungeon Heart.

At the end of Runner's turn, Overlord loses one life point for each Dungeon Heart destroyed, and advances his Masterplan by one step for each Dungeon Heart survived. Then all rooms are healed restored (but not heroes!).

Whoever reaches their goal first - wins!

(Note: Server must be restarted to play again!)
