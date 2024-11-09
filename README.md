# ProjectX

A project I chose to work on for my Unity Game Scripting assignment, in which I had to create a 3D Game.<br><br>
I decided to make a horror game in which the Player would have to escape an abandoned hospital while trying to avoid a crazy butcher.

<p align="center">
  <img src="Media/demo.gif"><br/>
  *<i>Low frame rate caused by gif limitations</i>*
</p>

## Overview

Throughout the course, I had the chance to familiarize myself with the Unity 3D Game Engine, covering concepts as NavMesh, Physics, Transforms, Object linking, Animations, Scene management, UI, Audio and Lighting.

## Software Architecture

I have relied on two main principles - Singleton and Event-based programming.<br><br>
I have utilized the Singleton pattern by creating several managers that all keep track of their own tasks:
- **Game Manager**
- **Time Manager**
- **Inventory Manager**
- **UI Manager**
- **Audio Manager**<br>

Then, they communicate through Events, updating the state of the game.

## Features

### Gameplay

- **Enemy Movement:**
  1. Build upon the NavMeshAgent provided by Unity
  2. Patrolling state - traverses the level by picking points in a random way
  3. Targeting state - starts chasing the Player after spotting it
  4. Retreating state - runs away from the Player after being damaged by it
 
- **Player:**
  1. FPS View
  2. Interacting with objects - Opening and Closing Doors, Picking up Notes, Keys and Knives
  3. Inventory - keeping track of knives and keys
  4. Throwing knives
 
- **Doors:**
  1. Opens and closes
  2. Some locked doors require having the right key to unlock and open them
 
- **Time management:**
  1. Different time periods which change the behavior of the Enemy

 
### UI

- **Start and End Screens:**
- **Pause Screen:**
  1. Controls settings - adjusting the mouse sensitivity and volume
- **HUD**
  1. Time indicator - updated by TimeManager
  2. Knives count - updated by InventoryManager
  3. Interactable text - updated by InputHandler if any interaction is currently possible

### Music and SFX

- **Music** - handled by AudioManager
- **SFX** - 2D and 3D Sounds

### Lighting

- **Point lights**: Lamps
- **Spot light**: Player's torch


### Assets
- **[Environment](https://assetstore.unity.com/packages/3d/environments/pbr-hospital-horror-pack-free-80117)**
- **[Note](https://assetstore.unity.com/packages/3d/props/clipboard-137662)**
- **[Key, Knife](https://assetstore.unity.com/packages/3d/props/horror-starter-pack-free-178413)**
- **[Medical Saw](https://assetstore.unity.com/packages/3d/props/tools/medical-saw-110165)**
- **[Enemy](https://www.cgtrader.com/free-3d-models/various/various-models/enemy-4003351d-3419-49be-81ae-6fb2b3824ffb)**
- **[Note Overlay](https://pngtree.com/freepng/yellowed-kraft-paper-vintage-scrapbook-torn-paper_6118761.html)**
- **[Font](https://www.dafont.com/stranger-back-in-the-night.font)**
- **Sounds:**
  1. [First Library](https://assetstore.unity.com/packages/audio/sound-fx/horror-game-essentials-153417)
  2. [Second Library](https://assetstore.unity.com/packages/audio/sound-fx/horror-elements-112021)
