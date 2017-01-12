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
        base.FixedUpdate();
        if (transform.parent.position == downPosition && blockHit)
        {
            anim.SetBool("BlockHit", true);
        }
    }

    protected override void OnCollisionEnter2D(Collision2D coll)
    {
        base.OnCollisionEnter2D(coll);
        if (!blockHit) {
            blockHit = true;
        }
    }

}
