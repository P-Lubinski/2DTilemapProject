using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
// code below is an implementation of a cave generator based on Conway's Game of Life
public class Generator : MonoBehaviour
{
    [Range(0, 100)]             // range selector
    public int iniChance;       // initial chance of a tile to be alive

    [Range(1, 8)]
    public int birthLimit;      // these two variables limit how many "dead" or "alive" tiles can spawn next to eachother

    [Range(1, 8)]
    public int deathLimit;

    [Range(1, 10)]
    public int numRep;          // number of repetitions

    private int count = 0;      // number of save files

    private int[,] terrainMap;  // storing terrain tiles files

    public Vector3Int tmapSize; // tilemap size

    public Tilemap topMap;      // 2 tile maps one could be the terrain, 
    public Tilemap botMap;      // other could be lava/water/sky

    public RuleTile topTile;    // tiles I use to draw
    public Tile botTile;        // currently not in use

    int width;
    int height;

    public void doSim(int numRep)      // Simulation taking number of repetitions
    {
        clearMap(false);
        width = tmapSize.x;
        height = tmapSize.y;

        if (terrainMap == null)                     // if terrain map doesn't exist
        {
            terrainMap = new int[width, height];    // create a new one using the size variables
            initPos();
        }

        for (int i = 0; i < numRep; i++)
        {
            terrainMap = genTilePos(terrainMap);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (terrainMap[x, y] == 1)      // if the tile has a value of 1 on a position in the terrain array then place it on the map
                {
                    topMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), topTile);
                }
                else
                {
                    botMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), botTile);
                }
            }
        }
    }

    public void initPos()                   // generates random tiles on the grid
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                terrainMap[x, y] = Random.Range(1, 101) < iniChance ? 1 : 0;  // 1 is allive 0 is not allve
            }
        }
    }

    public int[,] genTilePos(int[,] oldMap)
    {
        int[,] newMap = new int[width, height];             // output array after processing the oldMap
        int neighb;                                         // number of neigbours
        BoundsInt myB = new BoundsInt(-1, -1, 0, 3, 3, 1);  // virtual rectangle xyz axis around our current position and size xyz

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                neighb = 0;
                foreach (var b in myB.allPositionsWithin)
                {
                    if (b.x == 0 && b.y == 0) continue;                                         // checking if this is my current position
                    if (x + b.x >= 0 && x + b.x < width && y + b.y >= 0 && y + b.y < height)    // checking if this is our neighbour
                    {
                        neighb += oldMap[x + b.x, y + b.y];     // if this is the case, add the value of oldMap to neighbour counter
                    }
                    else
                    {
                        neighb++;                   // otherwise, assume border has been reached
                    }
                }

                if (oldMap[x, y] == 1)              // if the tile at the position in the old map == 1
                {
                    if (neighb < deathLimit)        // and the number of neigbours is below the death limit
                    {
                        newMap[x, y] = 0;           // then this tile will not be on the newMap
                    }

                    else
                    {
                        newMap[x, y] = 1;           // otherwise, transfer the position to newMap

                    }
                }

                if (oldMap[x, y] == 0)              // same as above, but with reverse logic
                {
                    if (neighb > birthLimit)
                    {
                        newMap[x, y] = 1;
                    }

                    else
                    {
                        newMap[x, y] = 0;
                    }
                }
            }
        }

        return newMap;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // left mouse button to generate
        {
            doSim(numRep);
        }

        if (Input.GetMouseButtonDown(1)) // right mouse button to clear
        {
            clearMap(true);
        }

        if (Input.GetMouseButtonDown(2)) // middle mouse button to save
        {
            saveAssetMap();
            count++;
        }
    }

    public void saveAssetMap()          // saves the generated map into a prefab
    {
        string saveName = "tmapXY_" + count;
        var mf = GameObject.Find("Grid");

        if (mf)
        {
            var savePath = "Assets/" + saveName + ".prefab";
            if (AssetDatabase.LoadAssetAtPath(savePath, typeof(GameObject))) // if file of this name exsists;
            {
                Add(); // add +1 to the name file and restart this function
            }
            else
            {
                if (PrefabUtility.CreatePrefab(savePath, mf))
                {
                    EditorUtility.DisplayDialog("Tilemap saved", "Your tilemap was saved under" + savePath, "Continue");
                }
                else
                {
                    EditorUtility.DisplayDialog("Tilemap NOT saved", "An error occured while trying to save under" + savePath, "Continue");
                }
            }
        }
    }

    public void Add()
    {
        count++;
        saveAssetMap();
    }

    public void clearMap(bool complete)    // clearing the whole map
    {
        topMap.ClearAllTiles();
        botMap.ClearAllTiles();

        if (complete)
        {
            terrainMap = null;               // if true -> null array
        }
    }
}
