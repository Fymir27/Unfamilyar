using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VolumeSlider : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    Slider slider;

    public Image background;
    public Color selected;
    Color unselected;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        float volume = PlayerPrefs.GetFloat("volume", 0.5f);
        slider.value = volume;
        AudioListener.volume = volume;

        unselected = background.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("volume", value);
    }

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        background.color = selected;
    }

    void IDeselectHandler.OnDeselect(BaseEventData eventData)
    {
        //Debug.Log("Deselect");
        background.color = unselected;
    }
}
