# GlitchCycle
A modern reimagining of the classic Tron: Light Cycle (1982) arcade experience, enhanced with a lateral jump mechanic for thrilling, strategic duels.

Play now!
[Click Here to play GlitchCycle!](https://jeffhouse.itch.io/glitchcyclev1)

Watch the Full Video Showacse [Here!](https://youtu.be/7FAw-UNqLKQ)

To see feature implementations as per individual contributions, See the Documentations folder. 

---
**Team Name:** Group 30 (G30)  

## Team
- **Team Name:** Group 30 (G30)  
- **Team Members:**  
  - Jeff Paller (21677646)  
  - Mridul Nohria (74243379)  
  - Raunak Khanna (89443253)  
  - Muhammad Ahsan Kalam (86275237)  

---

# Core Concept
Our game reimagines the classic Tron: Light Cycle arcade experience, where competing riders leave brightly colored trails behind them and must avoid colliding with any trail. **We introduce two key twists** to enhance the classic formula:
1. A **lateral jump mechanic** that allows players to vault over their own or an opponent's trail, creating opportunities for strategic escapes and maneuvers.  
2. A **shooting mechanic** that lets players fire a projectile to eliminate opponents if aimed and timed correctly, adding a new layer of offense and risk-taking.

These additions create fast-paced, tactical gameplay that builds upon the neon-dueling action of the original Tron.

---

# Core Gameplay
In each round, players control light cycles that continuously leave glowing trails behind them, requiring careful navigation to avoid collisions with any line—whether their own or an opponent's. The **core loop** now involves three main elements:

- **Strategic movement** to avoid trail collisions and corner opponents.  
- **Lateral jump** to vault over trails when cornered.  
- **Shooting** to pick off an opponent from a distance, but with limited reaction windows.

Because a single collision or successful shot instantly knocks a player out, every decision carries high stakes. Visual and audio feedback—through distinctive jump, shoot, and crash animations, as well as corresponding sound effects—further enriches the experience.

---

# Game Modes

## Tutorial Mode
A single-player experience where players compete against an **AI opponent** designed to teach the fundamentals of gameplay. The AI demonstrates basic **movement**, **jumping**, and **shooting** strategies while providing a manageable challenge for beginner players to learn the controls and mechanics.

## 2-Player Mode
A competitive mode where two players face off on the same device using the control scheme detailed below. This mode allows friends to challenge each other in direct competition across multiple arenas, showcasing **both** the lateral jump and shooting mechanics in intense, head-to-head matches.

---

# Level & Progression

## Game Structure
Our game consists of **five handcrafted levels**, each set in a uniquely designed arena featuring different obstacles and wall patterns. These levels are available in **2-Player mode**, ensuring variety in gameplay environments and forcing players to adapt both their jumping and shooting tactics to new layouts.

## Scoring System
GlitchCycle employs a **persistent scoring system** that tracks the total number of rounds won by each player throughout the entire play session. Each time a player wins a round (by being the last cycle standing—whether through outmaneuvering or successfully shooting opponents), their score increments. This running tally continues until players choose to exit the game, providing an ongoing competitive experience without predetermined match endpoints. The scoring display remains visible after every round, allowing players to track their performance history across multiple rounds.


---
# Credits
### Models & Art
- **Arena Grid/Background:**  
  [Grid HUD](https://opengameart.org/content/grid-hud) By Ivan Voirol - Sourced from OpenGameArt
- **Player Bikes/Spaceship Building Kit:**  
  [Spaceship Building Kit](https://opengameart.org/content/spaceship-building-kit) By BizmasterStudios – Sourced from OpenGameArt
- **Walls/Trails:**  
  Created using Unity’s shape tool for rectangles, line renderer for trail generation, etc.
- **Jump Animation:**  
  [Warp Effect](https://opengameart.org/content/explosion-set-1-m484-games) By Master484 – Sourced from OpenGameArt
- **Crash Explosion Animation:**  
  [Bubble Explosion](https://opengameart.org/content/bubble-explosion) By Tiao Ferreira – Sourced from OpenGameArt

### Sound & Music
- **Retro Jump Sound:**  
  [RETRO JUMP SOUND](https://freesound.org/people/CJspellsfish/sounds/727650/) By CJspellsfish – Sourced from FreeSound
- **Crash Sound Effect:**  
  [Crash Sound Effect](https://freesound.org/people/squareal/sounds/237375/) By squareal – Sourced from FreeSound
- **Main Menu Music (Tron vibe):**  
  [Main Menu Music](https://www.youtube.com/watch?v=Net67QKNBEk&ab_channel=WhiteBatAudio) By WhiteBatAudio – Sourced from Youtube 
- **Selection Sound Effect:**  
  [Selection SFX](https://freesound.org/people/Bertrof/sounds/131658/) By Bertrof – Sourced from FreeSound
- **In-Game Music Options:**  
  [In-game Music](https://freesound.org/people/furbyguy/sounds/331876/)  By furbyguy - Sourced from Freesound

---

## UI
We aim to create a retro-futuristic interface that complements the Tron-inspired aesthetic.

### Fonts
- [Press Start 2P](https://fonts.google.com/specimen/Press+Start+2P) – Sourced from Google Fonts  

---

## Cloning & Setting Up the Project
Below are detailed instructions for preparing your environment and opening the project:

1. **Install Required Software**  
   - **Unity Hub**  
     - Download and install from [Unity Hub](https://unity.com/download).  
     - Ensure you have Unity version **6000.0.31f1** (or the version specified in the project’s metadata).  
   - **Git** or **GitHub Desktop**  
     - Download Git from [Git SCM](https://git-scm.com/download) or GitHub Desktop from [GitHub Desktop](https://desktop.github.com/).  

2. **Clone the Project**  
   - **Via Command Line:**  
     ```bash
     git clone https://github.com/YourUsername/GlitchCycle.git
     ```
     *Replace `YourUsername` and `GlitchCycle` with your GitHub repo details.*  
   - **Via GitHub Desktop:**  
     - Click **File** > **Clone Repository…**  
     - Choose **GitHub.com** and select the `GlitchCycle` repository.  
     - Click **Clone** to begin downloading the project.  

3. **Open the Project in Unity Hub**  
   - Launch **Unity Hub**.  
   - In the **Projects** tab, click **Open** (or **Add**).  
   - Browse to the location where you cloned the repository (containing `Assets`, `ProjectSettings`, and `Packages` folders).  
   - Select the folder and click **Open**.  
   - Unity will initialize the project. Make sure you have the correct Unity version installed.

4. **Build & Run**  
   - Once the project opens in Unity, you can select **File** > **Build Settings…**  
   - Choose your build platform (e.g., PC, Mac, Linux Standalone, WebGL, etc.).  
   - Click **Build** or **Build and Run** to export and test the game.

---

Enjoy your retro-futuristic GlitchCycle experience!
