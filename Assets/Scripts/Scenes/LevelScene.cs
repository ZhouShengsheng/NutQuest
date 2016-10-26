using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelScene : MonoBehaviour {

	public Button[] btnLevels;
	public Level[] levels;

	// Sprites.
	public Sprite starFilledImg;
	public Sprite starUnfilledImg;
	public Sprite medalGoldenImg;
	public Sprite medalSilverImg;
	public Sprite medalBronzeImg;
	public Sprite medalNoneImg;

	// Use this for initialization
	void Start () {
		LevelManager levelManager = LevelManager.Instance;
		switch (levelManager.currentDistrict) {
			case "Sea": {
				levels = levelManager.seaLevels;
				break;
			}
			case "Foreast": {
				levels = levelManager.forestLevels;
				break;
			}
			default: {
				levels = levelManager.seaLevels;
				break;
			}
		}

		initUI ();
	}

	void initUI() {
		for (int i = 0; i < LevelManager.levelCounts; i++) {
			Level l = levels [i];
			Button btnLevel = btnLevels [i];

			Text txtLevel = btnLevel.transform.Find ("Txt_Level").gameObject.GetComponent<Text>();
			txtLevel.text = string.Format ("Level {0}", l.number);

			Image imgStar = btnLevel.transform.Find ("Img_Star").gameObject.GetComponent<Image>();
			if (l.points > 0) {
				imgStar.sprite = starFilledImg;
			} else {
				imgStar.sprite = starUnfilledImg;
			}

			Image imgMedal = btnLevel.transform.Find ("Img_Medal").gameObject.GetComponent<Image>();
			print ("imgMedal: " + imgMedal);
			print ("l.points: " + l.points);
			switch (l.points) {
				case 0: {
					imgMedal.sprite = medalNoneImg;
					break;
				}
				case 1: {
					imgMedal.sprite = medalBronzeImg;
					break;
				}
				case 2: {
					imgMedal.sprite = medalSilverImg;
					break;
				}
				case 3: {
					imgMedal.sprite = medalGoldenImg;
					break;
				}
			}

			Text txtPoints = btnLevel.transform.Find ("Txt_Points").gameObject.GetComponent<Text>();
			if (l.points > 1) {
				txtPoints.text = string.Format ("{0} Points", l.points);
			} else {
				txtPoints.text = string.Format ("{0} Point", l.points);
			}

			Text txtInfo = btnLevel.transform.Find ("Txt_Info").gameObject.GetComponent<Text>();
			btnLevel.onClick.RemoveAllListeners ();
			if (l.unlocked) {
				txtInfo.text = "Tap to start!";
				btnLevel.interactable = true;
				btnLevel.onClick.AddListener (() => onLevelTapped(l.number));
			} else {
				txtInfo.text = "Locked";
				btnLevel.interactable = false;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onDistrictsTapped() {
		SceneManager.LoadScene ("DistrictScene");
	}

	public void onLevelTapped(int level) {
		LevelManager.Instance.currentLevel = level;
		if (LevelManager.Instance.currentDistrict.Equals ("Sea")) {
			SceneManager.LoadScene ("SeaScene");
		} else if (LevelManager.Instance.currentDistrict.Equals ("Foreast")) {
			SceneManager.LoadScene ("ForestScene");
		}
	}
}
