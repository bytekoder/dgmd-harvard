using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : TacticsMove 
{
    GameObject target;

    public override void Reset()
    {
        // // updated by: emgillis
        // // updated on: 2018-11-08
        // // updated for: setting default of NPC to always be "within turn" and "moving" so that it acts like AI
        // Is moving?
        // because NPC, yes, always moving ... AI actor
//        moving = true;

   
        // // updated by: emgillis
        // // updated on: 2018-11-08
        // // updated for: setting default of NPC to always be "within turn" and "moving" so that it acts like AI
        // In turn?
        // because NPC, yes, always in turn ... AI actor
//        turn = true;
        
    }

	// Use this for initialization
	void Start () 
	{
//	    Debug.Log("Inside NPCMove class");

        Init();
	}
	
    // // updated by: egillis
    // // updated at: 2018-11-11
    // // updated for: changed Update to FixedUpdate so that triggering happens consistently
    void FixedUpdate()
	{
//	      Debug.Log("Inside NPCMove#Update");
//        Debug.DrawRay(transform.position, transform.forward);

        if (!turn)
        {
            return;
        }

        if (!moving)
        {
//            TurnManager.StartTurn();
            FindNearestTarget();
            CalculatePath();
            // removed following to hide red tiles of NPC
//            FindSelectableTiles();
            actualTargetTile.target = true;
        }
        else
        {
            Move();
        }
	}

    void CalculatePath()
    {
        Tile targetTile = GetTargetTile(target);
        FindPath(targetTile);
    }

    void FindNearestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

        GameObject nearest = null;
        float distance = Mathf.Infinity;

        foreach (GameObject obj in targets)
        {
            if (obj.GetComponent<PlayerMove>().visible)
            {
                float d = Vector3.Distance(transform.position, obj.transform.position);

                if (d < distance)
                {
                    distance = d;
                    nearest = obj;
                }
                
            }
            else
            {
                nearest = null;
            }
        }

        if (nearest)
        {
//            TurnManager.StartTurn();
            moving = true;
            turn = true;
            target = nearest;
        }
        else
        {
//            TurnManager.EndTurn();
            moving = false;
            turn = false;
        }
    }
}
