using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Script to generate backgrounds, nuts and obstacles.
 */
public class GeneratorScript : MonoBehaviour {

	// Screen.
	private float screenHeightInPoints;
	private float screenWidthInPoints;

	// Sea.
	public GameObject[] availableSeas;
	public List<GameObject> currentSeas;
	private float seaWidth = 0;

	public GameObject[] nutPrefabs;
	public GameObject[] obstaclePrefabs;

	// To be removed.
	public GameObject[] availableObjects;
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
	}

	void FixedUpdate () {
		GenerateSeaIfRequred();
		GenerateObjectsIfRequired();    
	}

	void AddSea(float farhtestSeaEndX) {
		int randomSeaIndex = Random.Range(0, availableSeas.Length);
		GameObject sea = (GameObject)Instantiate(availableSeas[randomSeaIndex]);
		float seaWidth = sea.transform.FindChild("floor").localScale.x;
		float seaCenter = farhtestSeaEndX + seaWidth * 0.5f;
		sea.transform.position = new Vector3(seaCenter, 0, 0);

		currentSeas.Add(sea);			
	} 

	void GenerateSeaIfRequred() {
		List<GameObject> seasToRemove = new List<GameObject>();
		bool addSeas = true;        
		float playerX = transform.position.x;
		float removeSeaX = playerX - screenWidthInPoints;        
		float addSeaX = playerX + screenWidthInPoints;
		float farthestSeaEndX = 0;

		//print ("currentSeas.Count: " + currentSeas.Count);
		foreach(var sea in currentSeas) {
			if (seaWidth - 0.0f == 0.0f) {
				Transform floor = sea.transform.FindChild ("floor");
				if (floor == null) {
					return;
				}
				seaWidth = floor.localScale.x;
			}
			//float seaWidth = floor.localScale.x;
			float seaStartX = sea.transform.position.x - (seaWidth * 0.5f);    
			float seaEndX = seaStartX + seaWidth;                            

			if (seaStartX > addSeaX)
				addSeas = false;
			
			if (seaEndX < removeSeaX)
				seasToRemove.Add(sea);
			
			farthestSeaEndX = Mathf.Max(farthestSeaEndX, seaEndX);
		}

		foreach(var sea in seasToRemove) {
			currentSeas.Remove(sea);
			Destroy(sea);
		}

		if (addSeas)
			AddSea(farthestSeaEndX);
	}

	void AddObject(float lastObjectX) {
		int randomIndex = Random.Range(0, availableObjects.Length);

		GameObject obj = (GameObject)Instantiate(availableObjects[randomIndex]);

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
