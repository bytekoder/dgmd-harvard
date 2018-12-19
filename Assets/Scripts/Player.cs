using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;

namespace Completed
{
	//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
	public class Player : MovingObject
	{
		public int enemyDamage; 							//The amount of damage points to subtract from the enemy when attacking.
		public float restartLevelDelay = 1f;		//Delay time in seconds to restart level.
		public int pointsPerFood = 10;				//Number of points to add to player food points when picking up a food object.
		public int pointsPerSoda = 20;				//Number of points to add to player food points when picking up a soda object.
		public Text foodText;						//UI Text to display current player food total.
		public Text goldKeyText;						//UI Text to display current player food total.
		public Text redKeyText;						//UI Text to display current player food total.
		public Text silverKeyText;						//UI Text to display current player food total.
		public AudioClip moveSound1;				//1 of 2 Audio clips to play when player moves.
		public AudioClip moveSound2;				//2 of 2 Audio clips to play when player moves.
		public AudioClip gameOverSound;				//Audio clip to play when player dies.
		public AudioClip gameOverWinSound;				//Audio clip to play when player wins.
		
		private Animator animator;					//Used to store a reference to the Player's animator component.
		private int food;                           //Used to store player food points total during level.
		private string foodTextLabel = "Health: ";    //label prefix for Food Text / player health.
		private bool moveAttack = false;

		private int goldKeyCount;                           //Used to store player gold key count total during level.
		private string goldKeyTextLabel= "Gold Keys: ";    //label prefix for gold key count.
		private int redKeyCount;                           //Used to store player red key count total during level.
		private string redKeyTextLabel= "Red Keys: ";    //label prefix for red key count.
		private int silverKeyCount;                           //Used to store player silver key count total during level.
		private string silverKeyTextLabel= "Silver Keys: ";    //label prefix for silver key count.

		private int horizontal;
		private int vertical;

	
		public AudioClip keyPickupSound;			//Audio clip to play when player collects a key object.
		public AudioClip chestOpenSound;			//Audio clip to play when player opens a chest object.
		public AudioClip attackSound;			//Audio clip to play when player attacks an enemy.

		// variable to hold a reference to our SpriteRenderer component
		private SpriteRenderer thisSpriteRenderer;

        // Four possible fireballs that can be shot
        public GameObject leftFireball, rightFireball, upFireball, downFireball;

        // Firing position. This will change depending on where the player is.
        private Transform firePos;

        // Direction of player facing
        private bool isFacingLeft, isFacingRight, isFacingUp, isFacingDown;


#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif
		
		
		//Start overrides the Start function of MovingObject
		protected override void Start ()
		{

			//Get a component reference to the Player's animator component
			animator = GetComponent<Animator>();
			
			//Get the current food point total stored in GameManager.instance between levels.
			food = GameManager.instance.playerFoodPoints;

			//get current key count values from the GameManager.instance between levels
			goldKeyCount = GameManager.instance.playerGoldKeyCount;
			redKeyCount = GameManager.instance.playerRedKeyCount;
			silverKeyCount = GameManager.instance.playerSilverKeyCount;

			//Set the foodText to reflect the current player food total.
			//not using text prefix.. using icon graphic instead
//			foodText.text = foodTextLabel + food;// foodTextGraph;

			foodText.text = food.ToString() + "%";
			goldKeyText.text = goldKeyCount.ToString();
			redKeyText.text = redKeyCount.ToString();
			silverKeyText.text = silverKeyCount.ToString();

			// get a reference to the SpriteRenderer component on this gameObject
			thisSpriteRenderer = GetComponent<SpriteRenderer>();

			//Call the Start function of the MovingObject base class.
			base.Start ();
			
			firePos = transform.Find("firePos");

		}
		
		
		//This function is called when the behaviour becomes disabled or inactive.
		private void OnDisable ()
		{
			//When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
			GameManager.instance.playerFoodPoints = food;
			GameManager.instance.playerGoldKeyCount = goldKeyCount;
			GameManager.instance.playerRedKeyCount = redKeyCount;
			GameManager.instance.playerSilverKeyCount = silverKeyCount;
			
		}
		
		
		private void Update ()
		{
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				isFacingLeft = true;
				isFacingRight = false;
				isFacingDown = false;
				isFacingUp = false;
			}

			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				isFacingLeft = false;
				isFacingRight = true;
				isFacingDown = false;
				isFacingUp = false;
			}

			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				isFacingLeft = false;
				isFacingRight = false;
				isFacingDown = false;
				isFacingUp = true;
			}

			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				isFacingLeft = false;
				isFacingRight = false;
				isFacingDown = true;
				isFacingUp = false;
			}


			if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1"))
			{
				Fire();
			}
			
			//If it's not the player's turn, exit the function.
			if(!GameManager.instance.playersTurn) return;
			
			horizontal = 0;  	//Used to store the horizontal move direction.
			vertical = 0;		//Used to store the vertical move direction.


			//Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBPLAYER

			//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
			horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
			
			//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
			vertical = (int) (Input.GetAxisRaw ("Vertical"));

			if (Input.GetButtonDown("Fire1"))
			{
				Attack();
				
			}
			
			//Check if moving horizontally, if so set vertical to zero.
			if(horizontal != 0)
			{
				vertical = 0;
			}
			//Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
			//Check if Input has registered more than zero touches
			if (Input.touchCount > 0)
			{
				//Store the first touch detected.
				Touch myTouch = Input.touches[0];
				
				//Check if the phase of that touch equals Began
				if (myTouch.phase == TouchPhase.Began)
				{
					//If so, set touchOrigin to the position of that touch
					touchOrigin = myTouch.position;
				}
				
				//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					//Set touchEnd to equal the position of this touch
					Vector2 touchEnd = myTouch.position;
					
					//Calculate the difference between the beginning and end of the touch on the x axis.
					float x = touchEnd.x - touchOrigin.x;
					
					//Calculate the difference between the beginning and end of the touch on the y axis.
					float y = touchEnd.y - touchOrigin.y;
					
					//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
					touchOrigin.x = -1;
					
					//Check if the difference along the x axis is greater than the difference along the y axis.
					if (Mathf.Abs(x) > Mathf.Abs(y))
						//If x is greater than zero, set horizontal to 1, otherwise set it to -1
						horizontal = x > 0 ? 1 : -1;
					else
						//If y is greater than zero, set horizontal to 1, otherwise set it to -1
						vertical = y > 0 ? 1 : -1;
				}
			}
			
#endif //End of mobile platform dependendent compilation section started above with #elif

			//Check if we have a non-zero value for horizontal or vertical
			if(horizontal != 0 || vertical != 0)
			{
				//Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
				//Pass in horizontal and vertical as parameters to specify the direction to move Player in.
//				AttemptMove<Wall> (horizontal, vertical);
				AttemptMove<Door> (horizontal, vertical);
				AttemptMove<Enemy> (horizontal, vertical);

			}
		}
		
		//AttemptMove overrides the AttemptMove function in the base class MovingObject
		//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{

			if (xDir > 0 && !animator.GetBool("right_walk"))
			{
				Vector3 theScale = transform.localScale;
				theScale.x = -1;
				transform.localScale = theScale;
//				thisSpriteRenderer.flipX = true;

				animator.SetBool("left_walk", false);
				animator.SetBool("right_walk", true);
				animator.SetBool("front_walk", false);
				animator.SetBool("up_walk", false);
				animator.SetBool("dead", false);
				
			}

			if (xDir < 0 && !animator.GetBool("left_walk"))
			{
				Vector3 theScale = transform.localScale;
				theScale.x = 1;
				transform.localScale = theScale;

				animator.SetBool("left_walk", true);
				animator.SetBool("right_walk", false);
				animator.SetBool("front_walk", false);
				animator.SetBool("up_walk", false);
				animator.SetBool("dead", false);

				
			}

			if (yDir < 0 && !animator.GetBool("front_walk"))
			{
				animator.SetBool("left_walk", false);
				animator.SetBool("right_walk", false);
				animator.SetBool("front_walk", true);
				animator.SetBool("up_walk", false);
				animator.SetBool("dead", false);

			}

			if (yDir > 0 && !animator.GetBool("up_walk"))
			{
				animator.SetBool("left_walk", false);
				animator.SetBool("right_walk", false);
				animator.SetBool("front_walk", false);
				animator.SetBool("up_walk", true);
				animator.SetBool("dead", false);

			}

			
			//Every time player moves, subtract from food points total.
			//ignored for Lurks
//			food--;
			

			foodText.text = food.ToString() + "%";
			goldKeyText.text = goldKeyCount.ToString();
			redKeyText.text = redKeyCount.ToString();
			silverKeyText.text = silverKeyCount.ToString();
			
			//Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
			base.AttemptMove <T> (xDir, yDir);
			
			//Hit allows us to reference the result of the Linecast done in Move.
			RaycastHit2D hit;
			
			//If Move returns true, meaning Player was able to move into an empty space.
			if (Move (xDir, yDir, out hit)) 
			{
				GameManager.instance.feedbackPanel.SetActive(false);
				GameManager.instance.needKeyText.SetActive(false);
				GameManager.instance.addKeyText.SetActive(false);
				GameManager.instance.subtractKeyText.SetActive(false);
				GameManager.instance.needGoldKeyImage.SetActive(false);
				GameManager.instance.needSilverKeyImage.SetActive(false);
				GameManager.instance.needRedKeyImage.SetActive(false);


					//Call PlaySfx1 of SoundManager to play the move sound, passing in two audio clips to choose from.
					SoundManager.instance.PlaySfx1 (0.1f, moveSound1, moveSound2);

			}
			else
			{
//				if (Input.GetButtonDown("Fire1"))
//				{
//					if (hit.transform.GetComponent<Enemy>())
//					{
//						Attack(hit.transform.GetComponent<Enemy>());
//					}
//					else
//					{
//						Attack();
//					}
//				}
			}
			
			//Since the player has moved and lost food points, check if the game has ended.
			CheckIfGameOver ();
			
			//Set the playersTurn boolean of GameManager to false now that players turn is over.
			GameManager.instance.playersTurn = false;
		}
		

		//OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject 
		//and takes a Enemy parameter which we use to pass in the component we expect to encounter, in this case Player
//		private void AttackEnemy(GameObject enemy)
//		{
//			if (enemy.tag == "EnemyWight")
//			{
//				return;
//			}


			
			
			//Call the PlaySfx2 function of SoundManager passing in the two audio clips to choose randomly between.
//			SoundManager.instance.PlaySfx2 (attackSound);

			//Set the attack trigger of animator to trigger attack animation.
//			animator.SetTrigger("attack_trigger");
			
			
//		}


		private void Attack()
		{
			Collider2D[] colliders = Physics2D.OverlapCircleAll (transform.position, 1.0f);

//			Collider2D closestCollider = new Collider2D(); 

			for (int i = 0; i < colliders.Length; i++)
			{

				if(colliders[i].GetComponent<BoxCollider2D>() == transform.GetComponent<BoxCollider2D>())
				{
					continue;
				}


				if (colliders[i].transform.tag == "Enemy" || colliders[i].transform.tag == "EnemyMinions" || colliders[i].transform.tag == "EnemyWightBoss")
				{
					//Call the TakeDamage function of enemy passing it enemyDamage.
					colliders[i].GetComponent<Enemy>().TakeDamage(enemyDamage);
					continue;
				}

			}
			


			//Call the PlaySfx2 function of SoundManager passing in the two audio clips to choose randomly between.
			SoundManager.instance.PlaySfx2 (attackSound);

			//Set the attack trigger of animator to trigger attack animation.
			animator.SetTrigger("attack_trigger");

		}

		//OnCantMove overrides the abstract function OnCantMove in MovingObject.
		//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
		protected override void OnCantMove<T>(T component)
		{
			if (component is Enemy)
			{
				moveAttack = true;

				//Declare enemy and set it to equal the encountered component.
				Enemy enemy = component as Enemy;
//				AttackEnemy(enemy);

				moveAttack = false;

			}
			else if (component is Door)
			{
				//Declare door and set it to equal the encountered component.
				Door door = component as Door;
				OpenDoor(door);
			}

		}
		
		private void OpenDoor(Door door)
		{
		//Set hitWall to equal the component passed in as a parameter.

			//Check if the tag of the trigger collided with a redKey.
			if(redKeyCount > 0 && door.tag == "WoodDoor")
			{
				//Add one to gold key count.
				redKeyCount--;
				
				//Update goldKeyText to represent current total and notify player that they gained points
				redKeyText.text = redKeyCount.ToString();

				if (!door.doorOpen)
				{
					GameManager.instance.feedbackPanel.SetActive(true);
					GameManager.instance.subtractKeyText.SetActive(true);
					GameManager.instance.needRedKeyImage.SetActive(true);
					
				}

				
				//Check if the tag of the trigger collided with a RedKey.
				door.OpenDoor("RedKey");
				
			}
			else if(redKeyCount <= 0 && door.tag == "WoodDoor")
			{
				// display text you need a gold key
				//Get a reference to our image LevelImage by finding it by name.
				if (!door.doorOpen)
				{
					GameManager.instance.feedbackPanel.SetActive(true);
					GameManager.instance.needKeyText.SetActive(true);
					GameManager.instance.needRedKeyImage.SetActive(true);
				}

			}

			if (goldKeyCount > 0 && door.tag == "Exit")
			{
				
				//Add one to gold key count.
				goldKeyCount--;
				
				//Update goldKeyText to represent current total and notify player that they gained points
				goldKeyText.text = goldKeyCount.ToString();
				
				//Check if the tag of the trigger collided with a GoldKey.
				door.OpenDoor("GoldKey");

				//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
				Invoke ("NextLevel", restartLevelDelay);
				
				//Disable the player object since level is over.
				enabled = false;

			}
			else if(goldKeyCount <= 0 && door.tag == "Exit")
			{
				if (!door.doorLocked)
				{
					
					door.OpenDoor();
						//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
						Invoke ("NextLevel", restartLevelDelay);

						//Disable the player object since level is over.
						enabled = false;

				}

				// display text you need a gold key
				//Get a reference to our image LevelImage by finding it by name.
				if (!door.doorOpen)
				{
					GameManager.instance.feedbackPanel.SetActive(true);
					GameManager.instance.needKeyText.SetActive(true);
					GameManager.instance.needGoldKeyImage.SetActive(true);
				}

			}

			//Check if the tag of the trigger collided with a redKey.
			if(silverKeyCount > 0 && door.tag == "SilverChest" && !door.doorOpen)
			{
				//Add one to gold key count.
				silverKeyCount--;
				goldKeyCount++;

				GameManager.instance.feedbackPanel.SetActive(true);
				GameManager.instance.addKeyText.SetActive(true);
				GameManager.instance.needGoldKeyImage.SetActive(true);

				//Update goldKeyText to represent current total and notify player that they gained points
				silverKeyText.text = silverKeyCount.ToString();
				goldKeyText.text = goldKeyCount.ToString();
				
				//Check if the tag of the trigger collided with a RedKey.
				door.OpenDoor("SilverKey");
				door.gameObject.layer = 8;

			}
			else if(silverKeyCount <= 0 && door.tag == "SilverChest")
			{
				// display text you need a gold key
				//Get a reference to our image LevelImage by finding it by name.
				if (!door.doorOpen)
				{
					GameManager.instance.feedbackPanel.SetActive(true);
					GameManager.instance.needKeyText.SetActive(true);
					GameManager.instance.needSilverKeyImage.SetActive(true);
//					animator.SetTrigger ("gold_key");
					animator.SetBool("gold_key", true);

				}

			}

			
		}
		
		
		//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
		private void OnTriggerExit2D(Collider2D other)
		{
//			animator.SetBool("gold_key", false);
//			animator.SetBool("silver_key", false);
//			animator.SetBool("bronze_key", false);

//			animator.ResetTrigger("gold_key_trigger");
//			animator.ResetTrigger("silver_key_trigger");
//			animator.ResetTrigger("bronze_key_trigger");
		}

		//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
		private void OnTriggerEnter2D (Collider2D other)
		{

			//Check if the tag of the trigger collided with a GoldKey.
			if(other.tag == "GoldKey")
			{
				//Add one to gold key count.
				goldKeyCount++;
				
				GameManager.instance.feedbackPanel.SetActive(true);
				GameManager.instance.addKeyText.SetActive(true);
				GameManager.instance.needGoldKeyImage.SetActive(true);

				//Update goldKeyText to represent current total and notify player that they gained points
				goldKeyText.text = goldKeyCount.ToString();
				
				//Call the PlaySfx2 function of SoundManager and pass in two eating sounds to choose between to play the key pickup sound effect.
				SoundManager.instance.PlaySfx2 (keyPickupSound);
//				animator.SetTrigger ("gold_key");
//				animator.SetBool("gold_key", true);
				
				//Disable the goldKey object the player collided with.
				other.gameObject.SetActive (false);
			}

			//Check if the tag of the trigger collided with a RedKey.
			else if(other.tag == "RedKey")
			{
				//Add one to red key count.
				redKeyCount++;

				GameManager.instance.feedbackPanel.SetActive(true);
				GameManager.instance.addKeyText.SetActive(true);
				GameManager.instance.needRedKeyImage.SetActive(true);

				//Update redKeyText to represent current total and notify player that they gained points
				redKeyText.text = redKeyCount.ToString();
				
				//Call the PlaySfx2 function of SoundManager and pass in two eating sounds to choose between to play the key pickup sound effect.
				SoundManager.instance.PlaySfx2 (keyPickupSound);
//				animator.SetTrigger ("bronze_key");
//				animator.SetBool("bronze_key", true);
				
				//Disable the redKey object the player collided with.
				other.gameObject.SetActive (false);
			}

			//Check if the tag of the trigger collided with a SilverKey.
			else if(other.tag == "SilverKey")
			{
				//Add one to silver key count.
				silverKeyCount++;

				GameManager.instance.feedbackPanel.SetActive(true);
				GameManager.instance.addKeyText.SetActive(true);
				GameManager.instance.needSilverKeyImage.SetActive(true);

				//Update silverKeyText to represent current total and notify player that they gained points
				silverKeyText.text = silverKeyCount.ToString();
				
				//Call the PlaySfx2 function of SoundManager and pass in two eating sounds to choose between to play the key pickup sound effect.
				SoundManager.instance.PlaySfx2 (keyPickupSound);
				animator.SetTrigger ("silver_key");
//				animator.SetBool("silver_key", true);
				
				//Disable the silverKey object the player collided with.
				other.gameObject.SetActive (false);
			}

			SoundManager.instance.MuteSfx1(false);
			
		}
		
		
		//Restart reloads the scene when called.
		private void Restart ()
		{
			//Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
            //and not load all the scene object in the current scene.
			/// USE HERE to load different levels
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
		}

		//NextLevel loads the next scene when called.
		private void NextLevel()
		{
			//Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
			//and not load all the scene object in the current scene.
			/// USE HERE to load different levels
			int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
			SceneManager.LoadScene(nextScene, LoadSceneMode.Single);

//			int nextScene = GameManager.instance.level + 1;
//			int nextSceneLabel = GameManager.instance.level;
//			string levelName = "Level " + (nextSceneLabel);
//			if (nextScene > 4)
//			{
//				levelName = "Boss Room";
//			}

//			SceneManager.LoadScene(levelName, LoadSceneMode.Single);
		}

		
		//LoseFood is called when an enemy attacks the player.
		//It takes a parameter loss which specifies how many points to lose.
		public void LoseFood (int loss)
		{
			//Set the trigger for the player animator to transition to the playerHit animation.
			animator.SetTrigger ("damage_trigger");
			
			//Subtract lost food points from the players total.
			food -= loss;
			if (food <= 0)
			{
				food = 0;
			}

			if (food == 32)
			{
				food = 33;
			}


			//Update the food display with the new total.
			foodText.text = food + "%";

			if (food <= 0)
			{
				GameManager.instance.foodImage.SetActive(false);
				GameManager.instance.foodImage66.SetActive(false);
				GameManager.instance.foodImage33.SetActive(false);
				GameManager.instance.foodImage0.SetActive(true);
				
			}
			else if (food <= 33)
			{
				GameManager.instance.foodImage.SetActive(false);
				GameManager.instance.foodImage66.SetActive(false);
				GameManager.instance.foodImage33.SetActive(true);
				GameManager.instance.foodImage0.SetActive(false);
				
			}
			else if (food <= 66)
			{
				GameManager.instance.foodImage.SetActive(false);
				GameManager.instance.foodImage66.SetActive(true);
				GameManager.instance.foodImage33.SetActive(false);
				GameManager.instance.foodImage0.SetActive(false);
				
			}
			else if (food <= 100)
			{
				GameManager.instance.foodImage.SetActive(true);
				GameManager.instance.foodImage66.SetActive(false);
				GameManager.instance.foodImage33.SetActive(false);
				GameManager.instance.foodImage0.SetActive(false);
			}



			//Check to see if game has ended.
			CheckIfGameOver ();
		}
		
		
		//CheckIfGameOver checks if the player is out of food points and if so, ends the game.
		private void CheckIfGameOver ()
		{
			//Check if food point total is less than or equal to zero.
			if (food <= 0) 
			{
				animator.SetBool("left_walk", false);
				animator.SetBool("right_walk", false);
				animator.SetBool("front_walk", false);
				animator.SetBool("up_walk", false);
				animator.SetBool("dead", true);

                //Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
                SoundManager.instance.PlaySingle(gameOverSound);

                //Stop the background music.
                SoundManager.instance.musicSource.Stop();

                //Call the GameOver function of GameManager.
                GameManager.instance.GameOver();
            }
			else if (SceneManager.GetActiveScene().name == "The BOSS!" && GameManager.instance.GetEnemyCount() < 1)
			{
				//Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
				SoundManager.instance.PlaySingle (gameOverWinSound);
				
				//Stop the background music.
				SoundManager.instance.musicSource.Stop();
				
				//Call the GameOver function of GameManager.
				GameManager.instance.GameOverWin ();

			}
        }

        /// <summary>
        /// Fires the fireball 
        /// </summary>
        void Fire()
        {
            if (isFacingRight)
            {
                Debug.Log("Right facing...");
                Instantiate(rightFireball, firePos.position, Quaternion.identity);
            }

            if (isFacingLeft)
            {
                Debug.Log("Left facing...");
                Instantiate(leftFireball, firePos.position, Quaternion.identity);
            }

            if (isFacingUp)
            {
                Debug.Log("Up facing...");
                Instantiate(upFireball, firePos.position, Quaternion.identity);
            }

            if (isFacingDown)
            {
                Debug.Log("Down facing...");
                Instantiate(downFireball, firePos.position, Quaternion.identity);
            }
        }
    }
}

