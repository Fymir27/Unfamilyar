using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupAnimation : MonoBehaviour
{
    public Image background;
    public Image pickupImage;

    public Transform starTransform;
    public Transform pickupTransform;

    [Range(0f, 1f)]
    public float fadeSpeed;

    [Range(0f, 1f)]
    public float zoomSpeed;

    [Range(0f, 360f)]
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        PlayerMovement.Instance.movementEnabled = false;

        var col = background.color;
        col.a = 0f;
        background.color = col;

        starTransform.localScale = new Vector3(0, 0, 0);
        starTransform.rotation = Quaternion.identity;

        pickupTransform.localScale = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Submit") || Input.GetButton("Jump"))
        {
            Destroy(gameObject);
            PlayerMovement.Instance.movementEnabled = true;
        }
        //fade background in
        var col = background.color;
        col.a = col.a + fadeSpeed * Time.deltaTime;
        background.color = col;

        // zoom pickup in
        if (starTransform.localScale.x < 1f)
        {
            starTransform.localScale += new Vector3(zoomSpeed, zoomSpeed, zoomSpeed) * Time.deltaTime;
            pickupTransform.localScale += new Vector3(zoomSpeed, zoomSpeed, zoomSpeed) * Time.deltaTime;
        }

        // rotate star
        starTransform.Rotate(Vector3.back, rotationSpeed * Time.deltaTime);
    }

    public void SetSprite(Sprite sprite)
    {
        pickupImage.sprite = sprite;
    }


}
