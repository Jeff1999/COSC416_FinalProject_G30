### **Individual Feature Contributions – Raunak Khanna**

### **Feature 1: Implementing Game Over (Pacman Death Sound)**

**Description**  
The level 1 music was interfering with the Game Over panel when a player lost. To resolve this, I implemented a dedicated audio control strategy in `GameController.cs` and `TwoPlayerGameController.cs`. The `StopAllOtherAudio()` method was introduced to ensure that all background music and active sound sources are halted before playing the Game Over sound clip. This gave the crash moment more emotional clarity, reduced sound clutter, and ensured the Pacman-inspired “death” sound could be heard cleanly and impactfully. While working on sound effects, I ensured that the `hitSoundGam` audio clip plays consistently when either player crashes or when a tie occurs. This involved:

- Adding an `AudioSource` component fallback in both controller scripts  
- Ensuring the sound is played *after* stopping other sources  
- Preventing multiple overlapping plays of the same clip  

**Technical Implementation**  
This feature improves the game since the regular retro music was being played in a loop:

- Used game over sound from Pacman Death to fit the retro mood of the game:- [https://classicgaming.cc/classics/pac-man/sounds](https://classicgaming.cc/classics/pac-man/sounds)  
- Introduced a `hitSoundGam` audio clip to be played upon game over events (player crash, AI crash, or tie)  
- Implemented a `StopAllOtherAudio()` method to mute background music and other active audio sources, ensuring the game over sound plays clearly and without interference  
- Integrated the `AudioSource.PlayOneShot()` method to trigger the Pacman-inspired death sound at the exact moment of crash detection  
- Ensured the audio plays consistently across all crash scenarios (`PlayerCrashed()`, `AIPlayerCrashed()`, and `TieGame()`), enhancing player feedback and emotional impact  
- Added fallback logic to automatically attach an `AudioSource` component if not already present on the controller object  
- Verified compatibility with both `GameController.cs` and `TwoPlayerGameController.cs` for seamless integration across all gameplay modes  
**Video Demonstration**
  
[https://youtu.be/JRUrtRuNdi0](https://youtu.be/JRUrtRuNdi0)

You can see whenever the game is over there is a sound effect which is the pacman death (copyright free) sound which stops the annoying loop
**Relevant Commits**

- Changing GameAudioManager.cs file, making changes into already existing loop of retro song which was getting stuck 
- Making Changes in GameController.cs and TwoPlayerGameController.cs
- Fix the sound in the tutorial
- The Game Over pac man sound is implemented in all the levels and the tutorial as well
---
### **Feature 2: Level 5 – Invisible Wall Challenge**

**Description**  
To raise the difficulty curve and provide a distinct skill-testing experience, I designed Level 5 as the hardest level in the game for Player 1. The key challenge of this level is the use of invisible walls, forcing the player to navigate blindly through a maze-like structure. This level design encourages memorization, trial-and-error learning, and precise movement — rewarding experienced players while challenging new ones. The invisible obstacles increase tension, elevate focus, and differentiate Level 5 as a finale-style challenge.

**Technical Implementation**  

- Created a new level scene (`Level5.unity`) and integrated it into the level selection flow.
- Placed `BoxCollider2D` components in strategic maze-like patterns across the scene.
- Removed or disabled `SpriteRenderer` components on these walls to render them fully invisible during gameplay.
- Ensured the walls still interact with Player 1's movement and collision logic using Unity’s physics engine.
- Tested the layout multiple times to ensure the level was challenging but possible without visual guidance.
- Added visual cues and audio feedback (e.g., crash sound) to indicate collisions with invisible obstacles.
- Balanced the camera and lighting to avoid unintentionally revealing the position of the walls.

**Video Demonstration**
[https://www.youtube.com/watch?v=X5fEYb_yXVI&feature=youtu.be](https://www.youtube.com/watch?v=X5fEYb_yXVI&feature=youtu.be)

There are invisible walls and shapes that seem like theyre walls but they are actually not walls. Therefore this makes it the hardest level.
