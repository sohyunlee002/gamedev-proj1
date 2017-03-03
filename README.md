# Project 1-2 Spec [Due 2/6 at 11:59 pm]:   
## How to get the Project 
1. Make a new git repository
2. Add the course remote with  
  ```git remote add course git@github.com:berkeleyGamedev/Project1-2.git```
3. Pull Project 1-2 code with   
  ```git pull course master```
4. Pull from the course often for bug fixes!

## About the Project
This project is for teams of 1-2 (we would prefer teams of 2). Every team must have at least 1 programmer. Every team must do the Editor and Programming sections - teams only have do the Art section if they have an artist in their group.

This project is a clean slate from 1-1. You are free to change partners, and you are not required to reimplement any of your art or code (unless you want to as part of this project).

## Grade Breakdown:
Requirements: 30%  
Complexity: 30%  
Creativity: 40%    
	
## Project Specifications	
This project is even more open-ended than the last one. (Note the updated grade breakdown for this project.) In short, you will be completing the mostly-complete Goomba implementation. From there, like Project 1-1, you will be adding 1 new Item and 1 new Enemy from scratch (including the art). The general Item and Enemy frameworks are provided, and the Goomba and Magic Mushroom GameObjects will provide good examples for your new implementations. 

Most importantly this is only a week long project, so we are not expecting you to create a masterpiece! Keep it realistic and manageable. If you have the free time and are really enjoying yourself, then feel free to go crazy - we will definitely award some form of bonus points for projects that go above and beyond but we are NOT expecting all the students to do this.

### Goomba Description
The Goombas have a parent GameObject “Goomba” with a Rigidbody2D, the Goomba script (which inherits from Enemy), and an Animator. It has three child GameObjects.
  1. Sprite - The Sprite Renderer is placed in a separate GameObject in order to offset it by 0.5 from the parent GameObject. This ensures that we can determine the Goomba’s position using integers instead of decimals: attached to the parent GameObject, we would always have to offset the parent GameObject’s position by 0.5 to make sure the sprite is in the right place. Simply a quality-of-life decision
  2. HitByPlayer_Collider - The HitByPlayer collider, or in Goomba’s case, the Top Collider, is placed in a separate GameObject so that when the PlayerController interacts with this collider, it can check the tag separately. If both the HitByPlayer and HitPlayer collider were in the same GameObject, we would only be able to put them under the same tag.
  3. HitPlayer_Collider - Same as above.

### Magic Mushroom Description
The Magic Mushrooms have a parent GameObject, a Rigidbody2D, its Collider, and its Magic Mushroom script. It only has a single child GameObject with the Sprite Renderer, for the same reason as the Goomba. The colliders do not need to be differentiated. 

**Note**: We’ve made some changes to Mario’s state system. The “Jumping” state has now become the “InAir” state. The preliminary jumpForce is added in Grounded.Exit() now. Feel free to add your own physics numbers or change it however you like. 

**Also Note**: When editing animations, its best to select the GameObject you want to edit -> Animation tab -> select Animation clip through dropdown menu, rather than double-clicking the Animation clip in the Assets folder. 

### Unity Editor (Artists and/or Programmers):
1. On the NES - Super Mario Bros. - Enemies.png sprite sheet: Edit the “Sprite Mode” options, and slice it to get the Goomba sprites. There should be 16 pixels per unit.
2. Add animation to Goombas using the sprite sheet you sliced in the previous step.  
   1. Make an Animator Controller and add it to the Animator component on the Goomba prefab (in Assets/Prefabs)
   2. Add a default “Walking” state with its corresponding animation.
   3. Add a “Player Hit” state with its corresponding animation (where the Goomba is squashed).
   4. Add a transition from “Walking” to “Player Hit” that activates when the Player hits the Goomba. 

### Art (Artists):
* Work with your partner to design a new item as well as a new enemy.
* Your requirement will be to create the required art for both of them.
* The main goal of this section is to help students gain some practice with communicating ideas across disciplines effectively. Do your best to make sure both you and your partner are on the same page about:
  * Art style
  * Art required
  * Dimensions (resolution of sprite)
  * Who is in charge of making changes to the Animator in the editor
  * Time required/ being realistic
  * Don’t design something that will require more art than you can handle making in this timeframe. 

### Programming (Programmers):
Your changes will be made in several different scripts. You will find the text “YOUR CODE HERE” wherever you need to implement something.
1. Goomba.cs
   1. HitByPlayer function
   2. HitPlayer function
   3. Hint: don’t forget about your animator variables!
2. PlayerController.cs
   1. OnCollisionEnter2D function - the enemy case
3. Add your own enemy to the game!
   1. Build a GameObject (like Goomba) with a Rigidbody2D, an Animator, and a Sprite Renderer. For the colliders, the basic idea is to have two colliders: one for Mario to hit (in Goomba’s case the HitByPlayer collider) and one for the enemy to hit Mario with (in Goomba’s case the HitPlayer collider). However, your enemy does not need to follow this system. If you change enemy collision behavior, make sure to update our framework in the OnCollisionEnter2D function of PlayerController.
   2. Attach a script to your enemy that inherits from the Enemy superclass. 
1. Add your own item to the game!
   1. Build a GameObject (like Magic Mushroom) with a single collider (for Mario to hit), a Rigidbody2D, and a Sprite Renderer.
   2. Attach a script to your item that inherits from the Item superclass.
   3. Note: You are welcome to tie in the special ability you created in 1-1 (you would have to re-implement it) as part of this item/powerup
   4. It is perfectly fine for your item to start on the ground. If you want your item to start behind a block and rise up (like the Magic Mushroom), it will require a little extra work. We implemented this behavior with Coroutines in MagicMushroom.cs. Check out the Unity Coroutine Tutorial or come ask us after class or during office hours. 

## Submission:
* Place a copy of all your art files into a separate folder. Make sure this folder is included in your git repository.
* Add a text file called teamInfo to your repo that lists your team members as well as who worked on what.
* Make sure you have added the github user berkeleyGamedev as a contributor to your github repository.
* Add a commit with a “final submission” tag and push it to github.
* Double check that the github repository contains a working version of your project.