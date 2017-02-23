using UnityEngine;
using System.Collections;

namespace WhitDataTypes {
	public class Tri {
		public Vector2 point1 = Vector2.zero;
		public Vector2 point2 = Vector2.zero;
		public Vector2 point3 = Vector2.zero;

		public Tri(Vector2 point1, Vector2 point2, Vector2 point3) {
			this.point1 = point1;
			this.point2 = point2;
			this.point3 = point3;
		}
	}
}