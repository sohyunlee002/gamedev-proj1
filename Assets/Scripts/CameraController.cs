using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    GameObject player;
    Camera myCam;
	// Use this for initialization
	void Start () {
        player = GameObject.Find("MarioHolder");
        myCam = GetComponent<Camera>();
        Vector3 cameraBottomLeft = myCam.ScreenToWorldPoint(new Vector3(0, 0));
        Vector3 cameraCenter = myCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));
        Debug.Log(cameraBottomLeft);
        float distanceFromZero = 0 - cameraBottomLeft.x;
        Debug.Log(distanceFromZero);
        cameraCenter.x += distanceFromZero;
        gameObject.transform.position = cameraCenter;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 myPos = gameObject.transform.position;
        if (player.transform.position.x > myCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)).x)
        {
            this.gameObject.transform.position = new Vector3(player.transform.position.x, myPos.y, myPos.z);
        }
    }
}
