using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SeaScene : MonoBehaviour {

	// Screen.
	private float screenHeightInPoints;
	private float screenWidthInPoints;

	// Sea.
	public List<GameObject> seas;
	private float seaWidth = 0;

	// Beat.
	public GameObject beatPrefab;
	private List<GameObject> beats;
	public float beatOffset = 5;	// Offset between first beat and left border.
	public float beatInterval = 2;	// Interval between two beats.
	private int beatCountsPerSea;	// Equals seaWidth/beatInterval.

	// Nuts and obstacles.
	public GameObject[] nutPrefabs;
	public GameObject[] obstaclePrefabs;
	public List<GameObject> objects;
	public float objectsMinY = -0.6f;
	public float objectsMaxY = 0.6f;

	// Nuts.
	private int nutsColleted = 0;
	public Text nutsColletedLabel;
	public AudioClip nutCollectSound;

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

	// Squirrel movements.
	public float forwardSpeed = 3.0f;
	public float upDownSpeed = 3.0f;
	private bool upDown = true;			// true: up, false: down
	private bool onBeat = false;		// Squirrel can only change direction on beat.
	private float hitFrozenTime = 0;	// When hit by obstacle, this time is set to 3.
	Animator squirrelAnimator;

	// Controller.
	public GameObject restartDialog;
	private bool isGameOver = false;
	public static int bronzeLevelNuts = 60;
	public static int silverLevelNuts = 80;
	public static int goldenLevelNuts = 100;

	// Sprites.
	public Sprite medalGoldenImg;
	public Sprite medalSilverImg;
	public Sprite medalBronzeImg;
	public Sprite medalNoneImg;


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
		if (isGameOver) {
			return;
		}
		if (checkIfGameOver()) {
			gameOver ();
			isGameOver = true;
		}
	}

	void Update () {
		if (isGameOver) {
			return;
		}

		moveSquirrel ();

		if (hitFrozenTime > 0) {
			hitFrozenTime -= Time.deltaTime;
			if (hitFrozenTime <= 0) {
				squirrelAnimator.SetBool ("hit", false);
			}
		}
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
			removeObjects ();
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
			addObjects (beat);
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
			addObjects (beat);
		}

	}

	/**
	 * Move squirrel according to user inputs.
	 */
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

	/**
	 * Add nuts or obstacles on the beat.
	 */
	void addObjects(GameObject beat) {
		float addX = beat.transform.position.x;
		int number = Random.Range (1, 16);
		if (number <= 8) {
			number = 1;
		} else if (number <= 12) {
			number = 2;
		} else if (number <= 14) {
			number = 3;
		} else {
			number = 0;
		}
		for (int i = 0; i < number; i++) {
			int randomThre = Random.Range (1, 101);
			print ("randomThre: " + randomThre);
			if (randomThre <= slowNutThre && totalNuts <= 0) {
				return;
			}
			if (randomThre > slowNutThre && randomThre <= obstacleThre && totalObstacles <= 0) {
				return;
			}
			GameObject newObj = null;
			if (randomThre <= normalNutThre) {
				newObj = (GameObject)Instantiate (nutPrefabs [0]);
				totalNuts--;
			} else if (randomThre <= fastNutThre) {
				newObj = (GameObject)Instantiate (nutPrefabs [1]);
				totalNuts--;
			} else if (randomThre <= slowNutThre) {
				newObj = (GameObject)Instantiate (nutPrefabs [2]);
				totalNuts--;
			} else if (randomThre <= obstacleThre) {
				int randomIndex = Random.Range (0, obstaclePrefabs.Length);
				newObj = (GameObject)Instantiate (obstaclePrefabs [randomIndex]);
				totalObstacles--;
			} else {
				newObj = (GameObject)Instantiate (nutPrefabs [0]);
				totalNuts--;
			}


			float objY;
			if (randomThre > slowNutThre) {
				// Put birds in the air and seaweeds in the water.
				if (newObj.CompareTag ("ObstacleBird")) {
					objY = Random.Range (0, objectsMaxY);
				} else {
					objY = Random.Range (objectsMinY, 0);
				}
			} else {
				objY = Random.Range(objectsMinY, objectsMaxY);
			}

			newObj.transform.position = new Vector3(addX,objY,0); 
			objects.Add(newObj);
		}
	}

	/**
	 * 	Remove left objects.
	 */
	void removeObjects() {
		float leftSeaX = seas [0].transform.position.x;

		int totalCount = objects.Count;
		for (int i = 0; i < totalCount; i++) {
			GameObject obj = objects [0];
			if (obj == null) {
				objects.Remove (obj);
			} else if (obj.transform.position.x < leftSeaX) {
				objects.Remove (obj);
				Destroy (obj);
			}
		}
	}

	// Collision detection method (for unity 2D).
	void OnTriggerEnter2D(Collider2D collider) {
//		print ("collider: " + collider);
		if (collider.gameObject.tag.StartsWith ("Nut")) {
//			print ("Collided with normal nut.");
			collectNut(collider, collider.gameObject.tag);
		} else if (collider.gameObject.CompareTag ("Border")) {
//			print ("Collided with border.");
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (forwardSpeed, 0);
		} else if (collider.gameObject.tag.StartsWith("Obstacle")) {
			hitObstacle ();
		} else if (collider.gameObject.CompareTag ("Beat")) {
//			print ("Collided with beat.");
			onBeat = true;
		}
	}

	void OnTriggerExit2D(Collider2D collider) {
		if (collider.gameObject.CompareTag ("Beat")) {
//			print ("Beat exit.");
			onBeat = false;
		}
	}

	/**
	 * Hit by obstacle.
	 */
	void hitObstacle() {
//		print ("Collided with obstacle.");
		if (hitFrozenTime > 0) {	// Currently frozen.
			return;
		}
		squirrelAnimator.SetBool ("hit", true);
		hitFrozenTime = 3;
		nutsColleted -= 3;
		if (nutsColleted < 0) {
			nutsColleted = 0;
		}
		updateNutsCollectedlabel ();
	}

	/**
	 * 	Collect nut.
	 */
	void collectNut(Collider2D nutCollider, string nutTag) {
		nutsColleted++;
		Destroy(nutCollider.gameObject);
		AudioSource.PlayClipAtPoint(nutCollectSound, transform.position);
		updateNutsCollectedlabel ();
	}

	void updateNutsCollectedlabel() {
		nutsColletedLabel.text = nutsColleted.ToString ();
	}

	/**
	 * 	Check if the game can be over now.
	 */
	bool checkIfGameOver() {
		bool over = true;
		if (totalNuts <= 0) {
			int count = objects.Count;
			for (int i = 0; i < count; i++) {
				GameObject obj = objects [i];
				if (obj == null) {
					continue;
				} else if (obj.tag.StartsWith ("Nut")) {
					over = false;
				}
			}
		} else {
			over = false;
		}
		return over;
	}

	/**
	 * 	Game over here. Display restart dialog.
	 */
	void gameOver() {
		Text txtPoints = restartDialog.transform.Find ("Txt_Points").gameObject.GetComponent<Text>();
		if (nutsColleted <= 1) {
			txtPoints.text = string.Format ("{0} nut collected.", nutsColleted);
		} else {
			txtPoints.text = string.Format ("{0} nuts collected.", nutsColleted);
		}
		Text txtTitle = restartDialog.transform.Find ("Txt_Title").gameObject.GetComponent<Text>();
		Image imgMedal = restartDialog.transform.Find ("Img_Medal").gameObject.GetComponent<Image>();
		if (nutsColleted < bronzeLevelNuts) {
			txtTitle.text = "Challenge failed...";
			imgMedal.sprite = medalNoneImg;
		} else {
			txtTitle.text = "Level completed!";
			if (nutsColleted >= goldenLevelNuts) {	// Golden.
				imgMedal.sprite = medalGoldenImg;
			} else if (nutsColleted >= silverLevelNuts) {	// Silver.
				imgMedal.sprite = medalSilverImg;
			} else if (nutsColleted >= bronzeLevelNuts) {	// Bronze.
				imgMedal.sprite = medalBronzeImg;
			}
		}
		restartDialog.SetActive (true);
	}

	public void restartGame() {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public void exit() {
		SceneManager.LoadScene ("LevelScene");
	}
}
