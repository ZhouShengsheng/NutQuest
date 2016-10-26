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
		seaDistrict.unlocked = true;
		seaDistrict.starCount = PlayerPrefs.GetInt ("sea_starCount", 0);
		seaDistrict.points = PlayerPrefs.GetInt ("sea_points", 0);

		foreastDistrict = new District ();
		foreastDistrict.name = "Foreast";
		foreastDistrict.unlocked = PlayerPrefs.GetInt ("foreast_unlocked", 0) > 0;
		foreastDistrict.starCount = PlayerPrefs.GetInt ("foreast_starCount", 0);
		foreastDistrict.points = PlayerPrefs.GetInt ("foreast_points", 0);
	}

	private void save() {
		PlayerPrefs.SetInt ("sea_starCount", seaDistrict.starCount);
		PlayerPrefs.SetInt ("sea_points", seaDistrict.points);

		PlayerPrefs.SetInt ("foreast_unlocked", foreastDistrict.unlocked ? 1 : 0);
		PlayerPrefs.SetInt ("foreast_starCount", foreastDistrict.starCount);
		PlayerPrefs.SetInt ("foreast_points", foreastDistrict.points);
	}

	/**
	 * 	Called on level completed.
	 */
	public void levelCompleted(string district, int points, int starCounts) {
		if (district.Equals ("Sea")) {
			seaDistrict.points = points;
			if (points >= unlockDistrictPoints) {
				foreastDistrict.unlocked = true;
			}
			seaDistrict.starCount = starCounts;
		} else if (district.Equals ("Foreast")) {
			foreastDistrict.points = points;
			foreastDistrict.starCount = starCounts;
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
