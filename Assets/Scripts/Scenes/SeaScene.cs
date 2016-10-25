using UnityEngine;
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

	// Squirrel.
	public GameObject squirrel;

	// Sea.
	public List<GameObject> seas;
	private float seaWidth = 0;

	public GameObject[] nutPrefabs;
	public GameObject[] obstaclePrefabs;

	// To be removed.
	public List<GameObject> objects;

	public float objectsMinDistance = 5.0f;    
	public float objectsMaxDistance = 10.0f;

	public float objectsMinY = -1.4f;
	public float objectsMaxY = 1.4f;

	//	public float objectsMinRotation = -45.0f;
	//	public float objectsMaxRotation = 45.0f;


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
	}

	void FixedUpdate () {
		moveSea();
		GenerateObjectsIfRequired();    
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
			print ("seaWidth: " + seaWidth);
		}
		float seaCenterX = currentSea.transform.position.x + seaWidth * 0.5f;
		float seaRightX = currentSea.transform.position.x + seaWidth;
		if (squirrel.transform.position.x > seaCenterX) {
			preSea.transform.position = new Vector3(seaRightX + seaWidth, 0, 0);
			seas.Remove (preSea);
			seas.Add (preSea);
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

		objects.Add(obj);
	}

	void GenerateObjectsIfRequired() {
		float playerX = transform.position.x;        
		float removeObjectsX = playerX - screenWidthInPoints;
		float addObjectX = playerX + screenWidthInPoints;
		float farthestObjectX = 0;

		List<GameObject> objectsToRemove = new List<GameObject>();

		foreach (GameObject obj in objects) {
			if (obj == null) {
				continue;
			}
			float objX = obj.transform.position.x;

			farthestObjectX = Mathf.Max(farthestObjectX, objX);

			if (objX < removeObjectsX)            
				objectsToRemove.Add(obj);
		}

		foreach (var obj in objectsToRemove) {
			objects.Remove(obj);
			Destroy(obj);
		}

		if (farthestObjectX < addObjectX)
			AddObject(farthestObjectX);
	}
}
