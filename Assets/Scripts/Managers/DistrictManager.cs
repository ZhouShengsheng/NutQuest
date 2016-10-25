using UnityEngine;
using System.Collections;

public class DistrictManager : Singleton<DistrictManager> {

	// Districts.
	public District seaDistrict;
	public District foreastDistrict;

	// Points to unlock next district.
	public static int unlockDistrictPoints = 6;


	// Persistence.
	private void load() {
		seaDistrict = new District ();
		seaDistrict.name = "Sea";
		seaDistrict.unlocked = PlayerPrefs.GetInt ("sea_unlocked", 0) > 0;
		seaDistrict.unlockedLevels = PlayerPrefs.GetInt ("sea_unlockedLevels", 0);
		seaDistrict.points = PlayerPrefs.GetInt ("sea_points", 0);

		foreastDistrict = new District ();
		foreastDistrict.name = "Foreast";
		foreastDistrict.unlocked = PlayerPrefs.GetInt ("foreast_unlocked", 0) > 0;
		foreastDistrict.unlockedLevels = PlayerPrefs.GetInt ("foreast_unlockedLevels", 0);
		foreastDistrict.points = PlayerPrefs.GetInt ("foreast_points", 0);
	}

	private void save() {
		PlayerPrefs.SetInt ("sea_unlocked", seaDistrict.unlocked ? 1 : 0);
		PlayerPrefs.SetInt ("sea_unlockedLevels", seaDistrict.unlockedLevels);
		PlayerPrefs.SetInt ("sea_points", seaDistrict.points);

		PlayerPrefs.SetInt ("foreast_unlocked", foreastDistrict.unlocked ? 1 : 0);
		PlayerPrefs.SetInt ("foreast_unlockedLevels", foreastDistrict.unlockedLevels);
		PlayerPrefs.SetInt ("foreast_points", foreastDistrict.points);
	}

	/**
	 * 	Called on level completed.
	 */
	public void levelCompleted(string district, int totalPoints) {
		if (district.Equals ("Sea")) {
			seaDistrict.points = totalPoints;
			if (totalPoints >= unlockDistrictPoints) {
				foreastDistrict.unlocked = true;
			}
		} else if (district.Equals ("Foreast")) {
			foreastDistrict.points = totalPoints;
		}
		save ();
	}

	void Start() {
		print ("DistrictManager start.");
		load ();
	}

	void onDestroy() {
		print ("DistrictManager onDestroy.");
		save ();
	}
}
