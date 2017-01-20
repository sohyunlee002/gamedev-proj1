using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Block : MonoBehaviour {

    protected Vector3 upPosition;
    protected Vector3 downPosition;
    protected bool moveUp = false;
    protected bool moveDown = false;
    protected float speed = 5;

    protected virtual void Start ()
    {
        upPosition = transform.parent.position;
        upPosition.y = upPosition.y + 0.5f;
        downPosition = transform.parent.position;
    }
	
	protected virtual void FixedUpdate ()
    {
        float step = Time.deltaTime * speed;
        if (moveUp)
        {
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, upPosition, step);
        }
        if (transform.parent.position == upPosition)
        {
            moveUp = false;
            moveDown = true;
        }
        if (moveDown)
        {
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, downPosition, step);
            if (transform.parent.position == downPosition)
            {
                moveDown = false;
            }
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.tag == "Player") {
            moveUp = true;
        }
    }

}
