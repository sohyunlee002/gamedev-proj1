using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperMarioForm : MarioForm {

    public SuperMarioForm(PlayerController controller, GameObject myGameObject, MarioForm prevMario) : base(controller, myGameObject, prevMario)
    {
        this.canDuck = true;
    }    

}
