using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearOnPress : MonoBehaviour
{
    void Update()
    {
        if (!PlayerMovement.Instance.movementEnabled)
            return;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        if (x != 0 || y != 0)
        {
            gameObject.SetActive(false);
        }
    }
}
