﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LineSpawnerScipt : MonoBehaviour {

	GameObject linePrefab;
	GameObject dotPrefab;
	Transform currentLine;

	public GameObject redPrefab;
	public GameObject bluePrefab;
	public GameObject greenPrefab;
	public GameObject orangePrefab;
	public GameObject blackPrefab;

	public GameObject redDot;
	public GameObject blueDot;
	public GameObject greenDot;
	public GameObject orangeDot;

	LineScript activeLine;

	private float xMin;
	private float xMax;
	private float yMin;
	private float yMax;

	public bool drawingScene;
	public bool dontDraw;

	// Use this for initialization
	void Start () {

		if (drawingScene == true) {
			linePrefab = blackPrefab;
			xMin = -3.8f;
			xMax = 3.8f;
			yMin = -3.83f;
			yMax = 3.8f;
		}
	}

	public void GetColor(int color) {
	
		if (color == 1) {
			xMin = -2.43f;
			xMax = -0.033f;
			yMin = 0.033f;
			yMax = 3.61f;
			linePrefab = redPrefab;
			dotPrefab = redDot;

		}

		else if (color == 2) {
			xMin = .033f;
			xMax = 2.43f;
			yMin = 0.033f;
			yMax = 3.61f;
			linePrefab = bluePrefab;
			dotPrefab = blueDot;

		}

		else if (color == 3) {
			xMin = .033f;
			xMax = 2.43f;
			yMin = -3.61f;
			yMax = -0.033f;
			linePrefab = greenPrefab;
			dotPrefab = greenDot;
		}

		else if (color == 4) {
			xMin = -2.43f;
			xMax = -0.033f;
			yMin = -3.61f;
			yMax = -0.033f;
			linePrefab = orangePrefab;
			dotPrefab = orangeDot;
		}
	
	}

	
	// Update is called once per frame
	void Update () {

		if (linePrefab == null) {
			return;
		}

		if (dontDraw == true) {
			return;
		}

		if (Input.GetKeyDown(KeyCode.Q) && drawingScene == true){

			UndoLine ();

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
			
			if (activeLine != null) {
				activeLine.DestroyCollider ();
			}

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

		if (midPoint == Vector2.zero) {
			Vector2 newMid = new Vector2 (point.x, point.y);
			midPoint = newMid;
		}

		activeLine = null;
		GameObject lineGo = Instantiate (linePrefab);
		activeLine = lineGo.GetComponent<LineScript> ();
		activeLine.lineSpawn = gameObject.GetComponent <LineSpawnerScipt> ();
		activeLine.SetPoint (midPoint);
		activeLine.SetPoint (point);
		lineGo.transform.SetParent (currentLine, false);

	}

	public void MakeNewDot (Vector3 firstPoint, Vector3 secondPoint, Vector3 thirdPoint, GameObject brushHit){
	
		GameObject dotGo = Instantiate (dotPrefab);
		LineRenderer dotRend = dotGo.GetComponent<LineRenderer> ();

		dotRend.numPositions = 3;
		dotRend.SetPosition (0, firstPoint);
		dotRend.SetPosition (1, secondPoint);
		dotRend.SetPosition (2, thirdPoint);
		dotGo.transform.SetParent (currentLine, false);
		currentLine.GetComponent<LineScript> ().ParentBrush (brushHit);

	}

	public void UndoLine () {

		int childCount = transform.childCount - 1;

		if (childCount < 0){
			return;
		}

		GameObject line = transform.GetChild(transform.childCount - 1).gameObject;

		List<GameObject> brushes = line.GetComponent<LineScript> ().hitBrushes;

		foreach (GameObject brush in brushes) {

			if (brush.tag == "Brush X") {
			
				brush.transform.DOLocalMoveY (0, 1.0f);
			
			} else {
			
				brush.transform.DOLocalMoveX (0, 1.0f);
			
			}

		}

		Destroy (line);

	}
		
}
