using UnityEngine;
using System.Collections;

namespace WhitDataTypes {
	public class Segment {
		public Point pointA;
		public Point pointB;

		public float slope {get; private set;}
		public float yIntercept {get; private set;}

		public Segment(Point pointA, Point pointB) {
			this.pointA = pointA;
			this.pointB = pointB;

			CalculateSlope();
			CalculateYIntercept();
		}

		private void CalculateSlope() {
			slope = (pointB.y - pointA.y) / (pointB.x - pointA.x);
		}

		private void CalculateYIntercept() {
			yIntercept = pointA.y - slope * pointA.x;
		}
	}
}