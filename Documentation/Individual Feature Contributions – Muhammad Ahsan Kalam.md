### **Individual Feature Contributions – Muhammad Ahsan Kalam**

---

### **Feature 1: Shooting Mechanic (Player 1 & Player 2\)**

**Description**  
 Implemented the shooting functionality for both Player 1 and Player 2, adding a more new generation game mechanic to the Tron-style gameplay. This mechanic enables players to fire projectiles at each other during the match, introducing offensive strategies and enhancing the competitive experience.

**Technical Implementation**  
 The shooting mechanic was built to work smoothly in a fast-paced two-player environment. Key implementation aspects include:

* **Projectile instantiation and physics-based movement** for both players  
* **Separate input mappings** (`Space` for Player 1 and `;` for Player 2\) to allow simultaneous firing  
* **Hit detection and collision logic** to register bullet impact with players  
* **Bullet despawn system** to optimize performance by removing off-screen projectiles

**Video Demonstration**  
[https://youtu.be/f3EGinvhVb8](https://youtu.be/f3EGinvhVb8) 

**Relevant Commits**

* The gun feature is working on tutorial mode  
* The Bullet fires in the direction of Player 1 and when it go to the opponent it phases right thru  
* The AIOpponent is now being destroyed  
* bullet feature working, needs exploding animation and gameover reset  
* Level1 and Level2 Gun feature on both players working  
* All levels have the firing features

---

### **Feature 2: Audio and Visual Effects for Shooting**

**Description**  
 Added immersive feedback through sound effects and animations when players fire bullets. These elements enhance the game's polish and responsiveness, making each action feel impactful and engaging.

**Technical Implementation**  
 This feature improves the game’s feedback loop with:

* **Custom shooting sound effects** triggered on fire input from ([https://pixabay.com/sound-effects/laser-gun-81720/](https://pixabay.com/sound-effects/laser-gun-81720/))   
* **Animation or particle system** activation to visually represent the shooting action  
* **Audio source management** to play sounds at appropriate volume and location  
* **Seamless integration** with both Player 1 and Player 2 shooting inputs  
* Opponent “**Blown up**” animation added once hit.  
* **Trail Bullet Deletion**: When Bullet touches a trail the bullet gets destroyed.

**Video Demonstration**  
[https://youtu.be/0QHFK5B3FhI](https://youtu.be/0QHFK5B3FhI) 

**Relevant Commits**

* Explosion Working for some levels  
* Firing Feature complete  
* Bullet destroyed on trail feature  
* Firing pew sounds when the cycle shoots.

