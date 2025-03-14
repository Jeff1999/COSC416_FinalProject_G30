# GlitchCycle
A modern reimagining of the classic Tron: Light Cycle (1982) arcade experience, enhanced with a lateral jump mechanic for thrilling, strategic duels.

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

## Core Concept
Our game reimagines the classic Tron: Light Cycle arcade experience, where competing riders leave brightly colored trails behind them and must avoid colliding with any trail. The twist introduces a **lateral jump mechanic** that allows players to vault over their own or an opponent's trail, creating opportunities for strategic escapes and maneuvers. Players must carefully time these jumps to transform what would be certain collisions into skillful evasions, resulting in fast-paced, tactical gameplay that builds upon the neon-dueling action of the original Tron.

---

## Core Gameplay
In each round, players control light cycles that continuously leave glowing trails behind them, requiring careful navigation to avoid collisions with any line—whether their own or an opponent's. The core loop centers on **strategic movement** and the **lateral jump** ability, which lets players vault over trails when cornered. Because colliding with any trail results in immediate knockout, every decision carries high stakes. Visual and audio feedback—through distinctive jump and crash animations, as well as jump and crash sound effects—enhances the experience.

---

## Game Modes
### Tutorial Mode
A single-player experience where players compete against an **AI opponent** designed to teach the fundamentals of gameplay. The AI demonstrates basic strategies while providing a tutorial-like challenge for beginner players to learn the controls and mechanics.

### 2-Player Mode
A competitive mode where two players face off on the same device using the control scheme detailed below. This mode allows friends to challenge each other in direct competition across multiple arenas.

---

## Level & Progression
### Game Structure
Our game consists of **five handcrafted levels**, each set in a uniquely designed arena featuring different obstacles and wall patterns. These levels are available in **2-Player mode**, ensuring variety in gameplay environments.

### Scoring System
GlitchCycle employs a **persistent scoring system** that tracks the total number of rounds won by each player throughout the entire play session. Each time a player wins a round (by being the last cycle standing), their score increments. This running tally continues until players choose to exit the game, providing an ongoing competitive experience without predetermined match endpoints. The scoring display remains visible after every round, allowing players to track their performance history across multiple rounds.

---
# Credits
### Models & Art
- **Arena Grid/Background:**  
  [Grid HUD](https://opengameart.org/content/grid-hud) – OpenGameArt
- **Player Bikes/Spaceship Building Kit:**  
  [Spaceship Building Kit](https://opengameart.org/content/spaceship-building-kit) – OpenGameArt
- **Walls/Obstacles:**  
  Created using Unity’s shape tool for rectangles, circles, etc.
- **Jump Animation:**  
  [Warp Effect 2](https://opengameart.org/content/warp-effect-2) – OpenGameArt
- **Crash Explosion Animation:**  
  [Bubble Explosion](https://opengameart.org/content/bubble-explosion) – OpenGameArt

### Sound & Music
- **Retro Jump Sound:**  
  [RETRO JUMP SOUND](https://freesound.org/people/CJspellsfish/sounds/727650/) – FreeSound
- **Crash Sound Effect:**  
  [Crash Sound Effect](https://freesound.org/people/squareal/sounds/237375/) – FreeSound
- **Main Menu Music (Tron vibe):**  
  [Music by Timbre](https://freesound.org/people/Timbre/sounds/561191/) – FreeSound
- **Selection Sound Effect:**  
  [Selection SFX by Bertrof](https://freesound.org/people/Bertrof/sounds/131658/) – FreeSound
- **In-Game Music Options:**  
  - [Music by furbyguy](https://freesound.org/people/furbyguy/sounds/331876/)  
  - [Music by Timbre](https://freesound.org/people/Timbre/sounds/496186/)  
  - [Music by 3ag1e](https://freesound.org/people/3ag1e/sounds/745852/)  
  - [Music by zagi2](https://freesound.org/people/zagi2/sounds/223475/)  
  - [Music by danlucaz](https://freesound.org/people/danlucaz/sounds/497109/)

---

## UI
We aim to create a retro-futuristic interface that complements the Tron-inspired aesthetic.

### Fonts
- [Press Start 2P](https://fonts.google.com/specimen/Press+Start+2P) – Google Fonts  
- [VT323](https://fonts.google.com/specimen/VT323) – Google Fonts

### Icons and UI Elements
- [Grayscale Icons](https://opengameart.org/content/grayscale-icons) – OpenGameArt
- [Game Icons and Buttons](https://opengameart.org/content/game-icons-and-buttons) – OpenGameArt

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
