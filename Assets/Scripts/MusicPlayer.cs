using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MusicPlayer : MonoBehaviour {

    public static MusicPlayer Instance;
    [HideInInspector]
    public AudioLowPassFilter lowPassFilter;
	[HideInInspector]
    public AudioSource audioSource;

    public AudioClip slowAudioClip;
    public AudioClip fastAudioClip;
    // Use this for initialization
    void Start () {
        this.lowPassFilter = GetComponent<AudioLowPassFilter>();
        this.audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(this.gameObject);
        SceneManager.activeSceneChanged += CheckMusicToPlay;
        if (MusicPlayer.Instance == null) {
            MusicPlayer.Instance = this;
        }


    }

    void CheckMusicToPlay(Scene current, Scene next) {
            if (next.buildIndex == 2) {
                this.audioSource.clip = this.fastAudioClip;
            }
            else {
                this.audioSource.clip = this.slowAudioClip;
            }
        }




    
}
