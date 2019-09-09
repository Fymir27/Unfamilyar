using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupPlacer : MonoBehaviour
{
    public static PickupPlacer Instance;

    public GameObject Food;
    public GameObject Vinyl;
    public GameObject Bubblebath;
    public GameObject VHS;

    private bool init = false;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!init)
        {
            init = true;
            var pickups = new GameObject("Pickups");

            var pos = Maze.Instance.GetRandomDeadEndPos(4);
            Debug.Log(pos);
            GameObject.Instantiate(VHS, pos, Quaternion.identity, pickups.transform);

            pos = Maze.Instance.GetRandomDeadEndPos(5);
            Debug.Log(pos);
            GameObject.Instantiate(Food, pos, Quaternion.identity, pickups.transform);

            pos = Maze.Instance.GetRandomDeadEndPos(6);
            Debug.Log(pos);
            GameObject.Instantiate(Vinyl, pos, Quaternion.identity, pickups.transform);

            pos = Maze.Instance.GetRandomDeadEndPos(3);
            Debug.Log(pos);
            GameObject.Instantiate(Bubblebath, pos, Quaternion.identity, pickups.transform);
        }
    }
}
