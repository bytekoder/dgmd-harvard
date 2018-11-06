using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public Maze mazePreFab = null;
	private Maze mazeInstance = null;
    private void Start()
    {
        BeginGame();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }

    private void BeginGame()
    {
        mazeInstance = Instantiate<Maze>(mazePreFab);
		StartCoroutine(mazeInstance.Generate());
    }

    private void RestartGame()
    {
		Debug.Log("Recreating maze...");
		StopAllCoroutines();
        //Destroy(mazeInstance.gameObject);
		BeginGame();
    }
}
