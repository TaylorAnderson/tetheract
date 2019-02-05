using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uween;
public class TrackCam : MonoBehaviour {

    private float initX;
    private float intervalX = 39;
	private float snapPos;
    public float levels = 0;
    
    // Use this for initialization
    void Start () {
        this.initX = transform.position.x;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TweenOver(int dist) {
       
        var realDist = dist * intervalX;
        var pos = transform.position.x + realDist;
        if (pos < initX || pos >= initX + intervalX * levels) {
            return;
        }
        TrackLoader.track += dist;
        TweenX.Add(gameObject, 0.3f, pos).EaseOutBack();
    }
}
