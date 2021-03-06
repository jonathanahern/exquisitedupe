﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PortraitScript : MonoBehaviour {

	public Text tutText;
	string tutText1 = "Welcome to Exquisite Dupe!";
	//string tutText2 = "First things first, draw your icon!";

	public SpriteRenderer outline;
	public RectTransform tutFigure;
	public GameObject speechBubble;
	public LineSpawnerScipt lineSpawn;
	public RectTransform buttonBottom;
	string myLineString;
	public GameObject blackLine;
	public Transform newFace;
	Vector2 newFacePos;
	float scaled;

	public RectTransform textTop;
	public RectTransform pedestals;
	string newSceneName;

	// Use this for initialization
	void Start () {

		tutText.text = tutText1;
		//Invoke ("SwitchToText2", 4.0f);
		Invoke ("StartDrawPhase", 2.5f);

		newFacePos = newFace.localPosition;
		scaled = newFace.lossyScale.x;

		Invoke ("OpenCurtains", .25f);

	}
	
	// Update is called once per frame
	void Update () {

//		if (Input.GetKeyDown (KeyCode.G)) {
//			GetDrawing ();
//		}
//
//		if (Input.GetKeyDown (KeyCode.C)) {
//			CreateDrawing ();
//		}

		
	}

	void OpenCurtains(){

		if (RoomManager.instance == null) {
			return;
		}

		RoomManager.instance.CurtainsOut();
	}

//	void SwitchToText2 (){
//
//		tutText.text = tutText2;
//
//	}

	void StartDrawPhase (){

		speechBubble.SetActive (false);
		Invoke ("SendDownTut", .75f);


	}

	void SendDownTut(){
		tutFigure.DOAnchorPosY (-1400, 1.0f).OnComplete(SendUpButtons);
	}

	void SendUpButtons(){
		
		//Vector4 clear = new Vector4 (0, 0, 0, 0);
		outline.DOColor(Vector4.zero,1.5f);
		buttonBottom.DOAnchorPosY (0, 1.0f);
		lineSpawn.dontDraw = false;

	}

	public void DoneDrawing(){

		GetDrawing ();
		if (myLineString.Length < 1) {
			return;
		}
		buttonBottom.DOAnchorPosY (-1000, 1.0f);
		textTop.DOAnchorPosY (1000, 1.0f).OnComplete(MoveCamera);
		SendDrawing ();

	}

	void MoveCamera(){
	
		Camera.main.transform.DOLocalMoveX (6.5f, 1.0f).OnComplete(SetupTutorial);

	}

	void SetupTutorial(){

		pedestals.DOAnchorPosY (0, 1.0f);

	}

	void GetDrawing (){
	
		GameObject[] lines = GameObject.FindGameObjectsWithTag ("Line");

		myLineString = "";

		foreach (GameObject line in lines) {

			LineRenderer lineRend = line.GetComponent<LineRenderer> ();
			int lineAmount = lineRend.positionCount;

			for (int i = 0; i < lineAmount; i++) {

				Vector2 point = lineRend.GetPosition (i);

				myLineString = myLineString + point.ToString ("F3") + "@";

				if (i == lineAmount - 1) {

					string[] charsToRemove = new string[] { "(", ")", " "};
					foreach (string character in charsToRemove)
					{
						myLineString = myLineString.Replace(character, string.Empty);
					}

					myLineString = myLineString.Replace("0.000", "0");
					myLineString = myLineString.Replace("-0.", "-.");
					myLineString = myLineString.Replace("0.", ".");
					myLineString = myLineString.Replace("$$", "$");

				}

			}

			myLineString = myLineString.TrimEnd('@');
			myLineString = myLineString + "$";

		}

		myLineString = myLineString.TrimEnd('$');
	
	}

	void SendDrawing(){
	
		StartCoroutine (SendDrawingServer ());
	
	}

	IEnumerator SendDrawingServer(){

		string URL = "http://dupesite.000webhostapp.com/storePortrait.php";
		UserAccountManagerScript userManager = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();
		string username = userManager.loggedInUsername;

		if (myLineString == "") {
			myLineString = "Empty";
		}

		userManager.selfPortrait = myLineString;

		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", username);
		form.AddField ("portraitPost", myLineString);

		WWW www = new WWW (URL, form);
		yield return www;

		Debug.Log (www.text);

	}
		
	void CreateDrawing (){

		GameObject lineFab = blackLine;

		string[] lines = myLineString.Split ('$');
		//Debug.Log (drawing);
		foreach (string line in lines) {

			GameObject lineGo = Instantiate (lineFab);
			LineRenderer lineRend = lineGo.GetComponent <LineRenderer> ();

			string[] points = line.Split ('@');

			lineRend.positionCount = points.Length;

			for (int i = 0; i < points.Length; i++) {
				//Debug.Log (points [i]);
				string[] vectArray = points [i].Split (',');
				float newPointX = float.Parse (vectArray [0]) + newFacePos.x;
				float newPointY = float.Parse (vectArray [1]) + newFacePos.y;
				Vector3 tempVect = new Vector3 (
					((newPointX-newFacePos.x)*scaled)+newFacePos.x,
					((newPointY-newFacePos.y)*scaled)+newFacePos.y,
					0);

				lineRend.SetPosition (i, tempVect);

			}
		}

	}

	public void GoToLobby(){
		RoomManager.instance.CurtainsIn ();
		newSceneName = "Lobby Menu";
		Invoke ("LoadScene", 1.5f);
	}

	public void GoToTutorial(){
		RoomManager.instance.CurtainsIn ();
		newSceneName = "Tutorial Lobby Menu";
		Invoke ("LoadScene", 1.5f);
	}

	void LoadScene(){
		SceneManager.LoadScene (newSceneName);
	}


}
