using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour {

    protected Rigidbody2D rb;
    protected Animator anim;
    protected Collider2D[] myColliders;
    protected float walkingSpeed = 5;
    protected float timeToDeath = 0.5f;
    protected bool dead = false;

    // Use this for initialization
    public virtual void Start () {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        anim = GetComponent<Animator>();
        myColliders = GetComponentsInChildren<Collider2D>();
    }

    public virtual void FixedUpdate () {
		
	}

    public abstract void HitByPlayer(PlayerController player);
    public abstract void HitPlayer(PlayerController player);
}
