using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using System.Linq;

public static class ListExtensions {
	public static void SortWithTwoPeasantsPolygonAlgorithm(this List<Vector2> points) {
		WhitTools.SortWithTwoPeasantsPolygonAlgorithm(points);
	}

	public static bool AssertIndexIsInBounds<T>(this List<T> list, int index) {
		if (list.Count == 0) {
			Debug.LogError("no items in list!");
			return false;
		}
		else if (index < 0) {
			Debug.LogError("index " + index + " is below zero!");
			return false;
		}
		else if (index >= list.Count) {
			Debug.LogError("index " + index + " is too high!");
			return false;
		}
		else return true;
	}

	public static void RemoveDuplicates<T>(this List<T> list, List<T> otherList) {
		foreach (T obj in otherList) {
			if (list.Contains(obj)) {
				list.Remove(obj);
			}
		}
	}

	public static void AddAll<T>(this List<T> list, List<T> otherList, bool ignoreDuplicates = false) {
		foreach (T item in otherList) {
			if (ignoreDuplicates) {
				if (!list.Contains(item)) {
					list.Add(item);
				}
			}
			else {
				list.Add(item);
			}
		}
	}

	public static void AddIfNotDuplicate<T>(this List<T> list, T obj) {
		if (!list.Contains(obj)) list.Add(obj);
	}

	public static void AddAll<T>(this List<T> list, T[] array) {
		foreach (T item in array) list.Add(item);
	}

	public static List<T> Copy<T>(this List<T> list) {
		List<T> newList = new List<T>();
		for (int i = 0; i < list.Count; i++) newList.Add(list[i]);
		return newList;
	}

	public static T GetLast<T>(this List<T> list) {
		if (list.Count == 0) return default(T);

		return list[list.Count - 1];
	}
		
	public static T GetFirst<T>(this List<T> list) {
		if (list.Count == 0) return default(T);

		return list[0];
	}

	public static T GetRandom<T>(this List<T> list) {
		if (list.Count == 0) return default(T);

		return list[UnityEngine.Random.Range(0, list.Count)];
	}
		
	public static T GetPenultimate<T>(this List<T> list) {
		if (list.Count < 2) return default(T);

		return list[list.Count - 2];
	}

	public static List<T> ToList<T>(this Array array) {
		return array.OfType<T>().ToList();
	}
}
