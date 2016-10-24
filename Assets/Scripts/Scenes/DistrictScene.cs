using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DistrictScene : MonoBehaviour {

	public Button btnSea;
	public Text txtSeaPoints;
	public Image[] imgSeaLevels;

	public Button btnForeast;
	public Text txtForeastPoints;
	public Image[] imgForeastLevels;

	// Sprites.
	public Sprite starFilledImg;
	public Sprite starUnfilledImg;
	public Sprite polygonGreenImg;
	public Sprite polygonGrayImg;


	// Change level images according to district.
	void changeLevelImages(Image[] imgLevels, District d) {
		int i;
		for (i = 0; i < d.unlockedLevels; i++) {
			imgLevels [i].sprite = starFilledImg;
		}
		for (; i < 5; i++) {
			imgLevels [i].sprite = starUnfilledImg;
		}
	}

	// Use this for initialization
	void Start () {
		DistrictManager districtManager = DistrictManager.Instance;
		District sea = districtManager.seaDistrict;
		txtSeaPoints.text = string.Format ("{0} Points", sea.points);
		changeLevelImages (imgSeaLevels, sea);

		District foreast = districtManager.seaDistrict;
		if (foreast.unlocked) {
			txtForeastPoints.text = string.Format ("{0} Points", foreast.points);
			changeLevelImages (imgForeastLevels, foreast);
			btnForeast.image.overrideSprite = polygonGreenImg;
		} else {
			txtForeastPoints.text = "Locked";
			btnForeast.interactable = false;
			btnForeast.image.overrideSprite = polygonGrayImg;
		}
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
