using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLineManager : MonoBehaviour {

    [HideInInspector]
    public bool frontCrossed = false;

	[HideInInspector]
    public bool validLap = true;


    [HideInInspector]
    public float bestLapTime = Mathf.Infinity;

    [HideInInspector]
    public float currentLapTime = 0;
	
	// Update is called once per frame
	void Update () {
        currentLapTime += Time.deltaTime;
    }


    //these two checks are essentially a system to ensure that the player can't cheat by passing the finish line backwards, then passing it going forwards
    public void OnFrontHit() {
        frontCrossed = true;
    }
    public void OnBackHit() {
        if (frontCrossed) {
            if (validLap) {
                Ship.Instance.laps++;
            }
            validLap = true;
        }
        else {
            validLap = false;
        }

        frontCrossed = false;
    }


}
