using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using System.Linq;
using WhitDataTypes;
using System.Security.Cryptography;

public static class WhitExtensions {
	public static void IncrementWithWrap(this int value, IntRange wrapRange) {
		value = WhitTools.IncrementWithWrap(value, wrapRange);
	}

	public static void DecrementWithWrap(this int value, IntRange wrapRange) {
		value = WhitTools.DecrementWithWrap(value, wrapRange);
	}

	public static void Push<T>(this Stack<T> stack, List<T> list) {
		foreach (T obj in list) {
			stack.Push(obj);
		}
	}

	public static Vector2 GetConnectedAnchorInWorldPosition(this SpringJoint2D springJoint) {
		return springJoint.connectedBody.transform.TransformPoint(springJoint.connectedAnchor);
	}

	public static Vector2 GetAnchorInWorldPosition(this SpringJoint2D springJoint) {
		return springJoint.transform.TransformPoint(springJoint.anchor);
	}

	public static Vector2[] ToVector2Array(this List<Point> list) {
		Vector2[] array = new Vector2[list.Count];
		for (int i = 0; i < list.Count; i++) array[i] = list[i].vector;
		return array;
	}

	public static void SetViewportPosition(this RectTransform rectTransform, Vector2 viewportPosition) {
		rectTransform.anchorMin = viewportPosition;
		rectTransform.anchorMax = viewportPosition;
	}

	public static void Shuffle<T>(this IList<T> list) {
		RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
		int n = list.Count;
		while (n > 1)
		{
			byte[] box = new byte[1];
			do provider.GetBytes(box);
			while (!(box[0] < n * (Byte.MaxValue / n)));
			int k = (box[0] % n);
			n--;
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static void SetWidth(this RectTransform rt, float size) {
		rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
	}

	public static void SetHeight(this RectTransform rt, float size) {
		rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
	}

	public static float GetWidth(this RectTransform rt) {
		return rt.rect.width;
	}

	public static float GetHeight(this RectTransform rt) {
		return rt.rect.height;
	}
}
