using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Fade : MonoBehaviour
{
    Image image;
    bool fadeIn = true;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        var col = image.color;
        if(col.a < 1f)
        {
            col.a += 1f * Time.deltaTime;
        }
        image.color = col;
    }
}
