using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{
    // All tiles as an array of GameObjects
    GameObject[] tiles;
    
    // List of selectable tiles
    List<Tile> selectableTiles = new List<Tile>();

    // In turn?
    public bool turn = false;
    
    // To get the path in reverse order
    Stack<Tile> path = new Stack<Tile>();
    
    // For tracking the current tile
    Tile currentTile;

    // Is moving?
    public bool moving = false;
    
    // // updated by: emgillis
    // // updated on: 2018-11-08
    // // updated for: removed "moved" option so that it allows better auto (AI) NPC move
    // //         & still maintain Player character control
    // Has moved?
    // public bool moved = false;

    // Max moves per turn
    public int move = 5;
    
    // Default jump height
    public float jumpHeight = 2;
    
    // Move speed
    public float moveSpeed = 2;
    
    // Jump velocity
    public float jumpVelocity = 4.5f;

    // Player velocity
    Vector3 velocity = new Vector3();
    
    // Direction vector
    Vector3 heading = new Vector3();

    // Height of the tile
    float halfHeight = 0;

    // Is falling down?
    bool fallingDown = false;
    
    // Is jumping up?
    bool jumpingUp = false;
    
    
    bool movingEdge = false;
    
    Vector3 jumpTarget;

    public Tile actualTargetTile;

    public void Reset()
    {

        // // updated by: emgillis
        // // updated on: 2018-11-08
        // // updated for: moved "turn" and "moving" resets to child classes (PlayerMove and NPCMove)
        // //        also helps with NPC to have continuous movement / tracking to get to player
        turn = false;
        moving = false;

        // // updated by: emgillis
        // // updated on: 2018-11-08
        // // updated for: removing "moved" check to allow player to continue having move options
        // //        also helps with NPC to have continuous movement / tracking to get to player
        //moved = false;
    }

    protected void Init()
    {
        // Cache all the tiles in an array
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        halfHeight = GetComponent<Collider>().bounds.extents.y;
        TurnManager.AddUnit(this);
    }

    public void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
    }

    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;

        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            tile = hit.collider.GetComponent<Tile>();
        }

        return tile;
    }

    public void ComputeAdjacencyLists(float jumpHeight, Tile target)
    {
        //tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors(jumpHeight, target);
        }
    }

    public void FindSelectableTiles()
    {
//        Debug.Log("Inside PlayerMove#FindSelectableTiles");

        ComputeAdjacencyLists(jumpHeight, null);
        GetCurrentTile();

        Queue<Tile> process = new Queue<Tile>();

        process.Enqueue(currentTile);
        currentTile.visited = true;
        //currentTile.parent = ??  leave as null

        while (process.Count > 0)
        {
            Tile t = process.Dequeue();

            selectableTiles.Add(t);
            t.selectable = true;

            if (t.distance < move)
            {
                foreach (Tile tile in t.adjacencyList)
                {
                    if (!tile.visited)
                    {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = 1 + t.distance;
                        process.Enqueue(tile);
                    }
                }
            }
        }
    }

    public void MoveToTile(Tile tile)
    {
        // Debug.Log(tile + "  -- " + tile.target);

        path.Clear();
        tile.target = true;
        if (turn)
        {
            moving = true;
        }

        Tile next = tile;
        while (next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    public void Move()
    {
        Vector3 tempPosition = transform.position;
        if (path.Count > 0)
        {
            // Debug.Log(path.Count);

            Tile t = path.Peek();
            Vector3 target = t.transform.position;

            //Calculate the unit's position on top of the target tile
            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {
                bool jump = transform.position.y != target.y;

                if (jump)
                {
                    Jump(target);
                }
                else
                {
                    CalculateHeading(target);
                    SetHorizotalVelocity();
                }

                //Locomotion
                transform.forward = heading;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                //Tile center reached
                transform.position = target;
                path.Pop();
            }
        }
        else
        {
            RemoveSelectableTiles();
            moving = false;
            // Debug.Log("stopped moving " + name);
            //moved = true;

            TurnManager.EndTurn();
        }
    }

    protected void RemoveSelectableTiles()
    {
//        Debug.Log("Inside PlayerMove#RemoveSelectableTiles");
        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }

        foreach (Tile tile in selectableTiles)
        {
            tile.Reset();
        }

        selectableTiles.Clear();
    }

    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }

    void SetHorizotalVelocity()
    {
        velocity = heading * moveSpeed;
    }

    void Jump(Vector3 target)
    {
        if (fallingDown)
        {
            FallDownward(target);
        }
        else if (jumpingUp)
        {
            JumpUpward(target);
        }
        else if (movingEdge)
        {
            MoveToEdge();
        }
        else
        {
            PrepareJump(target);
        }
    }

    void PrepareJump(Vector3 target)
    {
        float targetY = target.y;
        target.y = transform.position.y;

        CalculateHeading(target);

        if (transform.position.y > targetY)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = true;

            jumpTarget = transform.position + (target - transform.position) / 2.0f;
        }
        else
        {
            fallingDown = false;
            jumpingUp = true;
            movingEdge = false;

            velocity = heading * moveSpeed / 3.0f;

            float difference = targetY - transform.position.y;

            velocity.y = jumpVelocity * (0.5f + difference / 2.0f);
        }
    }

    void FallDownward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y <= target.y)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = false;

            Vector3 p = transform.position;
            p.y = target.y;
            transform.position = p;

            velocity = new Vector3();
        }
    }

    void JumpUpward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y > target.y)
        {
            jumpingUp = false;
            fallingDown = true;
        }
    }

    void MoveToEdge()
    {
        if (Vector3.Distance(transform.position, jumpTarget) >= 0.45f)
        {
            SetHorizotalVelocity();
        }
        else
        {
            movingEdge = false;
            fallingDown = true;

            velocity /= 3.0f;
            velocity.y = 1.5f;
        }
    }

    protected Tile FindLowestF(List<Tile> list)
    {
        Tile lowest = list[0];

        foreach (Tile t in list)
        {
            if (t.f < lowest.f)
            {
                lowest = t;
            }
        }

        list.Remove(lowest);

        return lowest;
    }

    protected Tile FindEndTile(Tile t)
    {
        Stack<Tile> tempPath = new Stack<Tile>();

        Tile next = t.parent;
        while (next != null)
        {
            tempPath.Push(next);
            next = next.parent;
        }

        if (tempPath.Count <= move)
        {
            return t.parent;
        }

        Tile endTile = null;
        for (int i = 0; i <= move; i++)
        {
            endTile = tempPath.Pop();
        }

        return endTile;
    }

    protected void FindPath(Tile target)
    {
        ComputeAdjacencyLists(jumpHeight, target);
        GetCurrentTile();

        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        openList.Add(currentTile);
        //currentTile.parent = ??
        currentTile.h = Vector3.Distance(currentTile.transform.position, target.transform.position);
        currentTile.f = currentTile.h;

//        Debug.Log(target.name + " = " + target.target);
        while (openList.Count > 0)
        {
            Tile t = FindLowestF(openList);
            closedList.Add(t);
            if (t == target)
            {
                actualTargetTile = FindEndTile(t);
                MoveToTile(actualTargetTile);
                return;
            }

            foreach (Tile tile in t.adjacencyList)
            {
                if (closedList.Contains(tile))
                {
                    //Do nothing, already processed
                }
                else if (openList.Contains(tile))
                {
                    float tempG = t.g + Vector3.Distance(tile.transform.position, t.transform.position);

                    if (tempG < tile.g)
                    {
                        tile.parent = t;

                        tile.g = tempG;
                        tile.f = tile.g + tile.h;
                    }
                }
                else
                {
                    tile.parent = t;

                    tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                    tile.h = Vector3.Distance(tile.transform.position, target.transform.position);
                    tile.f = tile.g + tile.h;

                    openList.Add(tile);
                }
            }
        }

        //todo - what do you do if there is no path to the target tile?
//        Debug.Log("Path not found");
    }

    public void BeginTurn()
    {
        turn = true;
        // Debug.Log("BeginTurn: " + name);
    }

    public void EndTurn()
    {
        turn = false;
        // Debug.Log("EndTurn: " + name);
    }

    // // updated by: emgillis
    // // updated on: 2018-11-08
    // // updated for: removed "moved" option so that it allows better auto (AI) NPC move
    // //         & still maintain Player character control
//    public void HasMoved()
//    {
//        moved = true;
//        // Debug.Log("HasMoved: " + name);
//    }

    // // updated by: emgillis
    // // updated on: 2018-11-08
    // // updated for: removed "moved" option so that it allows better auto (AI) NPC move
    // //         & still maintain Player character control
//    public void HasNotMoved()
//    {
//        moved = false;
//        // Debug.Log("HasNotMoved: " + name);
//    }
}