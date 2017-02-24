using UnityEngine;
using System.Collections;

namespace WhitDataTypes {
	public class UVRect {
		public Vector2 bottomLeft;
		public Vector2 topLeft;
		public Vector2 topRight;
		public Vector2 bottomRight;

		public float width {
			get {
				return bottomRight.x - bottomLeft.x;
			}
		}

		public float height {
			get {
				return topLeft.y - bottomLeft.y;
			}
		}

		public UVRect() {

		}

		public void SetPadding(float padding) {
			float horizontalPadding = width * padding;
			float verticalPadding = height * padding;

			bottomLeft.x += horizontalPadding;
			bottomRight.x -= horizontalPadding;
			topLeft.x += horizontalPadding;
			topRight.x -= horizontalPadding;

			bottomLeft.y += verticalPadding;
			topLeft.y -= verticalPadding;
			bottomRight.y += verticalPadding;
			topRight.y -= verticalPadding;
		}
	}
}