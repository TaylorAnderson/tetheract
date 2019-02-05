using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;
using EZCameraShake;
[RequireComponent(typeof(Rigidbody2D))]
public class Ship : MonoBehaviour
{
    public GameObject exhaust;
    public GameObject grapple;
    public MeterDisplay meter;
    public LayerMask grappleLayerMask;
    public float speedFalloff = 0.95f;
    public float normalCameraZoom = 15;
    public float slomoCameraZoom = 13;
    public float boostCameraZoom = 17;
    
    public SpriteRenderer blackScreen;

    public AudioClip grappleFireSfx;
    public AudioClip grappleConnectSfx;
    public AudioClip boostSfx;

    [HideInInspector]
    public int laps = 0;

    [HideInInspector]
    public float timeInTrack = 0;

    [HideInInspector]
    public static Ship Instance = null;

    private AudioLowPassFilter lowPassFilter;
    private float currentCutoffFrequency = 22000;
    private float maxCutoffFrequency = 22000;
    private float minCutoffFrequency = 1000;
    private float currentCamZoom = 15;

    private Rigidbody2D rb2d;

    private float maxVel = 25;
    private float normalMaxVel = 25;
    private float boostMaxVel = 60;

    private float accel = 10;

    private float boostDuration = 0.5f;
    private float boostTimer = 0;

    private LinearCameraFollower camFollower;
    private Camera cam;

    private Animator exhaustAnimator;

    private LineRenderer lineRenderer;
    private GameObject currentGrapple = null;
    private AudioSource audioPlayer;
    private Vector3[] positions;
    private float meterToAdd = 0;

    private bool inSloMo = false;
    // Start is called before the first frame update
    private void Awake() {
        if (Ship.Instance == null) {
            Ship.Instance = this;
        }
        else if (Ship.Instance != this) {
            Destroy(gameObject);
        }

        this.lowPassFilter = MusicPlayer.Instance.lowPassFilter;
    }
    void Start() {
        this.audioPlayer = GetComponent<AudioSource>();
        this.rb2d = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        camFollower = Camera.main.GetComponentInParent<LinearCameraFollower>();
        cam = Camera.main;
        this.exhaustAnimator = exhaust.GetComponent<Animator>();

    }
    void Update() {

        //we zoom in when we go into slomo, and then zoom out when we go into boost
        //helps create this dynamic movement that sells the feeling of speed
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, this.currentCamZoom, 0.1f);

        //we want to muffle the music if we go into slomo
        lowPassFilter.cutoffFrequency = Mathf.Lerp(lowPassFilter.cutoffFrequency, this.currentCutoffFrequency, 0.1f);

        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 pointingVector = mousePos - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, pointingVector.normalized, Mathf.Infinity, this.grappleLayerMask);
        Vector3 pos = hit ? (Vector3) hit.point : transform.position + (Vector3) pointingVector;

        
        positions = new Vector3[] { transform.position, pos };
        lineRenderer.SetPositions(positions);


        rb2d.MoveRotation(Mathf.Atan2(pointingVector.y, pointingVector.x) * Mathf.Rad2Deg - 90);


        rb2d.AddForce(pointingVector.normalized * accel);
        if (rb2d.velocity.magnitude > maxVel) { rb2d.velocity *= speedFalloff; }

        this.currentCamZoom = normalCameraZoom;

        if (inSloMo) this.currentCamZoom = slomoCameraZoom;

        if (Input.GetMouseButton(0) && !currentGrapple) {
            currentGrapple = Instantiate(grapple, transform.position, transform.rotation);
            var grappleController = currentGrapple.GetComponent<Grapple>();
            grappleController.Attach();
            audioPlayer.PlayOneShot(this.grappleFireSfx, 0.5f);
            currentGrapple.transform.position = pos - (pos - transform.position)*0.05f;
            

            //we use the grapple to build up meter
            //when the meter is full, the player can boost forward.
            if (this.rb2d.velocity.magnitude > 1) {
                this.meterToAdd += 0.1f;
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            Destroy(currentGrapple);
            this.meter.Add(this.meterToAdd);
            meterToAdd = 0;
        }

        if (Input.GetMouseButtonDown(1)) {
            if (meter.IsFull()) {
                EnterSlomo();
                currentCamZoom = slomoCameraZoom;
            }
            
        }
        if (Input.GetMouseButtonUp(1)) {
            if (meter.IsFull()) {
                meter.Empty();
                ExitSlomo();
                this.audioPlayer.PlayOneShot(boostSfx);
                this.rb2d.velocity = pointingVector.normalized * boostMaxVel;
                boostTimer = boostDuration;
                camFollower.SetVelocityMatchDelay(0.1f);
                CameraShaker.Instance.ShakeOnce(16f, 22f, 0.1f, 0.5f);
            }
        }

        boostTimer -= Time.deltaTime;
        if (boostTimer >= 0) {
            maxVel = boostMaxVel;
            exhaustAnimator.Play("Boost3");
            exhaust.transform.localPosition = Vector2.up * -1.3f;
            currentCamZoom = boostCameraZoom;
        }
        else {
            maxVel = normalMaxVel;
            exhaustAnimator.Play("Boost1");
            exhaust.transform.localPosition = Vector2.up * -0.7f;
        }
    } 

    private void LateUpdate() {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pointingVector = mousePos - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, pointingVector.normalized, Mathf.Infinity, this.grappleLayerMask);
        Vector3 pos = hit ? (Vector3) hit.point : transform.position + (Vector3) pointingVector;
        positions = new Vector3[] { transform.position, pos };
        lineRenderer.SetPositions(positions);
    }

    void EnterSlomo() {
        Time.timeScale = 0.1f;
        //not doing this results in physics weirdness
        Time.fixedDeltaTime = Time.timeScale * 1 / 50;
        inSloMo = true;
        currentCutoffFrequency = minCutoffFrequency;

        //we darken the screen to sell the slomo effect
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0.5f);
    }
    void ExitSlomo() {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale * 1 / 50;
        inSloMo = false;
        currentCutoffFrequency = maxCutoffFrequency;
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0f);
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(ObjectTags.PICKUP)) {
            Destroy(other.gameObject);
            meter.Add(0.1f);
        }
    }
}
