using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SeaScene : MonoBehaviour {

	// Total nut number and obstacle number.
	public int totalNuts = 120;
	public int totalObstacles = 30;

	// Frequencies for nuts and obstacles.
	public float normalNutFreq = 0.6f;
	public float fastNutFreq = 0.1f;
	public float slowNutFreq = 0.1f;
	public float obstacleFreq = 0.2f;
	private int normalNutThre;
	private int fastNutThre;
	private int slowNutThre;
	private int obstacleThre;

	// Screen.
	private float screenHeightInPoints;
	private float screenWidthInPoints;

	// Sea.
	public List<GameObject> seas;
	private float seaWidth = 0;

	public GameObject[] nutPrefabs;
	public GameObject[] obstaclePrefabs;

	// Beat.
	public GameObject beatPrefab;
	private List<GameObject> beats;
	public float beatOffset = 5;	// Offset between first beat and left border.
	public float beatInterval = 2;	// Interval between two beats.
	private int beatCountsPerSea;	// Equals seaWidth/beatInterval.

	// Objects to be destroyed.
	public List<GameObject> objectsToBeDestroyed;

	public float objectsMinDistance = 5.0f;    
	public float objectsMaxDistance = 10.0f;

	public float objectsMinY = -1.4f;
	public float objectsMaxY = 1.4f;

	// Squirrel movements.
	public float forwardSpeed = 3.0f;
	public float upDownSpeed = 3.0f;
	private bool upDown = true;	// true: up, false: down
	private bool onBeat = false;	// Squirrel can only change direction on beat.

	// Nuts.
	private uint nutsColleted = 0;
	public Text nutsColletedLabel;
	public AudioClip nutCollectSound;

	// Animation.
	Animator squirrelAnimator;

	// Controller.
	public GameObject restartDialog;


	// Use this for initialization
	void Start () {
		screenHeightInPoints = 2.0f * Camera.main.orthographicSize;
		screenWidthInPoints = screenHeightInPoints * Camera.main.aspect;
		print ("screenHeightInPoints: " + screenHeightInPoints);
		print ("screenWidthInPoints: " + screenWidthInPoints);

		normalNutThre = (int)(normalNutFreq * 100) + 1;
		fastNutThre = normalNutThre + (int)(fastNutFreq * 100) + 1;
		slowNutThre = fastNutThre + (int)(slowNutFreq * 100) + 1;
		obstacleThre = slowNutThre + (int)(obstacleFreq * 100) + 1;
		print ("normalNutThre: " + normalNutThre);
		print ("fastNutThre: " + fastNutThre);
		print ("slowNutThre: " + slowNutThre);
		print ("obstacleThre: " + obstacleThre);

		squirrelAnimator = GetComponent<Animator>();
		restartDialog.SetActive (false);

		float speed = upDown ? upDownSpeed : -upDownSpeed;
		GetComponent<Rigidbody2D> ().velocity = new Vector2 (forwardSpeed, speed);
	}

	void FixedUpdate () {
		moveSea();
		GenerateObjectsIfRequired();
	}

	void Update () {
		moveSquirrel ();
	}

	/**
	 *	Move the left sea into right position whenever necessary.
	 */
	void moveSea() {
		GameObject preSea = seas [0];
		GameObject currentSea = seas [1];
		if (seaWidth == 0) {
			Transform floor = currentSea.transform.FindChild ("floor");
			if (floor == null) {
				return;
			}
			seaWidth = floor.localScale.x;
			beatCountsPerSea = (int)(seaWidth / beatInterval);
			print ("seaWidth: " + seaWidth);
			print ("beatCountsPerScreen: " + beatCountsPerSea);
			initBeats ();
		}
		float seaCenterX = currentSea.transform.position.x + seaWidth * 0.5f;
		float seaRightX = currentSea.transform.position.x + seaWidth;
		if (transform.position.x > seaCenterX) {
			preSea.transform.position = new Vector2(seaRightX + seaWidth, 0);
			seas.Remove (preSea);
			seas.Add (preSea);

			moveBeats ();
		}			
	}

	void initBeats() {
		beats = new List<GameObject> ();
		int totalBeats = beatCountsPerSea * 3;
		float rightSeaX = seas [2].transform.position.x;
		for (int i = 0; i < totalBeats; i++) {
			GameObject beat = GameObject.Instantiate (beatPrefab);
			beats.Add (beat);
		}
		for (int i = 0; i < beatCountsPerSea*2; i++) {
			GameObject beat = beats [i];
			beat.transform.position = new Vector2(-(rightSeaX + i*beatInterval), 0);
		}
		for (int i = beatCountsPerSea*2; i < totalBeats; i++) {
			GameObject beat = beats [i];
			beat.transform.position = new Vector2(rightSeaX + (i - beatCountsPerSea*2)*beatInterval, 0);
		}
	}

	/**
	 * 	Move left beats to right positions.
	 */
	void moveBeats() {
		float rightX = beats [beats.Count-1].transform.position.x;
		for (int i = 0; i < beatCountsPerSea; i++) {
			GameObject beat = beats [0];
			beat.transform.position = new Vector2(rightX + (i+1)*beatInterval, 0);
			beats.RemoveAt(0);
			beats.Add (beat);
		}

	}

	void moveSquirrel() {
		// Get is tapped.
		bool tapped = Input.GetButtonUp("Fire1");

		if (tapped && onBeat) {
			onBeat = false;
			upDown = !upDown;
			float speed = upDown ? upDownSpeed : -upDownSpeed;
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (forwardSpeed, speed);
		}
	}

	void AddObject(float lastObjectX) {
		int randomThre = Random.Range (1, 101);
		GameObject obj = null;
		if (randomThre <= normalNutThre) {
			obj = (GameObject)Instantiate (nutPrefabs [0]);
		} else if (randomThre <= fastNutThre) {
			obj = (GameObject)Instantiate (nutPrefabs [1]);
		} else if (randomThre <= slowNutThre) {
			obj = (GameObject)Instantiate (nutPrefabs [2]);
		} else if (randomThre <= obstacleThre) {
			int randomIndex = Random.Range (0, obstaclePrefabs.Length);
			obj = (GameObject)Instantiate (obstaclePrefabs [randomIndex]);
		} else {
			obj = (GameObject)Instantiate (nutPrefabs [0]);
		}

		float objectPositionX = lastObjectX + Random.Range(objectsMinDistance, objectsMaxDistance);
		float randomY = Random.Range(objectsMinY, objectsMaxY);
		obj.transform.position = new Vector3(objectPositionX,randomY,0); 

		//		float rotation = Random.Range(objectsMinRotation, objectsMaxRotation);
		//		obj.transform.rotation = Quaternion.Euler(Vector3.forward * rotation);

		objectsToBeDestroyed.Add(obj);
	}

	void GenerateObjectsIfRequired() {
		float playerX = transform.position.x;        
		float removeObjectsX = playerX - screenWidthInPoints;
		float addObjectX = playerX + screenWidthInPoints;
		float farthestObjectX = 0;

		List<GameObject> objectsToRemove = new List<GameObject>();

		foreach (GameObject obj in objectsToBeDestroyed) {
			if (obj == null) {
				continue;
			}
			float objX = obj.transform.position.x;

			farthestObjectX = Mathf.Max(farthestObjectX, objX);

			if (objX < removeObjectsX)            
				objectsToRemove.Add(obj);
		}

		foreach (var obj in objectsToRemove) {
			objectsToBeDestroyed.Remove(obj);
			Destroy(obj);
		}

		if (farthestObjectX < addObjectX)
			AddObject(farthestObjectX);
	}

	// Collision detection method (for unity 2D).
	void OnTriggerEnter2D(Collider2D collider) {
		print ("collider: " + collider);
		if (collider.gameObject.CompareTag ("NutNormal")) {
			print ("Collided with normal nut.");
			CollectNut(collider);
		} else if (collider.gameObject.CompareTag ("Border")) {
			print ("Collided with border.");
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (forwardSpeed, 0);
		} else if (collider.gameObject.CompareTag ("Obstacle")) {
			print ("Collided with obstacle.");
		} else if (collider.gameObject.CompareTag ("Beat")) {
			print ("Collided with beat.");
			onBeat = true;
		}
	}

	void OnTriggerExit2D(Collider2D collider) {
		if (collider.gameObject.CompareTag ("Beat")) {
			print ("Beat exit.");
			onBeat = false;
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
