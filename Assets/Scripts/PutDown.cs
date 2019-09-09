using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutDown : MonoBehaviour
{
    public int type;
    public bool solved = false; //this room

    public static int solvedRooms = 0;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (solved)
            return;

        if(!Maze.Instance.RoomSolvable[type + 2])
        {
            return;
        }

        Maze.Instance.Space(true);

        if (collision.gameObject.name == "Player")
        {
            if ((Input.GetButton("Jump") || Input.GetButton("Submit")) && Maze.Instance.pickedUp[type + 2])
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>().Play();
                Maze.Instance.Space(false);
                transform.GetChild(0).gameObject.SetActive(false);

                solved = true;
                solvedRooms++;

                Debug.Log("Solved rooms: " + solvedRooms);

                switch(type)
                {
                    case 1:
                        GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(-4.26f, 1.8f);
                        break;

                    case 2:
                        GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(0.8f, 3.97f);
                        break;

                    case 3:
                        GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(-0.39f, -0.85f);
                        break;

                    case 4:
                        GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(7.91f, 3.31f);
                        break;
                }

                // fade to black

                //var player = GameObject.FindGameObjectWithTag("Player");
                //player.transform.position = transform.position + new Vector3(0.1f, 0, 0);

                Maze.Instance.SolveRoom(type + 2);
                if (solvedRooms >= 4)
                {
                    Maze.Instance.Win();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Maze.Instance.Space(false);
    }
}
