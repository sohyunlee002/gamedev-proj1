# Berkeley Gamedev Introductory Project Specification and Directions

## Overview

In this project, you will extend our simple 2D platformer with new abilities, items, and enemies!
	
## Project Setup

1. Make a new directory and create a new git repo: ``git init``
2. Add the course remote: ``git remote add course https://github.berkeley.edu/berkeley-gamedev/Project-1.git``  
3. Pull from the remote to get the skeleton code: ``git pull course master``
4. Pull often for bug fixes!
5. Stage the skeleton code in your local repository: ``git add -A``
6. Commit the skeleton code: ``git commit -m "Initial commit"``

Note: For now you can create your own repositories. We will give further instructions on using our new gamedev github which we are working on getting set up. 
1. Create a repository in the **berkeley-gamedev** organization
2. Add your project partner as as collaborator
3. Add your remote repository as the origin: ``git remote add origin "your-repo-address-here"``
4. Push the skeleton code to the origin: ``git push origin master``
5. You're ready to go!

This document is split into two parts: [documentation](#game-documentation) and [specifications](#project-specifications). 

The following documentation outlines the various objects and systems used for the project. We highly encourage reading over these to understand how things work and how to add your own items and enemies. Documentation is provided for your reference and most of the things listed will not need to be modified by you. 

The [specifications](#project-specifications) are at the bottom of the page and list the things you need to do as well as some pointers on where to get started. The specifications must all be completed to get full credit on this project.

## Game Documentation

### The Mario GameObject

The scripts and components that make up the player character in our game are all encapsulated in the **MarioHolder**. This shows the capability of GameObjects to simply act as organizational tools for the objects in your game. More than just organizational, however, the parent GameObject serves another important purpose: to keep all Mario objects in the same location - as the position of a child GameObject is relative to the position of the parent GameObject. In this way, we can move the player character by moving MarioHolder's Transform but keeping the Transforms of **Little Mario**, **Super Mario**, and **Ducking Mario** set to (0, 0, 0).

Super Mario and Little Mario correspond to the different forms that Mario can take on in the game. Ducking Mario is associated with an ActionState instead and its existence is just a quirk of the implementation. We'll get to this later. Little Mario will begin the game, and upon eating a Magic Mushroom will turn into Super Mario. In order to switch between these different Mario bodies, we use the logic in the Player Controller to [activate](https://unity3d.com/learn/tutorials/topics/scripting/activating-gameobjects) the new Mario GameObject, and deactivate the old Mario GameObject with ``gameObject.SetActive(Boolean bool)``. When a GameObject is deactivated, it is temporarily removed from the scene. 

Each Mario GameObject has a Transform, a Sprite Renderer, an Animator, several geometric Colliders, and a Script. If you click on one of these GameObjects, you can see that there is a disconnect between the sprite and the collider: they do not fill the exact same space. It is possible to have a collider that automatically contours to the exact form of your sprite by using the **Polygon Collider**. However, using the Polygon Collider would create many more small surfaces for collisions, which introduces many more opportunities for performance hits and bugs. In our case, it is best to keep it simple with the pill-shaped collider system we use. 

#### Mario "Forms"

The classes ``MarioForm`` and ``SuperMarioForm``, which inherits from ``MarioForm`` do NOT inherit ``Monobehaviour`` and therefore cannot be attached as components to GameObjects in the scene. They simply encapsulate the game logic for the Mario forms. Their properties are the PlayerController, their associated GameObject (that they will activate on ``Enter`` and deactivate on ``Exit``), and the MarioForm that they will transition to when they "shrink" (hit by an enemy). In this case ``MarioForm`` will become ``null`` (signalling that Mario has lost a life) and ``SuperMarioForm`` will shrink to Little Mario. 

### PlayerController

We've covered the basics of scripting in class. The ``Start()`` function is called on initialization, ``Update()`` is called once per frame and is best for catching input, and ``FixedUpdate()`` is called on a fixed interval and is best for phyiscs updates. The Player Controller uses a [State Machine](http://gameprogrammingpatterns.com/state.html) to intercept the input and carry out actions based on what state Mario is in, so the ``Update`` function simply has to get the input and delegate it to the state. The ``FixedUpdate`` function handles flipping the player character left and right, delegates to the current state, and handles the transitions between states.

A State Machine is a "machine" that can be in one state at a time. It transitions between states in response to external inputs: in our case either Mario's movements in the game, or input from the player.

#### States

The PlayerState interface dictates the functions that a state must implement:  

* ``Enter`` to be called in ``PlayerController.FixedUpdate`` when the player character enters the state  
* ``FixedUpdate`` to be called by ``PlayerController.FixedUpdate`` on a fixed timescale for physics updates  
* ``Update`` to be called by ``PlayerController.Update`` per frame  
* ``Exit`` to called by ``PlayerController.FixedUpdate`` when the player character leaves the state  
* ``HandleInput`` to be called by ``PlayerController.Update`` when an input is received and to return the next state that the current state that Mario will transition to  

##### Walking

On entering, the ``Walking`` state will set the animator to play the walking animation by using the boolean flag "Walking". In its ``Update`` and ``FixedUpdate`` functions, the state will move Mario along the ground based on input, duck Mario based on input and current Mario form, and check for exiting the state: when Mario begins falling or Mario jumps. On exit, the ``Exit`` function will simply turn off the current Walking animation. 

##### InAir

The ``InAir`` state does many of the same things as the Grounded state. In its ``FixedUpdate`` function, it allows the player to control Mario's horizontal movement in air. It also continuously checks if Mario has reached the ground in the ``CheckForGround`` function with a Raycast. When the player has reached the ground again, ``InAir`` will transition back to the ``Walking`` state. 

##### Jumping

``Jumping`` inherits from the ``InAir`` class to inherit the same horizontal movement and checking for ground capabilites. On entrance, the Jumping state will apply the preliminary jumping force and activate the Jumping animation. In its ``FixedUpdate``, it will continue to add force as the player holds down the Jump button as long as Mario has not exceeded its maximum speed or the allotted time to add jumping force (``jumpingTime`` = 1 second). When Mario reaches the apex of his jump and begins falling, Mario will transition to the InAir state. 

##### Ducking

Ducking is a unique ActionState in that its implementation depends on ``duckingMarioGO`` (when Mario ducks, his colliders change, and its simply easier to make a new GameObject that has these colliders than to edit the collider components on the same GameObject). The Ducking state must also keep track of the GameObject to transition back to after the player releases the Ducking button. 

### World 1-1

The GameObject for World 1-1 is organized in much the same way as the GameObject for the player character: with parent and child GameObjects. Again, this serves to orient all child GameObjects to the parent. In our case, the world map as the top-level parent ensures that all GameObjects will be oriented with the bottom left of the world map at the origin even though their actual position in space is different.

### Basic Blocks

The **Basic Block** object is created from the Basic Block prefab, indicated by the blue color of the object's name in the hierarchy. You can view the prefab by navigating to Assets/Prefabs/ in the Project folder. All blocks, items, and enemies are prefabs. The bold values in the transform of each of the block GameObjects indicate that the default prefab values are being overridden. 

The Basic Block prefab parent object only has a Transform so that all the child GameObjects are in the same position. The **Top_Collider** object holds the top collider of the block, and the **Bottom_Collider** holds the bottom collider of the block and the Block script. The reason for splitting the colliders between two separate GameObjects is that it is easier to check if Mario is walking on the top collider or hitting the bottom collider by checking for different GameObjects than it is to check the location of the collision, or even to differentiate between collider components on the same GameObject. The **Sprite** object simply holds the sprite, but its transform gives it a 0.5 offset from its parent. This ensures that the sprite itself is lined up with its parent GameObject: the bottom left corner of the sprite is the origin of its parent GameObject. All blocks, items, and enemies with the **SpriteRenderer** component on a separate GameObject use this technique. 

#### Script

``Block`` defines the basic behavior for a block. Since ``Top_Collider`` has no special behavior other than being a floor, the script is attached to the ``Bottom_Collider`` so that the appropriate collision method ``OnCollisionEnter2D`` is called when the player hits the bottom collider. The main behavior of a Block is to bounce up and down when hit by a player, or break if hit by Super Mario. In this case, the bouncing behavior is implemented with a [Coroutine](https://docs.unity3d.com/Manual/Coroutines.html). Essentially, the ``MoveUpAndDown`` function is called once per frame; the function does not run to completion when it is called. The same behavior could also have been implemented using the ``Update`` function or an Animator component. 

### Mystery Blocks

The **Mystery Block** object is functionally identical to the Basic Block.

#### Script

a``MysteryBlock`` inherits from the ``Block`` class so it has many of the same behaviors. However, after a Mystery Block is bounced up and down by the player it becomes unbreakable. This is implemented with a coroutine that simply calls the ``MoveUpAndDown`` coroutine that it inherits and then changes the sprite and makes the block unbreakable on completion.

### Goombas

The **Goomba** is an agent in the world and therefore has a ``Rigidbody`` component. The colliders are once again separated into different GameObjects so we can more easily check whether Mario has bounced on top of the Goomba to kill it, or the Goomba has hit Mario on the side. 

#### Script

``Goomba`` inherits from the ``Enemy`` class, an abstract class that specifies the "contract" terms that Goomba must implement in order to be an Enemy:  

* ``GetScore`` - called by the ``UIManager`` to get the score value of the Enemy when it is killed by Mario  
* ``HitByPlayer`` - called by the player when the enemy is HIT BY the player  
* ``HitPlayer`` - called by the player when the enemy HITS the player  

The Goomba ``HitByPlayer`` function kills the Goomba, and the ``HitPlayer`` function shrinks the player. However, these methods are called from ``PlayerController.OnCollisionEnter``, which detects whether the player has hit the side or the top of the Goomba. ``HitByPlayer`` will cause the enemy to die, and therefore will probably have shared behavior between all subclasses. ``HitPlayer`` will cause the player to transition into the previous Mario state, or die if there are no states left (when the MarioForm returns ``null``). ``HitPlayer`` will call ``PlayerController.Shrink()`` .

### Magic Mushrooms

The **Magic Mushroom** is also an agent in the world and therefore has its own ``RigidBody`` along with a singular collider since we do not need to detect where the player hits the item. However, this collider is a [Trigger](https://unity3d.com/learn/tutorials/topics/physics/colliders-triggers). This simply means that the collider will not register collisions but rather send trigger events to its associated scripts. We use this for activation: we want the Magic Mushroom to begin rising out of its block at a controlled speed when Mario enters its trigger zone, rather than being bumped out by him. 

#### Script

``MagicMushroom`` inherits from the ``Item`` class, an abstract class like ``Enemy``. ``Item`` defines the following methods:  

* ``GetScore`` - called by the ``UIManager`` to get the score value of the Item when it is picked up by Mario  
* ``PickUpItem`` - called by the player character when the item is hit (triggered) by the player  
* ``ItemBehavior`` - an alias for ``FixedUpdate`` that is called by ``FixedUpdate`` on every time step  

The Magic Mushroom must also implement other important behaviors: it must rise out of the block it is behind when triggered by the player (Coroutine ``Activate``) and it must become visible as it rises (Coroutine ``ShowAndHide``). After the Magic Mushroom has been activated, ``myCollider.isTrigger`` is set to false and it takes on the normal behavior of a collider so that it can move across the ground and register collision with the player correctly. As in the case of the Goomba, collision is handled by the player controller, which calls ``PickUpItem`` on collision with the Item. ``PickUpItem`` will cause the player to change Mario forms, so it calls the ``Grow`` function in ``PlayerController`` and passes in the new ``MarioForm`` that the player will transition into. 

### Sprites

Sprites are 16x16 pixels, with the larger sprites - such as Super Mario - being 16x32 pixels. They use Bilinear filtering and have their pivot in the **Center**. Although we could accomplish the same functionality by setting the pivot to **Bottom Left**, it is easier and introduces less bugs to simply use the GameObject-offset method as outlined above. 

### UI Manager

The UI Manager is a [Singleton](http://gameprogrammingpatterns.com/singleton.html), which essentially means that there is only **one** instance of it that can ever exist, and that all scripts can access it through its own static member ``uiManager``. 

The UI Manager keeps track of important UI and game functions such as keeping score, keeping time, and keeping track of the player's lives. It uses a coroutine to keep time - but one that waits for one second between calls rather than between fixed time steps of the phyiscs engine. Loading the game scene (switching between the Main Menu and the Game Scene) would normally be implemented with [``SceneManager.LoadScene("Main Scene")``](https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadScene.html) but we've abstracted it with ``UIManager.LoadScene`` to make sure that some time is spent on the Loading Screen by using a coroutine timer. The Menu Scene can be loaded normally. ``LoadOnClick`` calls this method in the ``LoadOnClick.LoadScene`` function, which, rather than being a Script component on a GameObject, is tied to the UI through the **Button** component on the **1Player** GameObject, which registers the **On Click** event. 

## Project Specifications

### Part 1

Add a new enemy and a new item!

#### Tips For Adding an Enemy:
You'll probably want to make your enemy a prefab so that you can easily place them throughout the scene. You should reference the Goomba prefab for reference. Notice how it actually consists of several children, such as 2 different colliders. This can help if you want to tell which part of an enemy has been hit. This is especially useful in a game like Mario where it is necessary to check if the player hits the top of an enemy compared to other parts.

You should give your enemy prefab an "enemy" tag and "enemy" layer to make sure it interacts with the world as expected. For example there are invisible enemy walls that block enemies from walking off edges. It makes sure to only block enemies by using a layer that only collides with the enemy layer! Notice that the top collider of the Goomba has the "Enemy_Top" tag. You should give your enemy's top collider this tag as well if you want it to interact like the Goomba (ie Mario jumping on it triggers the HitByPlayer function).

You'll want to create a new script to give your enemy it's own behavior. It's script should extend Enemy abstract class (see documentation for details). Just by extending Enemy you get some built in behavior, like the HitByPlayer and HitPlayer functions. You're welcome to override these in your class if you want to add unique behavior. For example, you could override HitByPlayer to drop an item when the enemy dies or you could override HitPlayer to deduct points from the player's score. Otherwise you can just override the FixedUpdate function to give custom update behavior, for example a unique move pattern.

#### Tips For Adding An Item
Adding an item will probably actually be pretty similar to adding an enemy. You should make a prefab and give it some collider so that the player can pick it up. You'll want to add another new script, this time extending the Item abstract class (again, see documentation!) You'll need to override the PickUpItem and ItemBehavior functions to give custom behavior. PickUpItem happens when the player gets the item. This is where you can change Mario's state! (see Part 2). You'll probably also want to destroy the item here so that Mario can't get it twice. ItemBehavior is an alias for FixedUpdate so it's called every time step. This can be used for things like moving the item around. I'd recommend taking a look at the MagicMushroom class for an example of how this all works, as well as possibly the base Item class.

### Part 2

Make your item transition Mario into a new state!

#### Tips For Making A New State
Note: The term state is slightly confusing because it's used in two ways in this project. Mario has "action states", like walking, jumping, in air, and ducking. Mario also has "Mario states", like little Mario, big Mario, and ducking Mario. (Ducking is technically both an action state and a Mario state. This is because it needs custom enter and update behavior from an action state but also physically changes Mario like a Mario state. Don't worry if that didn't make sense!) In this part we're referring more to a "Mario State". For example, Fire Mario from the games could be a Mario State because it changes Mario's physical form but still allows him to perform the various action states.

Notice that the various Mario States are all implemented as GameObjects that are children of MarioHolder, with only one active at a time. This is the recommended way of implementing a Mario State. You'll want to create a new Game Object for your Mario state, giving any colliders and animator necessary. To actually give this state behavior, you'll need to edit the PlayerController class. Notice that there's a mario state variable of type "Mario". The Mario class provides the base behavior for all Mario States. You'll want to create a new class extending the Mario class for your state. Your state will need to do a super call to the constructor of the Mario class it extends. See "SuperMario" script for example. The constructor of the Mario class takes 3 variables: the player controller, the game object used for the state, and the Mario class to transition to after this one takes damage. You'll want to pass these things into your constructor when you create an instance in PlayerController.

To implement your custom state you'll need to add 2 instance variables to PlayerController. First take a look at the section where it defines the variables mario (of type Mario) and superMario (of type SuperMario). You should add a variable for your state with the type being the new class you created! Scrolling down a bit you should see some more variable declarations for marioGO, superMarioGO, and duckingMarioGO. These variables will store a reference to the Game Object used for Mario in each state! You'll want to add a new variable here that will reference the GameObject you added. 

Notice that all the variables mentioned so far are declared but not assigned to anything yet. Instead, everything is assigned in the Start function. The GameObject variables are assigned to the correct game object by using the GameObject.Find function. You should do the same with your GameObject variable. The mario and superMario variables are assigned to new instances of their corresponding classes. You should do the same and create a new instance of your class made for the Mario State. As mentioned above, you'll probably need to pass in 3 things: the player controller (which is conveniently just `this`), the game object for your state (which you should have just assigned) and the variable for the state to transition to after getting hit (which you should be able to pass in a reference to). In addition to assigning these variables you'll want to deactivate your gameobject with SetActive(false).

### Part 3 

Add a UI element to keep track of the player's lives.

#### Tips for The UI Element
You can take a look at the Menu Scene for UI examples. The best place to update your UI element is probably the UIManager.TakeLife() function.

## Project Submission

1. Commit your changes
2. Create your submission branch: ``git checkout -b submit``
3. Push both branches to origin: ``git push origin master submit``
