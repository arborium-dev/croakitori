# croakitori
## this is a game where food is illegal, and you do crimes.
Taishi (Jeremiah Jr.) opens a sketchy underground chinese restaurant much to Dazhi’s dismay, Now Jeremiah (Taishi) must venture off to prove the beauty of chinese cuisine to his father and venture from his home in Guandu, to Wulai to make his father proud.

### COMPILING FROM SOURCE NOTES:

To compile the game, download the source code from this repository, and open it in Unity 6000.3.8f1. Then, click on File > Build Settings, select your platform, and click Build.

***If you don't want to compile the game from source, a playable build is available at the following link or in the GitHub Releases: https://arboriumdev.itch.io/croakitori*** 

a web build for croakitori will be available soon™ (the file size is too big, i need to redo the cutscene system)

### Cast:
- Jeremiah Taishi Wang Jr. - The son of Dazhi who owns sketchy Chinese restaurant (yellow, fat)
- Jeremiah Dazhi Wang Sr. - Rejected his Chinese heritage when Japan conquered Taiwan (comically big eyebrows, massive)

### Gameplay:
- Explore the underground world of food in Taiwan using your toungue.
- Cook extravagant dishes to impress your father and gain respect in the culinary world.
- Finally seek your fathers approval and prove that Chinese cuisine is worth celebrating.

### Screenshots:

![frog using grapple](<Screenshots/Screenshot 2026-07-16 183322.png>)
![frog jumping](<Screenshots/Screenshot 2026-07-16 183331.png>)
![cooking](<Screenshots/Screenshot 2026-07-16 183354.png>)
![sunset](<Screenshots/Screenshot 2026-07-16 183404.png>)
![how to cook](<Screenshots/Screenshot 2026-07-16 183415.png>)
![food???](<Screenshots/Screenshot 2026-07-16 183427.png>)

Pictures from development and gameplay of Croakitori can be found in the Screenshots folder.

A full gameplay video is also available at: https://youtu.be/sL2dV9rfkVY (this video is a touch outdated, as its missing some small tweaks to scene transitions, but its basically the same game)

### Controls:
#### Cooking Minigame:
- Use your mouse to select 3 spices and start the cooking process
- Arrow Keys (or WASD) to cook the dish
#### Platforming Minigame:
- Arrow Keys (or WASD) to move
- C or Z to jump
- X to extend your tongue (release to pull yourself to the point of contact)
- Controller is supported for the platforming section (although it is untested). The controls are identical to those of Celeste (a to jump, x to grapple)
#### Misc:
- P to skip cutscenes

### How to Play:
#### Cooking Minigame:
- Select spices that match the order with your mouse and place them in the cooking pot.
- Click Cook to start cooking the dish.
- Use the arrow keys to cook the dish, following the on-screen prompts.
- Rinse and Repeat until you have completed the full meal.
- Do all this before running out of time
#### Platforming Minigame:
- Move around the level collecting spices and avoiding obstacles.
- Use your tougue to reach higher platforms and grab ingredients.
### Inspiration:
UNBEATABLE and Dave the Diver inspired the Cooking section, specifically UNBEATABLE's bartending minigame (this was also the main inspiration for the music!)

Celeste (and Celeste Classic 2) inspired the platforming section.
### Credits: 
- arborium did programming for main menu, platforming and cutscenes, and did the writing, the cutscenes, and art
- Khan did programming for the cooking minigame, did the music, and came up with the story concepts
- Random images on Google for cooking art
- Various asset packs for platforming art

```text
notes for hack club reviewers:
if you want to see specifically arborium's part, look at the main menu, platforming, and cutscenes, as well as the player art.

if you want to see specifically khan's part, look at the cooking minigame and the music during the cooking cutscenes and the cooking minigame itself.
```

### Development:
This game was developed in Unity 6000.3.8f1 over the course of 3 months (although mostly the last one) for Hack Club Horizons Polaris. 

The story behind how it was actually created is a little more complex.
(I refers to arborium here)

We originally started this project about 1 week after horizons was announced, and began brainstorming instantly. We based the core gameplay loop off of Dave the Diver, where you alternate between cooking and swimming. The cooking we (basically) stole from UNBEATABLE because its one of our favorite games, and the platforming was based off of Celeste (and Celeste Classic 2). Khan created the story entirely off of the idea he wanted Joy to the World to be the last song. With all this decided, it was time to get to work.

The only problem was that Horizons had a 33% art limit, and Khan's computer can't run Unity (Godot is not the same, its a lot harder to use imo).
So, every week we met up and I gave him my laptop to work on the game, while I used a desktop computer. Eventually, Khan got to his required 35h hours, and I began development (taking back my laptop).

fun little side story here, khan forgot to upload the platforming music so I had to find a replacement, which ended up being the music Watch My Soul Speak. the music will be replaced with original music in the future.

At this point its worth mentioning that theres **2 more weeks before the game is due, and I have to get 33h** (look I was busy with exams and projects and stuff). This wouldn't be a problem if I was at home and I could work on it for 8 hours a day and be done quick. 
Somehow, I was scheduled to be at a cottage, which ended up having very strict WiFi usage limits, and with the power going out occasionally. It was at this cottage that I finished all the platforming, the cutscenes, the menu, everything else basically (i am writing this readme on a porch).

That was how Croakitori was created.

Croakitori uses code from previous projects I've worked on. The predictive platforming engine is based on one of my unfinished projects, all the UI takes from (B)eat Em Up in some way or another, and anything neither of us knew how do to we looked up or found tutorials.
AI was used for debugging, and fixing some of the more intricate parts of TheBigUI (DoesEverything.cs) and PlayerPlatforming.

NO AI WAS USED FOR ANY OF THE ART, CUTSCENES, MUSIC, OR WRITING.


We plan to expand this game to include more levels, more cooking minigames, and more story content in the future.
This version of Croakitori was originally planned to have 3 platforming levels, a cohesive story, and in game cutscenes. It was also planned that you would alternate cooking and platforming levels, and that you would get stamina after each cooking level depending on your performance. However, due to time constraints, we were unable to implement these features. We plan to add these features in the future.

We also plan to recompile the cutscenes at higher resolutions and with less visual artifacts.

### Tools Used:
- Unity 6000.3.8f1
- Aseprite
- Musescore
- Audacity
- GitHub
- Jetbrains Rider

### AI Disclosure:
As stated above, AI was used for debugging, tutorials, and fixing some parts of TheBigUI (DoesEverything.cs), PlayerPlatforming, and a few other scripts. NO AI WAS USED FOR ANY OF THE ART, CUTSCENES, MUSIC, OR WRITING. 

(the readme was also made by a human if you couldn't already tell)

### Very Tentative Roadmap for 2026:
- Remove placeholder music and replace with original music
- Recompile current cutscenes at higher resolutions and with less visual artifacts
- Next Platforming Level: Cherry Blossom Festival
- Next Cooking Minigame: Remake the cooking minigame with rhythm mechanics
- New Platforming Mechanics: Stamina, Climbing, Wall Jumping, and more
- Restructure gameplay loop to originally intended alternating cooking and platforming levels
- TBD


### fun fact:
the name croakitori is a combination of the words croak and yakitori, which is a type of Japanese skewered chicken.