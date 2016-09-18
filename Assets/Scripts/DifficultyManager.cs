using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DifficultyManager : MonoBehaviour {

	// Buttons in the UI.
	public Button[] difficultyButtons;

	private int currentDifficulty = 0;

	// Use this for initialization.
	void Start () {
		Text text = difficultyButtons [0].GetComponentInChildren<Text> ();
		text.color = Color.yellow;
	}

	// Difficulty button tapped.
	public void OnDifficultyChanged(int difficulty)
	{
		if (currentDifficulty != difficulty) {
			currentDifficulty = difficulty;
			for (int i = 0; i < difficultyButtons.Length; i++) {
				Text text = difficultyButtons[i].GetComponentInChildren<Text> ();
				if (i == currentDifficulty) {
					text.color = Color.yellow;
				} else {
					text.color = Color.white;
				}
			}
		}
	}
}
