using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

	[SerializeField]
	private MeshFilter _filter;

	private Mesh _mesh;

	// The terrain points along the top of the mesh
	private List<Vector3> points = null;

	// Mutable lists for all the vertices and triangles of the mesh
	private List<Vector3> vertices = new List<Vector3> ();
	private List<int> triangles = new List<int> ();

	private float _pointPos = 0;

	[SerializeField]
	private Camera _camera;

	private float _yPos = 0, _lastYPos;

	private Vector3 CalculateBezierPoint (float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{

		float u = 1 - t;
		float tt = t * t;
		float uu = u * u;
		float uuu = uu * u;
		float ttt = tt * t;

		Vector3 p = uuu * p0;
		p += 3 * uu * t * p1;
		p += 3 * u * tt * p2;
		p += ttt * p3;

		return p;
	}

	void Awake ()
	{
		//points = new List<Vector3> ();
		//for (int i = 0; i < 4; i++) {            
		// Generate 4 random points
		//	points.Add (new Vector3 (_pointPos, 0, 0f));
		//	_pointPos += .25f;
		//}
		//GenerateCurve (0);
	}

	void Update ()
	{
		bool input = false;
		if (transform.position.y > -4) {
			if (Input.GetKey (KeyCode.S)) {
				input = true;
				_yPos -= .05f;
			}
		}

		if (transform.position.y < 4) {
			if (Input.GetKey (KeyCode.W)) {
				input = true;
				_yPos += .05f;
			}
		}

		if (!input) { 
			if (_yPos > 0) {
				_yPos -= .025f;
			} else if (_yPos < 0) {
				_yPos += .025f;
			} else if (_yPos <= .025f && _yPos >= -.025f) {
				_yPos = 0;
			}
		}

		transform.position = new Vector3 (_pointPos, Mathf.Clamp(transform.position.y + _yPos, -7.5f, 7.5f), 0);
		_pointPos += 1f;
	}

	void LateUpdate ()
	{
		//GenerateCurve (_yPos);
		_camera.transform.position = new Vector3 (transform.position.x - 20, 0, -10);
	}

	void AddTerrainPoint (Vector3 point)
	{
		if (vertices.Count > 500) {
			vertices.RemoveAt (0);
			vertices.RemoveAt (1);
			vertices.RemoveAt (2);
		}
		// Create a corresponding point along the bottom
		vertices.Add (new Vector3 (point.x, point.y - 1f, 0f));
		// Then add our top point
		vertices.Add (point);
		if (vertices.Count >= 4) {
			// We have completed a new quad, create 2 triangles
			int start = vertices.Count - 4;
			triangles.Add (start + 0);
			triangles.Add (start + 1);
			triangles.Add (start + 2);
			triangles.Add (start + 1);
			triangles.Add (start + 3);
			triangles.Add (start + 2);  
		}
	}

	private void GenerateCurve (float y)
	{
		_mesh = _filter.mesh;
		_mesh.Clear ();

		var bezVal = 0f;

		if (_yPos == _lastYPos)
			bezVal = 0f;
		else if (_yPos < _lastYPos)
			bezVal = -.01f;
		else
			bezVal = .01f;

		for (int i = 0; i < 4; i++) {            
			// Generate 4 random points
			points.Add (new Vector3 (_pointPos, y + (i * bezVal), 0f));
			_pointPos += .5f;
		}
		// Number of points to draw, how smooth the curve is
		int resolution = 20;
		for (int i = 0; i < resolution; i++) {
			float t = (float)i / (float)(resolution - 1);
			// Get the point on our curve using the 4 points generated above

			Vector3 p = CalculateBezierPoint (t, points [points.Count - 4], points [points.Count - 3], points [points.Count - 2], points [points.Count - 1]);
			AddTerrainPoint (p);
		}

		_mesh.vertices = vertices.ToArray ();
		_mesh.triangles = triangles.ToArray ();

		_lastYPos = y;
	}



}
