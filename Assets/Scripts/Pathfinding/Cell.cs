using UnityEngine;
using System.Collections;

public class Cell {


	public Vector2 coordinates;
	private bool walkable = true;
	private float movementCost = 0;


	public Cell (int x, int y) {

		this.coordinates.x = x;
		this.coordinates.y = y;

	}

	public void setWalkable(bool walkable) {
		this.walkable = walkable;
	}

	public bool IsWalkable() {
		return walkable;
	}

	public float MovementCost() {
		return movementCost;
	}
}