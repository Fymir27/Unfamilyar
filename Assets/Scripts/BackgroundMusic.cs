using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioSource Audio;
    public List<AudioClip> backgroundTracks;

    int nextTrackIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!Audio.isPlaying)
        {
            Audio.clip = backgroundTracks[nextTrackIndex];
            Audio.Play();
            nextTrackIndex = (nextTrackIndex + 1) % backgroundTracks.Count;
        }
    }
}
