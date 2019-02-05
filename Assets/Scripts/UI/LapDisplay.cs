using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapDisplay : MonoBehaviour {

    private SuperTextMesh textMesh;
    // Use this for initialization
    void Start () {
		textMesh = GetComponent<SuperTextMesh>();
		
	}
	
	// Update is called once per frame
	void Update () {
        textMesh.text = string.Format("Laps: {0}/{1}", Ship.Instance.laps, 3);
    }
}
