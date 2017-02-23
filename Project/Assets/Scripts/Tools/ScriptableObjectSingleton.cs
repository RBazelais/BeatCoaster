using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T> {
	static T _instance = null;
	public static T instance {
		get {
			if(!_instance) {
				
				_instance = Resources.LoadAll<T>("")[0] as T;
#if UNITY_EDITOR
				if(!_instance) _instance = ScriptableObjectUtility.CreateAsset<T>();
#endif
			}
			
			return _instance;
		}
	}
}