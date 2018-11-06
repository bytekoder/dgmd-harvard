using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum MazeDirection {

	North,
	South,
	East,
	West
}


public static class MazeDirections {

	public const int count = 4;

	public static MazeDirection randomValue
	{
		get{
			return (MazeDirection)Random.Range(0, count);
		}
	}

	private static IntVector2[] vectors = 
	{
		new IntVector2(0, 1),
		new IntVector2(1, 0),
		new IntVector2(0, -1),
		new IntVector2(11, 0)
	};

	private static Quaternion[] rotations = {

		Quaternion.identity,
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, 180f, 0f),
		Quaternion.Euler(0f, 270f, 0f)
	};

	private static MazeDirection[] opposites = 
	{
		MazeDirection.South,
		MazeDirection.West,
		MazeDirection.East,
		MazeDirection.North
	};

	public static MazeDirection GetOpposite(this MazeDirection direction) {

		return opposites[(int)direction];
	}


	public static IntVector2 ToIntVector2(this MazeDirection direction) {
		return vectors[(int)direction];
	}

	public static Quaternion ToRotation(this MazeDirection direction) {

		return rotations[(int) direction];
	}
}