using UnityEngine;
using System.Collections;

public class DistrictManager : Singleton<DistrictManager> {

	// Districts.
	public District seaDistrict;
	public District foreastDistrict;


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

	void Start() {
		print ("DistrictManager start.");
		load ();
	}

	void onDestroy() {
		print ("DistrictManager onDestroy.");
		save ();
	}
}
