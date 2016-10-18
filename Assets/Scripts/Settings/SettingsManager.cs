using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsManager : Singleton<SettingsManager> {

	// Settings.
	public bool isMute;
	public float volume;
	public int difficulty;

	// UIs.
	public Toggle muteToggle;
	public Slider volumeSlider;
	public Button[] difficultyButtons;


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
		isMute = PlayerPrefs.GetInt ("SettingsManager_isMute", 0) > 0;
		volume = PlayerPrefs.GetFloat ("SettingsManager_volume", 1);
		difficulty = PlayerPrefs.GetInt ("SettingsManager_difficulty", 0);

		muteToggle.isOn = !isMute;
		volumeSlider.value = volume;
		OnDifficultyChanged (this.difficulty);
	}

	/**
	 *	Save settings to player prefs.
	 */
	public void save() {
		PlayerPrefs.SetInt ("SettingsManager_isMute", isMute ? 1 : 0);
		PlayerPrefs.SetFloat ("SettingsManager_volume", volume);
		PlayerPrefs.SetInt ("SettingsManager_difficulty", difficulty);
	}

	/**
	 *	Difficulty button tapped.
	 */
	public void OnDifficultyChanged(int difficulty) {
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

	/**
	 *	Mute button tapped.
	 */
	public void OnMuteChanged() {
		this.isMute = !muteToggle.isOn;
		save ();
	}

	/**
	 *	Volume slide slided.
	 */
	public void OnVolumeChanged() {
		this.volume = volumeSlider.value;
		save ();
	}
}
