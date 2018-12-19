using UnityEngine;
using System.Collections;

namespace Completed
{
	public class Door : MonoBehaviour
	{
		public AudioClip openDoorSound;					//audio clip that plays when the door is opened by the player.
		public Sprite openedDoorSprite;					//Alternate sprite to display after door has been opened by player.
		public GameObject doorLeftGameObject;			//GameObject to Left of Door -- will change on open.
		public GameObject doorRightGameObject;			//GameObject to Right of Door -- will change on open.
		public Sprite openedDoorSpriteLeft;				//Alternate sprite to left of door after it has been opened by player.
		public Sprite openedDoorSpriteRight;			//Alternate sprite to right of door after it has been opened by player.
		public bool doorLocked = false;					//Boolean if door is locked and requires key
		public string keyTag;						//The Tag value of key required to open the door ("GoldKey", "RedKey", "SilverKey");
		
		
		[HideInInspector] public bool doorOpen = false;					//Boolean if door is currently open
		private SpriteRenderer spriteRenderer;		//Store a component reference to the attached SpriteRenderer.


		void Awake ()
		{
			//Get a component reference to the SpriteRenderer.
			spriteRenderer = GetComponent<SpriteRenderer> ();
		}

		public void OpenDoor(string passedKeyTag = null)
		{

			if (doorOpen)
			{
				return;
			}

			if (doorLocked)
			{
				
				Debug.Log("door locked");
				if (keyTag != passedKeyTag)
				{
					return;
				}
			}


			//Call the PlaySfx2 function of SoundManager to play openDoorSound;
			SoundManager.instance.PlaySfx2 (openDoorSound);

			gameObject.layer = 0;


			//Set spriteRenderer to the left and right objects around the door.
			spriteRenderer.sprite = openedDoorSprite;

			if (doorLeftGameObject != null)
			{
				doorLeftGameObject.GetComponent<SpriteRenderer> ().sprite = openedDoorSpriteLeft;
			}

			if (doorRightGameObject != null)
			{
				doorRightGameObject.GetComponent<SpriteRenderer> ().sprite = openedDoorSpriteRight;

			}
			doorLocked = false;
			doorOpen = true;
		}

	}
}
