using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DatabaseControl;
using UnityEngine.SceneManagement;

public class LocalTurnVoting : MonoBehaviour {

	TurnRoomScript myRoom;
	string drawing;
	int currentVote;
	bool fateSet;
	string myColor;

	GameObject roomMan;

	int secondVote;
	int thirdVote;

	public GameObject guessObject;
	//CaptureAndSave snapShot;
	//public Camera paintingCam;

	public GameObject pedestal;
	public Transform spawnPos;
	public GameObject sign;
	public Image signWords;
	private Sprite voteSprite;

	float pedScreenPos;
	float pedOffPos;
	float signScreenPos;
	float signOffPos;

	public GameObject voteFab;

	Vector2 dupeVotePos;
	Vector2 secondVotePos;
	Vector2 thirdVotePos;

	public Sprite monkeyArtistWords;
	public Sprite vagueArtistWords;
	public Sprite captainObviousWords;
	public Sprite dupeVotedElsewhere;
	public Sprite dupeVotedSelf;
	public Sprite wrongDupeGuess;
	public Sprite rightDupeGuess;
	public Sprite nonDupeGuess;

	public GameObject buttonObj;
	public GameObject cantVoteObj;

	bool dupeCaught = false;

	public GameObject dupeTrophy;
	public GameObject dupeColor;

	public Color[] dupeColors;
	public Color[] regColors;

	public GameObject redLine;
	public GameObject blueLine;
	public GameObject greenLine;
	public GameObject orangeLine;

	public GameObject redDot;
	public GameObject blueDot;
	public GameObject greenDot;
	public GameObject orangeDot;

	public Material[] lineMats;

	float guessScreenPos;
	float guessOffScreenPos;
	public GameObject guessNonDupe;

	Vector3 camPos1;
	Vector3 camPos2;
	Vector3 camPos3;
	Vector3 camPos4;

	Vector3 dupePos;

	string myDupeSubjectGuess;

	public Text dupeGuessTitle;

	private static string MYCOLOR_SYM = "[MYCOLOR]";

	// Use this for initialization
	void Start () {

		for (int i = 0; i < lineMats.Length; i++) {

			lineMats[i].color = regColors[i];

		}

		//snapShot = GameObject.FindObjectOfType<CaptureAndSave> ();
	
		pedScreenPos = pedestal.transform.position.y;
		pedOffPos = pedScreenPos - 3.0f;
		Vector3 offScreen = new Vector3 (pedestal.transform.position.x, pedOffPos, pedestal.transform.position.z);
		pedestal.transform.position = offScreen;

		guessScreenPos = guessNonDupe.transform.position.y;
		guessOffScreenPos = guessScreenPos - 3.0f;
		Vector3 offScreen2 = new Vector3 (guessNonDupe.transform.position.x, guessOffScreenPos, guessNonDupe.transform.position.z);
		guessNonDupe.transform.position = offScreen2;

		signScreenPos = sign.transform.position.y;
		signOffPos = signScreenPos - 3.0f;

//		foreach (Material lineMat in lineMats) {
//
//			Color lineColor = lineMat.color;
//			Color fullColor = new Color (lineColor.r, lineColor.g, lineColor.b, 1.0f);
//			lineMat.color = fullColor;
//			
//		}
	
		roomMan = GameObject.FindGameObjectWithTag ("Room Manager");

		if (roomMan == null) {
			Debug.Log ("not logged in");
			return;
		}

		Transform roomHolder = GameObject.FindGameObjectWithTag ("Room Holder").transform;

		TurnRoomScript[] rooms = roomHolder.GetComponentsInChildren<TurnRoomScript> ();
		foreach (TurnRoomScript room in rooms) {

			if (room.activeVoteRoom == true) {
				myRoom = room;
				drawing = room.drawings;
				CreateDrawing ();
			}

		}

		GameObject[] buttonTexts = GameObject.FindGameObjectsWithTag ("Button Text");
		int childCount = buttonTexts.Length;

		for (int i = 0; i < childCount; i++) {
			buttonTexts [i].GetComponent<Text> ().text = myRoom.words [i];
		}
			
		GameObject dupeVote = Instantiate (voteFab);
		dupeVote.transform.SetParent (spawnPos, false);
		VoteFabScript voteScript = dupeVote.GetComponent<VoteFabScript> ();
		voteScript.localTurn = this;
		currentVote = 1;

		camPos1 = new Vector3 (-1.28f, .7f, -10);
		camPos2 = new Vector3 (1.28f, .7f, -10);
		camPos3 = new Vector3 (1.28f, -3.15f, -10);
		camPos4 = new Vector3 (-1.28f, -3.15f, -10);

		if (myRoom.dupeNum == 1) {
			dupePos = camPos1;
		} else if (myRoom.dupeNum == 2) {
			dupePos = camPos2;
		} else if (myRoom.dupeNum == 3) {
			dupePos = camPos3;
		} else if (myRoom.dupeNum == 4) {
			dupePos = camPos4;
		}

		if (myRoom.dupeNum == myRoom.myColor) {
			dupeGuessTitle.text = "What is the correct subject";
		} else {
			dupeGuessTitle.text = "What was the dupe drawing?";
		}

		voteScript.SetupDupeVote (myRoom.myColor);

		Invoke ("OpenCurtains", 1.0f);
		Invoke ("MoveUpPedestal" , 3.0f);
		Invoke ("TakePicture" , 3.0f);

	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.D)) {
			
//			GameObject dupeVote = Instantiate (voteFab);
//			dupeVote.transform.SetParent (spawnPos, false);
//			VoteFabScript voteScript = dupeVote.GetComponent<VoteFabScript> ();
//			voteScript.localTurn = this;
//			currentVote = 1;
//			Invoke ("MoveUpPedestal" , 1.0f);

		}

	}

	void TakePicture (){

		//snapShot.CaptureAndSaveToAlbum(0, 0, 500, 1000, ImageType.JPG);

//		snapShot.CaptureAndSaveToAlbum(Screen.width * 1, Screen.height * 1, paintingCam, ImageType.JPG);
//		paintingCam.gameObject.SetActive (false);

	}

	void OpenCurtains(){

		roomMan.GetComponent<RoomManager> ().CurtainsOut();

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

	void MoveInGuesser(){
	
		guessObject.transform.DOLocalMoveX (0, 1.0f).SetEase (Ease.OutBounce);
	
	}

	void MoveOutGuesser(){

		guessObject.transform.DOLocalMoveX (-1600, 1.0f).SetEase (Ease.OutBounce);

	}

	void MoveUpNonDupeGuess(){
	
		signWords.sprite = nonDupeGuess;
		MoveUpSign ();
		guessNonDupe.transform.DOLocalMoveY (guessScreenPos, 1.0f).SetEase (Ease.OutBounce);
	
	}

	void MoveDownNonDupeGuess(){

		guessNonDupe.transform.DOLocalMoveY (guessOffScreenPos, 1.0f).SetEase (Ease.OutBounce);

	}

	public void FlipSignToConfirm(int onDupe){

		Vector3 oneEighty = new Vector3 (0, 180, 0);
		sign.transform.DORotate (oneEighty, 1.0f);

		if (onDupe == 0) {
			cantVoteObj.SetActive (true);
			buttonObj.SetActive (false);
		} else {
			cantVoteObj.SetActive (false);
			buttonObj.SetActive (true);
		}

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

			if (myRoom.myColor != myRoom.dupeNum) {
				
				if (myRoom.dupeNum == artistNum) {
					dupeStatus = "o";
				} else {
					dupeStatus = "x";
				}
					
				if (myRoom.dupeCaught != "o" || myRoom.dupeCaught != "x") {
					string stringRoomId = "|[ID]" + myRoom.roomID.ToString ();
					StartCoroutine (checkVoteStatus (stringRoomId, dupeStatus));
				}

				DupeGuessResult (artistNum);

				//Invoke ("LaunchVote2", 1.5f);

			} else {

				if (myRoom.dupeNum == artistNum) {
					signWords.sprite = dupeVotedSelf;
				} else {
					signWords.sprite = dupeVotedElsewhere;
				}

				FlipSignToWords ();

				Invoke ("MoveUpSign", 1.5f);
				Invoke ("MoveDownSign", 4.5f);
				Invoke ("LaunchDupeGuess", 4.5f);

			}
		}

		if (currentVote == 2) {
			secondVotePos = pos;

			Invoke ("LaunchVote3", 1.0f);
		}

		if (currentVote == 3) {
			thirdVotePos = pos;

			Invoke ("EndPhase2", 1.0f);
		}

		Destroy (vote);

	}

	void DupeGuessResult (int artistNum){
	
		if (myRoom.dupeNum == artistNum) {
			dupeCaught = true;
			signWords.sprite = rightDupeGuess;

			FlipSignToWords ();

			Invoke ("MoveUpPedAndSign", 1.5f);

		} else {
			dupeCaught = false;
			signWords.sprite = wrongDupeGuess;
			dupeColor.SetActive (true);
			string dupeColorString = "purple";

			if (myRoom.dupeNum == 1) {
				dupeColorString = "red";
			} else if (myRoom.dupeNum == 2) {
				dupeColorString = "blue";
			} else if (myRoom.dupeNum == 3) {
				dupeColorString = "green";
			} else if (myRoom.dupeNum == 4) {
				dupeColorString = "orange";
			} 

			dupeColor.GetComponent<Text> ().text = dupeColorString;
			FlipSignToWords ();

			Invoke ("MoveUpPedAndSign", 2.5f);
		}

	}

	void MoveUpPedAndSign (){
		if (dupeCaught == true) {
			dupeTrophy.SetActive (true);
			MoveUpPedestal ();
		}
		MoveUpSign ();
		Invoke ("MoveDownPedAndSign", 2.5f);
	}

	void MoveDownPedAndSign () {
		if (dupeCaught == true) {
			MoveDownPedestal ();
		}
		MoveDownSign ();

		if (dupeCaught == true) {
			dupeTrophy.SetActive (false);
		} else {
			dupeColor.SetActive (false);
		}

		Invoke ("MoveUpNonDupeGuess", 2.0f);
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
			myRoom.dupeCaught = "o";
		
		} else if (returnText.Contains ("x")) {

			myRoom.dupeCaught = "x";

		} else if (returnText.Contains ("o")) {

			myRoom.dupeCaught = "o";

		} else {

			Debug.Log ("caught dupe error 2");
			myRoom.dupeCaught = "o";

		}

	}

	public void GuessButton (){

		MoveDownSign ();
		MoveDownNonDupeGuess ();
		Invoke ("LaunchNonDupeGuess", 2.0f);


	}

	public void PassButton (){

		MoveDownSign ();
		myDupeSubjectGuess = "Pass";
		MoveDownNonDupeGuess ();
		Invoke ("LaunchVote2", 2.0f);

	}

	void LaunchVote2(){

		pedestal.SetActive (true);
		sign.SetActive (true);

		if (myRoom.dupeNum == 1) {
			lineMats[0].color = dupeColors[0];
			lineMats[1].color = regColors[1];
			lineMats[2].color = regColors[2];
			lineMats[3].color = regColors[3];
		} else if (myRoom.dupeNum == 2) {
			lineMats[1].color = dupeColors[1];
			lineMats[0].color = regColors[0];
			lineMats[2].color = regColors[2];
			lineMats[3].color = regColors[3];
		} else if (myRoom.dupeNum == 3) {
			lineMats[2].color = dupeColors[2];
			lineMats[1].color = regColors[1];
			lineMats[0].color = regColors[0];
			lineMats[3].color = regColors[3];
		} else if (myRoom.dupeNum == 4) {
			lineMats[3].color = dupeColors[3];
			lineMats[0].color = regColors[0];
			lineMats[2].color = regColors[2];
			lineMats[1].color = regColors[1];
		} 


		if (myRoom.awardNum > 0) {
		
			signWords.sprite = monkeyArtistWords;
			MoveUpSign ();

		}

		GameObject secondVote = Instantiate (voteFab);
		secondVote.transform.SetParent (spawnPos, false);
		VoteFabScript voteScript = secondVote.GetComponent<VoteFabScript> ();
		voteScript.localTurn = this;
		currentVote = 2;

		voteScript.SetupSecondVote (myRoom.myColor,myRoom.awardNum,myRoom.dupeNum);
		MoveUpPedestal ();

	}

	void LaunchVote3(){

		FlipSignToWords ();

		if (myRoom.dupeCaught == "x") {
			signWords.sprite = vagueArtistWords;
		} else {
			signWords.sprite = captainObviousWords;
		}

		MoveUpSign ();

		GameObject thirdVote = Instantiate (voteFab);
		thirdVote.transform.SetParent (spawnPos, false);
		VoteFabScript voteScript = thirdVote.GetComponent<VoteFabScript> ();
		voteScript.localTurn = this;
		currentVote = 3;

		voteScript.SetupThirdVote (myRoom.myColor, myRoom.dupeCaught,myRoom.dupeNum);
		MoveUpPedestal ();

	}

	void LaunchDupeGuess (){

		pedestal.SetActive (false);
		sign.SetActive (false);
		guessNonDupe.SetActive (false);

		Camera.main.transform.DOLocalMoveY (-2.6f, 1.5f).OnComplete(MoveInGuesser);
		DOTween.To(()=> Camera.main.orthographicSize, x=> Camera.main.orthographicSize = x, 6.7f, 1.5f);
	
	}

	void LaunchNonDupeGuess (){

		pedestal.SetActive (false);
		sign.SetActive (false);
		guessNonDupe.SetActive (false);

		if (myRoom.dupeNum == 1) {
			lineMats[1].color = dupeColors[1];
			lineMats[2].color = dupeColors[2];
			lineMats[3].color = dupeColors[3];
		} else if (myRoom.dupeNum == 2) {
			lineMats[2].color = dupeColors[2];
			lineMats[0].color = dupeColors[0];
			lineMats[3].color = dupeColors[3];
		} else if (myRoom.dupeNum == 3) {
			lineMats[1].color = dupeColors[1];
			lineMats[3].color = dupeColors[3];
			lineMats[0].color = dupeColors[0];
		} else if (myRoom.dupeNum == 4) {
			lineMats[1].color = dupeColors[1];
			lineMats[2].color = dupeColors[2];
			lineMats[0].color = dupeColors[0];
		} 

		Camera.main.transform.DOMove (dupePos, 1.5f).OnComplete(MoveInGuesser);
		DOTween.To(()=> Camera.main.orthographicSize, x=> Camera.main.orthographicSize = x, 3.3f, 1.5f);

	}

	public void GuessSubmitted (string subject) {
	
		if (myRoom.myColor == myRoom.dupeNum) {

			string roomId = "|[ID]" + myRoom.roomID.ToString ();

			string[] charsToRemove = new string[] { "(", ")" };
			string voteOne = dupeVotePos.ToString ("F3");
			foreach (string character in charsToRemove) {
				voteOne = voteOne.Replace (character, string.Empty);
			}

			string dupeString = "|[DUPEGUESS]" + subject + "$" + voteOne;

			string myColor;

			if (myRoom.myColor == 1) {
				myColor = "a";
			} else if (myRoom.myColor == 2) {
				myColor = "b";
			} else if (myRoom.myColor == 3) {
				myColor = "c";
			} else if (myRoom.myColor == 4) {
				myColor = "d";
			} else {
				myColor = "poop";
			}

			roomMan.GetComponent<RoomManager> ().CurtainsIn ();

			StartCoroutine (addDupeGuess (roomId, dupeString, myColor));
		} else {
		
			myDupeSubjectGuess = subject;
			MoveOutGuesser ();

			Vector3 cameraStartPos = new Vector3 (0, -1.25f, -10);
			Camera.main.transform.DOMove (cameraStartPos, 1.5f);
			DOTween.To(()=> Camera.main.orthographicSize, x=> Camera.main.orthographicSize = x, 5.5f, 1.5f);
			Invoke ("LaunchVote2", 2.5f);

		}

	}

	IEnumerator addDupeGuess (string roomId, string dupeString, string playerId){
		
		IEnumerator e = DCP.RunCS ("turnRooms", "AddDupeGuess", new string[3] {roomId, dupeString, playerId});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("Returned Dupe:" + returnText);

		myRoom.activeVoteRoom = false;
		myRoom.status = "waiting...";
		myRoom.statusNum = 3;

		RoomManager.instance.cameFromTurnBased=true;
		SceneManager.LoadScene ("Lobby Menu");

	}

	void EndPhase2 () {
	
		roomMan.GetComponent<RoomManager> ().CurtainsIn ();

		string[] charsToRemove = new string[] { "(", ")" };

		string voteOne = dupeVotePos.ToString ("F2");
		string voteTwo = secondVotePos.ToString ("F2");
		string voteThree  = thirdVotePos.ToString ("F2");

		foreach (string character in charsToRemove)

		{
			voteOne = voteOne.Replace(character, string.Empty);
			voteTwo = voteTwo.Replace(character, string.Empty);
			voteThree = voteThree.Replace(character, string.Empty);
		}

		string colorString = myRoom.myColor.ToString();
		string votingPositions = colorString + "$" + voteOne + "$" + voteTwo + "$" + voteThree + "$" + myDupeSubjectGuess + "@";

		string roomId = "|[ID]" + myRoom.roomID.ToString();

		if (myRoom.myColor == 1) {
			myColor = "a";
		} else if (myRoom.myColor == 2) {
			myColor = "b";
		} else if (myRoom.myColor == 3) {
			myColor = "c";
		} else if (myRoom.myColor == 4) {
			myColor = "d";
		}

		StartCoroutine (addVotingPos (roomId, votingPositions, myColor));

		Debug.Log (votingPositions);

	}

	IEnumerator addVotingPos (string roomId, string votingPositions, string myColor){

		IEnumerator e = DCP.RunCS ("turnRooms", "AddVotingPos", new string[3] {roomId, votingPositions, myColor});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		returnText = returnText.TrimStart ('|');
		returnText = returnText.TrimEnd ('@');

		Debug.Log ("Returned Positions:" + returnText);

		myRoom.activeVoteRoom = false;
		myRoom.status = "waiting...";
		myRoom.statusNum = 3;


		if (myRoom.dupeNum == 1) {
			lineMats[0].color = regColors[0];
		} else if (myRoom.dupeNum == 2) {
			lineMats[1].color = regColors[1];
		} else if (myRoom.dupeNum == 3) {
			lineMats[2].color = regColors[2];
		} else if (myRoom.dupeNum == 4) {
			lineMats[3].color = regColors[3];
		} 

		RoomManager.instance.cameFromTurnBased=true;
		SceneManager.LoadScene ("Lobby Menu");

	}

}

//lion skunk rhino gerbil cow bear pig dog camel horse
