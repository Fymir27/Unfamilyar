using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Singleton
    public static PlayerMovement Instance;

    public bool movementEnabled = false;

    public float min;
    public float max;

    public Sprite[] mainBody;
    public Sprite[] mainHead;

    private Rigidbody2D rb;
    private Transform head;
    private float direction = -1f;
    private SpriteRenderer bodyRend;
    private SpriteRenderer headRend;
    private Vector3 headPos;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyRend = GetComponent<SpriteRenderer>();
        head = transform.GetChild(0);
        headRend = head.GetComponent<SpriteRenderer>();
        headPos = head.localPosition;
    }

    private void Update()
    {
        headPos.y += 0.0001f * direction;
        head.localPosition = headPos;
        if (headPos.y < 0.008f)
            direction = 1f;
        if (headPos.y > 0.016f)
            direction = -1f;
    }

    void FixedUpdate()
    {
        if (!movementEnabled)
            return;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float speed = 2f;
        if (x > 0)
        {
            bodyRend.sprite = mainBody[1];
            headRend.sprite = mainHead[1];
        }
        else if (x < 0)
        {
            bodyRend.sprite = mainBody[3];
            headRend.sprite = mainHead[3];
        }
        else if (y > 0)
        {
            bodyRend.sprite = mainBody[2];
            headRend.sprite = mainHead[2];
        }
        else
        {
            bodyRend.sprite = mainBody[0];
            headRend.sprite = mainHead[0];
        }
        rb.velocity = new Vector3(x*speed, y*speed, 0);
    }
}
