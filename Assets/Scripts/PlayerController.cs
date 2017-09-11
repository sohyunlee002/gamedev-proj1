using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    UIManager uiManager;

    //The action that Mario is currently performing.
    public ActionState myState;
    //The form that Mario is currently in.
    public MarioForm marioForm;
    //The animator of the form that Mario is currently in (on the form's
    //associated GameObject and set by MarioForm)
    public Animator anim;

    //References to the two Mario forms. 
    //Little Mario
    public MarioForm littleMarioForm;
    //Super Mario
    public SuperMarioForm superMarioForm;

    //The GameObjects on the MarioHolder.
    GameObject marioGO;
    GameObject superMarioGO;
    GameObject duckingMarioGO;

    //These vars to be set in the editor. 
    public float jumpForce = 750;
    public LayerMask whatIsGround;
    
    float groundAcceleration = 15;
    float airHorizAcceleration = 5;
    float airJumpAcceleration = 18;
    float maxSpeed = 7.5f;
    float moveX;
    float moveJump;
    bool facingRight = true;
    bool playerHit = false;
    Rigidbody2D rb;

    int playerLayer = 12;
    int playerInvincibleLayer = 14;

    // Use this for initialization
    void Start () {
        marioGO = GameObject.Find("Little Mario");
        superMarioGO = GameObject.Find("Super Mario");
        duckingMarioGO = GameObject.Find("Ducking Mario");
        uiManager = UIManager.uiManager;
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        //Initialize Mario forms
        littleMarioForm = new MarioForm(this, marioGO);
        superMarioForm = new SuperMarioForm(this, superMarioGO, littleMarioForm);
        //Set initial form and action state
        myState = new Walking(this);
        marioForm = littleMarioForm;
        marioForm.Enter();
        //Always start in Little Mario
        duckingMarioGO.SetActive(false);
        superMarioGO.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        moveX = Input.GetAxis("Horizontal");
        moveJump = Input.GetAxis("Jump");
	//Delegate to current action state
        myState.Update();
    }

    void FixedUpdate() {
        if (anim.gameObject.activeSelf) {
            anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
            anim.SetFloat("vSpeed", Mathf.Abs(rb.velocity.y));
        }
        if (moveX < 0 && facingRight)
        {
            Flip();
        }
        else if (moveX > 0 && !facingRight)
        {
            Flip();
        }
	//Delegate to current action state
        myState.FixedUpdate();
        if (gameObject.transform.localPosition.y < -1) {
            uiManager.TakeLife();
            gameObject.SetActive(false);
        }
    }

    //Transition between action states. Called from the current action
    //state with the state to transtition to as the argument.
    void TransitionActionState(ActionState nextState)
    {
        myState.Exit();
        myState = nextState;
        myState.Enter();
    }

    //Transition to the next Mario form. Called by the Item with which the
    //player collides, with the next Mario form as the argument.
    public void Grow(MarioForm nextMario)
    {
        marioForm = marioForm.Exit(nextMario);
        marioForm.Enter();
    }

    public void Shrink()
    {
        if (marioForm != null)
        {
            marioForm = marioForm.Exit(marioForm.prevMario);
        }
       
        if (marioForm == null)
        {
            uiManager.TakeLife();
            gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine("Invulnerable");
        }
    }

    IEnumerator Invulnerable() {
        marioForm.Enter();
        playerHit = true;

        marioGO.layer = playerInvincibleLayer;
        superMarioGO.layer = playerInvincibleLayer;
        duckingMarioGO.layer = playerInvincibleLayer;

        yield return new WaitForSeconds(1);

        marioGO.layer = playerLayer;
        superMarioGO.layer = playerLayer;
        duckingMarioGO.layer = playerLayer;

        playerHit = false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = gameObject.transform.localScale;
        scale.x = scale.x * -1;
	    gameObject.transform.localScale = scale;
        rb.AddForce(new Vector3(-25 * rb.velocity.x, 0));
    }

    bool CheckForGround()
    {
        SpriteRenderer mySprite = marioForm.gameObject.GetComponent<SpriteRenderer>();
        float castHeight = mySprite.sprite.bounds.size.y / 2 + 0.25f;
        Vector3 origin = new Vector3(transform.position.x, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.down, castHeight, whatIsGround);
        Debug.DrawRay(origin, Vector3.down * castHeight);
        return hit.collider != null;
    }

    public void OnCollisionEnter2D(Collision2D coll) {
        switch (LayerMask.LayerToName(coll.gameObject.layer))
        {
            case "Item":
                Item item = coll.collider.GetComponent<Item>();
                if (!item.isPickedUp()) {
                    item.PickUpItem(this);
                    uiManager.UpdateScore(item.GetScore());
                }
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
                    if (!playerHit) {
                        enemy.HitPlayer(this);
                    }
                }
                break;
        }
    }

    private class Walking : ActionState
    {

        float lastFrameVertical;

        public Walking(PlayerController controller) : base(controller)
        {
            
        }

        public override void Enter()
        {
            controller.moveX = Input.GetAxis("Horizontal");
            controller.moveJump = Input.GetAxis("Jump");
            lastFrameVertical = Input.GetAxis("Vertical");
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
            else if (Input.GetAxis("Vertical") < lastFrameVertical && Input.GetAxis("Vertical") <= -0.01f 
                && controller.marioForm.CanDuck())
            {
                controller.TransitionActionState(new Ducking(controller, controller.marioForm.gameObject));
            }
            lastFrameVertical = Input.GetAxis("Vertical");
        }

        public override void FixedUpdate() 
        {
            if(Mathf.Abs(controller.rb.velocity.magnitude) <= controller.maxSpeed)
            {
                controller.rb.AddForce(new Vector3(controller.groundAcceleration * controller.moveX, 0));
            }
            //Check if falling. Pause animation at current frame
            //and add the extra gravity.
            if (controller.rb.velocity.y < -2)
            {
                controller.TransitionActionState(new InAir(controller));
            }
        }

        public override void Exit()
        {
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

        public override string Type
        {
            get
            {
                return "InAir";
            }
        }

    }

    private class Jumping : ActionState
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

	public override void Update()
        {
            controller.moveJump = Input.GetAxis("Jump");
            controller.moveX = Input.GetAxis("Horizontal");
        }
	
        public override void FixedUpdate()
        {
            if (Mathf.Abs(controller.rb.velocity.x) <= controller.maxSpeed)
            {
                controller.rb.AddForce(new Vector3(controller.moveX * controller.airHorizAcceleration, 0));
            }
            jumpingTime -= Time.deltaTime;
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

        public override string Type
        {
            get
            {
                return "Jumping";
            }
        }

    }

    private class Ducking : ActionState
    {

        GameObject prevMarioGO;
        GameObject duckingMarioGO;

        public Ducking(PlayerController controller, GameObject prevMarioGO) : base(controller)
        {
            this.prevMarioGO = prevMarioGO;
            this.duckingMarioGO = controller.duckingMarioGO;
        }

        public override void Enter()
        {
            //Activate DuckingMario,
            //and stop it from sliding
            duckingMarioGO.SetActive(true);
            duckingMarioGO.transform.position = new Vector3(controller.rb.position.x, duckingMarioGO.transform.position.y);
            controller.rb.velocity = Vector3.zero;
            Vector3 scale = controller.gameObject.transform.localScale;
            scale.x = scale.x * Mathf.Sign(scale.x);
            prevMarioGO.SetActive(false);
        }

        public override void Exit()
        {
            //Deactivate DuckingMario, and reactivate previous Mario
            duckingMarioGO.SetActive(false);
            prevMarioGO.SetActive(true);
        }

        public override void Update()
        {
            if (Input.GetButtonUp("Vertical"))
            {
                  controller.TransitionActionState(new Walking(controller));
            }
        }

        public override string Type
        {
            get
            {
                return "Ducking";
            }
        }
    }

}
