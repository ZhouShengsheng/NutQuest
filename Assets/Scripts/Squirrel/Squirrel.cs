using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/**
 * Script to controll squirrel.
 * 
 */
public class Squirrel : MonoBehaviour {

	// Movements.
	public float forwardSpeed = 3.0f;
	public float upDownSpeed = 3.0f;
	private bool upDown = true;	// true: up, false: down

	// Nuts.
	private uint nutsColleted = 0;
	public Text nutsColletedLabel;
	public Texture2D nutIconTexture;
	public AudioClip nutCollectSound;

	// Animation.
	Animator squirrelAnimator;

	// Controller.
	public GameObject restartDialog;


	// Use this for initialization
	void Start () {
		squirrelAnimator = GetComponent<Animator>();
		restartDialog.SetActive (false);

		float speed = upDown ? upDownSpeed : -upDownSpeed;
		GetComponent<Rigidbody2D> ().velocity = new Vector2 (forwardSpeed, speed);
	}

	void Update () {
		// Get is tapped.
		bool tapped = Input.GetButtonUp("Fire1");

		if (tapped) {
			upDown = !upDown;
			float speed = upDown ? upDownSpeed : -upDownSpeed;
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (forwardSpeed, speed);
		}
	} 

	// Collision detection method (for unity 2D).
	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.CompareTag ("Coins")) {
			CollectNut(collider);
		}
	}

	void CollectNut(Collider2D nutCollider) {
		nutsColleted++;

		Destroy(nutCollider.gameObject);

		AudioSource.PlayClipAtPoint(nutCollectSound, transform.position);

		nutsColletedLabel.text = nutsColleted.ToString ();
	}

	public void RestartGame() {
		//Application.LoadLevel (Application.loadedLevelName);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public void ExitToMenu() {
		//Application.LoadLevel ("MenuScene");
		SceneManager.LoadScene ("LevelScene");
	}
}
