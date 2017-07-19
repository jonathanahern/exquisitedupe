using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DatabaseControl;
using UnityEngine.SceneManagement;

public class LocalTurnScoring : MonoBehaviour {

	TurnRoomScript myRoom;
	string dupeGuess;
	string drawing;
	int currentVote;

	public Vector3[] redPos;
	public Vector3[] bluePos;
	public Vector3[] greenPos;
	public Vector3[] orangePos;

	public int awardThree;

	public GameObject voteFab;

	public GameObject questionRotation;

	public GameObject redLine;
	public GameObject blueLine;
	public GameObject greenLine;
	public GameObject orangeLine;

	public GameObject redDot;
	public GameObject blueDot;
	public GameObject greenDot;
	public GameObject orangeDot;

	private static string MYCOLOR_SYM = "[MYCOLOR]";

	// Use this for initialization
	void Start () {

		redPos = new Vector3[3];
		bluePos = new Vector3[3];
		greenPos = new Vector3[3];
		orangePos = new Vector3[3];
	
		GameObject roomMan = GameObject.FindGameObjectWithTag ("Room Manager");

		if (roomMan == null) {
			Debug.Log ("not logged in");
			return;
		}

		TurnRoomScript[] rooms = roomMan.transform.GetComponentsInChildren<TurnRoomScript> ();
		foreach (TurnRoomScript room in rooms) {

			if (room.activeScoreRoom == true) {
				myRoom = room;
				drawing = room.drawings;
				CreateDrawing ();
				GetVoteData ();
			}

		}

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.T)) {
			
			BeginDupeReveal();

		}

	}

	//3 $ 1.76, 3.07$-1.60, -1.84$2.00, -1.29 @ 4$1.22, -2.28$-1.23, -2.17$-0.99, 2.59 @ 1$1.18, 2.12$-1.22, -1.68$1.18, -1.73


	void GetVoteData (){
	
		string[] voteDataWhole = myRoom.votePoses.Split ('@');

		foreach (string voteData in voteDataWhole) {

			string[] oneData = voteData.Split ('$');

			if (oneData [0] == "1") {
			
				for (int i = 1; i < 4; i++) {

					string point = oneData[i];
					string[] vectArray = point.Split (',');

					Vector3 tempVect = new Vector3 (
						float.Parse (vectArray [0]),
						float.Parse (vectArray [1]),
						-1);
					
					redPos[i-1] = tempVect;

				}

			} else if (oneData [0] == "2") {

				for (int i = 1; i < 4; i++) {
					
					string point = oneData[i];
					string[] vectArray = point.Split (',');

					Vector3 tempVect = new Vector3 (
						float.Parse (vectArray [0]),
						float.Parse (vectArray [1]),
						-2);

					bluePos[i-1] = tempVect;

				}

			} else if (oneData [0] == "3") {

				for (int i = 1; i < 4; i++) {

					string point = oneData[i];
					string[] vectArray = point.Split (',');

					Vector3 tempVect = new Vector3 (
						float.Parse (vectArray [0]),
						float.Parse (vectArray [1]),
						-3);

					greenPos[i-1] = tempVect;

				}

			} else if (oneData [0] == "4") {

				for (int i = 1; i < 4; i++) {

					string point = oneData[i];
					string[] vectArray = point.Split (',');

					Vector3 tempVect = new Vector3 (
						float.Parse (vectArray [0]),
						float.Parse (vectArray [1]),
						-4);

					orangePos[i-1] = tempVect;

				}

			}

		}

		string[] dupeInfo = myRoom.dupeGuess.Split('$');
		dupeGuess = dupeInfo [0];
		string[] vectArray3 = dupeInfo [1].Split (',');

		Vector3 tempVect3 = new Vector3 (
			float.Parse (vectArray3 [0]),
			float.Parse (vectArray3 [1]),
			-5);

		if (myRoom.dupeNum == 1) {
			redPos [0] = tempVect3;
		} else if (myRoom.dupeNum == 2) {
			bluePos [0] = tempVect3;
		} else if (myRoom.dupeNum == 3) {
			greenPos [0] = tempVect3;
		} else if (myRoom.dupeNum == 4) {
			orangePos [0] = tempVect3;
		}

		Invoke ("BeginDupeReveal", 1.5f);

	}

	void CreateDrawing (){

		drawing = drawing.TrimEnd ('$');
		string[] drawingInfos = drawing.Split ('$');

		Debug.Log ("drawing: " + drawing);

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

	void BeginDupeReveal () {
	
		questionRotation.transform.DORotate (Vector3.zero, 1.0f).SetEase (Ease.OutBounce).OnComplete(RevealDupeVotes);
	
	}

	void RevealDupeVotes(){

		GameObject dupeVoteRed = Instantiate (voteFab, redPos[0], Quaternion.identity);
		dupeVoteRed.GetComponent<VoteFabScript> ().SetupDupeVote(1);
		dupeVoteRed.GetComponent<VoteFabScript> ().CheckColor();

		GameObject dupeVoteBlue = Instantiate (voteFab, bluePos[0], Quaternion.identity);
		dupeVoteBlue.GetComponent<VoteFabScript> ().SetupDupeVote(2);
		dupeVoteBlue.GetComponent<VoteFabScript> ().CheckColor();

		GameObject dupeVoteGreen = Instantiate (voteFab, greenPos[0], Quaternion.identity);
		dupeVoteGreen.GetComponent<VoteFabScript> ().SetupDupeVote(3);
		dupeVoteGreen.GetComponent<VoteFabScript> ().CheckColor();

		GameObject dupeVoteOrange = Instantiate (voteFab, orangePos[0], Quaternion.identity);
		dupeVoteOrange.GetComponent<VoteFabScript> ().SetupDupeVote(4);
		dupeVoteOrange.GetComponent<VoteFabScript> ().CheckColor();

	}


}
