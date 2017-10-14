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

	//string intro1Words1 = "Your subject for this painting is \"elephant\"";
	string intro1Words2 = "This outline is just a suggestion to show DIRECTION AND SHAPE";
	string intro1Words3 = "You're drawing the top right corner, so you must DRAW THE ELEPHANT'S HEAD";
	string intro1Words4 = "Connect to the other players by drawing to these brushes";
	string intro1Words5 = "Drawing to these brushes will create a connected masterpiece!";
	string intro1Words6 = "Start drawing an elephant head!";

	string intro2Words1 = "This time the category is \"SUPER HEROES\" and your subject is \"THE FLASH\"";
//	string intro2Words2 = "Try not to draw so obviously this time so the dupe can't guess it";

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

	public RectTransform bubbleOne;

	public bool drawing1;
	public bool drawing2;

	int playerNum = 0;

	public GameObject drawButton;

	public RectTransform bottomPanel;
	Vector3 panelScreen;
	Vector3 panelScreenOff;

	public GameObject readyButts;
	public GameObject drawingButts;

	public RectTransform fingerOne;
	Vector2 offscreenPosF1;
	Vector2 screenPosF1;

	public Transform brushOne;
	public Transform brushTwo;

//	public RectTransform fingerTwo;
//	Vector2 offscreenPosF2;
//	Vector2 screenPosF2;
//	Vector2 midScreenPos;


	void Awake (){
		roomMan = GameObject.FindGameObjectWithTag ("Room Manager").GetComponent<RoomManager> ();
	}

	// Use this for initialization
	void Start () {

		screenPosF1 = fingerOne.anchoredPosition;
		offscreenPosF1 = new Vector2 (screenPosF1.x, screenPosF1.y - 300);
		fingerOne.anchoredPosition = offscreenPosF1;

		if (drawing1 == true) {

//			screenPosF2 = fingerTwo.anchoredPosition;
//			offscreenPosF2 = new Vector2 (screenPosF2.x-500, screenPosF2.y);
//			midScreenPos = new Vector2 (screenPosF2.x-250, screenPosF2.y);
//			fingerTwo.anchoredPosition = offscreenPosF2;



			introText.text = intro1Words2;
			playerNum = 2;
			InvokeRepeating ("DrawShake", 3.0f, 3.0f);
			Invoke ("MoveFingerOneIn", 4.0f);
		}

		if (drawing2 == true) {
			introText.text = intro2Words1;
			playerNum = 4;
			Invoke ("MoveFingerOneIn", 6.5f);
			InvokeRepeating ("DrawShake", 4.5f, 1.5f);
		}

		panelScreen = bottomPanel.anchoredPosition;
		panelScreenOff = new Vector3 (panelScreen.x, panelScreen.y - 500, panelScreen.z);

		cameraOne.ZoomIn(playerNum);
		lineSpawn.dontDraw = true;
		Invoke ("OpenCurtains", 1.5f);

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown(0) && roomMan.curtainMoving == false)
		{

			if (roomMan == null) {
				return;
			}

			if (introText.text == intro1Words3 && speechBubble.activeInHierarchy == true) {
				CancelInvoke ();
//				Invoke ("MoveTheFing", 2.5f);
				InvokeRepeating ("BubbleOneShake", 4.0f, 3.0f);
				FadeOutElephant ();
				Invoke("JiggleBrushes", 1.5f);
				LoadBrushes ();
				introText.text = intro1Words5;

			} else if (introText.text == intro1Words5 && speechBubble.activeInHierarchy == true) {
				tutorialDupe.SetActive (false);
				lineSpawn.dontDraw = false;
				okayToClick = true;
				CancelInvoke ();
				DOTween.Kill ("brushjig");
				Vector3 twelve = new Vector3 (.12f, .12f, .12f);
				brushOne.DOScale (twelve, .5f);
				brushTwo.DOScale (twelve, .5f);
			}

		}
		
	}

	void BubbleOneShake (){

		bubbleOne.DOShakeScale(1.0f, .2f, 10);

	}

	void DrawShake (){

		drawButton.transform.DOShakeScale(1.0f, .4f, 10);

	}

	void OpenCurtains (){
	
		roomMan = GameObject.FindGameObjectWithTag ("Room Manager").GetComponent<RoomManager>();
		if (roomMan != null) {
			roomMan.CurtainsOut ();
		}

	}

	public void IntroBubble () {

		if (introText.text == intro1Words3){
			FadeOutElephant ();
			LoadBrushes ();
			introText.text = intro1Words4;
			CancelInvoke ();
			InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
		} else if (introText.text == intro1Words4){
			introText.text = intro1Words5;
			CancelInvoke ();
			InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
		} else if (introText.text == intro1Words5){
			introText.text = intro1Words6;
			CancelInvoke ();
			InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
		} else if (introText.text == intro1Words6){
			tutorialDupe.SetActive (false);
			lineSpawn.dontDraw = false;
			okayToClick = true;
			CancelInvoke ();
		}
	}

	public void DrawButtonHit(){
	
		DOTween.Kill ("finger1");
		MoveFingerOneOut ();
		speechBubble.SetActive (false);
		cameraOne.MoveToSectionTutorial ();
		introText.text = intro1Words3;
		CancelInvoke ();
		InvokeRepeating ("BubbleOneShake", 5.0f, 3.0f);
		bottomPanel.DOAnchorPos (panelScreenOff, .5f).OnComplete(SwitchToDrawingButts);

	}

	public void DrawButtonHitSecond(){

		if (introText.text == intro2Words1) {

			DOTween.Kill ("finger1");
			MoveFingerOneOut ();
			speechBubble.SetActive (false);
			cameraOne.MoveToSectionTutorial ();
			introText.text = "";
			CancelInvoke ();
			tutorialDupe.SetActive (false);
			bottomPanel.DOAnchorPos (panelScreenOff, .5f).OnComplete (SwitchToDrawingButts);

		}
	}


	void SwitchToDrawingButts (){
	
		readyButts.SetActive (false);
		drawingButts.SetActive (true);

	}
		
	public void StartGame(){

		localRoom.MoveUpPanel ();
		if (drawing1 == true) {
			Invoke ("BubbleTwo", 1.0f);
		} else if (drawing2 == true) {
			FadeOutElephant ();
			lineSpawn.dontDraw = false;
		}
		lineSpawn.GetColor (playerNum);

	}

	void BubbleTwo(){

		speechBubble.SetActive (true);
	}

	void FadeOutElephant (){
	
		Color clear = new Color (1,1,1,0);
		elephant.DOColor(clear,1.0f);
		if (drawing2 == true) {
			LoadBrushes ();
			OkayToClick ();
		}
	
	}

	void LoadBrushes (){
		int myColor = playerNum;

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

		int myColor = playerNum;

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
			okayToClick = false;
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
		if (drawing1 == true) {
			myLineString = "[MYCOLOR]" + "2:";
		} else {
			myLineString = "[MYCOLOR]" + "4:";
		}

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

		if (drawing1 == true) {
			SceneManager.LoadScene ("Tutorial Lobby Menu 2");
		} else {
			SceneManager.LoadScene ("Tutorial Turn Based Voting 2");
		}

	}

	void MoveFingerOneIn(){

		fingerOne.DOAnchorPos (screenPosF1, .6f).SetId ("finger1").OnComplete(PressDownF1);
		Invoke ("MoveFingerOneOut", 2.0f);

	}

	void PressDownF1(){
		Vector3 smaller = new Vector3 (-.15f, -.15f, -.15f);
		fingerOne.DOPunchScale (smaller, .5f, 1, .01f);
	}

	void MoveFingerOneOut(){

		fingerOne.DOAnchorPos (offscreenPosF1, .6f);

	}

	void JiggleBrushes(){

		brushOne = GameObject.FindGameObjectWithTag ("Brush X").transform;
		brushTwo = GameObject.FindGameObjectWithTag ("Brush Y").transform;

		Vector3 jiggleSize = new Vector3 (.15f, .15f, .15f);
		brushOne.DOPunchScale (jiggleSize, .5f, 1, .1f).SetDelay (1.5f).SetId ("brushjig").OnComplete(JiggleBrushes);
		brushTwo.DOPunchScale (jiggleSize, .5f, 1, .1f).SetDelay (1.5f).SetId ("brushjig");

	}




}
