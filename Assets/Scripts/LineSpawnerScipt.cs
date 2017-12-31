using System.Collections;
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
	public bool portraitScene;
	public bool dontDraw;

	public Material[] lineMats;
	public Color[] regColors;

	bool drawing = false;
	public Transform portraitCircle;
	Vector2 centerPos;
	public float radius = 1.5f;
	float offset = 0;

	// Use this for initialization
	void Start () {

		if (drawingScene == true) {
			linePrefab = blackPrefab;
			xMin = -3.8f;
			xMax = 3.8f;
			yMin = -3.83f;
			yMax = 3.8f;
		}

		if (portraitScene == true) {
			linePrefab = blackPrefab;
			centerPos = portraitCircle.localPosition;
			xMin = -3.8f + offset;
			xMax = 3.8f + offset;
			yMin = -3.83f;
			yMax = 3.8f;
		}

		for (int i = 0; i < lineMats.Length; i++) {
			lineMats[i].color = regColors[i];
		}

	}

	public void GetColor(int color) {
	
		if (color == 1) {
			xMin = -2.46f;
			xMax = -0.032f;
			yMin = 0.03f;
			yMax = 3.64f;
			linePrefab = redPrefab;
			dotPrefab = redDot;

		}

		else if (color == 2) {
			xMin = .032f;
			xMax = 2.46f;
			yMin = 0.03f;
			yMax = 3.64f;
			linePrefab = bluePrefab;
			dotPrefab = blueDot;

		}

		else if (color == 3) {
			xMin = .032f;
			xMax = 2.46f;
			yMin = -3.64f;
			yMax = -0.03f;
			linePrefab = greenPrefab;
			dotPrefab = greenDot;
		}

		else if (color == 4) {
			xMin = -2.46f;
			xMax = -0.032f;
			yMin = -3.64f;
			yMax = -0.03f;
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
			

		if (Input.GetMouseButton (0) && drawing == false) {
		
			//Debug.Log ("SCREEN!");

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

			drawing = true;

			GameObject lineGo = Instantiate (linePrefab);
			activeLine = lineGo.GetComponent<LineScript> ();
			activeLine.lineSpawn = gameObject.GetComponent <LineSpawnerScipt> ();
			lineGo.transform.SetParent (gameObject.transform, false);
			currentLine = lineGo.transform;
		
		}

		if (Input.GetMouseButtonUp (0)) {

			drawing = false;

			if (activeLine != null) {
				activeLine.SmoothTheLine ();
				activeLine.DestroyCollider ();
			}


			activeLine = null;
		
		}

		if (activeLine != null) {

		
			Vector2 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			if (portraitScene == false) {

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
			} else {
				
				 //radius of *black circle*
				//Vector2 centerPosition = portraitCircle.localPosition; //center of *black circle*
				float distance = Vector3.Distance(mousePos, centerPos); //distance from ~green object~ to *black circle*

				if (distance > radius) //If the distance is less than the radius, it is already within the circle.
				{
					Vector2 fromOriginToObject = mousePos - centerPos; //~GreenPosition~ - *BlackCenter*
					fromOriginToObject *= radius / distance; //Multiply by radius //Divide by Distance
					mousePos = centerPos + fromOriginToObject; //*BlackCenter* + all that Math
				}

				activeLine.UpdateLine (mousePos);

			}
		}
	}

	public void MakeNewLine (Vector2 point, Vector2 midPoint) {

		if (midPoint == Vector2.zero) {
			Vector2 newMid = new Vector2 (point.x, point.y);
			midPoint = newMid;
		}
		activeLine.SmoothTheLine ();
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

		dotRend.positionCount = 3;
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
