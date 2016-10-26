using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsManager : Singleton<SettingsManager> {

	// Settings.
	public float volume;
	public int difficulty;
	public bool beatIndicatorOn;

	// UIs.
	public Button[] difficultyButtons;
	public Button[] beatIndicatorButtons;


	void Start() {
		print ("SettingsManager start.");
		init ();
	}

	void onDestroy() {
		print ("SettingsManager onDestroy.");
		save ();
	}

	/**
	 *	Load settings from player prefs and set UIs.
	 */
	private void init() {
		volume = PlayerPrefs.GetFloat ("SettingsManager_volume", 1);
		difficulty = PlayerPrefs.GetInt ("SettingsManager_difficulty", 0);
		beatIndicatorOn = PlayerPrefs.GetInt ("SettingsManager_beatIndicatorOn", 0) > 0;

		onDifficultyChanged (difficulty);
		onBeatIndicatorOnOffChanged (beatIndicatorOn);
	}

	/**
	 *	Save settings to player prefs.
	 */
	public void save() {
		PlayerPrefs.SetFloat ("SettingsManager_volume", volume);
		PlayerPrefs.SetInt ("SettingsManager_difficulty", difficulty);
		PlayerPrefs.SetInt ("SettingsManager_beatIndicatorOn", beatIndicatorOn ? 1 : 0);
	}

	/**
	 *	Difficulty button tapped.
	 */
	public void onDifficultyChanged(int difficulty) {
		this.difficulty = difficulty;
		for (int i = 0; i < difficultyButtons.Length; i++) {
			Text text = difficultyButtons[i].GetComponentInChildren<Text> ();
			if (i == this.difficulty) {
				text.color = Color.yellow;
			} else {
				text.color = Color.white;
			}
		}
		save ();
	}

	public void onBeatIndicatorOnOffChanged(bool on) {
		this.beatIndicatorOn = on;
		Text textOn = beatIndicatorButtons [0].GetComponentInChildren<Text> ();
		Text textOff = beatIndicatorButtons [1].GetComponentInChildren<Text> ();
		if (on) {
			textOn.color = Color.yellow;
			textOff.color = Color.white;
		} else {
			textOn.color = Color.white;
			textOff.color = Color.yellow;
		}
		save ();
	}
}
