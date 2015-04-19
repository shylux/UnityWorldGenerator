using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // used for Sum of array
using System.Text; // StringBuilder

public class MazeGenerator: MonoBehaviour {
	Maze2D maze;

	public int width = 10;
	public int length = 10;
	public Transform WallPrefab;
    private Vector3 wallSize;
    public Transform WallSeparatorPrefab;
	float unitSize; // length of one sqare containing a cell and two walls

    public int LanesMaxLength = 0;

	List<List<Maze2D.Cell>> cellGroups = new List<List<Maze2D.Cell>>();

	void Start() {
		maze = new Maze2D (width, length);
        foreach (Maze2D.Wall wall in maze.Walls())
            wall.set(true);

        KnoepfelLanes();

        wallSize = getBounds(WallPrefab);
        unitSize = wallSize.x + wallSize.z;
        BuildStartFinish();
		BuildMazeInSzene ();
	}

	void BuildMazeInSzene() {
        GameObject wallContainer = new GameObject();
        wallContainer.name = "Walls";
        wallContainer.transform.parent = transform;
		
		foreach (Maze2D.Wall wall in maze.Walls()) {
            Vector3 pos = new Vector3(wall.x * unitSize, 0, Mathf.Min(wall.y / 2) * unitSize);
            // WALL SEPARATORS
            if (wall.y % 2 == 0) {
                Transform wallSepObj = Instantiate(WallSeparatorPrefab) as Transform;
                wallSepObj.transform.parent = wallContainer.transform;
                wallSepObj.position = wallSepObj.position + pos + transform.position + new Vector3(wallSize.z / 2, 0, wallSize.z / 2);
                if (wall.x == width - 1) {
                    Transform wallSepObjLast = Instantiate(WallSeparatorPrefab) as Transform;
                    wallSepObjLast.transform.parent = wallContainer.transform;
                    wallSepObjLast.position = wallSepObjLast.position + pos + transform.position + new Vector3(wallSize.z / 2 + unitSize, 0, wallSize.z / 2);
                }

            }

            // WALLS
            if (wall.get()) {
                Vector3 offset;
                Quaternion rot;
                if (wall.y % 2 == 0) { // horizontal
                    rot = Quaternion.identity;
                    offset = new Vector3(wallSize.z, 0, wallSize.z / 2);
                } else { // vertical
                    rot = Quaternion.AngleAxis(-90, Vector3.up);
                    offset = new Vector3(wallSize.z / 2, 0, wallSize.z);
                }
                Transform wallObj = Instantiate(WallPrefab, pos + offset + transform.position, rot) as Transform;
                wallObj.transform.parent = wallContainer.transform;
            }
		}
		// Debug.Log (size.x + " " + size.y + " " + size.z);
	}

    void BuildStartFinish() {
        maze.wall(width/2, 0).set(false);
        maze.mwall(width/2, -1).set(false);
        Transform start = transform.Find("Start");
        start.position = start.position + new Vector3(width / 2 * unitSize + unitSize / 2f, 0, 0);
        Transform finish = transform.Find("Finish");
        finish.position = finish.position + new Vector3(width / 2 * unitSize + unitSize / 2f, 0, length * unitSize);
    }

	Vector3 getBounds(Transform parent) {
		Bounds bounds = parent.GetComponent<Renderer>().bounds;
		foreach (Transform child in parent.transform) {
			bounds.Encapsulate(child.GetComponent<Renderer>().bounds);
		}
		return bounds.size;
	}

	private void RandomizedKruskal() {
		foreach (Maze2D.Cell c in maze.Cells()) {
			cellGroups.Add(new List<Maze2D.Cell> {c});
		}

        List<Maze2D.Wall> walls = maze.Walls().ToList();
        maze.Shuffle(walls);
		foreach (Maze2D.Wall w in walls) {
            Maze2D.Cell a = w.getAdjacentCell(0);
            Maze2D.Cell b = w.getAdjacentCell(1);
            if (a == null || b == null) continue;
            //Debug.Log(w.ToString() + "" + a.ToString() + "" + b.ToString());
            if (!areCellsInSameGroup(a, b)) {
                mergeCellGroup(a, b);
                w.set(false);
            }
		}
	}

    private void KnoepfelLanes() {
        if (LanesMaxLength < 2) LanesMaxLength = int.MaxValue;
        List<Maze2D.Cell> unassignedCells = maze.Cells().ToList();
        maze.Shuffle(unassignedCells);
        while (unassignedCells.Count > 0) {
            Maze2D.Cell startCell = unassignedCells[0];
            unassignedCells.Remove(startCell);
            List<Maze2D.Cell> currentGroup = new List<Maze2D.Cell>() { startCell };
            cellGroups.Add(currentGroup);
            Maze2D.Cell currentCell = startCell;
            Vector2 direction = Maze2D.DIRECTIONS[Random.Range(0, Maze2D.DIRECTIONS.Count)];
            int stepCounter = 1;
            while (true) {
                if (stepCounter >= LanesMaxLength) break;
                Maze2D.Cell nextCell = currentCell + direction;
                if (nextCell == null || !unassignedCells.Contains(nextCell)) break;
                currentCell.getWall(direction).set(false);
                unassignedCells.Remove(nextCell);
                currentGroup.Add(nextCell);
                stepCounter++;
                currentCell = nextCell;
            }                       
        }
        while (cellGroups.Count > 1) {
            List<Maze2D.Cell> currentGroup = getSmallestCellGroup();
            Maze2D.Cell currentCell = currentGroup[Random.Range(0, currentGroup.Count)];
            Vector2 direction = Maze2D.DIRECTIONS[Random.Range(0, Maze2D.DIRECTIONS.Count)];
            while (true) {
                Maze2D.Cell nextCell = currentCell + direction;
                if (nextCell == null) break;
                if (areCellsInSameGroup(currentCell, nextCell)) {
                    currentCell = nextCell;
                    continue;
                }
                // other cell must be in other group
                currentCell.getWall(direction).set(false);
                mergeCellGroup(currentCell, nextCell);
                break;
            }
        }
    }

    private List<Maze2D.Cell> getCellGroup(Maze2D.Cell cell) {
        foreach (List<Maze2D.Cell> lst in cellGroups) {
            if (lst.Contains(cell)) return lst;
        }
        return null;
    }
    private List<Maze2D.Cell> getSmallestCellGroup() {
        List<Maze2D.Cell> smallestGroup = cellGroups[0];
        foreach (List<Maze2D.Cell> lst in cellGroups) {
            if (lst.Count < smallestGroup.Count) smallestGroup = lst;
        }
        return smallestGroup;
    }
    private bool areCellsInSameGroup(Maze2D.Cell a, Maze2D.Cell b) {
        return getCellGroup(a).Contains(b);
    }
    private void mergeCellGroup(Maze2D.Cell a, Maze2D.Cell b) {
        // Debug.Log(a.ToString() + "" + getCellGroup(a) + "" + b.ToString() + "" + getCellGroup(b));
        List<Maze2D.Cell> lsta = getCellGroup(a);
        List<Maze2D.Cell> lstb = getCellGroup(b);
        lsta.AddRange(lstb);
        cellGroups.Remove(lstb);

    }


}

/*

*/