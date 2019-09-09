using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    public GameObject pickupOverlay; // as reference for pickup script

    bool visible = true;
    bool newGamePressed = false;

    public GameObject parent;
    public Button newGameButton;

    Random rnd = new Random();
    bool isBackgroundDark = true;
    int framesUntilBackroundChange = 0;

    public Image backgroundImage;
    public Sprite backgroundLight;
    public Sprite backgroundDark;

    private void Awake()
    {
        Instance = this;   
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (framesUntilBackroundChange == 0)
        {
            if (isBackgroundDark)
            {
                backgroundImage.sprite = backgroundLight;
            }
            else
            {
                backgroundImage.sprite = backgroundDark;
            }
            isBackgroundDark = !isBackgroundDark;
            framesUntilBackroundChange = Random.Range(5, 60);
        }
        else
        {
            framesUntilBackroundChange--;
        }

        if (Input.GetButtonDown("Cancel"))
        {
            if (visible)
            {
                if (newGamePressed) // only hide when game already started
                {
                    Hide();
                }
            }
            else
            {
                Show();
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Show()
    {
        PlayerMovement.Instance.movementEnabled = false;
        Camera.main.GetComponent<AudioSource>().Pause();

        visible = true;
        parent.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null, null);
        newGameButton.Select();
    }

    public void Hide()
    {
        visible = false;
        parent.SetActive(false);
        PlayerMovement.Instance.movementEnabled = true;
        Camera.main.GetComponent<AudioSource>().Play();
    }

    public void ChangeNewGameButtonText(string text)
    {
        newGamePressed = true;
        newGameButton.GetComponentInChildren<Text>().text = text;
    }
}
