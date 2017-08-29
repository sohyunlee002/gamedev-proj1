using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mario
{

    protected PlayerController controller;
    protected GameObject myGameObject;
    //The Mario form that this form would canonically shrink into.
    //Set by the controller.
    public Mario prevMario;

    public Mario(PlayerController controller, GameObject myGameObject, Mario prevMario = null)
    {
        this.controller = controller;
        this.myGameObject = myGameObject;
        //Is it best to do these things in the constructor? 
        //Or just add an Enter() function and call it each time.
        if (prevMario != null) {
            this.prevMario = prevMario;
        }
    }

    public void Enter()
    {
        controller.anim = myGameObject.GetComponent<Animator>();
        myGameObject.SetActive(true);
        myGameObject.transform.position = 
            new Vector3(controller.transform.position.x, myGameObject.transform.position.y);
    }

    /* These are the two ways in which this state can Exit. By shrinking to the previous form, 
     * or growing into the next form. */

    //Shrink to the previous Mario.
    public Mario Shrink(Mario prevMario)
    {
        if (prevMario == null) {
            UIManager.uiManager.TakeLife();
            controller.gameObject.SetActive(false);
            return null;
        } else
        {
            return prevMario;
        }
        
    }

    //Grow to the next Mario.
    public Mario Grow(Mario nextMario)
    {
        myGameObject.SetActive(false);
        return nextMario;
    }
}
