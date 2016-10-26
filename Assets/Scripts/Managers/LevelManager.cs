using UnityEngine;
using System.Collections;

public class LevelManager : Singleton<LevelManager> {

	public Level[] seaLevels;
	public Level[] forestLevels;

	public string currentDistrict = "Sea";
	public int currentLevel = 0;
	public static int levelCounts = 5;


	// Persistence.
	private void load() {
		// Sea levels.
		seaLevels = new Level[5];
		for (int i = 0; i < levelCounts; i++) {
			Level l = new Level ();
			l.number = i + 1;
			int unlocked = PlayerPrefs.GetInt ("sea_" + l.number + "_unlocked", -1);
			if (unlocked == -1) {
				l.unlocked = (l.number == 1);
			} else {
				l.unlocked = unlocked > 0;
			}
			if (l.number == 1) {
				l.unlocked = true;
			}
			l.points = PlayerPrefs.GetInt ("sea_" + l.number + "_points", 0);
			seaLevels [i] = l;
		}

		// Foreast levels.
		forestLevels = new Level[5];
		for (int i = 0; i < levelCounts; i++) {
			Level l = new Level ();
			l.number = i + 1;
			int unlocked = PlayerPrefs.GetInt ("foreast_" + l.number + "_unlocked", -1);
			if (unlocked == -1) {
				l.unlocked = (l.number == 1);
			} else {
				l.unlocked = unlocked > 0;
			}
			if (l.number == 1) {
				l.unlocked = true;
			}
			l.points = PlayerPrefs.GetInt ("foreast_" + l.number + "_points", 0);
			forestLevels [i] = l;
		}
	}

	private void save() {
		// Sea levels.
		for (int i = 0; i < levelCounts; i++) {
			Level l = seaLevels[i];
			PlayerPrefs.SetInt ("sea_" + l.number + "_unlocked", l.unlocked ? 1 : 0);
			PlayerPrefs.SetInt ("sea_" + l.number + "_points", l.points);
		}

		// Foreast levels.
		for (int i = 0; i < levelCounts; i++) {
			Level l = forestLevels[i];
			PlayerPrefs.SetInt ("foreast_" + l.number + "_unlocked", l.unlocked ? 1 : 0);
			PlayerPrefs.SetInt ("foreast_" + l.number + "_points", l.points);
		}
	}

	/**
	 * 	Called on level completed.
	 */
	public void levelCompleted(int points) {
		Level level = null;
		if (currentDistrict.Equals ("Sea")) {
			level = seaLevels [currentLevel-1];
		} else if (currentDistrict.Equals ("Foreast")) {
			level = forestLevels [currentLevel-1];
		} 
		if (points > level.points) {
			level.points = points;
			if (currentLevel < levelCounts) {
				Level nextLevel = null;
				if (currentDistrict.Equals ("Sea")) {
					nextLevel = seaLevels [currentLevel];
				} else if (currentDistrict.Equals ("Foreast")) {
					nextLevel = forestLevels [currentLevel];
				}
				nextLevel.unlocked = true;
			}
			save ();
			DistrictManager.Instance.levelCompleted (currentDistrict, 
				totalPointsForDistrict(currentDistrict), totalStarCountForDistrict(currentDistrict));
		}
	}

	public int totalPointsForDistrict(string district) {
		Level[] levels = null;
		if (district.Equals ("Sea")) {
			levels = seaLevels;
		} else if (district.Equals ("Foreast")) {
			levels = forestLevels;
		} else {
			return 0;
		}
		int totalPoints = 0;
		for (int i = 0; i < levelCounts; i++) {
			totalPoints += levels [i].points;
		}
		return totalPoints;
	}

	public int totalStarCountForDistrict(string district) {
		Level[] levels = null;
		if (district.Equals ("Sea")) {
			levels = seaLevels;
		} else if (district.Equals ("Foreast")) {
			levels = forestLevels;
		} else {
			return 0;
		}
		int totalStarCount = 0;
		for (int i = 0; i < levelCounts; i++) {
			if (levels [i].points > 0) {
				totalStarCount++;
			}
		}
		return totalStarCount;
	}

	public int totalUnlockedLevelsForDistrict(string district) {
		Level[] levels = null;
		if (district.Equals ("Sea")) {
			levels = seaLevels;
		} else if (district.Equals ("Foreast")) {
			levels = forestLevels;
		} else {
			return 0;
		}
		int totalUnlockedLevels = 0;
		for (int i = 0; i < levelCounts; i++) {
			if (levels [i].unlocked) {
				totalUnlockedLevels++;
			}
		}
		return totalUnlockedLevels;
	}

	void Start() {
		print ("LevelManager start.");
		load ();
	}

	void onDestroy() {
		print ("LevelManager onDestroy.");
		save ();
	}
}
