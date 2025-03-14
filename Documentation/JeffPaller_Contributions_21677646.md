# Individual Feature Contributions - Jeff Paller

## Feature 1: Twist Feature - Lateral Jump Implementation

### Description
Implemented the game's core twist mechanic: a lateral jump system that allows light cycles to vault over trails. This innovative addition to the classic Tron gameplay creates new strategic possibilities and escape options, fundamentally transforming the traditional light cycle experience.

### Technical Implementation
The lateral jump system uses a sophisticated approach to temporarily bypass collision detection while maintaining game integrity. Implementation includes:
- Vector-based displacement calculations to determine jump trajectory and landing positions
- Cooldown system to prevent ability overuse and maintain gameplay balance
- Visual feedback through particle effects and animation to communicate the jump action clearly
- Collision management to handle interactions between jumping cycles and existing trails

### Video Demonstration
[Link to video showing the lateral jump system in action](https://www.youtube.com/watch?v=jT1SGxZmfv8&ab_channel=JeffPaller)

### Relevant Commits
- [Implemented Twist Feature Lateral Jump](https://github.com/Jeff1999/COSC416_FinalProject_G30/commit/6b713ce5689af7ad29d3088bcb28c9081199e338)
- [Refined and Finalized Lateral Jump](https://github.com/Jeff1999/COSC416_FinalProject_G30/commit/2245a75aa58cc7ccd3c8fcb94e050952dfb994ce)

---

## Feature 2: 2-Player Mode Implementation

### Description
Developed the 2-Player mode that enables competitive gameplay on the same device. This mode allows two players to compete directly against each other, creating an engaging multiplayer experience that forms the centerpiece of the game.

### Technical Implementation
The 2-Player mode implementation required creating separate control systems and game states for simultaneous gameplay:
- Dual input handling system that processes commands for both players independently
- Separate control schemes with unique key bindings for each player
- Collision detection that distinguishes between player trails
- Independent scoring system that tracks wins for each player across multiple rounds
- Camera management to ensure both players remain in view throughout gameplay

### Video Demonstration
[Link to video showing the 2-Player mode in action](https://www.youtube.com/watch?v=pFpkHwEGdPY&ab_channel=JeffPaller)

### Relevant Commits
- [Implemented 2P Mode](https://github.com/Jeff1999/COSC416_FinalProject_G30/commit/9da39ceeac133fe57ca332f7850676db3a44b739)

---

## Feature 3: Basic AI Opponent

### Description
Implemented a basic AI opponent that allows single-player gameplay. The AI provides a tutorial-like challenge by reacting to player movement and avoiding obstacles, creating a more dynamic and engaging experience when no second player is available.

### Technical Implementation
The AI opponent logic includes:
- Pathfinding logic that determines the best movement strategy based on the player's actions
- Obstacle avoidance to prevent self-collisions and crashing into walls or trails
- Basic decision-making to introduce unpredictability and challenge the player
- Speed and difficulty adjustments to balance AI performance

### Video Demonstration
[Link to video showing AI opponent in action](https://www.youtube.com/watch?v=jT1SGxZmfv8&ab_channel=JeffPaller)

### Relevant Commits
- [Implemented Basic AI Opponent](https://github.com/Jeff1999/COSC416_FinalProject_G30/commit/bb2873c8eb3e5337a9189af1823e558e8655e1d5)

---

## Feature 4: Basic Player Movement and Line Generation Logic

### Description
Implemented the core movement system for the player, including smooth directional control and the logic for generating light trails. This system serves as the foundation for all gameplay mechanics, ensuring responsive controls and accurate trail rendering.

### Technical Implementation
- Directional input handling with smooth transitions between movement states
- Real-time trail generation that dynamically updates based on player movement
- Collision detection that determines when a player crashes into a trail or the game boundary
- Visual enhancements such as fading trails and real-time updates for improved aesthetics

### Video Demonstration
[Link to video showing basic movement and line generation](https://www.youtube.com/watch?v=jT1SGxZmfv8&ab_channel=JeffPaller)

### Relevant Commits
- [Implemented Basic Movement](https://github.com/Jeff1999/COSC416_FinalProject_G30/commit/70121dba43b143e148b41dae1969277c1c794295)
- [Implemented Basic Trail Generation](https://github.com/Jeff1999/COSC416_FinalProject_G30/commit/1e2bbb0d3726865ebb771fb620ee4e6e0b60c629)

---

## Additional Contributions
- WebGL build configuration and optimization
- itch.io deployment setup
- Camera system implementation
- Project structure and version control setup
- Scene management between Tutorial and 2-Player modes
```


