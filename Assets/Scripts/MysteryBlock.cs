using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBlock : Block
{

    Animator anim;
    bool blockHit = false;

    protected override void Start()
    {
        base.Start();
        anim = transform.parent.GetComponent<Animator>();
    }

    protected override void FixedUpdate()
    {
        if (!blockHit)
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
                    blockHit = true;
                    anim.SetBool("BlockHit", blockHit);
                }
            }
        }
    }

    protected override void OnCollisionEnter2D(Collision2D coll)
    {
        base.OnCollisionEnter2D(coll);
    }

}
