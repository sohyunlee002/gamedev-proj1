using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBlock : Block {

    bool breakBlock = false;

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (breakBlock)
        {
            //Play the animation for breaking: TBA
            //Then deactivate object.
            transform.parent.gameObject.SetActive(false);
        }
    }

    protected override void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log(coll.collider.tag);
        if (coll.collider.name == "Super Mario")
        {
            breakBlock = true;
            return;
        }
        base.OnCollisionEnter2D(coll);
    }

}
