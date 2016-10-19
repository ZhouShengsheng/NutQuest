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
	}

	public void OpenSettings() {
		titleGroup.SetBool ("isHidden", true);
		dialog.SetBool ("isHidden", false);
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
