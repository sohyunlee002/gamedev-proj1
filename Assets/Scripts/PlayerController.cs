using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed;
    public float jumpForce;
    public float jumpSpeedCap;
    public float jumpHeightCap;
    public LayerMask whatIsGround;
    float moveX;
    float moveJump;
    bool facingRight = true;
    Rigidbody2D rb;
    Animator anim;
    GameObject duckingMario;
    bool grounded = true;
    float jumpingTime = 1;

	// Use this for initialization
	void Start () {
        rb = this.transform.root.gameObject.GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        anim = this.gameObject.GetComponent<Animator>();
        duckingMario = GameObject.Find("Ducking Mario");
        duckingMario.SetActive(false);
	}
	
    void FixedUpdate()
    {
        Vector2 origin = new Vector2(rb.position.x + 0.5f, rb.position.y + 1);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1.2f, whatIsGround);
        Debug.DrawRay(origin, Vector2.down *  1.2f);
        Debug.Log(hit.collider);
        if (hit.collider != null)
        {
            //Debug.Log("grounded");
            grounded = true;
        }
        else {
            //Debug.Log("in air!");
            grounded = false;
        }
        anim.SetBool("Grounded", grounded);
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);
        if (moveX < 0 && facingRight)
        {
            Flip();
        }
        else if (moveX > 0 && !facingRight)
        {
            Flip();
        }
        if (!grounded)
        {
            rb.AddForce(new Vector2(0, -30));
        }
        if (Input.GetKey(KeyCode.Space))
        {
            jumpingTime -= Time.deltaTime;
            if (grounded)
            {
                jumpingTime = 1;
                rb.AddForce(new Vector2(0, moveJump * jumpForce));
            }
            else if (jumpingTime >= 0)
            {
                rb.AddForce(new Vector2(0, 13));
            }
        }
    }

    // Update is called once per frame
    void Update() {
        moveX = Input.GetAxis("Horizontal");
        moveJump = Input.GetAxis("Jump");
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        //Leave the ground with some basic velocity.
        //Then holding down the space button adds some force.
        if (Input.GetKey(KeyCode.S)) {
            duckingMario.SetActive(true);
            duckingMario.transform.position = new Vector3(rb.position.x, duckingMario.transform.position.y);
            rb.velocity = Vector3.zero;
            if (!facingRight) {
                Vector3 scale = duckingMario.transform.localScale;
                scale.x = scale.x * -1;
                duckingMario.transform.localScale = scale;
            }
            this.gameObject.SetActive(false);
        }
    }

    void Flip() {
        facingRight = !facingRight;
        Vector3 scale = this.gameObject.transform.localScale;
        scale.x = scale.x * -1;
        this.gameObject.transform.localScale = scale;
    }
}
