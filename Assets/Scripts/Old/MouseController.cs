using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MouseController : MonoBehaviour {

	public float jetpackForce = 75.0f;

	public float forwardMovementSpeed = 3.0f;

	public Transform groundCheckTransform;
	
	private bool grounded;
	
	public LayerMask groundCheckLayerMask;
	
	Animator mouseAnimator;

	public ParticleSystem jetpack;

	private bool dead = false;

	private uint coins = 0;

	public Texture2D coinIconTexture;

	public AudioClip coinCollectSound;

	public AudioSource jetpackAudio;
	
	public AudioSource footstepsAudio;

	public ParallaxScroll parallax;

	public Text coinsLabel;

	public GameObject restartDialog;

	// Use this for initialization
	void Start () {
		mouseAnimator = GetComponent<Animator>();
		restartDialog.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		// Get is tapped.
		bool jetpackActive = Input.GetButton("Fire1");
		
		jetpackActive = jetpackActive && !dead;
		
		if (jetpackActive) {
			// Add upward force.
			GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jetpackForce));
		}
		
		if (!dead) {
			Vector2 newVelocity = GetComponent<Rigidbody2D>().velocity;
			newVelocity.x = forwardMovementSpeed;
			GetComponent<Rigidbody2D>().velocity = newVelocity;
		}
		
		UpdateGroundedStatus();
		
		AdjustJetpack(jetpackActive);

		AdjustFootstepsAndJetpackSound(jetpackActive);

		parallax.offset = transform.position.x;
	} 

	// Check if mouse reached ground.
	void UpdateGroundedStatus() {
		// 1. Check if mouse reached ground.
		grounded = Physics2D.OverlapCircle(groundCheckTransform.position, 0.1f, groundCheckLayerMask);
		
		// 2. Change animation.
		mouseAnimator.SetBool("grounded", grounded);
	}

	// Update jetpack particle.
	void AdjustJetpack (bool jetpackActive) {
		jetpack.enableEmission = !grounded;
		jetpack.emissionRate = jetpackActive ? 300.0f : 75.0f; 
	}

	// Collision detection method (for unity 2D).
	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.CompareTag("Coins"))
			CollectCoin(collider);
		else
			HitByLaser(collider);
	}

	void HitByLaser(Collider2D laserCollider) {
		if (!dead)
			laserCollider.gameObject.GetComponent<AudioSource>().Play();

		dead = true;

		mouseAnimator.SetBool("dead", true);
		restartDialog.SetActive (true);
	}

	void CollectCoin(Collider2D coinCollider) {
		coins++;
		
		Destroy(coinCollider.gameObject);

		AudioSource.PlayClipAtPoint(coinCollectSound, transform.position);

		coinsLabel.text = coins.ToString ();
	}

	// Adjust sounds.
	void AdjustFootstepsAndJetpackSound(bool jetpackActive) {
		footstepsAudio.enabled = !dead && grounded;
		
		jetpackAudio.enabled =  !dead && !grounded;
		jetpackAudio.volume = jetpackActive ? 1.0f : 0.5f;        
	}

	public void RestartGame() {
		//Application.LoadLevel (Application.loadedLevelName);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public void ExitToMenu() {
		//Application.LoadLevel ("MenuScene");
		SceneManager.LoadScene ("MenuScene");
	}

}
