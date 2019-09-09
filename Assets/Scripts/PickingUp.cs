using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickingUp : MonoBehaviour
{
    public int type;

    public Sprite VHS;
    public Sprite Food;
    public Sprite Bubble;
    public Sprite Vinyl;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>().Play();

            Destroy(gameObject);
            Maze.Instance.PickUp(type + 2);
            var overlay = Instantiate(MainMenu.Instance.pickupOverlay);

            Sprite spr; 

            switch(type)
            {
                case 1: // VHS
                    spr = PickupPlacer.Instance.VHS.GetComponent<SpriteRenderer>().sprite;
                    break;

                case 2: // Food
                    spr = PickupPlacer.Instance.Food.GetComponent<SpriteRenderer>().sprite;
                    break;

                case 3: // Vinyl
                    spr = PickupPlacer.Instance.Vinyl.GetComponent<SpriteRenderer>().sprite;
                    break;

                case 4: // Bubble
                    spr = PickupPlacer.Instance.Bubblebath.GetComponent<SpriteRenderer>().sprite;
                    break;

                default:
                    spr = PickupPlacer.Instance.VHS.GetComponent<SpriteRenderer>().sprite;
                    break;                    
            }
            overlay.GetComponent<PickupAnimation>().SetSprite(spr);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Maze.Instance.Space(false);
    }
}
