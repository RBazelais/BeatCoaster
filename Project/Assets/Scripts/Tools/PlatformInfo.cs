using UnityEngine;
using System.Collections;

public static class PlatformInfo {
	public static bool IsMobile() {
#if UNITY_IPHONE || UNITY_ANDROID
		return true;
#else
		return false;
#endif
	}

	public static bool IsiOS() {
#if UNITY_IPHONE
		return true;
#else
		return false;
#endif
	}

	public static bool IsAndroid() {
#if UNITY_ANDROID
		return true;
#else
		return false;
#endif
	}

	public static bool IsStandalone() {
#if UNITY_STANDALONE
		return true;
#else
		return false;
#endif
	}

	public static bool IsEditor() {
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
		return true;
#else
		return false;
#endif
	}

	public static bool IsLowMemoryiOSDevice() {
		if (IsiOS()) {
			if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen ||
				UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen ||
				UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen ||
				UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen ||
			    UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch5Gen ||
			    UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPad1Gen      ||
			    UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPad2Gen      ||
			    UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone        ||
			    UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone3GS     ||
			    UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone3G      ||
			    UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone4       ||
			    UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone4S)
				return true;
		}
		
		return false;
	}
}
