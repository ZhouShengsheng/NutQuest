using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelScene : MonoBehaviour {

	public Button[] btnLevels;

	// Use this for initialization
	void Start () {
		btnLevels [3].interactable = false;
		btnLevels [4].interactable = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnDistrictsTapped() {
		SceneManager.LoadScene ("DistrictScene");
	}

	public void OnLevelTapped(int level) {
		SceneManager.LoadScene ("SeaScene");
	}
}
