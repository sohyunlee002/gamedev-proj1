using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalMarioForm : MarioForm {

	public MagicalMarioForm(PlayerController controller, GameObject myGameObject, MarioForm prevMario) : base(controller, myGameObject, prevMario)
	{
		this.canDuck = true;
		//this.jumpingTime = 2;
	}    

}
