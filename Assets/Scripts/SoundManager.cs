/**/
using UnityEngine;
using System.Collections;

namespace Completed
{
	public class SoundManager : MonoBehaviour 
	{
		public AudioSource sfxSource1;					//Drag a reference to the audio source which will play the sound effects.
		public AudioSource sfxSource2;					//Drag a reference to the audio source which will play the sound effects.
		public AudioSource musicSource;					//Drag a reference to the audio source which will play the music.
		public static SoundManager instance = null;		//Allows other scripts to call functions from SoundManager.				
		public float lowPitchRange = .95f;				//The lowest a sound effect will be randomly pitched.
		public float highPitchRange = 1.05f;			//The highest a sound effect will be randomly pitched.
		
		
		void Awake ()
		{
			//Check if there is already an instance of SoundManager
			if (instance == null)
				//if not, set it to this.
				instance = this;
			//If instance already exists:
			else if (instance != this)
				//Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
				Destroy (gameObject);
			
			//Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
			DontDestroyOnLoad (gameObject);
		}
		
		
		//Used to play single sound clips.
		public void PlaySingle(AudioClip clip)
		{
			//Set the clip of our efxSource audio source to the clip passed in as a parameter.
			sfxSource1.clip = clip;
			
			//Play the clip.
			sfxSource1.Play ();
		}

		public void MuteSfx1(bool mute)
		{
			sfxSource1.mute = mute;
		}

		public void MuteSfx2(bool mute)
		{
			sfxSource1.mute = mute;
		}

		//RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
		public void PlaySfx2(params AudioClip[] clips)
		{
			PlaySfx1(1.0f, clips);
		}
		

		//RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
		public void PlaySfx2 (float volume, params AudioClip[] clips)
		{
			//Generate a random number between 0 and the length of our array of clips passed in.
			int randomIndex = Random.Range(0, clips.Length);
			
			//Choose a random pitch to play back our clip at between our high and low pitch ranges.
			float randomPitch = Random.Range(lowPitchRange, highPitchRange);

			sfxSource1.volume = 1.0f;
			if (volume != 1.0f)
			{
				sfxSource2.volume = volume;
			}

			//Set the clip to the clip at our randomly chosen index.
			AudioClip clip = clips[randomIndex];

			//Play the clip.
			sfxSource2.PlayOneShot(clip, volume);
		}

		//RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
		public void PlaySfx1(params AudioClip[] clips)
		{
			PlaySfx1(1.0f, clips);
		}
		
		//RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
		public void PlaySfx1 (float volume, params AudioClip[] clips)
		{
			//Generate a random number between 0 and the length of our array of clips passed in.
			int randomIndex = Random.Range(0, clips.Length);
			
			//Choose a random pitch to play back our clip at between our high and low pitch ranges.
			float randomPitch = Random.Range(lowPitchRange, highPitchRange);

			sfxSource1.volume = 1.0f;
			if (volume != 1.0f)
			{
				sfxSource1.volume = volume;
			}
			
			//Set the clip to the clip at our randomly chosen index.
			AudioClip clip = clips[randomIndex];

			//Play the clip.
			sfxSource2.PlayOneShot(clip, volume);
		}
	}
}
/**
using UnityEngine;
using System.Collections;

namespace Completed
{
	public class SoundManager : MonoBehaviour 
	{
		public AudioSource efxSource;					//Drag a reference to the audio source which will play the sound effects.
		public AudioSource musicSource;					//Drag a reference to the audio source which will play the music.
		public AudioSource musicSourceLoop;				//Drag the Music that will Loop in the Level
		public static SoundManager instance = null;		//Allows other scripts to call functions from SoundManager.				
		public float lowPitchRange = .95f;				//The lowest a sound effect will be randomly pitched.
		public float highPitchRange = 1.05f;			//The highest a sound effect will be randomly pitched.
		private bool startedLoop;
		public float introTime = 0;
		public float loopTime = 0;
		
		
		void Awake ()
		{
			//Check if there is already an instance of SoundManager
			if (instance == null)
				//if not, set it to this.
				instance = this;
			//If instance already exists:
			else if (instance != this)
				//Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
				Destroy (gameObject);
			
			//Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
			DontDestroyOnLoad (gameObject);


			//Start the Intro Music
			musicSource.Play();
			//musicSource.time = 43; //DEBUG
			//PlayMusic();

		}

		//Method to play music with Intro and then Loop it
		public void PlayMusic(){
			//yield return new WaitForSeconds(musicSource.clip.length);
			musicSourceLoop.Play();

		}
		
		void FixedUpdate()
		{
//			if (!musicSource.isPlaying  && !startedLoop)
//			{
//				musicSourceLoop.Play();
//				Debug.Log("Done playing");
//				startedLoop = true;
//			}
			
			if (musicSource.time > introTime  && !startedLoop)
			{
				musicSourceLoop.Play();
				//musicSourceLoop.time = 115; //DEBUG
				Debug.Log("Done playing");
				startedLoop = true;
			}
			//if (startedLoop){Debug.Log(musicSourceLoop.time);}
		
			if (startedLoop && musicSourceLoop.time > loopTime)
			{
				musicSourceLoop.time = 0;
			}

		}

		
		//Used to play single sound clips.
		public void PlaySingle(AudioClip clip)
		{
			//Set the clip of our efxSource audio source to the clip passed in as a parameter.
			sfxSource1.clip = clip;
			
			//Play the clip.
			sfxSource1.Play ();
		}
		
		
		//RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
		public void RandomizeSfx (params AudioClip[] clips)
		{
			//Generate a random number between 0 and the length of our array of clips passed in.
			int randomIndex = Random.Range(0, clips.Length);
			
			//Choose a random pitch to play back our clip at between our high and low pitch ranges.
			float randomPitch = Random.Range(lowPitchRange, highPitchRange);
			
			//Set the pitch of the audio source to the randomly chosen pitch.
			sfxSource1.pitch = randomPitch;
			
			//Set the clip to the clip at our randomly chosen index.
			sfxSource1.clip = clips[randomIndex];
			
			//Play the clip.
			sfxSource1.Play();
		}
	}
}
/**/