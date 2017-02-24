using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using WhitDataTypes;
using System.Reflection;
using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Globalization;
using System.Xml; 
using System.Xml.Serialization; 
using System.Text; 

public static class WhitTools {
	public const float PixelsToUnits = 1.0f / 100.0f;
	public const float UnitsToPixels = 100.0f;
	public const float GameUnitsToUnityUnits = 10.0f;
	public const float UnityUnitsToGameUnits = 1.0f/10.0f;
	public const float Slope2Rad = Mathf.PI / 2f;
	public const float Rad2Slope = 2f / Mathf.PI;
	public const float Slope2Deg = (Mathf.PI / 2f) * Mathf.Rad2Deg;
	public const float Deg2Slope = (2f / Mathf.PI) * Mathf.Deg2Rad;

	public static string UTF8ByteArrayToString(byte[] characters) {      
		UTF8Encoding encoding = new UTF8Encoding(); 
		string constructedString = encoding.GetString(characters); 
		return (constructedString); 
	} 

	public static byte[] StringToUTF8ByteArray(string pXmlString) { 
		UTF8Encoding encoding = new UTF8Encoding(); 
		byte[] byteArray = encoding.GetBytes(pXmlString); 
		return byteArray; 
	} 

	public static string SerializeObjectToXMLData<T>(object pObject) { 
		string XmlizedString = null; 
		MemoryStream memoryStream = new MemoryStream(); 
		XmlSerializer xs = new XmlSerializer(typeof(T)); 
		XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
		xs.Serialize(xmlTextWriter, pObject); 
		memoryStream = (MemoryStream)xmlTextWriter.BaseStream; 
		XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray()); 
		return XmlizedString; 
	} 

	public static T DeserializeObjectFromXMLData<T>(string pXmlizedString) { 
		XmlSerializer xs = new XmlSerializer(typeof(T)); 
		MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString)); 
		return (T)xs.Deserialize(memoryStream); 
	} 

	public static void CreateXML(string xmlData, string fileLocation, string fileName) { 
		StreamWriter writer; 
		FileInfo t = new FileInfo(fileLocation + "/" + fileName); 
		if(!t.Exists) { 
			writer = t.CreateText(); 
		} 
		else { 
			t.Delete(); 
			writer = t.CreateText(); 
		} 
		writer.Write(xmlData); 
		writer.Close(); 
	} 

	public static string LoadXML(string fileLocation, string fileName) {
		string path = fileLocation + "/" + fileName;
		if (!File.Exists(path)) CreateXML("", fileLocation, fileName);
		StreamReader r = File.OpenText(path); 
		string _info = r.ReadToEnd(); 
		r.Close(); 
		string xmlData = _info; 
		return xmlData;
	}

	public static string ColorToHex(Color32 color) {
		string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		return hex;
	}

	public static Color HexToColor(string hex) {
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		return new Color32(r,g,b, 255);
	}

	public static Texture2D TextureFromSprite(Sprite sprite) {
		if(sprite.rect.width != sprite.texture.width){
			Texture2D newText = new Texture2D((int)sprite.rect.width,(int)sprite.rect.height);
			Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x, 
				(int)sprite.textureRect.y, 
				(int)sprite.textureRect.width, 
				(int)sprite.textureRect.height );
			newText.SetPixels(newColors);
			newText.Apply();
			return newText;
		} else
			return sprite.texture;
	}

	public static bool IsLeft(Vector2 direction) {
		bool horizontal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
		return horizontal && direction.x < 0;
	}

	public static bool IsRight(Vector2 direction) {
		bool horizontal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
		return horizontal && direction.x > 0;
	}

	public static bool IsUp(Vector2 direction) {
		bool vertical = Mathf.Abs(direction.x) < Mathf.Abs(direction.y);
		return vertical && direction.y > 0;
	}

	public static bool IsDown(Vector2 direction) {
		bool vertical = Mathf.Abs(direction.x) < Mathf.Abs(direction.y);
		return vertical && direction.y < 0;
	}

	public static MethodInfo GetMethod(string methodName, object target) {
		MethodInfo method = target.GetType().GetMethod(methodName,
			System.Reflection.BindingFlags.Instance |
			System.Reflection.BindingFlags.Public |
			System.Reflection.BindingFlags.NonPublic |
			System.Reflection.BindingFlags.InvokeMethod);
		return method;
	}

	public static int GetRandomSign() {
		return (int)Mathf.Sign(UnityEngine.Random.value - 0.5f);
	}

	public static Delegate CreateDelegate<T>(string methodName, object target, T Default) where T : class {
		MethodInfo method = GetMethod(methodName, target);
		return CreateDelegate<T>(method, target, Default);
	}

	public static Delegate CreateDelegate<T>(MethodInfo method, object target, T Default) where T : class {
		if (method == null) return Default as Delegate;
		else return Delegate.CreateDelegate(typeof(T), target, method);
	}

	public static Color GetColorWithRandomHue(float s, float v, float a = 1.0f) {
		HSVColor c = new HSVColor(UnityEngine.Random.value, s, v, a);
		return WadeUtils.HSVToRGB(c);
	}

	public static float GetFrustumHeight(Camera camera, float distance) {
		float frustumHeight = 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
		return frustumHeight;
	}

	public static float GetFrustumWidth(Camera camera, float distance) {
		return FrustumHeightToWidth(camera, GetFrustumHeight(camera, distance));
	}

	public static float FrustumHeightToWidth(Camera camera, float frustumHeight) {
		float frustumWidth = frustumHeight * camera.aspect;
		return frustumWidth;
	}

	public static float FrustumWidthToHeight(Camera camera, float frustumWidth) {
		float frustumHeight = frustumWidth / camera.aspect;
		return frustumHeight;
	}

	public static float GetDistanceAtFrustumHeight(Camera camera, float frustumHeight) {
		float distance = frustumHeight * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
		return distance;
	}

	public static float GetDistanceAtFrustumWidth(Camera camera, float frustumWidth) {
		return GetDistanceAtFrustumHeight(camera, FrustumWidthToHeight(camera, frustumWidth));
	}

	public static float GetFieldOfView(Camera camera, float distance, float frustumHeight) {
		float fieldOfView = 2.0f * Mathf.Atan(frustumHeight * 0.5f / distance) * Mathf.Rad2Deg;
		return fieldOfView;
	}
		
	public static void SetTimeScale(float timeScale) {
		Time.timeScale = timeScale;
		Time.fixedDeltaTime = (1.0f/60.0f) * timeScale;
	}

	public static float Project(Vector2 a, Vector2 b) {
		float projection = Vector2.Dot(a, b) / a.magnitude;
		return projection;
	}

	public static Vector2 SlopeToDirection(float slope) {
		Vector2 slopeVector = new Vector2();
		float angle = slope * Slope2Rad;
		slopeVector.x = Mathf.Cos(angle);
		slopeVector.y = Mathf.Sin(angle);
		slopeVector.Normalize();
		return slopeVector;
	}

	public static float DirectionToSlope(Vector2 direction) {
		float angle = Mathf.Atan2(direction.y, direction.x);
		float slope = angle * Rad2Slope;
		return slope;
	}

	public static Vector2 GetAveragePoint(params Vector2[] points) {
		Vector2 sumPoints = GetSumPoint(points);
		Vector2 averagePoint = sumPoints / points.Length;
		return averagePoint;
	}

	public static Vector2 GetSumPoint(params Vector2[] points) {
		Vector2 sumPoints = Vector2.zero;
		foreach (Vector2 point in points) sumPoints += point;
		return sumPoints;
	}

	public static T CreateGameObjectWithComponent<T>(string name = "GameObject") {
		T obj = new GameObject(name, typeof(T)).GetComponent<T>();
		return obj;
	}

	public static void Invoke(UnityEvent unityEvent) {
		if (unityEvent != null) unityEvent.Invoke();
	}

	public static bool Assert(bool condition, string errorString = "") {
		if (!condition) Debug.LogError(errorString);
		return condition;
	}

	public static int IncrementWithWrap(int value, IntRange wrapRange) {
		value++;
		if (value >= wrapRange.max) value -= wrapRange.difference;
		return value;
	}

	public static int DecrementWithWrap(int value, IntRange wrapRange) {
		value--;
		if (value <= wrapRange.min) value += wrapRange.difference;
		return value;
	}

	public static Vector2 AngleToDirection(float angle) {
		Vector2 direction = (Vector2)(Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right);
		return direction;
	}

	public static bool IsInLayer(GameObject obj, LayerMask layerMask) {
		int objLayerMask = (1 << obj.layer);
		bool isInLayer = (layerMask.value & objLayerMask) > 0;
		return isInLayer;
	}

	public static bool IsInLayer(GameObject obj, string layerName) {
		bool isInLayer = LayerMask.NameToLayer(layerName) == obj.layer;

		return isInLayer;
	}

	public static bool CompareLayerMasks(LayerMask mask1, LayerMask mask2) {
		bool masksAreTheSame = (mask1.value & mask2.value) > 0;
		return masksAreTheSame;
	}

	public static float DirectionToAngle(Vector2 direction) {
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		return angle;
	}

	// based on algorithm found here: http://www.geometrylab.de/applet-29-en#twopeasants
	public static void SortWithTwoPeasantsPolygonAlgorithm(List<Vector2> pointVectors) {
		List<Point> points_preSort = new List<Point>();
		List<Point> points_postSort = new List<Point>();
		foreach (Vector2 pointVector in pointVectors) points_preSort.Add(new Point(pointVector));

		Segment segment = GetMaxXSegment(points_preSort);
		points_preSort.Remove(segment.pointA);
		points_preSort.Remove(segment.pointB);
		points_postSort.Add(segment.pointA);

		Point previousPoint;
		List<Point> pointsAbove;
		List<Point> pointsBelow;

		pointsAbove = GetPointsAboveSegment(points_preSort, segment);
		previousPoint = segment.pointA;
		while (pointsAbove.Count > 0) {
			Point nextPoint = GetPointClosestToPoint(previousPoint, pointsAbove);
			pointsAbove.Remove(nextPoint);
			points_preSort.Remove(nextPoint);
			points_postSort.Add(nextPoint);
			previousPoint = nextPoint;
		}

		points_postSort.Add(segment.pointB);

		pointsBelow = GetPointsBelowSegment(points_preSort, segment);
		previousPoint = segment.pointB;
		while (pointsBelow.Count > 0) {
			Point nextPoint = GetPointClosestToPoint(previousPoint, pointsBelow);
			pointsBelow.Remove(nextPoint);
			points_preSort.Remove(nextPoint);
			points_postSort.Add(nextPoint);
			previousPoint = nextPoint;
		}

		pointVectors.Clear();

		for (int i = 0; i < points_postSort.Count; i++) {
			Point point = points_postSort[i];
			pointVectors.Add(point.vector);
		}
	}

	private static Segment GetMaxXSegment(List<Point> points) {
		Point leftMost = points[0];
		Point rightMost = points[points.Count - 1];
		foreach (Point point in points) {
			leftMost = point.vector.x < leftMost.vector.x ? point : leftMost;
			rightMost = point.vector.x > rightMost.vector.x ? point : rightMost;
		}
		return new Segment(leftMost, rightMost);
	}

	private static List<Point> GetPointsAboveSegment(List<Point> points, Segment segment) {
		var abovePoints = new List<Point>();
		foreach (Point point in points) {
			if (PointIsAboveSegment(point, segment)) abovePoints.Add(point);
		}
		return abovePoints;
	}

	private static List<Point> GetPointsBelowSegment(List<Point> points, Segment segment) {
		var belowPoints = new List<Point>();
		foreach (Point point in points) {
			if (!PointIsAboveSegment(point, segment)) belowPoints.Add(point);
		}
		return belowPoints;
	}

	private static Point GetPointClosestToPoint(Point pointToCheck, List<Point> points) {
		float closestDist = Mathf.Infinity;
		Point closestPoint = null;
		foreach (Point point in points) {
			float dist = Mathf.Abs(pointToCheck.x - point.x);
			if (dist < closestDist) {
				closestDist = dist;
				closestPoint = point;
			}
		}

		return closestPoint;
	}

	private static bool PointIsAboveSegment(Point point, Segment segment) {
		float projectedY = segment.slope * point.x + segment.yIntercept;
		return point.y > projectedY;
	}
}
