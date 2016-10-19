using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DistrictScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnMainMenuTapped() {
		SceneManager.LoadScene ("MenuScene");
	}

	public void OnDistrictTapped(int districtNumber) {
		SceneManager.LoadScene ("LevelScene");
	}
}
