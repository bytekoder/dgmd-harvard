using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    static Dictionary<string, List<TacticsMove>> units = new Dictionary<string, List<TacticsMove>>();
    static Queue<string> turnKey = new Queue<string>();
    // static Queue<TacticsMove> turnTeam = new Queue<TacticsMove>();
    static List<TacticsMove> turnTeamList = new List<TacticsMove>();

	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{
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

        if (turnTeamList.Count > 0)
        {
          // Debug.Log("turnTeamList.Count = " + turnTeamList.Count);

          foreach(TacticsMove teamMember in turnTeamList)
          {
            // Debug.Log("Building List: " + teamMember.name + " as " + teamMember.tag);

              if(teamMember.tag == "NPC")
              {
                teamMember.BeginTurn();
              }
          }

        }

    }

    public static void EndTurn()
    {
        // Debug.Log("TurnManager.EndTurn()");

        for(int i=0; i < turnTeamList.Count; i++)
        {
          TacticsMove unit = turnTeamList[i];

          unit.moving = false;
          if(unit.moved)
          {
            // Debug.Log("Removing TeamMember: " + turnTeamList[i].name);
            // turnTeamList[i].EndTurn();
            turnTeamList.RemoveAt(i);
            unit.EndTurn();
          }
        }

        // TacticsMove unit = turnTeam.Dequeue();
        // unit.EndTurn();
        // unit.HasMoved();

        // Debug.Log("TurnManagerEndTurn: " + unit.name);

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
