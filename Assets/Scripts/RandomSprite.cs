using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{

    public List<Sprite> sprites = new List<Sprite>();

    // Start is called before the first frame update
    void Start()
    {
        var renderer = GetComponent<SpriteRenderer>();
        int i = Random.Range(0, sprites.Count);
        renderer.sprite = sprites[i];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
