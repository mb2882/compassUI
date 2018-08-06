using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRelativeScaler : MonoBehaviour {
    public Camera m_camera;
    public float objectScale = 1.0f;
    public Vector2 scaleRange = new Vector2(0.5f, 2.0f);
    private Vector3 initialScale;
    private Vector3 finalScale;

    // Use this for initialization
    void Start () {

        // record initial scale, use this as a basis
        initialScale = transform.localScale;

        // if no specific camera, grab the default camera
        if (m_camera == null)
            m_camera = Camera.main;

    }
	
	// Update is called once per frame
	void Update () {

        Plane plane = new Plane(m_camera.transform.forward, m_camera.transform.position);
        float dist = plane.GetDistanceToPoint(transform.position);
        finalScale.x = Mathf.Clamp(initialScale.x * dist * objectScale, scaleRange.x, scaleRange.y);
        finalScale.y = Mathf.Clamp(initialScale.y * dist * objectScale, scaleRange.x, scaleRange.y);
        finalScale.z = Mathf.Clamp(initialScale.z * dist * objectScale, scaleRange.x, scaleRange.y);
        transform.localScale = finalScale;


    }
}
