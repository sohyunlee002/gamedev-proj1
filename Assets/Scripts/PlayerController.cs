using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    float groundAcceleration = 15;
    float maxSpeed = 7.5f;
    public float jumpForce = 750;
    public LayerMask whatIsGround;
    float moveX;
    float moveJump;
    bool facingRight = true;
    Rigidbody2D rb;
    Animator anim;
    bool grounded = true;
    float jumpingTime = 1;
    PlayerState myState;
    PlayerState nextState;
    bool stateEnded;
    GameObject duckingMario;
    GameObject littleMario;
    GameObject superMario;
    UI ui;
    bool super;
    bool little;
    float castHeight;

    //Awake is called before any Start function
    void Awake() {
        littleMario = GameObject.Find("Little Mario");
        superMario = GameObject.Find("Super Mario");
    }

    // Use this for initialization
    void Start () {
        rb = this.transform.root.gameObject.GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        anim = this.gameObject.GetComponent<Animator>();
        myState = new Grounded(this);
        ui = GameObject.Find("UI_Canvas").GetComponent<UI>();
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
        myState.Update();
        if (Input.anyKeyDown || stateEnded)
        {
            nextState = myState.HandleInput();
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
        if (nextState != null)
        {
            stateEnded = false;
            myState.Exit();
            myState = nextState;
            nextState = null;
            myState.Enter();
        }
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
        castHeight = mySprite.sprite.bounds.size.y / 2 + 0.25f;
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
            superMario.transform.position = new Vector3(this.transform.position.x, this.transform.position.y);
            littleMario.SetActive(false);
        }
    }

    public void Shrink() {
        //If littleMario then gameOver.
        //If superMario turn into littleMario.
        //If fireMario turn into superMario.
        if (little)
        {
            Debug.Log("Game Over!");
            //Add other stuff here but for now, just
            ui.TakeLife();
        }
        else if (super) {
            littleMario.SetActive(true);
            littleMario.transform.position = new Vector3(this.transform.position.x, littleMario.transform.position.y + 0.5f);
            superMario.SetActive(false);
        }
    }

    public void OnCollisionEnter2D(Collision2D coll) {
        switch (LayerMask.LayerToName(coll.gameObject.layer))
        {
            case "Item":
                Item item = coll.collider.GetComponent<Item>();
                item.PickUpItem(this);
                ui.UpdateScore(item.GetScore());
                break;
            case "Enemy":
                //On top collider: kill enemy
                //Anywhere else: Mario takes damage
                Enemy enemy = coll.transform.parent.GetComponentInChildren<Enemy>();
                if (coll.collider.tag == "Enemy_Top")
                {
                    enemy.HitByPlayer(this);
                    ui.UpdateScore(enemy.GetScore());
                }
                else
                {
                    enemy.HitPlayer(this);
                }
                break;
        }
    }

    private class Grounded : PlayerState
    {

        PlayerController controller;
        Rigidbody2D rb;
        Animator anim;
        float moveX;
        float moveJump;
        float jumpForce;
        float maxSpeed;
        float groundAcceleration;

        public Grounded(PlayerController controller) {
            this.controller = controller;
            this.rb = controller.rb;
            this.anim = controller.anim;
            this.moveX = controller.moveX;
            this.moveJump = controller.moveJump;
            this.jumpForce = controller.jumpForce;
            this.maxSpeed = controller.maxSpeed;
            this.groundAcceleration = controller.groundAcceleration;

        }

        public void Enter()
        {
            moveX = Input.GetAxis("Horizontal");
            moveJump = Input.GetAxis("Jump");
            anim.SetBool("Grounded", true);
        }

        public void Update()
        {
            moveX = Input.GetAxis("Horizontal");
            moveJump = Input.GetAxis("Jump");
            if (Input.GetButton("Vertical") && controller.super)
            {
                controller.Duck();
            }
        }

        public void FixedUpdate() {
            if(Mathf.Abs(rb.velocity.magnitude) <= maxSpeed)
            {
                rb.AddForce(new Vector3(groundAcceleration * moveX, 0));
            }
            /*if (Mathf.Abs(rb.velocity.x) <= 3)
            {
                Debug.Log("falling slowly");
                rb.velocity = Vector3.zero;
            }*/
            //Check if falling. Pause animation at current frame
            //and add the extra gravity.
            Debug.Log(rb.velocity.y);
            if (rb.velocity.y < -0.05f)
            {
                controller.stateEnded = true;
            }
        }

        public void Exit()
        {
            /*Determine the animation state. */
            if (Input.GetButton("Jump"))
            {
                rb.AddForce(new Vector3(0, moveJump * jumpForce));
                anim.SetBool("Grounded", false);
                anim.SetBool("Jumping", true);
            }
            else
            {
                anim.enabled = false;
            }

        }

        public PlayerState HandleInput()
        {
            //controller.anim.SetBool("Grounded", false);
            if (Input.GetButton("Jump") || controller.stateEnded)
            {
                return new InAir(controller);
            }
            else
            {
                return null;
            }
        }
    }

    private class InAir : PlayerState
    {

        PlayerController controller;
        Rigidbody2D rb;
        Animator anim;
        float moveX;
        float moveJump;
        float jumpingTime;
        float airHorizAcceleration;
        float airVerticalAcceleration;

        public InAir(PlayerController controller)
        {
            this.controller = controller;
            this.rb = controller.rb;
            this.anim = controller.anim;
            this.moveX = controller.moveX;
            this.moveJump = controller.moveJump;
            if(Input.GetButton("Jump"))
            {
              jumpingTime = 1;
            }
            else
            {
              jumpingTime = 0;
            }
        }

        public void Enter()
        {
            moveJump = Input.GetAxis("Jump");
            moveX = Input.GetAxis("Horizontal");
            //This only adds force based on the value of
            //moveJump - which should be zero if its not
            //pressed.
        }

        public void Update()
        {
            moveJump = Input.GetAxis("Jump");
            moveX = Input.GetAxis("Horizontal");
        }

        public void FixedUpdate()
        {
            //Jumping timer
            jumpingTime -= Time.deltaTime;
            //Control in the air
            if (Mathf.Abs(rb.velocity.x) <= controller.maxSpeed)
            {
                rb.AddForce(new Vector3(moveX * 5, 0));
            }
            if (jumpingTime >= 0 && Input.GetButton("Jump"))
            {
                rb.AddForce(new Vector3(0, 15));
            }
            //Continuously check that you haven't hit the ground.
            if (controller.CheckForGround())
            {
                controller.stateEnded = true;
            }
        }

        public void Exit()
        {
            anim.enabled = true;
            anim.SetBool("Jumping", false);
            rb.velocity = new Vector3(rb.velocity.x, 0);
        }

        public PlayerState HandleInput()
        {
            if (controller.stateEnded)
            {
                return new Grounded(controller);
            }
            else
            {
                return null;
            }
        }
    }

}
