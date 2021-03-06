﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DistrictScene : MonoBehaviour {

	public Button btnSea;
	public Text txtSeaPoints;
	public Image[] imgSeaLevels;

	public Button btnForest;
	public Text txtForestPoints;
	public Image[] imgForestLevels;

	// Sprites.
	public Sprite starFilledImg;
	public Sprite starUnfilledImg;
	public Sprite polygonGreenImg;
	public Sprite polygonGrayImg;


	// Change level images according to district.
	void changeLevelImages(Image[] imgLevels, District d) {
		int i;
		for (i = 0; i < d.starCount; i++) {
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
		btnSea.onClick.AddListener (() => onDistrictTapped("Sea"));

		District foreast = districtManager.forestDistrict;
		btnForest.onClick.RemoveAllListeners ();
		if (foreast.unlocked) {
			txtForestPoints.text = string.Format ("{0} Points", foreast.points);
			changeLevelImages (imgForestLevels, foreast);
			btnForest.image.overrideSprite = polygonGreenImg;
			btnForest.onClick.AddListener (() => onDistrictTapped("Foreast"));
			btnForest.interactable = true;
		} else {
			txtForestPoints.text = "Locked";
			btnForest.interactable = false;
			btnForest.image.overrideSprite = polygonGrayImg;
		}
	}

	public void onMainMenuTapped() {
		SceneManager.LoadScene ("MenuScene");
	}

	public void onDistrictTapped(string districtName) {
		LevelManager.Instance.currentDistrict = districtName;
		SceneManager.LoadScene ("LevelScene");
	}
}
