using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TutorialPhaseOneScript : MonoBehaviour {

	RoomManager roomMan;
	public Text introText;

	public GameObject tutorialDupe;
	public GameObject speechBubble;
	public CameraScript cameraOne;

	string intro1Words1 = "Your subject for this painting is \"elephant\"";
	string intro1Words2 = "This outline is just a suggestion to show direction and general shape";
	string intro1Words3 = "You were assigned the top right corner, so you only need to draw the elephant's head";
	string intro1Words4 = "These brushes are where you connect with the other players' drawings";
	string intro1Words5 = "By drawing on the brushes you will create a complete masterpiece!";
	string intro1Words6 = "Start drawing an elephant head!";

	public LineSpawnerScipt lineSpawn;
	public LocalRoomManager localRoom;

	public SpriteRenderer elephant;

	public Vector2[] brushes;
	public GameObject brushHolder;

	public GameObject brushPrefabY;
	public GameObject brushPrefabX;

	bool okayToClick = false;
	bool readyToAdvance;

	string drawing1Location = "drawing1Location";

	// Use this for initialization
	void Start () {

		cameraOne.ZoomInTutorial(2);
		lineSpawn.dontDraw = true;
		Invoke ("OpenCurtains", 1.5f);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OpenCurtains (){
	
		roomMan = GameObject.FindGameObjectWithTag ("Room Manager").GetComponent<RoomManager>();
		if (roomMan != null) {
			roomMan.CurtainsOut ();
		}

	}

	public void IntroBubble () {

		if (introText.text == intro1Words1) {
			introText.text = intro1Words2;
		} else if (introText.text == intro1Words2){
			speechBubble.SetActive (false);
			cameraOne.MoveToSectionTutorial ();
			introText.text = intro1Words3;
		} else if (introText.text == intro1Words3){
			FadeOutElephant ();
			LoadBrushes ();
			introText.text = intro1Words4;
		} else if (introText.text == intro1Words4){
			introText.text = intro1Words5;
		} else if (introText.text == intro1Words5){
			introText.text = intro1Words6;
		} else if (introText.text == intro1Words6){
			tutorialDupe.SetActive (false);
			lineSpawn.dontDraw = false;
			okayToClick = true;
		}
	}

	public void StartGame(){

		localRoom.MoveUpPanel ();
		Invoke ("BubbleTwo", 1.0f);
		lineSpawn.GetColor (2);

	}

	void BubbleTwo(){

		speechBubble.SetActive (true);
	}

	void FadeOutElephant (){
	
		Color clear = new Color (1,1,1,0);
		elephant.DOColor(clear,1.0f);
	
	}

	void LoadBrushes (){

		int myColor = 2;

		for (int i = 0; i < brushes.Length; i++) {

			Vector3 brushPos = brushes [i];
			Vector3 newBrushPos;

			if (brushes [i].x == 0) {

				if (myColor == 1 || myColor == 4) {
					newBrushPos = new Vector3 (brushPos.x + 1, brushPos.y, brushPos.z);
				} else {
					newBrushPos = new Vector3 (brushPos.x - 1, brushPos.y, brushPos.z);
				}

				GameObject newBrush = (GameObject)Instantiate (brushPrefabY, newBrushPos, Quaternion.identity);
				newBrush.transform.SetParent (brushHolder.transform, false);
				newBrush.transform.DOLocalMoveX (0, 1.0f);

			} else {

				if (myColor == 1 || myColor == 2) {
					newBrushPos = new Vector3 (brushPos.x, brushPos.y - 1, brushPos.z);
				} else {
					newBrushPos = new Vector3 (brushPos.x, brushPos.y + 1, brushPos.z);
				}

				GameObject newBrush = (GameObject)Instantiate (brushPrefabX, newBrushPos, Quaternion.identity);
				newBrush.transform.SetParent (brushHolder.transform, false);
				newBrush.transform.DOLocalMoveY (0, 1.0f);
			}
		}

	}
	public void DoneDrawing (){

		int myColor = 2;

		if (okayToClick == false) {
			return;
		}

		okayToClick = false; 
		Invoke ("OkayToClick", 2.5f);

		int brushCount = brushHolder.transform.childCount;

		float xMax;
		float xMin;
		float yMax;
		float yMin;

		if (myColor == 1) {
			xMax = 0;
			xMin = -10;
			yMax = 10;
			yMin = 0;
		} else if (myColor == 2) {
			xMax = 10;
			xMin = 0;
			yMax = 10;
			yMin = 0;
		} else if (myColor == 3) {
			xMax = 10;
			xMin = 0;
			yMax = 0;
			yMin = -10;
		} else if (myColor == 4) {
			xMax = 0;
			xMin = -10;
			yMax = 0;
			yMin = -10;
		} else {
			xMax = 0;
			xMin = -10;
			yMax = 10;
			yMin = 0;
		}

		readyToAdvance = true;

		for (int i = 0; i < brushCount; i++) {

			Vector3 brushPos = brushHolder.transform.GetChild (i).transform.position;
			if (brushPos.y == 0 && brushPos.x > xMin && brushPos.x < xMax) {
				//Debug.Log (brushPos.y);
				readyToAdvance = false;
			} else if (brushPos.x == 0 && brushPos.y > yMin && brushPos.y < yMax) {
				readyToAdvance = false;
				//Debug.Log (brushPos.x);
			}
		}

		if (readyToAdvance == true) {
			CollectYourLineData ();
		} else {
			localRoom.StillHaveBrushes ();
		}
	}

	void OkayToClick (){
		okayToClick = true;
	}

	void CollectYourLineData () {


		string myLineString;
		roomMan.CurtainsIn ();

		GameObject[] lines = GameObject.FindGameObjectsWithTag ("Line");

		myLineString = "[MYCOLOR]" + "2:";

		foreach (GameObject line in lines) {

			LineRenderer lineRend = line.GetComponent<LineRenderer> ();
			int lineAmount = lineRend.numPositions;

			for (int i = 0; i < lineAmount; i++) {

				Vector2 point = lineRend.GetPosition (i);


				myLineString = myLineString + point.ToString ("F2") + "@";

				if (i == lineAmount - 1) {

					string[] charsToRemove = new string[] { "(", ")", " "};
					foreach (string character in charsToRemove)
					{
						myLineString = myLineString.Replace(character, string.Empty);
					}


					myLineString = myLineString.Replace("0.00", "0");
					myLineString = myLineString.Replace("-0.", "-.");
					myLineString = myLineString.Replace("0.", ".");
					myLineString = myLineString.Replace("++", "+");
					myLineString = myLineString.Replace(":+", ":");


				}

			}

			myLineString = myLineString.TrimEnd('@');
			myLineString = myLineString + "+";
		}
		myLineString = myLineString.TrimEnd('+');
		myLineString = myLineString + ":";

		GameObject[] dots = GameObject.FindGameObjectsWithTag ("Dot");

		foreach (GameObject dot in dots) {

			LineRenderer lineRend = dot.GetComponent<LineRenderer> ();
			int lineAmount = lineRend.numPositions;

			for (int i = 0; i < lineAmount; i++) {

				Vector2 point = lineRend.GetPosition (i);

				myLineString = myLineString + point.ToString ("F2") + "@";

				if (i == lineAmount - 1) {

					string[] charsToRemove = new string[] { "(", ")", " "};
					foreach (string character in charsToRemove)
					{
						myLineString = myLineString.Replace(character, string.Empty);
					}

					myLineString = myLineString.Replace("0.00", "0");
					myLineString = myLineString.Replace("-0.", "-.");
					myLineString = myLineString.Replace("0.", ".");

				}

			}

			myLineString = myLineString.TrimEnd('@');
			myLineString = myLineString + "+";
		}

		myLineString = myLineString.TrimEnd('+');
		myLineString = myLineString + "$";

		Debug.Log (myLineString);

		PlayerPrefs.SetString (drawing1Location, myLineString);

		Invoke ("StartNewScene", 1.0f);

	}

	void StartNewScene (){

		SceneManager.LoadScene ("Tutorial Lobby Menu 2");

	}

}
