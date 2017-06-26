using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSpawnerScipt : MonoBehaviour {

	GameObject linePrefab;
	Transform currentLine;

	public GameObject redPrefab;
	public GameObject bluePrefab;
	public GameObject greenPrefab;
	public GameObject orangePrefab;

	LineScript activeLine;

	private float xMin;
	private float xMax;
	private float yMin;
	private float yMax;


	// Use this for initialization
	void Start () {

		
	}

	public void GetColor(int color) {
	
		if (color == 1) {
			xMin = -2.43f;
			xMax = -0.033f;
			yMin = 0.033f;
			yMax = 3.61f;
			linePrefab = redPrefab;

		}

		else if (color == 2) {
			xMin = .033f;
			xMax = 2.43f;
			yMin = 0.033f;
			yMax = 3.61f;
			linePrefab = bluePrefab;

		}

		else if (color == 3) {
			xMin = .033f;
			xMax = 2.43f;
			yMin = -3.61f;
			yMax = -0.033f;
			linePrefab = greenPrefab;
		}

		else if (color == 4) {
			xMin = -2.43f;
			xMax = -0.033f;
			yMin = -3.61f;
			yMax = -0.033f;
			linePrefab = orangePrefab;
		}
	
	}

	
	// Update is called once per frame
	void Update () {

		if (linePrefab == null) {
			return;
		}

		if (Input.GetMouseButtonDown (0)) {
		
			Vector2 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			if (mousePos.x > xMax) {
				return;
			} else if (mousePos.x < xMin) {
				return;
			}

			if (mousePos.y > yMax) {
				return;
			} else if (mousePos.y < yMin) {
				return;
			}


			GameObject lineGo = Instantiate (linePrefab);
			activeLine = lineGo.GetComponent<LineScript> ();
			activeLine.lineSpawn = gameObject.GetComponent <LineSpawnerScipt> ();
			lineGo.transform.SetParent (gameObject.transform, false);
			currentLine = lineGo.transform;
		
		}

		if (Input.GetMouseButtonUp (0)) {
		
			activeLine = null;
		
		}

		if (activeLine != null) {

		
			Vector2 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			if (mousePos.x > xMax) {
				mousePos.x = xMax;
			} else if (mousePos.x < xMin) {
				mousePos.x = xMin;
			}

			if (mousePos.y > yMax) {
				mousePos.y = yMax;
			} else if (mousePos.y < yMin) {
				mousePos.y = yMin;
			}

			activeLine.UpdateLine (mousePos);
		
		}
	}

	public void MakeNewLine (Vector2 point, Vector2 midPoint) {

		activeLine = null;
		GameObject lineGo = Instantiate (linePrefab);
		activeLine = lineGo.GetComponent<LineScript> ();
		activeLine.lineSpawn = gameObject.GetComponent <LineSpawnerScipt> ();
		activeLine.SetPoint (midPoint);
		activeLine.SetPoint (point);
		lineGo.transform.SetParent (currentLine, false);

	}

	public void UndoLine () {

		int childCount = transform.childCount - 1;

		if (childCount < 0){
			return;
		}

		GameObject line = transform.GetChild(transform.childCount - 1).gameObject;

		Destroy (line);

	}
		
}
