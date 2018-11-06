using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Maze : MonoBehaviour
{

    public IntVector2 size;
    public MazeCell cellPreFab;
    public float generationStepDelay;
	public MazePassage passagePreFab;
	public MazeWall wallPreFab;

    public IntVector2 RandomCoordinates
    {
        get
        {
            return new IntVector2(Random.Range(0, size.X), Random.Range(0, size.Z));
        }
    }

    private MazeCell[,] cells;

	public MazeCell GetCell(IntVector2 coordinates) {
		return cells[coordinates.X, coordinates.Z];
	}

    public IEnumerator Generate()
    {

        WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
        cells = new MazeCell[size.X, size.Z];

        IntVector2 coordinates = RandomCoordinates;

        while (ContainsCoordinates(coordinates) && GetCell(coordinates) ==null)
        {
            yield return delay;
            CreateCell(coordinates);
            coordinates += MazeDirections.randomValue.ToIntVector2();
        }
    }

	private void DoNextGenerationStep(List<MazeCell> activeCells){

		int currentIndex = activeCells.Count - 1;
		MazeCell currentCell = activeCells[currentIndex];

		MazeDirection direction = MazeDirections.randomValue;
		IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2();

		if (ContainsCoordinates(coordinates)){

			MazeCell neighbor = GetCell(coordinates);

			if (neighbor == null) {

				neighbor = CreateCell(coordinates);
				CreatePassage(currentCell, neighbor, direction);
				activeCells.Add(neighbor);
			} else {
				CreateWall(currentCell, neighbor, direction);
				activeCells.RemoveAt(currentIndex);
			}
		} else {
			CreateWall(currentCell, null, direction);
			activeCells.RemoveAt(currentIndex);
		}
	}

    private void CreatePassage(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazePassage passage = Instantiate<MazePassage>(passagePreFab);
		passage.Initialize(cell, otherCell, direction);
		passage = Instantiate<MazePassage>(passagePreFab);
		passage.Initialize(otherCell, cell, direction.GetOpposite());
    }

    private void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazeWall wall = Instantiate<MazeWall>(wallPreFab);
		wall.Initialize(cell, otherCell, direction);

		if (otherCell != null) {

			wall = Instantiate<MazeWall>(wallPreFab);
			wall.Initialize(otherCell, cell, direction.GetOpposite());
		}
    }

    public bool ContainsCoordinates(IntVector2 coordinates)
    {

        return coordinates.X >= 0 && coordinates.X < size.X && coordinates.Z >= 0 && coordinates.Z < size.Z;

    }

    private MazeCell CreateCell(IntVector2 coordinates)
    {
        MazeCell newCell = Instantiate<MazeCell>(cellPreFab);
        cells[coordinates.X, coordinates.Z] = newCell;
        newCell.coordinates = coordinates;
        newCell.name = "Maze Cell " + coordinates.X + ", " + coordinates.Z;
        newCell.transform.parent = transform;
        newCell.transform.localPosition = new Vector3(coordinates.X - size.X * 0.5f + 0.5f, coordinates.Z - size.Z * 0.5f + 0.5f);

		return newCell;
    }
}
