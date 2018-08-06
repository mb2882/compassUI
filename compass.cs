using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class compass : MonoBehaviour {

    public float northOffset = 0f;
    private Vector3 northDirection;
    public Transform player;
    public RectTransform compassUI;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        changeNorthDirection();
	}

    public void changeNorthDirection()
    {
        northDirection.z = player.eulerAngles.y + northOffset;
        compassUI.localEulerAngles = northDirection;
    }
}
