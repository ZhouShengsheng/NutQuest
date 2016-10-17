using UnityEngine;

/**
 *	Abstrac singleton class. Extend this class to get the singleton capability.
 *	Notes: Subclass must be attached to a GameObject.
 */
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {
	public static T Instance { get; private set; }

	protected virtual void Awake() {
		if (Instance == null) {
			Instance = (T)GameObject.FindObjectOfType(typeof(T));
			if (!Instance) {
				Debug.LogError("There needs to be one active " + this.GetType() + " script on a GameObject in your scene.");
			}
		} else {
			Debug.LogError("Got a second instance of the class " + this.GetType());
		}
	}
}
