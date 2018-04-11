using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject grid;

    [Range(0, 10)]
    public int NumberOfLevels = 2;      // amount of ready-to-play levels

    public GameObject[] level;          // parameters of levels
    [Range(0, 5)]
    public int count = 0;               // tracker of current level

    public Transform startPos;          // gameObject which indicates the players starting position

    bool levelLoaded = false;           // deleys the A* calculation (explained in AstarGrid.cs)

    public GameObject Astar;            // need this to access an A* function
    public AstarGrid script;

    void Start()    // setup references
    {
        grid = GameObject.Find("Grid");
        level = new GameObject[NumberOfLevels];
        level[0] = GameObject.Find("tmapXY_0");
        level[1] = GameObject.Find("tmapXY_1");

        Astar = GameObject.Find("A*");
        script = Astar.GetComponent<AstarGrid>();
    }

    void Update()
    {
        if (grid.activeSelf == true) // disables the grid (disabling the level generation)
        {
            grid.SetActive(false);
        }

        foreach (GameObject item in level)  // disables the innactive levels
        {
            item.SetActive(false);
        }

        level[count].SetActive(true); // activates the current level

        if (levelLoaded == false)
        {
            levelLoaded = true;
            script.CreateGrid();    // creates a grid for A* path finding
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.name == "Cube")      // if player collides with a cude
        {
            transform.position = startPos.position;     // teleport him back to start
            count++;                                    // notify which level should be loaded
            if (count > (NumberOfLevels - 1))           // if last level has been completed, restart from level 1
            {
                count = 0;
            }
            levelLoaded = false;                // creates a new grid for A* path finding
        }
    }
}
