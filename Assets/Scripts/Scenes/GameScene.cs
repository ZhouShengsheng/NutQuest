using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameScene : MonoBehaviour {

	// Screen.
	private float screenHeightInPoints;
	private float screenWidthInPoints;

	// Bg.
	public List<GameObject> bgs;
	private float bgWidth = 0;
	public AudioClip seaBgm;
	public AudioClip forestBgm;

	// Beat.
	public GameObject beatWithIndicatorPrefab;
	public GameObject beatNoIndicatorPrefab;
	private GameObject beatPrefab;
	private List<GameObject> beats;
	public float beatOffset = 5;	// Offset between first beat and left border.
	public float beatInterval = 2;	// Interval between two beats.
	private int beatCountsPerBg;	// Equals bgWidth/beatInterval + 1.

	// Nuts and obstacles.
	public GameObject[] nutPrefabs;
	public GameObject[] obstaclePrefabs;
	public List<GameObject> objects;
	public float objectsMinY = -0.6f;
	public float objectsMaxY = 0.6f;

	// Nuts.
	private int nutsColleted = 0;
	public Text nutsColletedLabel;
	public AudioClip nutNormalCollectSound;
	public AudioClip nutFastCollectSound;
	public AudioClip nutSlowCollectSound;

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
	private float speedFactor = 1.0f;
	public float speedUpFactor = 1.2f;
	public float speedDownFactor = 0.8f;
	public float speedChangeTimeout = 5;
	private float _speedChangeTimeout;
	private bool collidedBorder = false;
	private bool upDown = true;			// true: up, false: down
	private bool onBeat = false;		// Squirrel can only change direction on beat.
	private float hitFrozenTime = 0;	// When hit by obstacle, this time is set to 3.
	Animator squirrelAnimator;

	// Controller.
	private float difficultyFactor = 1.0f;
	public GameObject pauseDialog;
	private bool isGamePaused = false;
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


	// Intializations.

	void Start () {
		screenHeightInPoints = 2.0f * Camera.main.orthographicSize;
		screenWidthInPoints = screenHeightInPoints * Camera.main.aspect;
		print ("screenHeightInPoints: " + screenHeightInPoints);
		print ("screenWidthInPoints: " + screenWidthInPoints);

		setNuts ();
		initSquirrelMovement ();
		prepareBgm ();
		prepareBeatPrefab ();
	}

	void setNuts() {
		int difficulty = SettingsManager.Instance.difficulty;
		int level = LevelManager.Instance.currentLevel;

		difficultyFactor = (float)(1 + (level-1) * 0.1 + difficulty * 0.26);
		print ("difficultyFactor: " + difficultyFactor);
		MusicManager.Instance.changeSpeed (difficultyFactor);

		totalNuts = (int)(totalNuts - (difficultyFactor-1)*(totalNuts/2));
		totalObstacles = (int)(totalObstacles + (difficultyFactor-1)*(totalObstacles));

		normalNutFreq *= (2 - difficultyFactor);
		fastNutFreq *= difficultyFactor;
		slowNutFreq *= (2 - difficultyFactor);
		obstacleFreq = 1 - normalNutFreq - fastNutFreq - slowNutFreq;;

		normalNutThre = (int)(normalNutFreq * 100) + 1;
		fastNutThre = normalNutThre + (int)(fastNutFreq * 100) + 1;
		slowNutThre = fastNutThre + (int)(slowNutFreq * 100) + 1;
		obstacleThre = slowNutThre + (int)(obstacleFreq * 100) + 1;
		print ("normalNutThre: " + normalNutThre);
		print ("fastNutThre: " + fastNutThre);
		print ("slowNutThre: " + slowNutThre);
		print ("obstacleThre: " + obstacleThre);
	}

	void initSquirrelMovement() {
		squirrelAnimator = GetComponent<Animator>();
		pauseDialog.SetActive (false);
		restartDialog.SetActive (false);
		float speed = upDown ? upDownSpeed : -upDownSpeed;
		GetComponent<Rigidbody2D> ().velocity = new Vector2 (forwardSpeed, speed);
	}

	void prepareBeatPrefab() {
		if (SettingsManager.Instance.beatIndicatorOn) {
			beatPrefab = beatWithIndicatorPrefab;
		} else {
			beatPrefab = beatNoIndicatorPrefab;
		}
	}

	void prepareBgm() {
		string district = LevelManager.Instance.currentDistrict;
		if (district.Equals ("Sea")) {
			MusicManager.Instance.playClip (seaBgm);
			beatInterval = 2.40f;
			beatOffset = -1.5f;
		} else {
			MusicManager.Instance.playClip (forestBgm);
			beatInterval = 2.40f;
			beatOffset = -1.5f;
		}
	}

	// Updates.

	void FixedUpdate () {
		moveBg();
		if (isGameOver || isGamePaused) {
			return;
		}
		if (checkIfGameOver()) {
			gameOver ();
			isGameOver = true;
		}
	}

	void Update () {
		if (isGameOver || isGamePaused) {
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
	 *	Move the left bg to right position whenever necessary.
	 */
	void moveBg() {
		GameObject preSea = bgs [0];
		GameObject currentSea = bgs [1];
		if (bgWidth == 0) {
			Transform floor = currentSea.transform.FindChild ("floor");
			if (floor == null) {
				return;
			}
			bgWidth = floor.localScale.x;
			beatCountsPerBg = (int)(bgWidth / beatInterval) + 1;
			print ("seaWidth: " + bgWidth);
			print ("beatCountsPerScreen: " + beatCountsPerBg);
			initBeats ();
		}
		float seaCenterX = currentSea.transform.position.x + bgWidth * 0.5f;
		float seaRightX = currentSea.transform.position.x + bgWidth;
		if (transform.position.x > seaCenterX) {
			preSea.transform.position = new Vector2(seaRightX + bgWidth, 0);
			bgs.Remove (preSea);
			bgs.Add (preSea);

			moveBeats ();
			removeObjects ();
		}			
	}

	void initBeats() {
		beats = new List<GameObject> ();
		int totalBeats = beatCountsPerBg * 3;
		float rightSeaX = bgs [2].transform.position.x;
		for (int i = 0; i < totalBeats; i++) {
			GameObject beat = GameObject.Instantiate (beatPrefab);
			beats.Add (beat);
		}
		for (int i = 0; i < beatCountsPerBg*2; i++) {
			GameObject beat = beats [i];
			beat.transform.position = new Vector2(-(rightSeaX + i*beatInterval), 0);
		}
		for (int i = beatCountsPerBg*2; i < totalBeats; i++) {
			GameObject beat = beats [i];
			beat.transform.position = new Vector2(rightSeaX + (i - beatCountsPerBg*2)*beatInterval + beatOffset, 0);
			addObjects (beat);
		}
	}

	/**
	 * 	Move left beats to right positions.
	 */
	void moveBeats() {
		float rightX = beats [beats.Count-1].transform.position.x;
		for (int i = 0; i < beatCountsPerBg; i++) {
			GameObject beat = beats [0];
			beat.transform.position = new Vector2(rightX + (i+1)*beatInterval, 0);
			beats.RemoveAt(0);
			beats.Add (beat);
			addObjects (beat);
		}

	}

	void updateSquirrelSpeed() {
		Rigidbody2D body = GetComponent<Rigidbody2D> ();
		if (isGamePaused) {
			body.velocity = new Vector2 (0, 0);
		} else if (collidedBorder) {
			body.velocity = new Vector2 (forwardSpeed * speedFactor * difficultyFactor, 0);
		} else {
			float speed = upDown ? upDownSpeed : -upDownSpeed;
			body.velocity = new Vector2 (forwardSpeed * speedFactor * difficultyFactor, speed * speedFactor * difficultyFactor);
		}
		MusicManager.Instance.changeSpeed (speedFactor * difficultyFactor);
	}

	/**
	 * Move squirrel according to user inputs.
	 */
	void moveSquirrel() {
		// Get is tapped.
		bool tapped = Input.GetButtonDown("Fire1");

		if (tapped && onBeat) {
			onBeat = false;
			upDown = !upDown;
			collidedBorder = false;
			updateSquirrelSpeed ();
		}

		if (_speedChangeTimeout > 0) {
			_speedChangeTimeout -= Time.deltaTime;
			if (_speedChangeTimeout <= 0) {
				speedFactor = 1.0f;
				updateSquirrelSpeed ();
			}
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
			//			print ("randomThre: " + randomThre);
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
					newObj.GetComponent<Rigidbody2D> ().velocity = new Vector2(-forwardSpeed * difficultyFactor, 0);
				} else {
					objY = Random.Range (objectsMinY, 0);
				}
			} else {
				objY = Random.Range(objectsMinY, objectsMaxY);
			}

			newObj.transform.position = new Vector2(addX,objY); 
			objects.Add(newObj);
		}
	}

	/**
	 * 	Remove left objects.
	 */
	void removeObjects() {
		float leftSeaX = bgs [0].transform.position.x;

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
			collidedBorder = true;
			updateSquirrelSpeed ();
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
		updateNutsCollectedlabel ();
		AudioClip clip;
		if (nutTag.Equals ("NutFast")) {
			if (speedFactor == speedUpFactor) {
				return;
			}
			if (speedFactor == speedDownFactor) {
				speedFactor = 1.0f;
			} else {
				speedFactor = speedUpFactor;
			}
			_speedChangeTimeout = speedChangeTimeout;
			updateSquirrelSpeed ();
			clip = nutFastCollectSound;
		} else if (nutTag.Equals ("NutSlow")) {
			if (speedFactor == speedDownFactor) {
				return;
			}
			if (speedFactor == speedUpFactor) {
				speedFactor = 1.0f;
			} else {
				speedFactor = speedDownFactor;
			}
			_speedChangeTimeout = speedChangeTimeout;
			updateSquirrelSpeed ();
			clip = nutSlowCollectSound;
		} else {
			clip = nutNormalCollectSound;
		}
		AudioSource.PlayClipAtPoint(clip, transform.position);
	}

	void updateNutsCollectedlabel() {
		nutsColletedLabel.text = nutsColleted.ToString ();
	}

	public void pauseGame() {
		isGamePaused = true;
		updateSquirrelSpeed ();
		pauseDialog.SetActive (true);
		MusicManager.Instance.pause ();

		// Pause birds.
		for (int i = 0; i < objects.Count; i++) {
			GameObject obj = objects [i];
			if (obj != null && obj.CompareTag("ObstacleBird")) {
				obj.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 0);
			}
		}
	}

	public void resumeGame() {
		isGamePaused = false;
		updateSquirrelSpeed ();
		pauseDialog.SetActive (false);
		MusicManager.Instance.resume ();

		// Resume birds.
		for (int i = 0; i < objects.Count; i++) {
			GameObject obj = objects [i];
			if (obj.CompareTag("ObstacleBird")) {
				obj.GetComponent<Rigidbody2D> ().velocity = new Vector2(-forwardSpeed * difficultyFactor, 0);
			}
		}
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
				LevelManager.Instance.levelCompleted (3);
			} else if (nutsColleted >= silverLevelNuts) {	// Silver.
				imgMedal.sprite = medalSilverImg;
				LevelManager.Instance.levelCompleted (2);
			} else if (nutsColleted >= bronzeLevelNuts) {	// Bronze.
				imgMedal.sprite = medalBronzeImg;
				LevelManager.Instance.levelCompleted (1);
			}
		}
		restartDialog.SetActive (true);
	}

	public void restartGame() {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		MusicManager.Instance.changeSpeed (1 * difficultyFactor);
		MusicManager.Instance.resume ();
	}

	public void exit() {
		SceneManager.LoadScene ("LevelScene");
		MusicManager.Instance.changeSpeed (1);
		MusicManager.Instance.resume ();
		MusicManager.Instance.playBgm ();
	}
}
