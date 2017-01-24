using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMushroom : Item {

    Vector3 activatedPosition;
    bool finishedActivation = false;
    bool onFloor = false;
    Vector3 currentSpeed = new Vector3(5, 0);
    float timeToShow = 0.2f;

    public override void Start()
    {
        base.Start();
        activatedPosition = transform.position;
        activatedPosition.y = activatedPosition.y + 1;
    }

    public override void ItemBehavior()
    {
        if (activated)
        {
            timeToShow -= Time.deltaTime;
            if (timeToShow <= 0)
            {
                mySprite.enabled = true;
            }
            if (onFloor)
            {
                if (rb.velocity.magnitude <= 0.1f)
                {
                    currentSpeed.x *= -1;
                    rb.velocity = currentSpeed;
                } else {
                    rb.velocity = currentSpeed;
                }
            }
            if (!finishedActivation) {
                rb.velocity = new Vector3(0, 2f);
            }
            if (transform.position.y >= (activatedPosition.y - 0.01))
            {
                rb.velocity = Vector3.zero;
                myCollider.isTrigger = false;
                finishedActivation = true;
                rb.isKinematic = false;
                rb.velocity = currentSpeed;
            }
        }
    }

    public override void PickUpItem(PlayerController player)
    {
        player.Grow();
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            activated = true;
        }
        mySprite.enabled = false;
    }

    void OnCollisionStay2D(Collision2D coll) {
        onFloor = true;
    }

    void OnCollisionExit2D(Collision2D coll) {
        onFloor = false;
        //rb.AddForce(new Vector3(-15, -200));
    }

}
