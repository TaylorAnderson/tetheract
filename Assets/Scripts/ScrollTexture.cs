using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollTexture : MonoBehaviour {

    private RawImage bg;
    void Start () {
		bg = GetComponent<RawImage>();
	}
	
	// Update is called once per frame
	void Update () {
		Rect uvRect = bg.uvRect;
		uvRect.x -= 0.02f;
		uvRect.y -= 0.02f;
		bg.uvRect = uvRect;
	}
}
