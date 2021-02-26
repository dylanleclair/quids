# quids

![intro](https://media.giphy.com/media/E807dDxYuoZ6QCxg3C/giphy.gif)

Quidditch is a game in the world of Harry Potter, where different Hogwarts houses duke it out, chasing a golden snitch to earn points. This is an implementation of such a game in Unity, with a number of behaviours being used to model what an actual game might look like. 

In this version, there are only the Gryffindor and Slytherin houses. The red and green spheres transparent spheres you see in the previews are the bases for each team.  

Players and the snitch all have a directional line sticking out of them, indicating where they are headed. 

## Snitch Behaviour

One of the trickiest but coolest challenges of this project was creating random, yet smooth motion for the snitch. 


![snitchmvmnt](https://media.giphy.com/media/Sh5ZyrcP4BI5jpXVAc/giphy.gif)

After playing around and trying to add small, random forces to the snitch using Unity's rigid body and direction system, I couldn't find any way to use this approach to give me nice and smooth motion. 

The perfectionist in me knew there was a better way - I ended up paramaterizing a "target" position using spherical coordinates from the center of the play area. 

The snitch is drawn to this target position, and upon reaching it, a new target position is created by adding small random values to the angle and radius of the previous target. This will ensure that the next target position is relatively close by and is not an extreme jump in angle. 

After refining this and finding a set of parameters that fit just right, I made sure to clamp the radius value so that the snitch wouldn't end up just smashing into a wall (which was the motivation behind this system!).

It was unclear what to do when the snitch was caught by a player, so I simply had it start at a new random position within the play area (as seen in the introductory gif).

This was the part of the assignment that took me (embarassingly) the most time, since the physics system was giving me a lot of trouble (and I wanted the result to be *just right*) ;-; 

## Player behaviour

Every player has a set of forces:

- A force that attracts it to the snitch
- Forces pushing it away from nearby players
- Forces to avoid the walls of the play area

Calculating the direction to the snitch was simple enough - it was just the vector from the position of the player to the snitch's position. 

I used a SphereCast to check for players in a radius of each player, adding a small force for each of these. These go in the direction of nearby players to the player in focus, since it made the most sense to me that way and had rather good success when I was testing it out. 

#### In addition to these, there is also a set of behaviours as follow:

| Respawn after unconscious | Exhausted player |
| ---------------- | ---------------- |
| ![a quid falling unconscious and respawning](https://media.giphy.com/media/VIDgtd0BNHrYVPe1OI/giphy.gif) | ![an exhausted quid](https://media.giphy.com/media/WdQL8Fi38zMa1Nu9zR/giphy.gif) |


### Collisions with other players

Upon colliding with another player, a player may be rendered unconscious - causing them to fall to the ground. When this happens, they are teleported to their team's base where they wait a few seconds before getting back into play. 

### Exhaustion

Every player also has a value for exhaustion. Players become exhausted according to their velocity. Exhausted players will briefly pause to regain energy, before chasing the snitch once more. 

After becoming exhausted a number of times, a player may be rendered unconscious, causing them to go through the unconscious behaviour as described above.

