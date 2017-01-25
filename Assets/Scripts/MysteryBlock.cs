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

    IEnumerator TurnUnbreakable()
    {
        yield return StartCoroutine("MoveUpAndDown");
        blockHit = true;
        anim.SetBool("BlockHit", blockHit);
        yield break;
    }

    protected override void HitByPlayer(GameObject player)
    {
        if (!blockHit)
        {
            StartCoroutine("TurnUnbreakable");
        }
    }


}
