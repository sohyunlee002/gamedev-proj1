using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioForm
{

    protected PlayerController controller;
    public GameObject gameObject;
    public MarioForm prevMario = null;
    protected bool canDuck = false;

    public MarioForm(PlayerController controller, GameObject gameObject, MarioForm prevMario = null)
    {
        this.controller = controller;
        this.gameObject = gameObject;
        this.prevMario = prevMario;
    }

    public void Enter()
    {
        controller.anim = gameObject.GetComponent<Animator>();
        gameObject.SetActive(true);
        gameObject.transform.position = 
            new Vector3(controller.transform.position.x, gameObject.transform.position.y);
        Vector3 scale = gameObject.transform.localScale;
        scale.x = scale.x * Mathf.Sign(controller.transform.localScale.x);
    }

    public MarioForm Exit(MarioForm newState) {
        gameObject.SetActive(false);
        return newState;
    }

    public bool CanDuck()
    {
        return canDuck;
    }

}
