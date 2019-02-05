using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackBestTime : MonoBehaviour {

    private SuperTextMesh textMesh;
    // Use this for initialization
    void Start () {
        textMesh = GetComponent<SuperTextMesh>();
        TrackTimes.Load();
        OnChangeLevel();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnChangeLevel() {
        textMesh.text = "Best Time: " + TimerDisplay.FormatTime(TrackTimes.times[TrackLoader.track]);
    }
}
