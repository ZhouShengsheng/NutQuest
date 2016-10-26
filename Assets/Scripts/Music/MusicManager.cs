using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {
	
	private GameObject musicManager;
	private AudioSource audioSource;

	public AudioClip bgmClip;

	public static MusicManager Instance { get; private set; }

	// For singleton.
	void Awake() {
		//When the scene loads it checks if there is an object called "MUSIC".
		musicManager = GameObject.Find("MUSIC");
		if(musicManager == null) {
			//If this object does not exist then it does the following:
			//1. Sets the object this script is attached to as the music player
			musicManager = this.gameObject;
			//2. Renames THIS object to "MUSIC" for next time
			musicManager.name = "MUSIC";
			//3. Tells THIS object not to die when changing scenes.
			DontDestroyOnLoad(musicManager);
			Instance = this;
		} else {
			if(this.gameObject.name != "MUSIC") {
				//If there WAS an object in the scene called "MUSIC" (because we have come back to
				//the scene where the music was started) then it just tells this object to 
				//destroy itself if this is not the original
				Destroy(this.gameObject);
			}
		}
	}

	// Use this for initialization
	void Start () {
		audioSource = musicManager.GetComponent<AudioSource> ();
	}

	public void changeSpeed(float speed) {
		audioSource.pitch = speed;
	}

	public void playClip(AudioClip clip) {
		audioSource.clip = clip;
		audioSource.Play ();
	}

	public void playBgm() {
		audioSource.clip = bgmClip;
		audioSource.Play ();
	}

	public void pause() {
		audioSource.Pause ();
	}

	public void resume() {
		audioSource.Play ();
	}
}
