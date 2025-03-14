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
[Link to video showing the lateral jump system in action]

### Relevant Commits
- [Implemented Twist Feature Lateral Jump](https://github.com/Jeff1999/COSC416_FinalProject_G30/commit/6b713ce5689af7ad29d3088bcb28c9081199e338)
- [Refined and Finalized Lateral Jump](https://github.com/Jeff1999/COSC416_FinalProject_G30/commit/2245a75aa58cc7ccd3c8fcb94e050952dfb994ce)

## Feature 2: 2-Player Mode Implementation

### Description
Developed the 2-Player mode that enables competitive gameplay on the same device. This mode allows two players to compete directly against each other across five different arenas, creating an engaging multiplayer experience that forms the centerpiece of the game.

### Technical Implementation
The 2-Player mode implementation required creating separate control systems and game states for simultaneous gameplay:
- Dual input handling system that processes commands for both players independently
- Separate control schemes with unique key bindings for each player
- Collision detection that distinguishes between player trails
- Independent scoring system that tracks wins for each player across multiple rounds
- Camera management to ensure both players remain in view throughout gameplay

### Video Demonstration
[Link to video showing the 2-Player mode in action]

### Relevant Commits
- [Implemented 2P Mode](https://github.com/Jeff1999/COSC416_FinalProject_G30/commit/9da39ceeac133fe57ca332f7850676db3a44b739)

## Additional Contributions
- WebGL build configuration and optimization
- itch.io deployment setup
- Camera system implementation
- Project structure and version control setup
- Scene management between Tutorial and 2-Player modes
- Basic Movement Logic for Player and AI
