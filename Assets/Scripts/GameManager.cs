using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Completed
{
	using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;					//Allows us to use UI.
	
	public class GameManager : MonoBehaviour
	{
		public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
		public float turnDelay = 0.1f;							//Delay between each Player turn.
		public int playerFoodPoints = 100;						//Starting value for Player food points.
		public int playerGoldKeyCount = 0;						//Starting value for Player gold key count.
		public int playerRedKeyCount = 0;						//Starting value for Player red key count.
		public int playerSilverKeyCount = 0;					//Starting value for Player silver key count.
		public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
		[HideInInspector] public bool playersTurn = true;		//Boolean to check if it's players turn, hidden in inspector but public.
		
		
		private Text levelText;									//Text to display current level number.
		private GameObject levelImage;							//Image to block out level as levels are being set up, background for levelText.
		private BoardManager boardScript;						//Store a reference to our BoardManager which will set up the level.
		[HideInInspector] public int level = 1;				//Current level number, expressed in game as "Day 1".

		[HideInInspector] public GameObject gameOverPanel;
		[HideInInspector] public GameObject gameOverWinPanel;

		
		[HideInInspector] public GameObject feedbackPanel;
		[HideInInspector] public GameObject needKeyText;
		[HideInInspector] public GameObject addKeyText;
		[HideInInspector] public GameObject subtractKeyText;
		[HideInInspector] public GameObject needGoldKeyImage;
		[HideInInspector] public GameObject needRedKeyImage;
		[HideInInspector] public GameObject needSilverKeyImage;
		[HideInInspector] public GameObject foodImage;
		[HideInInspector] public GameObject foodImage66;
		[HideInInspector] public GameObject foodImage33;
		[HideInInspector] public GameObject foodImage0;

		private List<Enemy> enemies;							//List of all Enemy units, used to issue them move commands.
		private List<Enemy> spawnedEnemies;							//List of all Enemy units, used to issue them move commands.
		private bool enemiesMoving;								//Boolean to check if enemies are moving.
		private bool doingSetup = true;							//Boolean to check if we're setting up board, prevent Player from moving during setup.
		
//		private List<PlayerFollower> playerFollowers;					//List of all Player Follower units, used to issue them move commands.
//		private GameObject[] playerFollowers;					//List of all Player Follower units, used to issue them move commands.
//		private bool playerFollowersMoving;								//Boolean to check if Player Followers are moving.
		
		
		//Awake is always called before any Start functions
		void Awake()
		{

            //Check if instance already exists
            if (instance == null)

                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);	

			Debug.Log("Current Level: " + instance.level);

			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);
			
			//Assign enemies to a new List of Enemy objects.
			enemies = new List<Enemy>();
			spawnedEnemies = new List<Enemy>();
			
			//Assign playerFollowers to a new List of player follower objects.
//			playerFollowers = new List<PlayerFollower>(); //GameObject.FindGameObjectsWithTag("Follower"); //new List<PlayerFollower>();
//			playerFollowers = GameObject.FindGameObjectsWithTag("Follower"); //new List<PlayerFollower>();
//			DontDestroyOnLoad(playerFollowers);

			
			//Get a component reference to the attached BoardManager script
			boardScript = GetComponent<BoardManager>();
			
			//Call the InitGame function to initialize the first level 
			InitGame();
		}

        //this is called only once, and the parameter tell it to be called only after the scene was loaded
        //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            //register the callback to be called everytime the scene is loaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        //This is called each time a scene is loaded.
        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
//            instance.level++;
            instance.InitGame();
        }

		
		//Initializes the game for each level.
		void InitGame()
		{
			//While doingSetup is true the player can't move, prevent player from moving while title card is up.
			doingSetup = true;

			// display text you need a gold key
			//Get a reference to our image LevelImage by finding it by name.
			feedbackPanel = GameObject.Find("FeedbackPanel");
			gameOverPanel = GameObject.Find("GameOverPanel");
			gameOverWinPanel = GameObject.Find("GameOverWinPanel");
			
			//Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
			needKeyText = GameObject.Find("NeedKeyText");
			addKeyText = GameObject.Find("AddKeyText");
			subtractKeyText = GameObject.Find("SubtractKeyText");
			needGoldKeyImage = GameObject.Find("NeedGoldKeyImage");
			needRedKeyImage = GameObject.Find("NeedRedKeyImage");
			needSilverKeyImage = GameObject.Find("NeedSilverKeyImage");
			
			foodImage = GameObject.Find("FoodImage");
			foodImage66 = GameObject.Find("FoodImage-66");
			foodImage33 = GameObject.Find("FoodImage-33");
			foodImage0 = GameObject.Find("FoodImage-0");

			foodImage.SetActive(true);
			foodImage66.SetActive(false);
			foodImage33.SetActive(false);
			foodImage0.SetActive(false);


			//Set levelImage to active blocking player's view of the game board during setup.
			needKeyText.SetActive(false);
			addKeyText.SetActive(false);
			subtractKeyText.SetActive(false);
			needGoldKeyImage.SetActive(false);
			needRedKeyImage.SetActive(false);
			needSilverKeyImage.SetActive(false);
			feedbackPanel.SetActive(false);
			gameOverPanel.SetActive(false);
			gameOverWinPanel.SetActive(false);

			//Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
			levelText = GameObject.Find("LevelText").GetComponent<Text>();
			
			//Set the text of levelText to the string "Day" and append the current level number.
			levelText.text = SceneManager.GetActiveScene().name;

			//Get a reference to our image LevelImage by finding it by name.
			levelImage = GameObject.Find("LevelImage");
			
			
			//Set levelImage to active blocking player's view of the game board during setup.
			levelImage.SetActive(true);
			
			//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
			Invoke("HideLevelImage", levelStartDelay);
			
			//Clear any Enemy objects in our List to prepare for next level.
			enemies.Clear();
			
			//Clear any Player Follower objects in our List to prepare for next level.
//			playerFollowers.Clear();

			//Call the SetupScene function of the BoardManager script, pass it current level number.
			boardScript.SetupScene(level);
			
		}
		
		
		//Hides black image used between levels
		void HideLevelImage()
		{
			//Disable the levelImage gameObject.
			levelImage.SetActive(false);
			
			//Set doingSetup to false allowing player to move again.
			doingSetup = false;
		}
		
		//Update is called every frame.
		void Update()
		{
			//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
			if(playersTurn || enemiesMoving || doingSetup)
//			if(doingSetup)
				//If any of these are true, return and do not start MoveEnemies.
				return;
			
			//Start moving enemies.
			StartCoroutine (MoveEnemies ());

			//Start moving player followers.
//			for (int i = 0; i < playerFollowers.Length; i++)
//			{
//				playerFollowers[i].transform.GetComponent<PlayerFollower>().MovePlayerFollower ();
//			}
			
//			StartCoroutine (MovePlayerFollowers ());

		}
		
		//Call this to add the passed in Enemy to the List of Enemy objects.
		public void AddEnemyToList(Enemy script)
		{
			//Add Enemy to List enemies.
			enemies.Add(script);
		}

		//Call this to remove the passed in Enemy from the List of Enemy objects.
		public void RemoveEnemyFromList(Enemy script)
		{
			//Add Enemy to List enemies.
			enemies.Remove(script);
		}

		//Call this to add the passed in Enemy to the List of Enemy objects.
		public void AddSpawnedEnemyToList(Enemy script)
		{
			//Add Enemy to List enemies.
			spawnedEnemies.Add(script);
		}

		//Call this to remove the passed in Enemy from the List of Enemy objects.
		public void RemoveSpawnedEnemyFromList(Enemy script)
		{
			//Add Enemy to List enemies.
			spawnedEnemies.Remove(script);
		}

		public int GetSpawnedEnemyCount()
		{
			return spawnedEnemies.Count;
		}

		public int GetEnemyCount()
		{
			return enemies.Count;
		}


		//Call this to add the passed in Enemy to the List of Enemy objects.
//		public void AddPlayerFollowerToList(PlayerFollower script)
//		{
			//Add Player Follower to List playerFollowers.
//			playerFollowers.Add(script);
//		}

		//GameOver is called when the player reaches 0 food points
		public void GameOver()
		{
			gameOverPanel.SetActive(true);

			//Set levelText to display number of levels passed and game over message
//			levelText.text = "Gave Over... The monster got you...";// "After " + level + " days, you starved.";
			
			//Enable black background image gameObject.
			//levelImage.SetActive(true);
			
			//Disable this GameManager.
			enabled = false;
		}

		//GameOver is called when the player reaches 0 food points
		public void GameOverWin()
		{
			gameOverWinPanel.SetActive(true);

			//Set levelText to display number of levels passed and game over message
//			levelText.text = "Gave Over... The monster got you...";// "After " + level + " days, you starved.";
			
			//Enable black background image gameObject.
			//levelImage.SetActive(true);
			
			//Disable this GameManager.
			enabled = false;
		}

		//Coroutine to move enemies in sequence.
		IEnumerator MoveEnemies()
		{
			//While enemiesMoving is true player is unable to move.
			enemiesMoving = true;
			
			//Wait for turnDelay seconds, defaults to .1 (100 ms).
			yield return new WaitForSeconds(turnDelay);
			
			//If there are no enemies spawned (IE in first level):
			if (enemies.Count == 0) 
			{
				//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
				yield return new WaitForSeconds(turnDelay);
			}
			
			//Loop through List of Enemy objects.
			for (int i = 0; i < enemies.Count; i++)
			{
				if(enemies[i].spawned == false || enemies[i].getDistanceToTarget() < 5.0f)
				{
					//Call the MoveEnemy function of Enemy at index i in the enemies List.
					enemies[i].MoveEnemy ();
				
					//Wait for Enemy's moveTime before moving next Enemy, 
					yield return new WaitForSeconds(enemies[i].moveTime);
					
				}
			}
			//Once Enemies are done moving, set playersTurn to true so player can move.
			playersTurn = true;
			
			//Enemies are done moving, set enemiesMoving to false.
			enemiesMoving = false;
		}
		
		//Coroutine to move playerFollowers in sequence.
//		IEnumerator MovePlayerFollowers()
//		{
			//While enemiesMoving is true player is unable to move.
//			enemiesMoving = true;
			
			//Wait for turnDelay seconds, defaults to .1 (100 ms).
//			yield return new WaitForSeconds(turnDelay);
			
			//If there are no enemies spawned (IE in first level):
//			if (enemies.Count == 0) 
//			{
				//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
//				yield return new WaitForSeconds(turnDelay);
//			}
			
			//Loop through List of Enemy objects.
//			for (int i = 0; i < playerFollowers.Count; i++)
//			{
				//Call the MoveEnemy function of Enemy at index i in the enemies List.
//				playerFollowers[i].MovePlayerFollower();
				
				//Wait for Enemy's moveTime before moving next Enemy, 
//				yield return new WaitForSeconds(enemies[i].moveTime);
//			}
			//Once Enemies are done moving, set playersTurn to true so player can move.
//			playersTurn = true;
			
			//Enemies are done moving, set enemiesMoving to false.
//			enemiesMoving = false;
			
//			yield return new WaitForSeconds(turnDelay);

//		}

	}
}

