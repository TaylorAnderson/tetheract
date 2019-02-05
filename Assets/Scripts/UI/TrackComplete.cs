using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackComplete : MonoBehaviour {

    public SuperTextMesh timeText;
    private bool fired = false;
    // Use this for initialization
    void Start () {
        transform.GetChild(0).gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (Ship.Instance.laps == 3 && !fired) {
            fired = true;
            transform.GetChild(0).gameObject.SetActive(true);
            timeText.text = "Time: " + TimerDisplay.FormatTime(Ship.Instance.timeInTrack);
			Time.timeScale = 0;
            MusicPlayer.Instance.audioSource.Pause();
            TrackTimes.times[TrackLoader.track] = Ship.Instance.timeInTrack;
            TrackTimes.Save();
        }
    }
    public void ResetEverything() {
        Time.timeScale = 1;
        MusicPlayer.Instance.audioSource.Play();
    }
}
