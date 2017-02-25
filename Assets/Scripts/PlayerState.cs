using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class LittleMario 
{

    protected PlayerController controller;
    protected GameObject myGameObject;

    protected MarioState(PlayerController controller, GameObject myGameObject)
    {
        this.controller = controller;
        this.myGameObject = myGameObject;
    }

    public LittleMario nextMario
    {
        get;
    }

    public LittleMario prevMario
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

    public abstract string myType();

}
