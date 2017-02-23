using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public abstract class MarioState 
{

    protected PlayerController controller;
    protected GameObject myGameObject;

    protected MarioState(PlayerController controller, GameObject myGameObject)
    {
        this.controller = controller;
        this.myGameObject = myGameObject;
    }

    protected abstract MarioState nextMario
    {
        get;
    }

    protected abstract MarioState prevMario
    {
        get;
    }

    //Grow to the next Mario.
    public virtual void Grow()
    {
        myGameObject.SetActive(false);
        controller.marioState = nextMario;
    }
    //Shrink back to the previous Mario.
    public virtual void Shrink()
    {
        myGameObject.SetActive(false);
        controller.marioState = prevMario;
    }
    public virtual void HandleInput()
    {
        controller.myState.HandleInput();
    }
}
