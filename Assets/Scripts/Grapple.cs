using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour {

    [HideInInspector]
    public Ship player;

    private LineRenderer lineRenderer;
    private List<Vector3> points = new List<Vector3>();

    // Use this for initialization
    void Start () {
        lineRenderer = GetComponent<LineRenderer>();
    }
	
    public void Attach() {
        this.player = Ship.Instance;
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        Vector3 pointerVec = (player.transform.position - transform.position);
        var joint = this.gameObject.AddComponent<DistanceJoint2D>();
        joint.distance = pointerVec.magnitude-0.5f;
        joint.enableCollision = false;
        joint.maxDistanceOnly = true;
        
        joint.connectedBody = player.GetComponent<Rigidbody2D>();
    }
	// Update is called once per frame
	void Update () {
        lineRenderer.SetPositions(new Vector3[] { transform.position, player.transform.position });
	}
}
