using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    /*Keep all the animators on the parent gameObject? 
    * And then the children just hold the colliders
    */

    UIManager uiManager;

    float groundAcceleration = 15;
    float airHorizAcceleration = 5;
    float airJumpAcceleration = 18;
    float maxSpeed = 7.5f;
    public float jumpForce = 750;
    public LayerMask whatIsGround;
    float moveX;
    float moveJump;
    bool facingRight = true;
    Rigidbody2D rb;
    Animator anim;
    public ActionState myState;
    public MarioState marioState;
    bool stateEnded;
    GameObject duckingMario;
    GameObject littleMario;
    GameObject superMario;
    GameObject fireMario;
    bool super;
    bool little;

    //Awake is called before any Start function
    void Awake() {
        littleMario = GameObject.Find("Little Mario");
        superMario = GameObject.Find("Super Mario");
    }

    // Use this for initialization
    void Start () {
        uiManager = UIManager.uiManager;
        rb = this.transform.root.gameObject.GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        anim = this.gameObject.GetComponent<Animator>();
        myState = new Walking(this);
        if (gameObject.name == "Super Mario")
        {
            super = true;
            little = false;
            duckingMario = GameObject.Find("Ducking Mario");
            duckingMario.SetActive(false);
            superMario.SetActive(false);
        }
        else
        {
            super = false;
            little = true;
            duckingMario = null;
        }
    }

    // Update is called once per frame
    void Update () {
        moveX = Input.GetAxis("Horizontal");
        moveJump = Input.GetAxis("Jump");
        myState.Update();
        if (Input.anyKey)
        {
            myState.HandleInput();
        }
    }

    void FixedUpdate() {
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("vSpeed", Mathf.Abs(rb.velocity.y));
        if (moveX < 0 && facingRight)
        {
            Flip();
        }
        else if (moveX > 0 && !facingRight)
        {
            Flip();
        }
        myState.FixedUpdate();
    }

    void TransitionActionState(ActionState nextState)
    {
        myState.Exit();
        myState = nextState;
        myState.Enter();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = this.gameObject.transform.localScale;
        scale.x = scale.x * -1;
        this.gameObject.transform.localScale = scale;
        rb.AddForce(new Vector3(-25 * rb.velocity.x, 0));
    }

    bool CheckForGround() {
        SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        float castHeight = mySprite.sprite.bounds.size.y / 2 + 0.25f;
        Vector3 origin = new Vector3(transform.position.x, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.down, castHeight, whatIsGround);
        Debug.DrawRay(origin, Vector3.down * castHeight);
        return hit.collider != null;
    }

    void Duck()
    {
        duckingMario.SetActive(true);
        duckingMario.transform.position = new Vector3(rb.position.x, duckingMario.transform.position.y);
        rb.velocity = Vector3.zero;
        if (!facingRight)
        {
            Vector3 scale = duckingMario.transform.localScale;
            scale.x = scale.x * -1;
            duckingMario.transform.localScale = scale;
        }
        this.gameObject.SetActive(false);
    }

    public void Grow() {
        //If littleMario turn into superMario.
        //If superMario turn into fireMario.
        if (little) {
            superMario.SetActive(true);
            superMario.transform.position = new Vector3(this.transform.position.x, superMario.transform.position.y);
            littleMario.SetActive(false);
        }
    }

    public void Shrink() {
        //If littleMario then gameOver.
        //If superMario turn into littleMario.
        //If fireMario turn into superMario.
        if (little)
        {
            uiManager.TakeLife();
            this.gameObject.SetActive(false);
        }
        else if (super) {
            littleMario.SetActive(true);
            littleMario.transform.position = new Vector3(this.transform.position.x, littleMario.transform.position.y);
            superMario.SetActive(false);
        }
    }

    public void OnCollisionEnter2D(Collision2D coll) {
        switch (LayerMask.LayerToName(coll.gameObject.layer))
        {
            case "Item":
                Item item = coll.collider.GetComponent<Item>();
                item.PickUpItem(this);
                uiManager.UpdateScore(item.GetScore());
                break;
            case "Enemy":
                //On top collider: kill enemy
                //Anywhere else: Mario takes damage
                Enemy enemy = coll.transform.parent.GetComponentInChildren<Enemy>();
                if (coll.collider.tag == "Enemy_Top")
                {
                    enemy.HitByPlayer(this);
                    uiManager.UpdateScore(enemy.GetScore());
                }
                else
                {
                    enemy.HitPlayer(this);
                }
                break;
        }
    }

    private class LittleMario
    {

        protected PlayerController controller;
        protected GameObject myGameObject;

        public LittleMario(PlayerController controller, GameObject myGameObject)
        {
            //Is it best to do these things in the constructor? 
            //Or just add an Enter() function and call it each time.
            controller.littleMario.SetActive(true);
            controller.littleMario.transform.position = 
                new Vector3(controller.transform.position.x, controller.littleMario.transform.position.y);
        }

        public LittleMario nextMario
        {
            get
            {
                return new SuperMario(controller);
            }
        }

        public LittleMario prevMario
        {
            get
            {
                return null;
            }
        }

        public string Type 
        {
            get
            {
                return "Little"
            }
        }

        //Shrink to the previous Mario.
        public void Shrink()
        {
            controller.uiManager.TakeLife();
            controller.gameObject.SetActive(false);
        }
        
        //Grow to the next Mario.
        public void Grow()
        {
            myGameObject.SetActive(false);
            controller.marioState = nextMario;
        }
        
        public void Update(){}
        
    }

    private class SuperMario : LittleMario
    {
        public SuperMario(PlayerController controller) : base(controller, controller.superMario)
        {
            controller.superMario.SetActive(true);
            controller.superMario.transform.position = 
                new Vector3(controller.transform.position.x, controller.superMario.transform.position.y);
        }

        public new LittleMario nextMario
        {
            get
            {
                return new FireMario(controller);
            }
        }

        public new LittleMario prevMario
        {
            get
            {
                return new LittleMario(controller);
            }
        }
        
        public new string Type 
        {
            get
            {
                return "Super"
            }
        }

        public new void Update()
        {
            if ((Input.GetButton("Vertical") && Input.GetAxis("Vertical") < -0.01f 
                && controller.marioState.myType() != "Little") && (controller.myState.Type && "Jumping" && controller.myState.Type != "Ducking"))
            {
                //Enter ducking state.
                controller.TransitionActionState(new Ducking(controller, this), this);
            }
        }

    }

    private class FireMario : SuperMario
    {
        public FireMario(PlayerController controller) : base(controller, controller.fireMario)
        { }

        public new LittleMario nextMario
        {
            get
            {
                return null;
            }
        }

        public new LittleMario prevMario
        {
            get
            {
                return new SuperMario(controller);
            }
        }
        
        public new string Type 
        {
            get
            {
                return "Fire"
            }
        }

        public new void Update()
        {
            //Check if you can throw stuff here.
        }

    }

    private class Walking : ActionState
    {

        public Walking(PlayerController controller) : base(controller)
        {
            
        }

        public override void Enter()
        {
            controller.moveX = Input.GetAxis("Horizontal");
            controller.moveJump = Input.GetAxis("Jump");
            controller.anim.SetBool("Grounded", true);
        }

        public override void Update()
        {
            controller.moveX = Input.GetAxis("Horizontal");
            controller.moveJump = Input.GetAxis("Jump");
            if (Input.GetButton("Jump"))
            {
                controller.TransitionActionState(new Jumping(controller));
            }
        }

        public override void FixedUpdate() 
        {
            if(Mathf.Abs(controller.rb.velocity.magnitude) <= controller.maxSpeed)
            {
                controller.rb.AddForce(new Vector3(controller.groundAcceleration * controller.moveX, 0));
            }
            /*if (Mathf.Abs(rb.velocity.x) <= 3)
            {
                Debug.Log("falling slowly");
                rb.velocity = Vector3.zero;
            }*/
            //Check if falling. Pause animation at current frame
            //and add the extra gravity.
            if (controller.rb.velocity.y < -2)
            {
                controller.TransitionActionState(new InAir(controller));
            }
        }

        public override void Exit()
        {
            /*Determine the animation state. */
            //This is why we need a "Walking" Animation state!
            //Walking won't always lead to Jumping - you could be 
            //shooting, or ducking!
            controller.anim.SetBool("Grounded", false);
        }
    }

    private class InAir : ActionState
    {

        public InAir(PlayerController controller) : base(controller);

        public override void Enter()
        {
            controller.anim.enabled = false;
        }

        public override void Exit()
        {
            controller.anim.enabled = true;
        }

        public override void FixedUpdate()
        {
            if (Mathf.Abs(controller.rb.velocity.x) <= controller.maxSpeed)
            {
                controller.rb.AddForce(new Vector3(controller.moveX * controller.airHorizAcceleration, 0));
            }
            if (controller.CheckForGround())
            {
                controller.TransitionActionState(new Walking(controller));
            }
        }

        public override void Update()
        {
            controller.moveJump = Input.GetAxis("Jump");
            controller.moveX = Input.GetAxis("Horizontal");
        }
    }

    private class Jumping : InAir
    {

        float jumpingTime;

        public Jumping(PlayerController controller) : base(controller)
        {
            this.jumpingTime = 1;
        }

        public new void Enter()
        {
            //This only adds force based on the value of
            //moveJump - which should be zero if its not
            //pressed.
            controller.rb.AddForce(new Vector3(0, controller.moveJump * controller.jumpForce));
            controller.anim.SetBool("Jumping", true);
        }

        public new void FixedUpdate()
        {
            base.FixedUpdate();
            //Jumping timer
            jumpingTime -= Time.deltaTime;
            //Control in the air
            if (jumpingTime >= 0)
            {
                controller.rb.AddForce(new Vector3(0, controller.moveJump * controller.airJumpAcceleration));
            }
            //Once you've reached the apex of your jump and you start falling,
            //re-enter the InAir state.
            if (controller.rb.velocity.y < -0.01f) 
            {
                controller.TransitionActionState(new InAir(controller))
            }
        }

        public new void Exit()
        {
            controller.anim.SetBool("Jumping", false);
            controller.rb.velocity = new Vector3(controller.rb.velocity.x, 0);
        }
    }

    private class Ducking : ActionState
    {

        GameObject prevMarioGO;

        public Ducking(PlayerController controller, GameObject prevMarioGO) : base(controller)
        {
            this.prevMarioGO = prevMarioGO;
        }

        public void Enter()
        {
            //Activate DuckingMario,
            //and stop it from sliding
        }

        public void Exit()
        {
            //Deactivate DuckingMario, and reactivate previous Mario
            prevMarioGO.SetActive(true);
        }

        public void Update()
        {
            if (Input.GetButtonUp("Vertical"))
            {
                //You can only duck when you're on the ground.
                controller.TransitionActionState(new Walking(controller));
            }
        }
    }

    private class Shooting : ActionState
    {

        ActionState prev;
        PlayerController controller;

        public Shooting(PlayerController controller, ActionState prev)
        {
            this.prev = prev;
            this.controller = controller;
        }

        public void Enter()
        {
            //Play the animation, spawn the projectile, and then exit.
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }

        public void FixedUpdate()
        {
            throw new NotImplementedException();
        }

        public void HandleInput()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }

}
