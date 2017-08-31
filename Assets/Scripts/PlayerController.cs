using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    /*Keep all the animators on the parent gameObject? 
    * And then the children just hold the colliders
    */

    UIManager uiManager;

    //The action that Mario is currently performing.
    public ActionState myState;
    //The form that Mario is currently in.
    public Mario marioState;
    //The animator on the form that Mario is currently in.
    public Animator anim;

    //Little Mario
    public Mario mario;
    //Super Mario
    public SuperMario superMario;
    //Walking state
    ActionState walking;
    //Jumping state
    ActionState jumping;
    //InAir state
    ActionState inAir;
    //Ducking state
    ActionState ducking;

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
    GameObject marioGO;
    GameObject superMarioGO;
    GameObject duckingMarioGO;

    //Awake is called before any Start function
    void Awake() {
        marioGO = GameObject.Find("Little Mario");
        superMarioGO = GameObject.Find("Super Mario");
    }

    // Use this for initialization
    void Start () {
        uiManager = UIManager.uiManager;
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        //anim = this.gameObject.GetComponent<Animator>();
        //Initialize states
        mario = new Mario(this, marioGO);
        superMario = new SuperMario(this, superMarioGO, mario);

        /*walking = new Walking(this);
        jumping = new Jumping(this);
        inAir = new InAir(this);
        ducking = new Ducking(this);*/

        //Set initial states
        myState = new Walking(this);
        EnterMarioState(mario);
        if (gameObject.name == "Super Mario")
        {
            duckingMarioGO = GameObject.Find("Ducking Mario");
            duckingMarioGO.SetActive(false);
            superMarioGO.SetActive(false);
        }
        else
        {
            duckingMarioGO = null;
        }
    }

    // Update is called once per frame
    void Update () {
        moveX = Input.GetAxis("Horizontal");
        moveJump = Input.GetAxis("Jump");
        myState.Update();
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

    void EnterMarioState(Mario nextMario) {
        marioState = nextMario;
        marioState.Enter();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = marioState.gameObject.transform.localScale;
        scale.x = scale.x * -1;
	marioState.gameObject.transform.localScale = scale;
        rb.AddForce(new Vector3(-25 * rb.velocity.x, 0));
    }

    bool CheckForGround()
    {
        SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        float castHeight = mySprite.sprite.bounds.size.y / 2 + 0.25f;
        Vector3 origin = new Vector3(transform.position.x, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.down, castHeight, whatIsGround);
        Debug.DrawRay(origin, Vector3.down * castHeight);
        return hit.collider != null;
    }

    void Duck()
    {
        duckingMarioGO.SetActive(true);
        duckingMarioGO.transform.position = new Vector3(rb.position.x, duckingMarioGO.transform.position.y);
        rb.velocity = Vector3.zero;
        if (!facingRight)
        {
            Vector3 scale = duckingMarioGO.transform.localScale;
            scale.x = scale.x * -1;
            duckingMarioGO.transform.localScale = scale;
        }
        this.gameObject.SetActive(false);
    }

    public void Grow(Mario nextMario) {
        //If littleMario turn into superMario.
        //If superMario turn into fireMario.
        marioState.Grow(nextMario);
    }

    public void Shrink(Mario prevMario) {
        //If littleMario then gameOver.
        //If superMario turn into littleMario.
        //If fireMario turn into superMario.
        marioState.Shrink(prevMario);
    }


    //Adapter pattern here
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
                    enemy.HitByPlayer();
                    uiManager.UpdateScore(enemy.GetScore());
                }
                else
                {
                    enemy.HitPlayer(this.marioState);
                }
                break;
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
            if (Input.GetButtonDown("Jump"))
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

        public override string Type
        {
            get
            {
                return "Walking";
            }
        }

    }

    private class InAir : ActionState
    {

        public InAir(PlayerController controller) : base(controller)
        { }

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

        public override string Type
        {
            get
            {
                return "InAir";
            }
        }

    }

    private class Jumping : InAir
    {

        float jumpingTime;

        public Jumping(PlayerController controller) : base(controller)
        {
            this.jumpingTime = 1;
        }

        public override void Enter()
        {
            //This only adds force based on the value of
            //moveJump - which should be zero if its not
            //pressed.
            controller.rb.AddForce(new Vector3(0, controller.moveJump * controller.jumpForce));
            controller.anim.SetBool("Jumping", true);
        }

        public override void FixedUpdate()
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
                controller.TransitionActionState(new InAir(controller));
            }
        }

        public override void Exit()
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

        public override void Enter()
        {
            //Activate DuckingMario,
            //and stop it from sliding
        }

        public override void Exit()
        {
            //Deactivate DuckingMario, and reactivate previous Mario
            prevMarioGO.SetActive(true);
        }

        public override void Update()
        {
            if (Input.GetButtonUp("Vertical"))
            {
                //You can only duck when you're on the ground.
                controller.TransitionActionState(new Walking(controller));
            }
        }

        public override string Type
        aaa{
            get
            {
                return "Ducking";
            }
        }
    }

    private class Shooting : ActionState
    {

        ActionState prev;

        public Shooting(PlayerController controller, ActionState prev) : base(controller)
        {
            this.prev = prev;
        }

        public override void Enter()
        {
            //Play the animation, spawn the projectile, and then exit.
        }

        public override void Exit()
        {
            throw new NotImplementedException();
        }

        public override void FixedUpdate()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        public override string Type
        {
            get
            {
                return "Shooting";
            }
        }
    }

}
