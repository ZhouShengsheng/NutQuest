using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

	public Animator titleGroup;
	public Animator dialog;
	public Animator contentPanel;
	public Animator gearImage;

	public void StartGame() {
		//Application.LoadLevel("RocketMouse");
		SceneManager.LoadScene ("SeaScene");
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
