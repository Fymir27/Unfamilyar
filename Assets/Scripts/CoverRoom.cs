using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverRoom : MonoBehaviour
{
    public SpriteRenderer[] roomCovers;
    public GameObject player;

    private void Start()
    {
        SetTo(1f);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == player)
            SetTo(0f);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player)
            SetTo(0.7f);
    }

    private void SetTo(float alpha)
    {
        foreach (SpriteRenderer roomCover in roomCovers)
        {
            Color temp = roomCover.color;
            temp.a = alpha;
            roomCover.color = temp;
        }
    }
}
