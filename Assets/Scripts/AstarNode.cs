using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarNode
{
    public bool walkable;           // node is walkable or not
    public Vector3 worldPosition;   // what point in the world this node represents

    public AstarNode(bool _walkable, Vector3 _wordlPos) // assign these variables
    {
        walkable = _walkable;
        worldPosition = _wordlPos;
    }
}
