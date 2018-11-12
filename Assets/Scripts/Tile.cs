using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // The tile player is standing on
    public bool current = false;

    // The tile where the player needs to go to
    public bool target = false;

    // The possible tiles a player can make a move to
    public bool selectable = false;

    // The tile is able to be used as cover and hide from monster
    public bool cover = false;

    // In order to mark a tile un-walkable (obstacles), this flag can be used
    // This also controls the ability to not bump into other players
    public bool walkable = true;

    // Contains the adjacency matrix that has neighbors
    public List<Tile> adjacencyList = new List<Tile>();

    #region BFSVariables

    // Flag to indicate if a tile has been processed. This only happens once per turn.
    public bool visited = false;
    public Tile parent = null;

    // How far each tile is from start tile
    public int distance = 0;

    #endregion

    //For A*
    public float f = 0;
    public float g = 0;
    public float h = 0;

    // Use this for initialization
    void Start()
    {
    }

    // // updated by: egillis
    // // updated at: 2018-11-11
    // // updated for: changed Update to FixedUpdate so that triggering happens consistently
    void FixedUpdate()
    {
        if (target)
        {
            // Get the target tile (green(
            GetComponent<Renderer>().material.color = Color.green;
        }
        else if (current)
        {
            // Get the current tile where the player is (magenta)
            GetComponent<Renderer>().material.color = Color.magenta;
        }
        else if (selectable)
        {
            // Get the selectable tiles (reds)
            GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            // Get the remaining tiles (whites)
            GetComponent<Renderer>().material.color = Color.white;
        }
    }

    /// <summary>
    /// Reset all the variables every turn and clear the adjacency list
    /// </summary>
    public void Reset()
    {
        adjacencyList.Clear();

        current = false;
        target = false;
        selectable = false;

        visited = false;
        parent = null;
        distance = 0;

        f = g = h = 0;
    }

    public void FindNeighbors(float jumpHeight, Tile target)
    {
        Reset();

        CheckTile(Vector3.forward, jumpHeight, target);
        CheckTile(Vector3.back, jumpHeight, target);
        CheckTile(Vector3.right, jumpHeight, target);
        CheckTile(Vector3.left, jumpHeight, target);

    }

    public void CheckTile(Vector3 direction, float jumpHeight, Tile target)
    {
        Vector3 halfExtents = new Vector3(0.25f, (1 + jumpHeight) / 2.0f, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            if (tile != null && tile.walkable)
            {
                RaycastHit hit;

                if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1) || (tile == target))
                {
                    adjacencyList.Add(tile);
                }
            }
        }
    }
}