using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

	public Animator titleGroup;
	public Animator dialog;
	public Animator contentPanel;
	public Animator gearImage;

	// Use this for initialization
	void Start () {
	}

	public void StartGame() {
		SceneManager.LoadScene ("DistrictScene");

		SettingsManager manager2 = SettingsManager.Instance;
		print ("manager2.difficulty: " + manager2.difficulty);
	}

	public void OpenSettings() {
		titleGroup.SetBool ("isHidden", true);
		dialog.SetBool ("isHidden", false);

		SettingsManager manager1 = SettingsManager.Instance;
		manager1.difficulty = 2;
		print ("manager1.difficulty: " + manager1.difficulty);
	}

	public void CloseSettings() {
		titleGroup.SetBool("isHidden", false);
		dialog.SetBool("isHidden", true);
	}

	public void ToggleMenu() {
		bool isHidden = contentPanel.GetBool ("isHidden");
		contentPanel.SetBool ("isHidden", !isHidden);
		gearImage.SetBool ("isHidden", !isHidden);
	}

}
