using UnityEngine;
using System.Collections;

namespace WhitDataTypes {
	[System.Serializable]
	public class Point {
		public Vector2 vector;

		public float x {
			get {return vector.x;}
			set {vector.x = value;}
		}
		public float y {
			get {return vector.y;}
			set {vector.y = value;}
		}

		public Point() {
			vector = Vector2.zero;
		}

		public Point(float x, float y) {
			vector = new Vector2(x, y);
		}

		public Point(Vector2 point) {
			this.vector = point;
		}
	}
}