using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_Bounce_Anim : MonoBehaviour {

    // Use this for initialization
    public void Start () {
        Animator anim = this.GetComponent<Animator>();
        AnimationClip clip = anim.runtimeAnimatorController.animationClips[0];

        AnimationClip clip2 = new AnimationClip();
        clip2.frameRate = 24;

        AnimationEvent evt0 = new AnimationEvent();
        AnimationEvent evt1 = new AnimationEvent();
        AnimationEvent evt2 = new AnimationEvent();

        //Event 0:
        evt0.time = 0.0f;
        evt0.functionName = "BlockStart";
        clip.AddEvent(evt0);

        //Event 1:
        evt1.time = 0.12f;
        evt1.functionName = "MoveBlockUp";
        clip.AddEvent(evt1);

        //Event 2:
        evt2.time = 0.24f;
        evt2.functionName = "MoveBlockDown";
        clip.AddEvent(evt2);
	}

    public void BlockStart() {}

    // Update is called once per frame
    public void MoveBlockUp() {
        Vector3 newPos = this.transform.position;
        newPos.y = newPos.y + 0.5f;
        this.transform.position = newPos;
        Debug.Log("idk why its not moving");
    }

    public void MoveBlockDown() {
        Vector3 newPos = this.transform.position;
        newPos.y = newPos.y - 0.5f;
        this.transform.position = newPos;
    }

}
