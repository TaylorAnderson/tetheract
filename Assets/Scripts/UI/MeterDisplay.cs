using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeterDisplay : MonoBehaviour {

    public RectTransform meterBar;
    public RectTransform deltaBar;
    public GameObject fire;

    private float maxWidth = 206; //sorry for magic number, just moved the meter to find this
    private float moveDelay = 0;
    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update() {
        if (moveDelay >= 0) moveDelay -= Time.deltaTime;
        if (moveDelay < 0) {
            meterBar.sizeDelta = Vector2.up * meterBar.sizeDelta.y + Vector2.right * Mathf.Lerp(meterBar.sizeDelta.x, deltaBar.sizeDelta.x, 0.25f);
        }

        fire.SetActive(IsFull());



    }

    /// <summary>Adds specified amount to meter.  Amount goes from 0 to 1.</summary>
    public void Add(float amt) {
        this.deltaBar.sizeDelta += Vector2.right * amt * maxWidth;

        moveDelay += 1;

    }

    public bool IsFull() {
        return deltaBar.sizeDelta.x >= this.maxWidth;
    }

    public void Empty() {
        this.meterBar.sizeDelta = new Vector2(0, deltaBar.sizeDelta.y);
        this.deltaBar.sizeDelta = new Vector2(0, deltaBar.sizeDelta.y);
    }
}
