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
	int currentAward = 0;
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
	public Sprite monaMasterpieceWords;
	public Sprite vagueArtistWords;
	public Sprite captainObviousWords;
	public Sprite dupeVotedElsewhere;
	public Sprite dupeVotedSelf;
	public Sprite wrongDupeGuess;
	public Sprite rightDupeGuess;
	public Sprite nonDupeGuess;
	public Sprite noVoteSelf;

	public GameObject buttonObj;
	public GameObject cantVoteObj;
	public GameObject cantVoteSelfObj;

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
	public GameObject confettiBlast;
	GameObject voteItem;

	private static string MYCOLOR_SYM = "[MYCOLOR]";

	public bool tutorialMode;
	public AnimationCurve inBump;

	float cameraZoom = 5.5f;
	Vector3 cameraStartPos;

	// Use this for initialization
	void Start () {

		cameraStartPos = new Vector3 (0, -1.25f, -10);

		if (Camera.main.aspect < .5f) {

			cameraStartPos = new Vector3 (0, 0, -10.0f);
			cameraZoom = 6.6f;
			Camera.main.transform.position = cameraStartPos;
			Camera.main.GetComponent<Camera> ().orthographicSize = 6.6f;

		}

		pedScreenPos = pedestal.transform.position.y;
		pedOffPos = pedScreenPos - 3.0f;
		Vector3 offScreen = new Vector3 (pedestal.transform.position.x, pedOffPos, pedestal.transform.position.z);
		pedestal.transform.position = offScreen;

		signScreenPos = sign.transform.position.y;
		signOffPos = signScreenPos - 3.0f;

		if (tutorialMode == true) {
			MoveDownSign ();
			return;
		}

		guessScreenPos = guessNonDupe.transform.position.y;
		guessOffScreenPos = guessScreenPos - 3.0f;
		Vector3 offScreen2 = new Vector3 (guessNonDupe.transform.position.x, guessOffScreenPos, guessNonDupe.transform.position.z);
		guessNonDupe.transform.position = offScreen2;

		for (int i = 0; i < lineMats.Length; i++) {

			lineMats[i].color = regColors[i];

		}
			
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

		voteItem = dupeVote;		
		ShakeTheVote ();

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

		if (myRoom.dupeNum == myRoom.myActualColor) {
			dupeGuessTitle.text = "What was everyone else drawing?";
		} else {
			dupeGuessTitle.text = "What was the dupe drawing?";
		}

		voteScript.SetupDupeVote (myRoom.myActualColor);

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

	void ShakeTheVote(){
	
		Vector3 punchSize = new Vector3 (.4f, .4f, .4f);
		voteItem.transform.DOPunchScale (punchSize, 1.0f,10,.01f).SetDelay(2.0f).SetId("voteshake").OnComplete(ShakeTheVote);
	
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

		//Debug.Log ("drawing: " + drawing);

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

			lineRend.positionCount = points.Length;

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

			lineRend.positionCount = points.Length;

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

	public void MoveUpPedestal(){

		pedestal.transform.DOLocalMoveY (pedScreenPos, 1.0f).SetEase (inBump);

	}

	public void MoveDownPedestal(){

		pedestal.transform.DOLocalMoveY (pedOffPos, .6f);

	}

	public void MoveUpSign(){

		sign.transform.DOLocalMoveY (signScreenPos, 1.0f).SetEase (inBump);

	}

	public void MoveDownSign(){

		sign.transform.DOLocalMoveY (signOffPos, .6f);

	}

	void MoveInGuesser(){

		guessObject.transform.DOLocalMoveX (0, .75f).SetEase (inBump);

	}

	public void MoveOutGuesser(){

		guessObject.transform.DOLocalMoveX (-1600, .6f);

	}

	void MoveUpNonDupeGuess(){

		if (dupeCaught == true) {
			dupeTrophy.SetActive (false);
		} else {
			dupeColor.SetActive (false);
		}

		signWords.sprite = nonDupeGuess;
		MoveUpSign ();
		guessNonDupe.transform.DOLocalMoveY (guessScreenPos, 1.0f).SetEase (inBump);

	}

	void MoveDownNonDupeGuess(){

		guessNonDupe.transform.DOLocalMoveY (guessOffScreenPos, .6f);

	}

	//0 onDupe, 1 offDupeandself, -1 onSelf
	public void FlipSignToConfirm(int onWho){

		Vector3 oneEighty = new Vector3 (0, 180, 0);
		sign.transform.DORotate (oneEighty, 1.0f);

		if (onWho == 0 && currentAward ==1) {
			cantVoteObj.SetActive (true);
			buttonObj.SetActive (false);
		} else if (onWho == -1 && currentAward == 2){
			cantVoteSelfObj.SetActive (true);
			buttonObj.SetActive (false);

		} else if (onWho == 0 && currentAward == 4){
			cantVoteObj.SetActive (true);
			buttonObj.SetActive (false);

		} else {
			cantVoteObj.SetActive (false);
			buttonObj.SetActive (true);
			if (tutorialMode == false) {
				cantVoteSelfObj.SetActive (false);
			}

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

			if (myRoom.myActualColor != myRoom.dupeNum) {

				if (myRoom.dupeNum == artistNum) {
					dupeStatus = "o";
				} else {
					dupeStatus = "x";
				}

				if (myRoom.dupeCaught != "o" || myRoom.dupeCaught != "x") {
					string stringRoomId = myRoom.roomID.ToString ();
					StartCoroutine (checkVoteStatus (stringRoomId, dupeStatus));
				}

				DupeGuessResult (artistNum);

				//Invoke ("LaunchVote2", 1.5f);

			} else {

				if (myRoom.dupeNum == artistNum) {
					signWords.sprite = dupeVotedSelf;
					confettiBlast.SetActive (true);
					Invoke ("TurnOffBlast", 3.5f);
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
			confettiBlast.SetActive (true);
			Invoke ("TurnOffBlast", 3.5f);
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

			Invoke ("MoveUpPedAndSign", 1.5f);
		}

	}

	void TurnOffBlast(){
		confettiBlast.SetActive (false);
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


		Invoke ("MoveUpNonDupeGuess", 2.0f);
	}

	IEnumerator checkVoteStatus (string roomId, string caughtVar){

//		IEnumerator e = DCP.RunCS ("turnRooms", "CheckVoteStatus", new string[2] {roomId, caughtVar});
//
//		while (e.MoveNext ()) {
//			yield return e.Current;
//		}
//
//		string returnText = e.Current as string;
//
//		returnText = returnText.TrimStart ('|');
//
//		Debug.Log ("Returned Status:" + returnText);

		string URL = "http://dupesite.000webhostapp.com/checkCaughtEscape.php";

		WWWForm form = new WWWForm ();
		form.AddField ("idPost", roomId);
		form.AddField ("caughtPost", caughtVar);

		WWW www = new WWW (URL, form);
		yield return www;

		string returnText = www.text;

		//Debug.Log ("Returned status: " + returnText);

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
		Invoke ("LaunchNonDupeGuess", .4f);


	}

	public void PassButton (){

		MoveDownSign ();
		myDupeSubjectGuess = "Pass";
		MoveDownNonDupeGuess ();
		Invoke ("LaunchVote2", 1.0f);

	}

	void PaleRed(){
		lineMats[0].color = dupeColors[0];
		lineMats[1].color = regColors[1];
		lineMats[2].color = regColors[2];
		lineMats[3].color = regColors[3];
	}

	void PaleBlue (){
		lineMats[1].color = dupeColors[1];
		lineMats[0].color = regColors[0];
		lineMats[2].color = regColors[2];
		lineMats[3].color = regColors[3];
	}

	void PaleGreen (){
		lineMats[2].color = dupeColors[2];
		lineMats[1].color = regColors[1];
		lineMats[0].color = regColors[0];
		lineMats[3].color = regColors[3];
	}

	void PaleOrange (){
		lineMats[3].color = dupeColors[3];
		lineMats[0].color = regColors[0];
		lineMats[2].color = regColors[2];
		lineMats[1].color = regColors[1];
	}

	void LaunchVote2(){

		pedestal.SetActive (true);
		sign.SetActive (true);
		int dupeNum = myRoom.dupeNum;
		int colorNum = myRoom.myActualColor;

		if (myRoom.awardNum == 1) {

			if (dupeNum == 1) {
				PaleRed ();
			} else if (dupeNum == 2) {
				PaleBlue ();
			} else if (dupeNum == 3) {
				PaleGreen ();
			} else if (dupeNum == 4) {
				PaleOrange ();
			} 

			currentAward = 1;
			signWords.sprite = monkeyArtistWords;
			MoveUpSign ();

		} else if (myRoom.awardNum > 1) {

			if (colorNum == 1) {
				PaleRed ();
			} else if (colorNum == 2) {
				PaleBlue ();
			} else if (colorNum == 3) {
				PaleGreen ();
			} else if (colorNum == 4) {
				PaleOrange ();
			} 

			currentAward = 2;
			signWords.sprite = monaMasterpieceWords;
			MoveUpSign ();

		}

		GameObject secondVote = Instantiate (voteFab);
		secondVote.transform.SetParent (spawnPos, false);
		VoteFabScript voteScript = secondVote.GetComponent<VoteFabScript> ();
		voteScript.localTurn = this;
		currentVote = 2;

		voteItem = secondVote;		
		ShakeTheVote ();

		voteScript.SetupSecondVote (myRoom.myActualColor,myRoom.awardNum,myRoom.dupeNum);
		MoveUpPedestal ();

	}

	void LaunchVote3(){
		
		int dupeNum = myRoom.dupeNum;
		currentAward = 4;

		if (dupeNum == 1) {
			PaleRed ();
		} else if (dupeNum == 2) {
			PaleBlue ();
		} else if (dupeNum == 3) {
			PaleGreen ();
		} else if (dupeNum == 4) {
			PaleOrange ();
		} 

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

		voteItem = thirdVote;		
		ShakeTheVote ();

		voteScript.SetupThirdVote (myRoom.myActualColor, myRoom.dupeCaught,myRoom.dupeNum);
		MoveUpPedestal ();

	}

	public void LaunchDupeGuess (){

		pedestal.SetActive (false);
		sign.SetActive (false);
		guessNonDupe.SetActive (false);

		Camera.main.transform.DOLocalMoveY (-2.6f, .75f).OnComplete(MoveInGuesser);
		DOTween.To(()=> Camera.main.orthographicSize, x=> Camera.main.orthographicSize = x, 6.7f, .75f);

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

		if (myRoom.myActualColor == myRoom.dupeNum) {

			string roomId = myRoom.roomID.ToString ();

			string[] charsToRemove = new string[] { "(", ")" };
			string voteOne = dupeVotePos.ToString ("F3");
			foreach (string character in charsToRemove) {
				voteOne = voteOne.Replace (character, string.Empty);
			}

			string dupeString = subject + "$" + voteOne;

//			string myColor;
//
//			if (myRoom.myActualColor == 1) {
//				myColor = "a";
//			} else if (myRoom.myActualColor == 2) {
//				myColor = "b";
//			} else if (myRoom.myActualColor == 3) {
//				myColor = "c";
//			} else if (myRoom.myActualColor == 4) {
//				myColor = "d";
//			} else {
//				myColor = "poop";
//			}

			roomMan.GetComponent<RoomManager> ().CurtainsIn ();

			StartCoroutine (addDupeGuess (roomId, dupeString));
		} else {

			myDupeSubjectGuess = subject;
			MoveOutGuesser ();


			Camera.main.transform.DOMove (cameraStartPos, 1.5f);
			DOTween.To(()=> Camera.main.orthographicSize, x=> Camera.main.orthographicSize = x, cameraZoom, 1.5f);
			Invoke ("LaunchVote2", 2.5f);

		}

	}

	IEnumerator addDupeGuess (string roomId, string dupeString){

//		IEnumerator e = DCP.RunCS ("turnRooms", "AddDupeGuess", new string[3] {roomId, dupeString, playerId});
//
//		while (e.MoveNext ()) {
//			yield return e.Current;
//		}
//
//		string returnText = e.Current as string;

		string URL = "http://dupesite.000webhostapp.com/addDupeGuess.php";
		Debug.Log ("Returned Dupe:" + roomId);
		Debug.Log ("Returned Dupe:" + dupeString);
		WWWForm form = new WWWForm ();
		form.AddField ("idPost", roomId);
		form.AddField ("dupePost", dupeString);
		form.AddField ("dupeIdPost", myRoom.myColor.ToString());

		WWW www = new WWW (URL, form);
		yield return www;

		string returnText = www.text;

		Debug.Log ("Returned Dupe:" + returnText);

		myRoom.activeVoteRoom = false;
		myRoom.status = "waiting...";
		myRoom.statusNum = 3;

		RoomManager.instance.cameFromTurnBased=true;
		Invoke ("GoToLobby", 1.25f);

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

		string colorString = myRoom.myActualColor.ToString();
		string votingPositions = colorString + "$" + voteOne + "$" + voteTwo + "$" + voteThree + "$" + myDupeSubjectGuess + "@";

		string roomId = myRoom.roomID.ToString();

//		if (myRoom.myActualColor == 1) {
//			myColor = "a";
//		} else if (myRoom.myActualColor == 2) {
//			myColor = "b";
//		} else if (myRoom.myActualColor == 3) {
//			myColor = "c";
//		} else if (myRoom.myActualColor == 4) {
//			myColor = "d";
//		}

		StartCoroutine (addVotingPos (roomId, votingPositions));

		Debug.Log (votingPositions);

	}

	void TrySendingAgain (string roomId, string votingPositions){

		Debug.Log ("Didn't go thru. Trying again.");

		StartCoroutine (addVotingPos (roomId, votingPositions));

	}

	IEnumerator addVotingPos (string roomId, string votingPositions){

//		IEnumerator e = DCP.RunCS ("turnRooms", "AddVotingPos", new string[3] {roomId, votingPositions, myColor});
//
//		while (e.MoveNext ()) {
//			yield return e.Current;
//		}
//
//		string returnText = e.Current as string;
//
//		returnText = returnText.TrimStart ('|');
//		returnText = returnText.TrimEnd ('@');
//
//		Debug.Log ("Returned Positions:" + returnText);
//
//		if (returnText == string.Empty) {
//
//			TrySendingAgain (roomId, votingPositions);
//			yield break;
//
//		}

		string URL = "http://dupesite.000webhostapp.com/addPlayerGuess.php";

		WWWForm form = new WWWForm ();
		form.AddField ("idPost", roomId);
		form.AddField ("votePost", votingPositions);
		form.AddField ("playerIdPost", myRoom.myColor.ToString());

		WWW www = new WWW (URL, form);
		yield return www;

		string returnText = www.text;

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
		Invoke ("GoToLobby", 1.25f);

	}

	void GoToLobby(){
		SceneManager.LoadScene ("Lobby Menu");
	}

}
