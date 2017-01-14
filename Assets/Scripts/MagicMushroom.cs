using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMushroom : Item {

    Vector3 activatedPosition;
    bool finishedActivation = false;
    bool onFloor = false;
    Vector3 currentSpeed = new Vector3(5, 0);

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
            //transform.position = Vector3.MoveTowards(transform.localPosition, activatedPosition, activationSpeed);
            //rb.MovePosition(new Vector2(activatedPosition.x, activatedPosition.y));*/
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

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            activated = true;
        }
    }

    /*void OnCollisionEnter2D(Collision2D coll) {
        if (coll.collider.tag == "Player") {
            PlayerController player = coll.collider.GetComponent<PlayerController>();
            PickUpItem(player);
        }
    }*/

    void OnCollisionStay2D(Collision2D coll) {
        onFloor = true;
    }

    void OnCollisionExit2D(Collision2D coll) {
        onFloor = false;
        rb.AddForce(new Vector3(-15, -200));
    }

}
