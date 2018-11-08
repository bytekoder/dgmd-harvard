using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{

    public override void Reset()
    {
        // // updated by: emgillis
        // // updated on: 2018-11-08
        // // updated for: setting default of turn & moving to false
        // //         Note: this is moved to here because the reset value should be different between player and NPC
        // //         because NPC is always moving (like AI actor) and Player is not
        // Is moving?
//        moving = false;

        // In turn?
//        turn = false;
        
    }

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

            // // updated by: emgillis
            // // updated on: 2018-11-08
            // // updated for: removing "moved" check to allow player to continue having move options
            // //        also helps with NPC to have continuous movement / tracking to get to player
                    if (this == p)// && !p.moved)
                    {
                        BeginTurn();
//                        Debug.Log("You Clicked Me! " + name);
                    }

                }


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