#define LEO

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class Enemy : MovingObject
{
	public int playerDamage;                                                        //The amount of food points to subtract from the player when attacking.
	public AudioClip attackSound1;                                          //First of two audio clips to play when attacking the player.
	public AudioClip attackSound2;                                          //Second of two audio clips to play when attacking the player.
	
	public BoardManager boardManager;
	
	private Animator animator;                                                      //Variable of type Animator to store a reference to the enemy's Animator component.
	private Transform playerTransform;                                                      //Transform to attempt to move toward each turn.
	private bool skipMove;                                                          //Boolean to determine whether or not enemy should skip a turn or move this turn.
	
	private Cell[,] grid;
	
	#if LEO
	public Type enemyType;
	
	public int HP = 3;
	private int prevHP = 3;
	
	//public Cell currentLocation;
	
	//public GameObject player;
	//public Pathfinder pathfinding;
	
	public State aiState;
	public Type aiType;
	
	//private Cell lastSpot;
	public int lastX;
	public int lastY;
	public int spottedCounter;
	public int lostCounter;
	
	private const int SPOTTED_THRESHOLD = 3;
	private const int LOST_THRESHOLD = 3;
	
	//private Cell A;
	public int aX;
	public int aY;
	//private Cell B;
	public int bX;
	public int bY;
	private bool aNext;
	
	public enum State
	{
		Guarding,
		Patrolling,
		Investigating,
		Attacking,
		Dead
	}
	
	public enum Type
	{
		Guard,
		Patroller,
		Passive
	}
	#endif
	
	//Start overrides the virtual Start function of the base class.
	protected override void Start ()
	{
		//Register this enemy with our instance of GameManager by adding it to a list of Enemy objects.
		//This allows the GameManager to issue movement commands.
		GameManager.instance.AddEnemyToList (this);
		
		//Get and store a reference to the attached Animator component.
		animator = GetComponent<Animator> ();
		
		//Find the Player GameObject using it's tag and store a reference to its transform component.
		playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;
		
		boardManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardManager>();
		grid = boardManager.grid;
		
		//Call the start function of our base class MovingObject.
		base.Start ();
		
		#if LEO
		if (aiType == Type.Guard) aiState = State.Guarding;
		else if (aiType == Type.Patroller) aiState = State.Patrolling;
		prevHP = HP;
		#endif
	}
	
	
	//Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
	//See comments in MovingObject for more on how base AttemptMove function works.
	protected override void AttemptMove <T> (int xDir, int yDir)
	{
		//Check if skipMove is true, if so set it to false and skip this turn.
		if(skipMove)
		{
			skipMove = false;
			//return;
			
		}
		
		//Call the AttemptMove function from MovingObject.
		base.AttemptMove <T> (xDir, yDir);
		
		//Now that Enemy has moved, set skipMove to true to skip next move.
		skipMove = true;
	}
	
	
	//MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
	public void MoveEnemy (int toX, int toY)
	{
		Debug.Log("TRYING TO MOVE TO x=" + toX.ToString() + ", y=" + toY.ToString());
		
		//Declare variables for X and Y axis move directions, these range from -1 to 1.
		//These values allow us to choose between the cardinal directions: up, down, left and right.
		int xDir = 0;
		int yDir = 0;
		
		int trax = (int)transform.position.x;
		int tray = (int)transform.position.y;
		
		//int targetx = (int)target.position.x;
		//int targety = (int)target.position.y;
		int targetx = toX;
		int targety = toY;
		
		//Haetaan paras reitti vastustajalta pelaajalle
		List<Cell> movePath = FindPath (grid[trax,tray],grid[targetx,targety]);
		
		Cell nextCell = new Cell(0, 0);
		try
		{
			nextCell = movePath[0];
		}
		catch
		{
			Debug.LogError("movePathin koko == 0, " + "aiState=" + aiState.ToString());
		}
		/*foreach (Cell c in movePath) {
                                //Debug.Log (c.coordinates.x+","+c.coordinates.y+","+c.IsWalkable());
                               
                        }
                       
                        for (int x = 0; x < 8; x++) {
                                                //Loop along y axis, starting from -1 to place floor or outerwall tiles.
                                for (int y = 0; y < 8; y++) {  
                                        Debug.Log(x+","+y+","+grid[x,y].IsWalkable());
                                }
                        }
                        */
		
		//If the difference in positions is approximately zero (Epsilon) do the following:
		if(Mathf.Abs (nextCell.coordinates.x - transform.position.x) < float.Epsilon)
			
			//If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
			yDir = nextCell.coordinates.y > transform.position.y ? 1 : -1;
		
		//If the difference in positions is not approximately zero (Epsilon) do the following:
		else
			//Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
			xDir = nextCell.coordinates.x > transform.position.x ? 1 : -1;
		
		//Call the AttemptMove function and pass in the generic parameter Player, because Enemy is moving and expecting to potentially encounter a Player
		AttemptMove <Player> (xDir, yDir);
	}
	
	
	//OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject
	//and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
	protected override void OnCantMove <T> (T component)
	{
		//Declare hitPlayer and set it to equal the encountered component.
		Player hitPlayer = component as Player;
		
		//Call the LoseFood function of hitPlayer passing it playerDamage, the amount of foodpoints to be subtracted.
		hitPlayer.LoseFood (playerDamage);
		
		//Set the attack trigger of animator to trigger Enemy attack animation.
		animator.SetTrigger ("enemyAttack");
		
		//Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
		SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
	}
	
	protected List<Cell> FindPath(Cell origin, Cell goal) {
		Pathfinder find = new Pathfinder (origin,goal,grid);
		return find.findPath ();
	}
	/*
                protected List<Cell> FindPath(Cell origin, Cell goal) {
                        AStar pathFinder = new AStar();
                        pathFinder.FindPath (origin, goal, grid, false);
                        return pathFinder.CellsFromPath ();
                }
                */
	
	#if LEO
	public void PlayTurn()
	{
		Observe();
		CheckHP();
		Act();
	}
	
	private void Observe()
	{
		Look();
	}
	
	private void Look()
	{
		//Ray2D ray = new Ray2D(transform.position, -transform.position + player.transform.position);
		GetComponent<BoxCollider2D>().enabled = false;
		RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.position + playerTransform.position);
		if (hit.collider != null)
		{
			if (hit.collider.tag == "Player")
			{
				OnEnemySeen();
				Debug.Log("Player seen");
			}
			else
			{
				OnEnemyLost();
				Debug.Log("Non-player object seen");
			}
		}
		else
		{
			OnEnemyLost();
			Debug.Log("Nothing seen (WAT???)");
		}
		GetComponent<BoxCollider2D>().enabled = true;
	}
	
	private void OnEnemySeen()
	{
		spottedCounter++;
		lostCounter = 0;
		//lastSpot = player.GetLocation();
		lastX = (int)playerTransform.position.x;
		lastY = (int)playerTransform.position.y;
		aiState = State.Investigating;
		if (spottedCounter >= SPOTTED_THRESHOLD)
		{
			aiState = State.Attacking;
		}
	}
	
	private void OnEnemyLost()
	{
		lostCounter++;
		spottedCounter = 0;
		if (aiState == State.Attacking && lostCounter >= LOST_THRESHOLD)
		{
			aiState = State.Investigating;
		}
	}
	
	private void CheckHP()
	{
		if (HP < prevHP)
		{
			//lastSpot = player.GetLocation();
			lastX = (int)playerTransform.position.x;
			lastY = (int)playerTransform.position.y;
			aiState = State.Attacking;
		}
		if (HP <= 0)
		{
			aiState = State.Dead;
		}
	}
	
	private void Act()
	{
		switch (aiState)
		{
		case State.Attacking:
			//TryToAttack();
			//MoveTo(pathfinding.DirectionFor(lastSpot));
			MoveEnemy(lastX, lastY);
			break;
			
		case State.Guarding:
			// Derp
			break;
			
		case State.Investigating:
			if ((int)transform.position.x == lastX && (int)transform.position.y == lastY)
			{
				if (aiType == Type.Guard) aiState = State.Guarding;
				else if (aiType == Type.Patroller) aiState = State.Patrolling;
				Debug.Log("INVESTIGATING->GUARDING/PATROLLING");
			}
			else MoveEnemy(lastX, lastY);
			//MoveTo(pathfinding.DirectionFor(lastSpot));
			break;
			
		case State.Patrolling:
			//if (aNext) MoveTo(pathfinding.DirectionFor(A));
			//else MoveTo(pathfinding.directionFor(B));
			if (aNext) MoveEnemy(aX, aY);
			else MoveEnemy(bX, bY);
			break;
			
		case State.Dead:
			// ded.
			break;
			
		default:
			break;
		}
	}
	
	private void TryToAttack()
	{
		
	}
	
	private void MoveTo()
	{
		
	}
	#endif
}