using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

    public static MusicManager instance;

    void Awake()
    {
        instance = this;
    }
	// Use this for initialization
	void Start ()
    {
        audio = GetComponent<AudioSource>();
        audio.clip = intro;
        audio.Play();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(!audio.isPlaying)
        {
            audio.loop = true;
            audio.clip = loop;
            audio.Play();
        }
	}
    
    public void restart()
    {
        audio.Stop();
        audio.clip = intro;
        audio.loop = false;
        audio.Play();
    }

    [SerializeField]
    private AudioClip intro;
    [SerializeField]
    private AudioClip loop;

    private AudioSource audio;
}
