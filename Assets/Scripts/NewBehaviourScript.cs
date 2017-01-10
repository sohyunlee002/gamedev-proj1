using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

    public float speed;
    public float jumpForce;
    public float jumpSpeedCap;
    public LayerMask whatIsGround;
    float moveX;
    float moveJump;
    bool facingRight = true;
    Rigidbody2D rb;
    Animator anim;
    GameObject duckingMario;
    bool grounded = true;
    float jumpingTime = 1;
    PlayerState myState;

    // Use this for initialization
    void Start () {
        rb = this.transform.root.gameObject.GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        anim = this.gameObject.GetComponent<Animator>();
        duckingMario = GameObject.Find("Ducking Mario");
        duckingMario.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate() {

    }

    private class Grounded : PlayerState
    {
        public Grounded() {

        }

        public void Enter()
        {
            throw new NotImplementedException();
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }

        public PlayerState HandleInput()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }

    private class Jumping : PlayerState
    {
        public void Enter()
        {
            throw new NotImplementedException();
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }

        public PlayerState HandleInput()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }

}
