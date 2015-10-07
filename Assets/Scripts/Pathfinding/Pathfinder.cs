using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinder : MonoBehaviour {

	private Cell start;
	private Cell target;
	private Cell[,] map;

	private Cell current;
	private Cell previous;
	private List<Cell> path;
	private List<Cell> neighbours;

	public Pathfinder(Cell start, Cell target, Cell[,] map) {
		this.start = start;
		this.target = target;
		this.map = map;
		current = start;
		previous = current;

		path = new List<Cell>();
		neighbours = new List<Cell>();
	}

	private void findCellNeighbours() { //Etsitään naapuriruudut voihin voidaan mennä
		neighbours.Clear();
		Vector2 coords = current.coordinates;

		int x = (int) coords.x;
		int y = (int) coords.y;

		if ((coords.x - 1 != -1) && (map [x - 1, y].IsWalkable ())) {
			neighbours.Add(map[x-1,y]);
		}

		if ((coords.x + 1 != 8) && (map [x + 1, y].IsWalkable ())) {
			neighbours.Add(map[x+1,y]);
		}

		if ((coords.y - 1 != -1) && (map [x, y-1].IsWalkable ())) {
			neighbours.Add(map[x,y-1]);
		}

		if ((coords.y + 1 != 8) && (map [x, y+1].IsWalkable ())) {
			neighbours.Add(map[x,y+1]);
		}
		//Debug.Log ("current: x"+current.coordinates.x+",y"+current.coordinates.y);
		Debug.Log ("neighbours count:"+neighbours.Count);
	}

	private bool matchPos(Cell one, Cell two) {	//Jos koordinaatit mätsäävät
		return ((one.coordinates.x == two.coordinates.x) && (one.coordinates.y == two.coordinates.y));
	}

	private void findPathList() {
		int count = 0;

		while ((!matchPos(current,target)) && (count != 10)) { //Tän kanssa varmaa vielä vähä häikkää. Rupee menemää edestakas kahden välil 
			findCellNeighbours ();
			Cell closestToTargetOnX = current;
			Cell closestToTargetOnY = current;
			int distx = 8;
			int disty = 8;
			count++;

			foreach(Cell c in neighbours) {
				if (!matchPos(c,previous) || neighbours.Count == 1) {
					if (Mathf.Abs(c.coordinates.x - target.coordinates.x) < distx) {
						distx = (int) Mathf.Abs(c.coordinates.x - (int) target.coordinates.x);
						closestToTargetOnX = c;
					}
					if (Mathf.Abs(c.coordinates.y - target.coordinates.y) < disty) {
						disty = (int) Mathf.Abs(c.coordinates.y - (int) target.coordinates.y);
						closestToTargetOnY = c;
					}
				}
			}

			//Debug.Log ("x"+distx);
			//Debug.Log ("y"+disty);


			if ((distx != 0) && (disty != 0)) { //Valitaan mihin celliin siirrytään. On kyl vähän sirkus
				if ((distx < disty)) {
					path.Add(closestToTargetOnX);
					//Debug.Log("add x");
				}
				else if ((disty < distx)) {
					path.Add(closestToTargetOnY);
					//Debug.Log("add y");
				}
				else {
					//Debug.Log ("add 0");
					path.Add(neighbours[0]);
					}
				}
			else {
				if ((distx == 0) && (current.coordinates.x !=0)) path.Add(closestToTargetOnX); 
				else if ((distx == 0)) path.Add(closestToTargetOnY);

				else if ((disty == 0) && (current.coordinates.y !=0)) path.Add(closestToTargetOnY);
				else path.Add(closestToTargetOnX);
			}


			previous = current;
			current = path[path.Count-1];
			Debug.Log ("move to: x"+current.coordinates.x+",y"+current.coordinates.y);
		}

	}

	public List<Cell> findPath() {
		findPathList();
		return path;
	}

}
