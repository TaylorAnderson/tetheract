using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDown : MonoBehaviour {

    // Use this for initialization

    private int count = 4;
    private SuperTextMesh textMesh;
    void Start () {
        textMesh = GetComponent<SuperTextMesh>();
        Time.timeScale = 0;
		StartCoroutine(StartCountdown());
    }
	
	// Update is called once per frame
	void Update () {
        
    }

	IEnumerator StartCountdown() {
		while (count > 0) {
			count--;
			if (count == 0) {
                textMesh.text = "GO!";
            }
			else {
				textMesh.text = count.ToString();
			}
           
			yield return new WaitForSecondsRealtime(0.5f);
		}


        Time.timeScale = 1;
        MusicPlayer.Instance.audioSource.Play();
        Destroy(gameObject);

    }
}
