using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Button : MonoBehaviour {

    // Use this for initialization
    private RectTransform rectTransform;
    void Start () {
        rectTransform = GetComponent<RectTransform>();
    }	
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MoveVertical(float amt) {
        rectTransform.transform.localPosition += Vector3.up * amt;
    }
	public void LoadScene(int sceneIndex) {
        SceneManager.LoadSceneAsync(sceneIndex);
    }
}
