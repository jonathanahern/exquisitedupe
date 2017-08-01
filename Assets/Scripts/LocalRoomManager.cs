﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using DatabaseControl;
using UnityEngine.SceneManagement;

public class LocalRoomManager : MonoBehaviour {

	TurnRoomScript myRoom;
	public CameraScript cameraScript;
	public LineSpawnerScipt lineSpawn;
	public GameObject brushPrefabX;
	public GameObject brushPrefabY;
	public GameObject brushHolder;
	public Text subject;
	public GameObject blackLine;

	string myLineString;

	public Sprite redDone;
	public Sprite blueDone;
	public Sprite greenDone;
	public Sprite orangeDone;

	public Image doneButton;

	public Sprite redUndo;
	public Sprite blueUndo;
	public Sprite greenUndo;
	public Sprite orangeUndo;

	public Image undoButton;
	public Material blackMat;

	GameObject roomMan;
	public GameObject frameMessage;
	Vector2 offScreen;

	bool readyToAdvance = false;
	bool okayToClick = false;

	// Use this for initialization
	void Start () {

		FindMyRoom ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FindMyRoom (){
	
		roomMan = GameObject.FindGameObjectWithTag ("Room Manager");
		
		if (roomMan == null) {
			Debug.Log ("not logged in");
			return;
		}

		Transform roomHolder = GameObject.FindGameObjectWithTag ("Room Holder").transform;

		TurnRoomScript[] rooms = roomHolder.GetComponentsInChildren<TurnRoomScript> ();
		foreach (TurnRoomScript room in rooms) {

			if (room.activeRoom == true) {
				myRoom = room;
			}
		}

		DrawGrounding ();

		Camera.main.GetComponent<CameraScript> ().ZoomIn (myRoom.myColor);

		if (myRoom.dupeNum == myRoom.myColor) {
			subject.text = myRoom.wrongword;
		} else {
			subject.text = myRoom.rightword;
		}

		if (myRoom.myColor == 1) {

			doneButton.sprite = redDone;
			undoButton.sprite = redUndo;

		} else if (myRoom.myColor == 2) {

			doneButton.sprite = blueDone;
			undoButton.sprite = blueUndo;

		} else if (myRoom.myColor == 3) {

			doneButton.sprite = greenDone;
			undoButton.sprite = greenUndo;

		} else if (myRoom.myColor == 4) {

			doneButton.sprite = orangeDone;
			undoButton.sprite = orangeUndo;

		}

		roomMan.GetComponent<RoomManager> ().CurtainsOut();

		UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();

		string roomsString = userAccount.activeRooms;

		if (roomsString.Contains("[ID]") == false) {
			roomsString = "[ID]" + roomsString;
		}

		roomsString = roomsString + myRoom.roomID.ToString() + "/";
		userAccount.activeRooms = roomsString;

		Invoke ("MoveToSection", 4.0f);

	}

	void MoveToSection () {

		cameraScript.MoveToSection ();
		Color clear = new Color (1, 1, 1, 0);
		blackMat.DOColor (clear, 1.0f).OnComplete (DestroyLines);

	}

	void DestroyLines (){
	
		GameObject[] lines = GameObject.FindGameObjectsWithTag ("Line");

		foreach (GameObject line in lines) {
			Destroy (line);
		}

		Color grey = new Color (.2f, .2f, .2f, 1.0f);
		blackMat.color = grey;

	
	}

	public void StartGame(){
	
		LoadBrushes ();
		lineSpawn.GetColor (myRoom.myColor);


	}

	void DrawGrounding () {

		Color grey = new Color (.2f, .2f, .2f, 1.0f);
		blackMat.color = grey;

		string[] lines = myRoom.grounding.Split ('$');

		foreach (string line in lines) {

			GameObject lineGo = Instantiate (blackLine);
			LineRenderer lineRend = lineGo.GetComponent <LineRenderer> ();

			string[] points = line.Split ('@');

			lineRend.numPositions = points.Length;

			for (int i = 0; i < points.Length; i++) {

				string[] vectArray = points[i].Split(',');

				Vector3 tempVect = new Vector3(
					float.Parse(vectArray[0]),
					float.Parse(vectArray[1]),
					0);
				lineRend.SetPosition (i, tempVect);

			}

		}

	}

	void LoadBrushes (){
	
		int myColor = myRoom.myColor;
		Invoke ("OkayToClick", 1.5f);

		for (int i = 0; i < myRoom.brushes.Length; i++) {

			Vector3 brushPos = myRoom.brushes [i];
			Vector3 newBrushPos;

			if (myRoom.brushes [i].x == 0) {

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

	void OkayToClick (){
		okayToClick = true;
	}

	public void CheckBrushes () {
	
		if (okayToClick == false) {
			return;
		}

		okayToClick = false; 
		Invoke ("OkayToClick", 2.5f);

		readyToAdvance = true;
		int brushCount = brushHolder.transform.childCount;
		int myColor = myRoom.myColor;

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

		for (int i = 0; i < brushCount; i++) {

			Vector3 brushPos = brushHolder.transform.GetChild (i).transform.position;
			if (brushPos.y == 0 && brushPos.x > xMin && brushPos.x < xMax) {
				readyToAdvance = false;
			} else if (brushPos.x == 0 && brushPos.y > yMin && brushPos.y < yMax){
				readyToAdvance = false;
			}
		}

		if (readyToAdvance == true) {
			CollectYourLineData ();
		} else {
			StillHaveBrushes ();
		}
	
	}

	void StillHaveBrushes(){
	
		offScreen = frameMessage.transform.position;

		frameMessage.transform.DOLocalMoveY(0,1.0f).SetEase (Ease.OutBounce);
		Invoke ("MoveOff", 2.5f);
	}

	void MoveOff() {
	
		frameMessage.transform.DOLocalMoveY(offScreen.y * -1, 1.0f).OnComplete (MoveBack);

	}

	void MoveBack(){

		frameMessage.transform.DOLocalMoveY(offScreen.y, 0.0f);
		okayToClick = true;
	}

	void CollectYourLineData () {

		roomMan.GetComponent<RoomManager> ().CurtainsIn ();

		GameObject[] lines = GameObject.FindGameObjectsWithTag ("Line");

		myLineString = "[MYCOLOR]" + myRoom.myColor.ToString () + ":";

		foreach (GameObject line in lines) {

			LineRenderer lineRend = line.GetComponent<LineRenderer> ();
			int lineAmount = lineRend.numPositions;

			for (int i = 0; i < lineAmount; i++) {

				Vector2 point = lineRend.GetPosition (i);


				myLineString = myLineString + point.ToString ("F2") + "@";

				if (i == lineAmount - 1) {

					string[] charsToRemove = new string[] { "(", ")" };
					foreach (string character in charsToRemove)
					{
						myLineString = myLineString.Replace(character, string.Empty);
					}

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

					string[] charsToRemove = new string[] { "(", ")" };
					foreach (string character in charsToRemove)
					{
						myLineString = myLineString.Replace(character, string.Empty);
					}

				}

			}

			myLineString = myLineString.TrimEnd('@');
			myLineString = myLineString + "+";
		}

		myLineString = myLineString.TrimEnd('+');
		myLineString = myLineString + "$";

		Debug.Log (myLineString);

		StartCoroutine (doneDrawing(myRoom.roomID, myLineString));

	}


	IEnumerator doneDrawing (int roomID, string drawingArray){

		string roomIdString = roomID.ToString();

		IEnumerator e = DCP.RunCS ("turnRooms", "AddDrawing", new string[3] { roomIdString, drawingArray, myRoom.myColor.ToString()});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("Returned:" + returnText);
		myRoom.activeRoom = false;
		myRoom.status = "waiting...";
		myRoom.statusNum = 2;

		RoomManager.instance.cameFromTurnBased=true;
		SceneManager.LoadScene ("Lobby Menu");

	}


}
