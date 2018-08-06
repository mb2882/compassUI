using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBillboard : MonoBehaviour {
    public Camera m_Camera;
    public bool autoInit = true;
    GameObject myContainer;

    // Use this for initialization
    void Awake () {
        if (autoInit == true)
        {
            m_Camera = Camera.main;
        }
    }
	
	// Update is called once per frame
	void Update () {
        this.transform.LookAt(this.transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);
		
	}
}
