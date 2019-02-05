using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour {

    private Camera cam;
    // Use this for initialization
    void Start () {
        cam = GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        cam.transform.localEulerAngles = Vector3.forward * Mathf.Sin(Time.unscaledTime*2)*10;
    }
}
