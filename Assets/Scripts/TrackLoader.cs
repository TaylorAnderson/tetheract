using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackLoader : MonoBehaviour {

    public GameObject[] tracks;
    public static int track;
    public Ship ship;
    // Use this for initialization
    void Start () {
        
        GameObject track = Instantiate(tracks[TrackLoader.track], Vector3.zero, Quaternion.identity);
        Ship trackShip = track.GetComponentInChildren<Ship>();
        ship.transform.position = trackShip.transform.position;
        ship.transform.localEulerAngles = trackShip.transform.localEulerAngles;
        Ship.Instance = ship;
        Camera.main.transform.parent.position = ship.transform.position;
        TrackLoader.track = 0;
        Destroy(trackShip);
    }
}
