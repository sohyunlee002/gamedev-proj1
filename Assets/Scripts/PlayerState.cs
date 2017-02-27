using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class LittleMario 
{

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
    
    public abstract string Type
    {
        get;
    }

    //Shrink back to the previous Mario.
    public virtual void Shrink()
    {
        myGameObject.SetActive(false);
        controller.marioState = prevMario;
    }
    
    //Grow to the next Mario.
    public virtual void Grow()
    {
        myGameObject.SetActive(false);
        controller.marioState = nextMario;
    }

    public virtual void HandleInput()
    {
        controller.myState.HandleInput();
    }

}
