using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaySoundOnSelect : MonoBehaviour, ISelectHandler
{
    public AudioClip Clip;
    public AudioSource Audio;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {       
        Audio.Pause();
        Audio.clip = Clip;
        Audio.Play();
    }
}
