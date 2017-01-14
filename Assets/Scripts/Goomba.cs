using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : Enemy {

    Vector3 currentSpeed;

    public override void Start() {
        base.Start();
        currentSpeed = new Vector3(walkingSpeed, 0);
        rb.velocity = currentSpeed;
    }

	// Update is called once per frame
	public override void FixedUpdate () {
        if (dead)
        {
            if (timeToDeath <= 0)
            {
                Destroy(this.gameObject);
            }
            timeToDeath -= Time.deltaTime;
        }
        else
        {
            if (rb.velocity.magnitude <= 0.1f)
            {
                currentSpeed.x *= -1;
                rb.velocity = currentSpeed;
            }
            else
            {
                rb.velocity = currentSpeed;
            }
        }
    }

    public override void HitByPlayer(PlayerController player)
    {
        anim.SetBool("Hit", true);
        rb.velocity = Vector3.zero;
        dead = true;
        foreach (Collider2D collider in myColliders) {
            Destroy(collider);
        }
        rb.isKinematic = true;
    }

    public override void HitPlayer(PlayerController player)
    {
        player.Shrink();
    }
}
