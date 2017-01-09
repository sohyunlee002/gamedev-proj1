using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 5;
    float moveX;
    bool facingRight = true;
    Rigidbody2D rb;
    Animator anim;

	// Use this for initialization
	void Start () {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        anim = this.gameObject.GetComponent<Animator>();
	}
	
    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);
        if (moveX < 0 && facingRight)
        {
            Flip();
        }
        else if (moveX > 0 && !facingRight) {
            Flip();
        }
    }

	// Update is called once per frame
	void Update () {
        moveX = Input.GetAxis("Horizontal");
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    void Flip() {
        facingRight = !facingRight;
        Vector3 scale = this.gameObject.transform.localScale;
        scale.x = scale.x * -1;
        this.gameObject.transform.localScale = scale;
    }
}
