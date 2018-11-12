using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    static Dictionary<string, List<TacticsMove>> units = new Dictionary<string, List<TacticsMove>>();

    static Queue<string> turnKey = new Queue<string>();

    // static Queue<TacticsMove> turnTeam = new Queue<TacticsMove>();
    static List<TacticsMove> turnTeamList = new List<TacticsMove>();


    void Start()
    {
//        Debug.Log("Inside TurnManager class");

    }

    // // updated by: egillis
    // // updated at: 2018-11-11
    // // updated for: changed Update to FixedUpdate so that triggering happens consistently
    void FixedUpdate()
    {
//        Debug.Log("Inside TurnManager#Update");

        // // replaced "turnTeam" as a Queue<T> to become "turnTeamList" as a List<T>
        // if (turnTeam.Count == 0)
        // {
        //     InitTeamTurnQueue();
        // }

        if (turnTeamList.Count == 0)
        {
            InitTeamTurnQueue();
        }
    }

    static void InitTeamTurnQueue()
    {
//        Debug.Log("Inside TurnManager#InitTeamTurnQueue");

        // Debug.Log(units);
        List<TacticsMove> teamList = units[turnKey.Peek()];

        // removed to allow dynamic choice of which player is active
        foreach (TacticsMove unit in teamList)
        {
            // Debug.Log(unit);
            unit.Reset();

            turnTeamList.Add(unit);
        }

        StartTurn();
    }

    public static void StartTurn()
    {
//        Debug.Log("Inside TurnManager#StartTurn");
        if (turnTeamList.Count > 0)
        {
//            Debug.Log("turnTeamList.Count = " + turnTeamList.Count);

            foreach (TacticsMove teamMember in turnTeamList)
            {
//                Debug.Log("Building List: " + teamMember.name + " as " + teamMember.tag);

                if (teamMember.CompareTag("NPC"))
                {
//                    Debug.Log("NPC about to play...");
                    teamMember.BeginTurn();
                   
                }
            }
        }
    }

    public static void EndTurn()
    {
//        Debug.Log("TurnManager.EndTurn()");

        for (int i = 0; i < turnTeamList.Count; i++)
        {
            TacticsMove unit = turnTeamList[i];

            unit.moving = false;

            // // updated by: emgillis
            // // updated on: 2018-11-08
            // // updated for: removing "moved" check to allow player to continue having move options
            // //        also helps with NPC to have continuous movement / tracking to get to player
            //if (unit.moved)
            //{
//                Debug.Log("Removing TeamMember: " + turnTeamList[i].name);
                // // turnTeamList[i].EndTurn();
               // turnTeamList.RemoveAt(i);
               // unit.EndTurn();
            //}
        }


        // Debug.Log("TurnManagerEndTurn: " + unit.name);

        // // removed as part of updating turnTeam to become turnTeamList
        // TacticsMove unit = turnTeam.Dequeue();
        // unit.EndTurn();
        // unit.HasMoved();

        // if (turnTeam.Count > 0)
        // {
        //     StartTurn();
        // }
        // else
        // {
        //     string team = turnKey.Dequeue();
        //     Debug.Log("EndTurn: " + team);
        //     turnKey.Enqueue(team);
        //     InitTeamTurnQueue();
        // }

        if (turnTeamList.Count > 0)
        {
            StartTurn();
        }
        else
        {
            string team = turnKey.Dequeue();
            turnKey.Enqueue(team);
            InitTeamTurnQueue();
        }
    }

    public static void AddUnit(TacticsMove unit)
    {
        List<TacticsMove> list;

        if (!units.ContainsKey(unit.tag))
        {
            list = new List<TacticsMove>();
            units[unit.tag] = list;

            if (!turnKey.Contains(unit.tag))
            {
                turnKey.Enqueue(unit.tag);
            }
        }
        else
        {
            list = units[unit.tag];
        }

        list.Add(unit);
    }
}