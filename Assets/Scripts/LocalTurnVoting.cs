using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DatabaseControl;

public class LocalTurnVoting : MonoBehaviour {

	TurnRoomScript myRoom;
	string drawing;
	int currentVote;
	bool fateSet;

	int secondVote;
	int thirdVote;

	public GameObject pedestal;
	public Transform spawnPos;
	public GameObject sign;
	public SpriteRenderer signWords;
	private Sprite voteSprite;

	float pedScreenPos;
	float pedOffPos;
	float signScreenPos;
	float signOffPos;

	public GameObject voteFab;

	Vector3 dupeVotePos;
	Vector3 secondVotePos;

	public Sprite monkeyArtistWords;


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

		pedScreenPos = pedestal.transform.position.y;
		pedOffPos = pedScreenPos - 3.0f;
		Vector3 offScreen = new Vector3 (pedestal.transform.position.x, pedOffPos, pedestal.transform.position.z);
		pedestal.transform.position = offScreen;

		signScreenPos = sign.transform.position.y;
		signOffPos = signScreenPos - 3.0f;


	
		GameObject roomMan = GameObject.FindGameObjectWithTag ("Room Manager");

		if (roomMan == null) {
			Debug.Log ("not logged in");
			return;
		}

		TurnRoomScript[] rooms = roomMan.transform.GetComponentsInChildren<TurnRoomScript> ();
		foreach (TurnRoomScript room in rooms) {

			if (room.activeVoteRoom == true) {
				myRoom = room;
				drawing = room.drawings;
				CreateDrawing ();
			}

		}

		GameObject dupeVote = Instantiate (voteFab);
		dupeVote.transform.SetParent (spawnPos, false);
		VoteFabScript voteScript = dupeVote.GetComponent<VoteFabScript> ();
		voteScript.localTurn = this;
		currentVote = 1;

		voteScript.SetupDupeVote (myRoom.myColor);
		MoveUpPedestal ();

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.D)) {
			
			FlipSignToConfirm();

		}

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


//			if (drawings.Length > 2) {
//				dotsString = drawings [2];
//			} else {
//				dotsString = "";
//			}

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

	void MoveUpPedestal(){

		pedestal.transform.DOLocalMoveY (pedScreenPos, 1.0f).SetEase (Ease.OutBounce);

	}

	void MoveDownPedestal(){

		pedestal.transform.DOLocalMoveY (pedOffPos, 1.0f).SetEase (Ease.OutBounce);

	}

	void MoveUpSign(){

		sign.transform.DOLocalMoveY (signScreenPos, 1.0f).SetEase (Ease.OutBounce);

	}

	void MoveDownSign(){

		sign.transform.DOLocalMoveY (signOffPos, 1.0f).SetEase (Ease.OutBounce);

	}

	public void FlipSignToConfirm(){

		Vector3 oneEighty = new Vector3 (0, 180, 0);
		sign.transform.DORotate (oneEighty, 1.0f);

	}

	public void FlipSignToWords () {
		
		sign.transform.DORotate (Vector3.zero, 1.0f);

	}


	public void TallyVote(){

		MoveDownSign ();
		MoveDownPedestal ();
		GameObject vote = GameObject.FindGameObjectWithTag ("Vote");
		Vector3 pos = vote.transform.position;
		int artistNum;


		if (pos.y < -3.255) {
			artistNum = 0;
		} else if (pos.x <= 0 && pos.y >= 0) {
			artistNum = 1;
		} else if (pos.x >= 0 && pos.y >= 0) {
			artistNum = 2;
		} else if (pos.x >= 0 && pos.y <= 0) {
			artistNum = 3;
		} else if (pos.x <= 0 && pos.y <= 0) {
			artistNum = 4;
		} else {
			artistNum = 0;
		}


		if (currentVote == 1) {
			dupeVotePos = pos;
			string dupeStatus;

			if (myRoom.dupeNum == artistNum) {
				dupeStatus = "c";
			} else {
				dupeStatus = "e";
			}
				
			if (myRoom.dupeCaught != "c" || myRoom.dupeCaught != "e" ) {
				string stringRoomId = "|[ID]" + myRoom.roomID.ToString ();
					StartCoroutine (checkVoteStatus (stringRoomId, dupeStatus));
			}

			Invoke ("LaunchVote2", 1.5f);
		}

		if (currentVote == 2) {
			secondVotePos = pos;

			//Invoke ("LaunchVote2", 1.5f);
		}

		Destroy (vote);

	}

	IEnumerator checkVoteStatus (string roomId, string caughtVar){

		IEnumerator e = DCP.RunCS ("turnRooms", "CheckVoteStatus", new string[2] {roomId, caughtVar});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		returnText = returnText.TrimStart ('|');

		Debug.Log ("Returned Status:" + returnText);

		if (returnText == "Error") {
		
			Debug.Log ("caught dupe error");
			myRoom.dupeCaught = "c";
		
		} else if (returnText.Contains ("e")) {

			myRoom.dupeCaught = "e";

		} else if (returnText.Contains ("c")) {

			myRoom.dupeCaught = "c";

		} else {

			Debug.Log ("caught dupe error 2");
			myRoom.dupeCaught = "c";

		}

	}

	void LaunchVote2(){

		FlipSignToWords ();

		if (myRoom.awardNum > 0) {
		
			signWords.sprite = monkeyArtistWords;
			MoveUpSign ();
		}

		GameObject secondVote = Instantiate (voteFab);
		secondVote.transform.SetParent (spawnPos, false);
		VoteFabScript voteScript = secondVote.GetComponent<VoteFabScript> ();
		voteScript.localTurn = this;
		currentVote = 2;

		voteScript.SetupSecondVote (myRoom.myColor,myRoom.awardNum);
		MoveUpPedestal ();

	}

}
