using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerDisplay : MonoBehaviour {
    private SuperTextMesh textMesh;

    // Use this for initialization
    void Start () {
        this.textMesh = GetComponent<SuperTextMesh>();
    }		
	
	// Update is called once per frame
	void Update () {
        Ship.Instance.timeInTrack += Time.deltaTime;
        textMesh.text = TimerDisplay.FormatTime(Ship.Instance.timeInTrack);
    }

    public static string FormatTime(float time) {
        string minutes = Mathf.Floor(time / 60).ToString("00");
		string seconds = Mathf.FloorToInt(time % 60).ToString("00");
        string milliseconds = Mathf.FloorToInt(time * 60 % 60).ToString("00");

        return string.Format("{0}:{1}:{2}", minutes, seconds, milliseconds);
    }
}
