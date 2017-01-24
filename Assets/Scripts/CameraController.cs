using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    GameObject player;
	// Use this for initialization
	void Start () {
        player = GameObject.Find("MarioHolder");
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 myPos = gameObject.transform.position;
        if (player.transform.position.x > 8.25f)
        {
            this.gameObject.transform.position = new Vector3(player.transform.position.x, myPos.y, myPos.z);
        }
    }
}
