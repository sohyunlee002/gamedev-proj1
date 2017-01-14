using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 10;
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
    bool super;
    bool little;

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
    }

    bool CheckForGround() {
        Vector2 origin = new Vector2(rb.position.x + 0.5f, rb.position.y + 1);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1.2f, whatIsGround);
        Debug.DrawRay(origin, Vector2.down * 1.2f);
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
            Destroy(this.gameObject);
        }
        else if (super) {
            littleMario.SetActive(true);
            superMario.SetActive(false);
        }
    }

    public void OnCollisionEnter2D(Collision2D coll) {
        switch (LayerMask.LayerToName(coll.gameObject.layer)) {
            case "Item":
                Item item = coll.collider.GetComponent<Item>();
                item.PickUpItem(this);
                break;
            case "Enemy":
                Debug.Log(coll.collider.tag);
                //On top collider: kill enemy
                //Anywhere else: Mario takes damage
                Enemy enemy = coll.transform.parent.GetComponentInChildren<Enemy>();
                if (coll.collider.tag == "Enemy_Top")
                {
                    enemy.HitByPlayer(this);
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
        float speed;

        public Grounded(PlayerController controller) {
            this.controller = controller;
            this.rb = controller.rb;
            this.anim = controller.anim;
            this.moveX = controller.moveX;
            this.speed = controller.speed;
        }

        public void Enter()
        {
            anim.SetBool("Grounded", true);
        }

        public void Update()
        {
            moveX = controller.moveX;
            //Debug.Log(moveX);
            if (Input.GetKey(KeyCode.S))
            {
                controller.Duck();   
            }
        }

        public void FixedUpdate() {
            rb.velocity = new Vector2(moveX * speed, rb.velocity.y);
            //Check if falling. Pause animation at current frame
            //and add the extra gravity.
            if (Mathf.Abs(rb.velocity.y) > 0)
            {
                anim.enabled = false;
                rb.AddForce(new Vector2(0, -30));
            }
            else {
                anim.enabled = true;
            }
        }

        public void Exit()
        {
            anim.SetBool("Grounded", false);
        }

        public PlayerState HandleInput()
        {
            controller.anim.SetBool("Grounded", false);
            if (Input.GetKey(KeyCode.Space))
            {
                return new Jumping(controller);
            }
            return null;
        }
    }

    private class Jumping : PlayerState
    {

        PlayerController controller;
        Rigidbody2D rb;
        Animator anim;
        float moveX;
        float moveJump;
        float jumpForce;
        float jumpingTime = 1;

        public Jumping(PlayerController controller)
        {
            this.controller = controller;
            this.rb = controller.rb;
            this.anim = controller.anim;
            this.moveX = controller.moveX;
            this.moveJump = controller.moveJump;
            this.jumpForce = controller.jumpForce;
        }

        public void Enter()
        {
            moveJump = Input.GetAxis("Jump");
            moveX = Input.GetAxis("Horizontal");
            //Add preliminary jump force
            rb.AddForce(new Vector2(0, moveJump * jumpForce));
            anim.SetBool("Jumping", true);
        }

        public void Update()
        {
            moveJump = Input.GetAxis("Jump");
        }

        public void FixedUpdate()
        {
            //Jumping timer
            jumpingTime -= Time.deltaTime;
            //Extra gravity
            rb.AddForce(new Vector2(0, -30));
            if (jumpingTime >= 0 && Input.GetKey(KeyCode.Space))
            {
                rb.AddForce(new Vector2(0, 13));
            }
            if (controller.CheckForGround())
            {
                controller.stateEnded = true;
            }
        }

        public void Exit()
        {
            anim.SetBool("Jumping", false);
        }

        public PlayerState HandleInput()
        {
            if (controller.stateEnded)
            {
                return new Grounded(controller);
            }
            else {
                return null;
            }
        }
    }

}
