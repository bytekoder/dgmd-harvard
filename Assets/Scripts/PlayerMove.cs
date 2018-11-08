using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{

    void Start()
    {
//        Debug.Log("Inside PlayerMove class");
        Init();
    }

    void Update()
    {
//        Debug.Log("Inside PlayerMove#Update");

//        Debug.DrawRay(transform.position, transform.forward);

        if (!turn)
        {
            CheckMouse();
            RemoveSelectableTiles();
            return;
        }

        //
        if (!moving) // && isSelected)
        {
            CheckMouse();
            FindSelectableTiles();
        }

        if (turn && moving)
        {
            Move();
        }
    }

    void CheckMouse()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    PlayerMove p = hit.collider.GetComponent<PlayerMove>();
//                    Debug.Log(p.ToString());

                    moving = false;
                    EndTurn();

                    if (this == p && !p.moved)
                    {
                        BeginTurn();
//                        Debug.Log("You Clicked Me! " + name);
                    }

                    // turn = false;
                    // RemoveSelectableTiles();
                    // EndTurn();
                    // // p.isSelected = true;
                    // p.BeginTurn();
                    // p.FindSelectableTiles();
                    // p.HasMoved();
                    // p.RemoveSelectableTiles();
                }
                // moved = true;


                if (hit.collider.CompareTag("Tile"))
                {
                    Tile t = hit.collider.GetComponent<Tile>();
                    // Debug.Log("target " + t.name + " = " + t.target);

                    if (t.selectable)
                    {
                        MoveToTile(t);
                    }
                }
            }
        }
    }
    
}