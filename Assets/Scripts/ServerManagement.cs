﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DatabaseControl;
using UnityEngine.UI;

public class ServerManagement : MonoBehaviour {

	public Text roomID;

	private static string DRAWING_SYM = "[DRAWING]";
	private static string MYCOLOR_SYM = "[MYCOLOR]";

	public GameObject redLine;
	public GameObject blueLine;
	public GameObject greenLine;
	public GameObject orangeLine;

	public GameObject redDot;
	public GameObject blueDot;
	public GameObject greenDot;
	public GameObject orangeDot;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void BackToZero (){

		StartCoroutine (resetScoresZero());

	}

	IEnumerator resetScoresZero (){

		IEnumerator e = DCP.RunCS ("accounts", "ResetScoresZero");

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("Back To Zero?:" + returnText);

	}

	public void GrabDrawing () {

		string roomIDstring = "|[ID]" + roomID.text;
		Debug.Log (roomIDstring);

		StartCoroutine (grabDrawing(roomIDstring));

	}

	IEnumerator grabDrawing (string roomID){

		IEnumerator e = DCP.RunCS ("turnRooms", "GrabDrawing",new string[1] {roomID});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("painting:" + returnText);

		if (returnText == "No Drawing") {
			yield break;
		}

		//string drawingString = returnText.Substring (DRAWING_SYM.Length);

		CreateDrawing (returnText);

	}

	void CreateDrawing (string drawing){

		drawing = drawing.TrimEnd ('$');
		string[] drawingInfos = drawing.Split ('$');


		foreach (string drawingInfo in drawingInfos) {

			string drawingString;
			string colorNum;
			string dotsString = "";

			string[] drawings = drawingInfo.Split (':');

			colorNum = drawings[0].Substring (MYCOLOR_SYM.Length);
			drawingString = drawings [1];
			dotsString = drawings [2];

			DrawLine (colorNum, drawingString, dotsString);

		}

	}

	void DrawLine (string colorNum, string drawing, string dotsString) {

		GameObject lineFab = new GameObject();
		GameObject dotFab = new GameObject();

		if (colorNum == "1") {
			lineFab = redLine;
			dotFab = redDot;
		} else if (colorNum == "2") {
			lineFab = blueLine;
			dotFab = blueDot;
		} else if (colorNum == "3") {
			lineFab = greenLine;
			dotFab = greenDot;
		} else if (colorNum == "4") {
			lineFab = orangeLine;
			dotFab = orangeDot;
		}

		string[] lines = drawing.Split ('+');

		foreach (string line in lines) {

			GameObject lineGo = Instantiate (lineFab);
			LineRenderer lineRend = lineGo.GetComponent <LineRenderer> ();

			string[] points = line.Split ('@');

			lineRend.numPositions = points.Length;

			for (int i = 0; i < points.Length; i++) {

				string[] vectArray = points [i].Split (',');

				Vector3 tempVect = new Vector3 (
					float.Parse (vectArray [0]),
					float.Parse (vectArray [1]),
					0);
				lineRend.SetPosition (i, tempVect);

			}
		}

		if (dotsString == "") {
			return;
		}

		string[] dots = dotsString.Split ('+');

		foreach (string dot in dots) {

			GameObject lineGo = Instantiate (dotFab);
			LineRenderer lineRend = lineGo.GetComponent <LineRenderer> ();

			string[] points = dot.Split ('@');

			lineRend.numPositions = points.Length;

			for (int i = 0; i < points.Length; i++) {

				string[] vectArray = points [i].Split (',');

				Vector3 tempVect = new Vector3 (
					float.Parse (vectArray [0]),
					float.Parse (vectArray [1]),
					0);
				lineRend.SetPosition (i, tempVect);

			}
		}
	}

	public void ResetRooms (){

		Debug.Log ("Reset");

		StartCoroutine (resetRooms());
		StartCoroutine (resetEverything());

	}

	IEnumerator resetRooms (){

		IEnumerator e = DCP.RunCS ("accounts", "ResetRooms");

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("Cleared Rooms?:" + returnText);

	}

	IEnumerator resetEverything (){

		IEnumerator ee = DCP.RunCS ("turnRooms", "ClearEverything");

		while (ee.MoveNext ()) {
			yield return ee.Current;
		}

		string returnText = ee.Current as string;

		Debug.Log ("Cleared Everything?:" + returnText);

	}

}
