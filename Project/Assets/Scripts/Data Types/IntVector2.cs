using UnityEngine;
using System.Collections;

namespace WhitDataTypes {
	[System.Serializable]
	public struct IntVector2 {
		public int x;
		public int y;

		public static IntVector2 zero {
			get {return new IntVector2(0, 0);}
		}
		
		public IntVector2(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public IntVector2(float x, float y) {
			this.x = (int)x;
			this.y = (int)y;
		}
	}
}