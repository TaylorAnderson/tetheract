using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector2.Distance(transform.position, Ship.Instance.transform.position) < 3) {
            transform.position = Bezier.Lerp(transform.position, Ship.Instance.transform.position, 0.1f);
        }
	}
}
