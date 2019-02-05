using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FinishLineType {
    FRONT,
    BACK
}
public class FinishLineController : MonoBehaviour {

    public FinishLineType type;
    private FinishLineManager finishLine;
    // Use this for initialization
    void Start () {
        finishLine = GetComponentInParent<FinishLineManager>();
    }
	
	// Update is called once per frame
	void Update () {
	}

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag(ObjectTags.PLAYER)) {
            if (type == FinishLineType.BACK) finishLine.OnFrontHit();
            else finishLine.OnBackHit();
        }
    }
}
