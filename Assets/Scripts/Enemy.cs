using UnityEngine;
using System.Collections;
using System.Collections.Generic;		//Allows us to use Lists.


namespace Completed
{
	//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
	public class Enemy : MovingObject
	{
		public int playerDamage; 							//The amount of food points to subtract from the player when attacking.
		public AudioClip attackSound1;						//First of two audio clips to play when attacking the player.
		public AudioClip attackSound2;						//Second of two audio clips to play when attacking the player.
		public AudioClip deathSound1;						//Second of two audio clips to play when attacking the player.
		public AudioClip deathSound2;						//Second of two audio clips to play when attacking the player.
		public AudioClip deathSound3;						//Second of two audio clips to play when attacking the player.
		public AudioClip deathSoundBoss;						//Second of two audio clips to play when attacking the player.
		public int enemyHealth = 100; 							//The amount of health the enemy has to start.
		public bool spawned = false;
		public GameObject enemySkullPrefab;
		public GameObject enemyWispPrefab;
		public bool canSpawnMinions = false;
		public GameObject spawnLocationObject;
		private GameObject spawnTempLocationObject;// = new GameObject();
		private Rigidbody2D spawnLocationTempRigidBody2D;// = new Rigidbody2D();

		
		private Animator animator;							//Variable of type Animator to store a reference to the enemy's Animator component.
		private Transform target;							//Transform to attempt to move toward each turn.
		private bool skipMove;								//Boolean to determine whether or not enemy should skip a turn or move this turn.
		private bool beginSpawning = true;
		private bool spawning = false;

		private int currentX;
		private int currentY;
		
		private int targetX;
		private int targetY;

		private SpriteRenderer thisSpriteRenderer;

		//Start overrides the virtual Start function of the base class.
		protected override void Start ()
		{
			beginSpawning = true;
//			StartCoroutine (spawnMinions(false));

//			spawnMinions();
			
			//Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
			//This allows the GameManager to issue movement commands.
			GameManager.instance.AddEnemyToList (this);
			
			//Get and store a reference to the attached Animator component.
			animator = GetComponent<Animator> ();
			
			if (spawned)
			{
				GameManager.instance.AddSpawnedEnemyToList(this);

				animator.SetBool("enemySpawn", true);
				spawnLocationObject.GetComponent<BoxCollider2D>().enabled = false;
			}

			//Find the Player GameObject using it's tag and store a reference to its transform component.
			target = GameObject.FindGameObjectWithTag ("Player").transform;
			
			//Call the start function of our base class MovingObject.
			base.Start ();
			
		}

		void FixedUpdate()
		{			

			if(canSpawnMinions && beginSpawning)
			{
				if (GameManager.instance.GetSpawnedEnemyCount() == 0)
				{
					Debug.Log("Spawning" + GameManager.instance.GetSpawnedEnemyCount());

					summonMinions();
					beginSpawning = false;
				}


			}

			if (GameManager.instance.GetSpawnedEnemyCount() > 0)
			{
				beginSpawning = true;
			}

		}

		private void summonMinions()
		{
			int randomIndex = Random.Range(0, 2);

			if (randomIndex == 0)
			{
				animator.SetTrigger("summonSkulls");
				StartCoroutine(spawnMinions(false));
				
			}
			else if (randomIndex == 1)
			{
				animator.SetTrigger("summonWisps");
				StartCoroutine(spawnMinions(false));			
			}

			
		}

		private IEnumerator spawnMinions(bool instantly = true)
		{
			if (!instantly)
			{
				yield return new WaitForSeconds(3.0f);
			}


			if (canSpawnMinions)
			{
				spawning = true;
				GameObject[] skullSpawnLocations = GameObject.FindGameObjectsWithTag("StoneGraveLocation");
				GameObject[] wispSpawnLocations = GameObject.FindGameObjectsWithTag("StoneBrazierLocation");

				foreach (GameObject skullSpawnLocation in skullSpawnLocations)
				{
					GameObject skullEnemy = Instantiate(enemySkullPrefab, skullSpawnLocation.transform.position, Quaternion.identity);
					
					skullEnemy.GetComponent<Enemy>().spawned = true;
					skullEnemy.GetComponent<Enemy>().spawnLocationObject = skullSpawnLocation;
				}
				
				foreach (GameObject wispSpawnLocation in wispSpawnLocations)
				{
					GameObject wispEnemy = Instantiate(enemyWispPrefab, wispSpawnLocation.transform.position, Quaternion.identity);
					wispEnemy.GetComponent<Enemy>().spawned = true;
					wispEnemy.GetComponent<Enemy>().spawnLocationObject = wispSpawnLocation;

				}

				beginSpawning = false;
				spawning = false;

			}

		}
		
		
		//Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
		//See comments in MovingObject for more on how base AttemptMove function works.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
			if (spawned)
			{
				animator.SetBool("enemySpawn", false);
			}

			//Check if skipMove is true, if so set it to false and skip this turn.
			if(skipMove)
			{
				skipMove = false;
				return;
				
			}
			
			//Call the AttemptMove function from MovingObject.
			base.AttemptMove <T> (xDir, yDir);

			if (spawned && !spawnLocationObject.GetComponent<BoxCollider2D>().enabled)
			{

				spawnLocationObject.GetComponent<BoxCollider2D>().enabled = true;
			}

			//Now that Enemy has moved, set skipMove to true to skip next move.
			skipMove = true;
		}

		public float getDistanceToTarget()
		{
			currentX = Mathf.RoundToInt(transform.position.x);
			currentY = Mathf.RoundToInt(transform.position.y);

			targetX = Mathf.RoundToInt(target.position.x);
			targetY = Mathf.RoundToInt(target.position.y);

			Vector2 currentPosition = new Vector2((float) currentX, (float) currentY); 
			Vector2 targetPosition = new Vector2((float) targetX, (float) targetY); 
			
	
			return Vector2.Distance( currentPosition, targetPosition );
//			return tempDistance;

		}
		
		//MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
		public void MoveEnemy ()
		{


/**			
			//Declare variables for X and Y axis move directions, these range from -1 to 1.
			//These values allow us to choose between the cardinal directions: up, down, left and right.
			int xDir = 0;
			int yDir = 0;
			
			//If the difference in positions is approximately zero (Epsilon) do the following:
			if(Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
				
				//If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
				yDir = target.position.y > transform.position.y ? 1 : -1;
			
			//If the difference in positions is not approximately zero (Epsilon) do the following:
			else
				//Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
				xDir = target.position.x > transform.position.x ? 1 : -1;
			
			//Call the AttemptMove function and pass in the generic parameter Player, because Enemy is moving and expecting to potentially encounter a Player
//			AttemptMove <Player> (xDir, yDir);
/**/
/**/
			//make sure the enemy and player are not in the same square
			//player / enemies can currently pass through

			AStarNode2D nextStep = null;

			GetComponent<BoxCollider2D>().enabled = false; // disabled for A star calculations
			target.GetComponent<BoxCollider2D>().enabled = false;

			float tempDistance = getDistanceToTarget();
			
//			if(spawned  && tempDistance < 5.0f)
//			{
//				return;
//			}


			if (tempDistance > float.Epsilon + 1.0f)
			{
				AStar astar = new AStar(new LineCastAStarCost(blockingLayer), currentX, currentY, Mathf.RoundToInt(target.position.x), Mathf.RoundToInt(target.position.y));
				astar.findPath();
				if (astar.solution.Count > 0)
				{
					nextStep = (AStarNode2D)astar.solution[1];
				}
			}


			GetComponent<BoxCollider2D>().enabled = true; //re-enabled post A star calculations
			target.GetComponent<BoxCollider2D>().enabled = true;

			int xDir = currentX;
			int yDir = currentY;

			if (nextStep != null)
			{
				xDir = nextStep.x - currentX;
				yDir = nextStep.y - currentY;
			}
			else
			{
				if (targetX == currentX && targetY != currentY)
				{
					xDir = 0;
					if (targetY > currentY)
					{
						yDir = 1;
					}
					else if(targetY < currentY)
					{
						yDir = -1;
					}
				}

				if (targetX != currentX && targetY == currentY)
				{
					yDir = 0;
					if (targetX > currentX)
					{
						xDir = 1;
					}
					else if(targetX < currentX)
					{
						xDir = -1;
					}
				}
			}

/**/

			if (yDir < 0 && !animator.GetBool("enemyForward"))
			{
				animator.SetBool("enemyLeft", false);
				animator.SetBool("enemyRight", false);
				animator.SetBool("enemyForward", true);
				animator.SetBool("enemyBackward", false);

			}

			if (yDir > 0 && !animator.GetBool("enemyBackward"))
			{
				animator.SetBool("enemyLeft", false);
				animator.SetBool("enemyRight", false);
				animator.SetBool("enemyForward", false);
				animator.SetBool("enemyBackward", true);
			}				

			if (xDir > 0 && !animator.GetBool("enemyRight"))
			{
				Vector3 theScale = transform.localScale;
				//theScale.x = -1;
				theScale.x = (int) -Mathf.Abs((float) theScale.x);
				transform.localScale = theScale;

				animator.SetBool("enemyLeft", false);
				animator.SetBool("enemyRight", true);
				animator.SetBool("enemyForward", false);
				animator.SetBool("enemyBackward", false);
			}

			if (xDir < 0 && !animator.GetBool("enemyLeft"))
			{
				Vector3 theScale = transform.localScale;
//				theScale.x = 1;
				theScale.x = (int) +Mathf.Abs((float) theScale.x);
				transform.localScale = theScale;

				animator.SetBool("enemyLeft", true);
				animator.SetBool("enemyRight", false);
				animator.SetBool("enemyForward", false);
				animator.SetBool("enemyBackward", false);

			}

/**/
			AttemptMove <Player> (xDir, yDir); // see above... and then that calls base
			
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
			
			//Call the PlaySfx2 function of SoundManager passing in the two audio clips to choose randomly between.
			SoundManager.instance.PlaySfx2 (attackSound1, attackSound2);
		}

				//LoseFood is called when an enemy attacks the player.
		//It takes a parameter loss which specifies how many points to lose.
		public void TakeDamage (int damage)
		{
			//Set the trigger for the player animator to transition to the playerHit animation.
//			animator.SetTrigger ("damage_trigger");
			
			//Subtract lost food points from the players total.
			enemyHealth -= damage;
			if (enemyHealth <= 0)
			{
				enemyHealth = 0;
				if (tag == "EnemyWightBoss")
				{
					
					SoundManager.instance.PlaySfx2 (deathSoundBoss);
				}
				else
				{
					SoundManager.instance.PlaySfx2 (deathSound1, deathSound2, deathSound3);
					
				}

				if (spawned)
				{
					GameManager.instance.RemoveSpawnedEnemyFromList(this);
				}

				GameManager.instance.RemoveEnemyFromList(this);

				gameObject.SetActive(false);
			}

			if (enemyHealth == 32)
			{
				enemyHealth = 33;
			}


			//Update the food display with the new total.
//			enemyHealth.text = enemyHealth + "%";

//			if (enemyHealth == 100)
//			{
//				GameManager.instance.foodImage.SetActive(true);
//				GameManager.instance.foodImage66.SetActive(false);
//				GameManager.instance.foodImage33.SetActive(false);
//				GameManager.instance.foodImage0.SetActive(false);
//			}
//			else if (enemyHealth > 66)
//			{
//				GameManager.instance.foodImage.SetActive(false);
//				GameManager.instance.foodImage66.SetActive(true);
//				GameManager.instance.foodImage33.SetActive(false);
//				GameManager.instance.foodImage0.SetActive(false);
				
//			}
//			else if (enemyHealth > 33)
//			{
//				GameManager.instance.foodImage.SetActive(false);
//				GameManager.instance.foodImage66.SetActive(false);
//				GameManager.instance.foodImage33.SetActive(true);
//				GameManager.instance.foodImage0.SetActive(false);
				
//			}
//			else if (enemyHealth > 0)
//			{
//				GameManager.instance.foodImage.SetActive(false);
//				GameManager.instance.foodImage66.SetActive(false);
//				GameManager.instance.foodImage33.SetActive(false);
//				GameManager.instance.foodImage0.SetActive(true);
				
//			}


		}




		
	}

	
}
