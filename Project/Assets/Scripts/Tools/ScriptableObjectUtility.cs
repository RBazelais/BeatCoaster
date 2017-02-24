using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

public static class ScriptableObjectUtility {
	public static T CreateAsset<T> (string name) where T : ScriptableObject {
		T asset = ScriptableObject.CreateInstance<T> ();
		#if UNITY_EDITOR
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") path = "Assets/Resources";
		else if (Path.GetExtension (path) != "") path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/" + name + ".asset");
		
		AssetDatabase.CreateAsset (asset, assetPathAndName);
		
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
		#endif
		return asset;
	}

	public static T CreateAsset<T> () where T : ScriptableObject {
		return CreateAsset<T>(typeof(T).ToString());
	}
}