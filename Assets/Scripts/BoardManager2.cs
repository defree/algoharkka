using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

	
	public class BoardManager : MonoBehaviour
	{
		// Using Serializable allows us to embed a class with sub properties in the inspector.
		[Serializable]
		public class Count
		{
			public int minimum; 			//Minimum value for our Count class.
			public int maximum; 			//Maximum value for our Count class.
			
			
			//Assignment constructor.
			public Count (int min, int max)
			{
				minimum = min;
				maximum = max;
			}
		}
		
		
		public int columns = 8; 										//Number of columns in our game board.
		public int rows = 8;											//Number of rows in our game board.
		public int mapSize = 40;
		public Count wallCount = new Count (5, 9);						//Lower and upper limit for our random number of walls per level.
		public Count foodCount = new Count (1, 5);						//Lower and upper limit for our random number of food items per level.
		public GameObject exit;											//Prefab to spawn for exit.
		public GameObject[] floorTiles;									//Array of floor prefabs.
		public GameObject[] wallTiles;									//Array of wall prefabs.
		public GameObject[] foodTiles;									//Array of food prefabs.
		public GameObject[] enemyTiles;									//Array of enemy prefabs.
		public GameObject[] outerWallTiles;								//Array of outer tile prefabs.

		public static int[,] map = new int[100,100];
		
		private Transform boardHolder;									//A variable to store a reference to the transform of our Board object.
		private List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.
		
		public Cell[,] grid = new Cell[100,100];
		
		//Clears our list gridPositions and prepares it to generate a new board.
		void InitialiseList ()
		{
			//Clear our list gridPositions.
			gridPositions.Clear ();
			
			//Loop through x axis (columns).
			for(int x = 1; x < columns-1; x++)
			{
				//Within each column, loop through y axis (rows).
				for(int y = 1; y < rows-1; y++)
				{
					//At each index add a new Vector3 to our list with the x and y coordinates of that position.
					gridPositions.Add (new Vector3(x, y, 0f));
				}
			}
		}
		
		
		//Sets up the outer walls and floor (background) of the game board.
		void BoardSetup ()
		{
			//Instantiate Board and set boardHolder to its transform.
			boardHolder = new GameObject ("Board").transform;
			
			//Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
			for(int x = -1; x < mapSize + 1; x++)
			{
				//Loop along y axis, starting from -1 to place floor or outerwall tiles.
				for(int y = -1; y < mapSize + 1; y++)
				{
					//Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
					GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
					
					//Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
					//if(x == -1 || x == columns || y == -1 || y == rows)
						//toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
					
					//Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
					GameObject instance =
						Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					
					//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
					instance.transform.SetParent (boardHolder);
				}
			}
		}
		
		
		//RandomPosition returns a random position from our list gridPositions.
		Vector3 RandomPosition ()
		{
			//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
			int randomIndex = Random.Range (0, gridPositions.Count);
			
			//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
			Vector3 randomPosition = gridPositions[randomIndex];
			
			//Remove the entry at randomIndex from the list so that it can't be re-used.
			gridPositions.RemoveAt (randomIndex);
			
			//Return the randomly selected Vector3 position.
			return randomPosition;
		}
		
		
		//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
		void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
		{
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			int objectCount = Random.Range (minimum, maximum+1);
			
			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();
				
				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
				
				//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}

		void LayoutArrayConstructor (int[,] map){
			for(int x = map.GetLength(0)-1; x >= 0; x--){
				for(int y = map.GetLength(1)-1; y >= 0; y--){
					Vector3 positijoni = new Vector3(x, y, 0f);

					if(map[x, y] == 0){
						//Floor
						// This is done in another method
					}
					if(map[x, y] == 1){
						//Wall
						GameObject tileChoice = wallTiles[Random.Range (0, wallTiles.Length)];
						Instantiate (tileChoice, positijoni, Quaternion.identity);
					}
					if(map[x, y] == 2){
						//Enemy
						GameObject tileChoice = enemyTiles[Random.Range (0, enemyTiles.Length)];
						Instantiate (tileChoice, positijoni, Quaternion.identity);
					}
					if(map[x, y] == 3){
						//Food
						GameObject tileChoice = foodTiles[Random.Range (0, foodTiles.Length)];
						Instantiate (tileChoice, positijoni, Quaternion.identity);
					}
					if(map[x, y] == 4){
						//Player

					}
					if(map[x, y] == 5){
						//Exit
						Instantiate (exit, positijoni, Quaternion.identity);
					}
				}
			}
		}
		
		
		//SetupScene initializes our level and calls the previous functions to lay out the game board
		public void SetupScene (int level)
		{
			//Creates the floor
			BoardSetup ();
			
			//Reset our list of gridpositions.
			//InitialiseList ();
			
			//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
			//LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
			
			//Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
			//LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);
			
			//Determine number of enemies based on current level number, based on a logarithmic progression
			int enemyCount = 5;
			
			//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
			//LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
			
			//Instantiate the exit tile in the upper right hand corner of our game board
			//Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);

			//testMap = RoomMapMaker (100);
			RoomMapMaker (mapSize);

			// Add enemies to map
			EnemyAdder (enemyCount);

			// Add food to map
			FoodAdder (foodCount.maximum);

			LayoutArrayConstructor (map);

			IntMaptoCellMap ();
		}

		private void FoodAdder(int max){
			int counter = 0;
			
			while (counter < max) {
				int randomX = Random.Range(0, mapSize);
				int randomY = Random.Range(0, mapSize);
				if( map[randomX, randomY] == 0){
					map[randomX, randomY] = 3;
					counter++;
				}
			}
		}

		private void EnemyAdder(int max){
			int counter = 0;
			
			while (counter < max) {
				int randomX = Random.Range(0, mapSize);
				int randomY = Random.Range(0, mapSize);
				if( map[randomX, randomY] == 0){
					map[randomX, randomY] = 2;
					counter++;
				}
			}

		}

		private void RoomMapMaker (int size){
			RoomArea room = new RoomArea (new Coordinate (0, 0), new Coordinate (size-1, size-1));

			
		}

		public class Coordinate
		{
			public int x; 			//Minimum value for our Count class.
			public int y; 			//Maximum value for our Count class.
			
			
			//Assignment constructor.
			public Coordinate (int xx, int yy)
			{
				y = yy;
				x = xx;
			}
		}

		public class RoomArea
		{
			public Coordinate upperLeftCorner;
			public Coordinate lowerRightCorner;

			private static bool firstTimeFlag = true;

			private bool trueMeansHorizontal;
			public bool finished = false;

			public RoomArea leftChild = null;
			public RoomArea rightChild = null;

			public int height;
			public int width;
			public int area;

			private int roomSizeVariable = 5;
			private int roomSizeConstant; 
			private int minRoomSize = 3; 


			//Assignment constructor.
			public RoomArea (Coordinate uLC, Coordinate lRC)
			{
				upperLeftCorner = uLC;
				lowerRightCorner = lRC;
				height = lowerRightCorner.y - upperLeftCorner.y+1;
				width = lowerRightCorner.x - upperLeftCorner.x+1;
				// Initialize the map if it is the first time
				if( firstTimeFlag ) {
					firstTimeFlag = false;
					for ( int x = 0; x < map.GetLength(0); x++){
						for ( int y = 0; y < map.GetLength(1); y++){
							map[0, 0] = 0;
						}
					}
					map[width-3, height-3] = 5;
					map[width-4, height-4] = 2;
				}



				area = height*width;

				// this is the smallest area that can be split and still generate two rooms in subrooms
				roomSizeConstant = (minRoomSize+4)*2; 

				if ((height > (roomSizeConstant)) && (width > (roomSizeConstant ))) {
					// split some shit up
					//	Debug.Log(height + " " + width);
					
					trueMeansHorizontal = Random.value > 0.5f;

					//cut the shit with HORIZONTAL LINE
					// i.e. first sub room has uLC and lRC is Coord(Random.range(minRoomSize+2
					if (trueMeansHorizontal){
						int splitPointY = Random.Range(upperLeftCorner.y+minRoomSize+1, lowerRightCorner.y-minRoomSize-2);
						Debug.Log("SP: " + splitPointY + " uLC:(" + upperLeftCorner.x + ", " + upperLeftCorner.y + ") lRC:(" + lowerRightCorner.x + ", " + lowerRightCorner.y + ")");
						leftChild = new RoomArea(upperLeftCorner, new Coordinate(lowerRightCorner.x, splitPointY));
						rightChild = new RoomArea(new Coordinate(upperLeftCorner.x, splitPointY+1), lowerRightCorner);
					} 
					// cut the shit with VERTICAL LINE
					else {
						int splitPointX  = Random.Range (upperLeftCorner.x+minRoomSize+1, lowerRightCorner.x-minRoomSize-2);
						leftChild = new RoomArea(upperLeftCorner, new Coordinate(splitPointX, lowerRightCorner.y));
						rightChild = new RoomArea(new Coordinate(splitPointX+1, upperLeftCorner.y), lowerRightCorner);

					}
					//we have now made child rooms so we should attach them to each other
					//problem is that we only want to attach nodes that are leafs or
					// that have already been combined
					// or maybe not?
					if (trueMeansHorizontal){
						//we splat the dungeon horizontally
						if(true){
							//select random horizontal wall point from upper child area( that can be connected)
							int rUX; //Random Upper X
							int rUY; //Random Upper Y
							int counter = 0;
							while(true){
								rUX = Random.Range(leftChild.upperLeftCorner.x+2, leftChild.lowerRightCorner.x-2);
								rUY = leftChild.lowerRightCorner.y-2;
								if(map[rUX, rUY] == 0){
									break;
								}
								if(counter++ > 10000){ // just to be sure #lol #yolo
									break;
								}

							}
							// poke a deep hole in it (uuh)
							map[rUX, rUY+1] = 0;
							map[rUX, rUY+2] = 0;

							// select random hor wall point from lower child area (that can be connected)
							int rLX; //Random Lower X
							int rLY; //Random Lower Y
							counter = 0;
							while(true){
								rLX = Random.Range(rightChild.upperLeftCorner.x+2, rightChild.lowerRightCorner.x-2);
								rLY = rightChild.upperLeftCorner.y+2;
								if(map[rLX, rLY] == 0){
									break;
								}
								if(counter++ > 10000){
									break;
								}
							}
							//poke a hole in it (aah)
							map[rLX, rLY-1] = 0;
							map[rLX, rLY-2] = 0;

							//connect them with a horizontal tunnel
							// line from (rUX, rUY+2) to (rLX, rLY+2)
							for(int i = rUX; true;){
								if(rUX > rLX){
									//Drill left
									map[i, rUY+2] = 0;
									if (map[i, rUY+3] == 0){
										break;
									}
									i--;
								} else if ( rUX < rLX){
									//Drill right
									map[i, rUY+2] = 0;
									if (map[i, rUY+3] == 0){
										break;
									}
									i++;
								} else {
									break;
								}
							}
						}
					}
						//we splat it vertically
					else{
						if(true){
							Debug.Log("Are we even trying, man");
							//select random vertical wall point from left child area( that can be connected)
							int rUX; //Random Upper X
							int rUY; //Random Upper Y
							int counter = 0;
							while(true){
								rUX = leftChild.lowerRightCorner.x-2;
								rUY = Random.Range(leftChild.upperLeftCorner.y+2, leftChild.lowerRightCorner.y-2);
								//check that we hit a free space
								if(map[rUX, rUY] == 0){
									break;
								}
								if(counter++ > 10000){ // just to be sure #lol #yolo
									break;
								}
								
							}
							// poke a deep hole in it (uuh)
							map[rUX+1, rUY] = 0;
							map[rUX+2, rUY] = 0;
							
							// select random ver wall point from right child area (that can be connected)
							int rLX; //Random Lower X
							int rLY; //Random Lower Y
							counter = 0;
							while(true){
								rLX = rightChild.upperLeftCorner.x+2;
								rLY = Random.Range(rightChild.upperLeftCorner.y+2, rightChild.lowerRightCorner.y-2);
								if(map[rLX, rLY] == 0){
									break;
								}
								if(counter++ > 10000){
									break;
								}
							}
							//poke a hole in it (aah)
							map[rLX-1, rLY] = 0;
							map[rLX-2, rLY] = 0;
							
							//connect them with a horizontal tunnel
							// line from (rUX, rUY+2) to (rLX, rLY+2)
							for(int i = rUY; true;){
								if(rUY > rLY){
									//Drill down
									map[rUX+2, i] = 0;
									if (map[rUX+3, i] == 0){
										break;
									}
									i--;
								} else if ( rUY < rLY){
									//Drill up
									map[rUX+2, i] = 0;
									if (map[rUX+3, i] == 0){
										break;
									}
									i++;
								} else {
									break;
								}
							}
						}

					}


				}
				// leaf nodes (no children) will generate rooms in them
				else {
					//just make full size rooms for now
					//second line for all walls is to make them thicker
					// horizontal walls
					for(int i = 0; i < width; i++){
						//north
						map[upperLeftCorner.x+i, upperLeftCorner.y] = 1;
						map[upperLeftCorner.x+i, upperLeftCorner.y+1] = 1;
						//south
						map[upperLeftCorner.x+i, lowerRightCorner.y] = 1;
						map[upperLeftCorner.x+i, lowerRightCorner.y-1] = 1;
					}
					// vertical walls
					for(int i = 0; i < height; i++){
						// west
						map[upperLeftCorner.x, upperLeftCorner.y+i] = 1;
						map[upperLeftCorner.x+1, upperLeftCorner.y+i] = 1;
						// east
						map[lowerRightCorner.x, upperLeftCorner.y+i] = 1;
						map[lowerRightCorner.x-1, upperLeftCorner.y+i] = 1;
					}
					//this is a leaf node
					finished = true;

				}

			}
	}
	public void IntMaptoCellMap () {

		for(int x = 0; x < 100; x++)
		{
			//Within each column, loop through y axis (rows).
			for(int y = 0; y < 100; y++)
			{
				//0,3
				Cell c = new Cell(x,y);
				if ((map[x,y] != 0) && (map[x,y] != 3)) c.setWalkable(false);
				grid[x,y] = c;
			}
		}
	}
}
