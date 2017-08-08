using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DatabaseControl;
using UnityEngine.SceneManagement;

public class LocalTurnScoring : MonoBehaviour {

	GameObject roomMan;
	TurnRoomScript myRoom;
	string dupeGuess;
	string drawing;
	int currentVote;
	bool dupeTie = false;
	int dupeGuessed;
	int award2Winner;
	int award3Winner;
	int myColor;
	int myRoomID;

	public GameObject intro;
	public Text questionText;
	public GameObject questionObj;
	public Text nameText;
	public GameObject nameObj;
	public Text dupeStatusText;
	public GameObject dupeStatusObj;
	public GameObject award3Q;
	public Text award3QText;
	public GameObject finale;
	public GameObject guess;


	public Vector3[] redPos;
	public Vector3[] bluePos;
	public Vector3[] greenPos;
	public Vector3[] orangePos;

	public Text redScoreText;
	public Text blueScoreText;
	public Text greenScoreText;
	public Text orangeScoreText;

	public int redScore;
	public int blueScore;
	public int greenScore;
	public int orangeScore;

	public GameObject redFlash;
	public GameObject blueFlash;
	public GameObject greenFlash;
	public GameObject orangeFlash;

	public Text[] players;

	public int awardThree;

	public GameObject voteFab;

	public GameObject redLine;
	public GameObject blueLine;
	public GameObject greenLine;
	public GameObject orangeLine;

	public GameObject redDot;
	public GameObject blueDot;
	public GameObject greenDot;
	public GameObject orangeDot;

	public Text[] guesserNames;
	public Text[] nonDupeGuesses;
	public GameObject[] guessObjs;

	public List<int> votes;
	public List<int> winnersCircle;

	private static string MYCOLOR_SYM = "[MYCOLOR]";

	// Use this for initialization
	void Start () {

		redPos = new Vector3[3];
		bluePos = new Vector3[3];
		greenPos = new Vector3[3];
		orangePos = new Vector3[3];
	
		roomMan = GameObject.FindGameObjectWithTag ("Room Manager");

		Transform roomHolder = GameObject.FindGameObjectWithTag ("Room Holder").transform;

		if (roomMan == null) {
			Debug.Log ("not logged in");
			return;
		}

		TurnRoomScript[] rooms = roomHolder.GetComponentsInChildren<TurnRoomScript> ();
		foreach (TurnRoomScript room in rooms) {

			if (room.activeScoreRoom == true) {
				myRoom = room;
				drawing = room.drawings;
				CreateDrawing ();
				GetVoteData ();
			}

		}

		//Debug.Log (myRoom.gameObject.name);

		for (int i = 0; i < myRoom.players.Length; i++) {

			players[i].text = myRoom.players [i];

		}

		myColor = myRoom.myColor;
		myRoomID = myRoom.roomID;

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.T)) {
			
			questionObj.transform.DOLocalMoveX (0, 1.0f).SetEase (Ease.OutBounce);

		}

	}

	//3 $ 1.76, 3.07$-1.60, -1.84$2.00, -1.29 @ 4$1.22, -2.28$-1.23, -2.17$-0.99, 2.59 @ 1$1.18, 2.12$-1.22, -1.68$1.18, -1.73


	void GetVoteData (){
	
		string[] voteDataWhole = myRoom.votePoses.Split ('@');

		int guessCount = 0;

		foreach (string voteData in voteDataWhole) {

			string[] oneData = voteData.Split ('$');
			oneData [4] = CheckIfPass (oneData[4]);

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

				nonDupeGuesses [guessCount].text = "\"" + oneData [4] + "\"";
				guesserNames [guessCount].text = myRoom.players [0];
				guessCount++;


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
				nonDupeGuesses [guessCount].text = "\"" + oneData [4] + "\"";
				guesserNames [guessCount].text = myRoom.players [1];
				guessCount++;

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
				nonDupeGuesses [guessCount].text = "\"" + oneData [4] + "\"";
				guesserNames [guessCount].text = myRoom.players [2];
				guessCount++;

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

				nonDupeGuesses [guessCount].text = "\"" + oneData [4] + "\"";
				guesserNames [guessCount].text = myRoom.players [3];
				guessCount++;

			}

		}

		string[] dupeInfo = myRoom.dupeGuess.Split('$');
		dupeGuess = dupeInfo [0];
		string[] vectArray3 = dupeInfo [1].Split (',');

		Vector3 tempVect3 = new Vector3 (
			float.Parse (vectArray3 [0]),
			float.Parse (vectArray3 [1]),
			-5);

		Vector3 offScreenVote = new Vector3 (-100, -100, 0);

		if (myRoom.dupeNum == 1) {
			redPos [0] = tempVect3;
			redPos [1] = offScreenVote;
			redPos [2] = offScreenVote;
		} else if (myRoom.dupeNum == 2) {
			bluePos [0] = tempVect3;
			bluePos [1] = offScreenVote;
			bluePos [2] = offScreenVote;
		} else if (myRoom.dupeNum == 3) {
			greenPos [0] = tempVect3;
			greenPos [1] = offScreenVote;
			greenPos [2] = offScreenVote;
		} else if (myRoom.dupeNum == 4) {
			orangePos [0] = tempVect3;
			orangePos [1] = offScreenVote;
			orangePos [2] = offScreenVote;
		}

		//int[] winners = FindWinner (0);
		//DupeWinner (FindWinner (0));

		dupeGuessed = DupeWinner (FindWinner (0));

		Invoke ("OpenCurtains", 1.5f);
		Invoke ("BeginDupeReveal", 4);

	}

	string CheckIfPass(string guessed){
	
		if (guessed == "Pass") {
			return "heckifiknow";
		} else {
			return guessed;
		}
	
	}

	void OpenCurtains (){
		
		roomMan.GetComponent<RoomManager> ().CurtainsOut();
	
	}

	int[] FindWinner (int awardNum){

		int one = 0;
		int two = 0;
		int three = 0;
		int four = 0;

		//int[] numbers = new int[5] {1, 2, 3, 4, 5};
		int[] voteCounts = new int[4] {WhoYouVoted(redPos [awardNum]), WhoYouVoted(bluePos [awardNum]), WhoYouVoted(greenPos [awardNum]), WhoYouVoted(orangePos [awardNum])};

//		Debug.Log (voteCounts[0]);
//		Debug.Log (voteCounts[1]);
//		Debug.Log (voteCounts[2]);
//		Debug.Log (voteCounts[3]);

		foreach (int vote in voteCounts) {
			if (vote == 1) {
				one++;
			} else if (vote == 2) {
				two++;
			} else if (vote == 3) {
				three++;
			} else if (vote == 4) {
				four++;
			}
		}

		votes = new List<int> ();
		votes.Add (one);
		votes.Add (two);
		votes.Add (three);
		votes.Add (four);
		votes.Sort ();

		int topCount = votes [3];

		winnersCircle = new List<int> ();

		if (one == topCount) {
			winnersCircle.Add (1);
		}
		if (two == topCount) {
			winnersCircle.Add (2);
		}
		if (three == topCount) {
			winnersCircle.Add (3);
		}
		if (four == topCount) {
			winnersCircle.Add (4);
		}
			
		int[] results = winnersCircle.ToArray ();

		return results;

	}

	int DupeWinner (int[]players){
	
		if (players.Length == 1) {
			return players [0];
		} else if (players.Length == 2) {

			if (players [0] == myRoom.dupeNum) {
				dupeTie = true;
				return players [1];
			} else {
				dupeTie = true;
				return players [0];
			}

		} else {
			dupeTie = true;
			return 0;
		}
	
	}

	int AwardWinner (int[]players){

		if (players.Length == 1) {
			return players [0];
		} else {
			return 0;
		
		}

	}

	int WhoYouVoted (Vector3 pos) {
	
		if (pos.y < -3.255) {
			return  0;
		} else if (pos.x <= 0 && pos.y >= 0) {
			return 1;
		} else if (pos.x >= 0 && pos.y >= 0) {
			return 2;
		} else if (pos.x >= 0 && pos.y <= 0) {
			return 3;
		} else if (pos.x <= 0 && pos.y <= 0) {
			return 4;
		} else {
			return 0;
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
	
		intro.transform.DOLocalMoveX (-1000, 1.0f).SetEase (Ease.OutBounce);
		questionObj.transform.DOLocalMoveX (0, 1.0f).SetEase (Ease.OutBounce);
		Invoke ("RevealDupeVotes", 2.0f);
		Invoke ("RevealDupeGuess", 4.0f);
		Invoke ("RevealDupeStatus", 5.0f);
//		Invoke ("WhoVotedCorrect", 10.0f);
//		Invoke ("GiveDupeGuessPoints", 13.0f);

		//
	
	}

	void RevealDupeVotes(){

		GiveDupeGuessPoints ();

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

		if (dupeTie == true) {
			nameText.text = "Noboby!? It's a TIE!!!";
		} else {
			nameText.text = players[dupeGuessed - 1].text;
		}

		//Vector3 neg90 = new Vector3 (0, 0, -90);
		nameObj.transform.DOLocalMoveX (0, 1.0f).SetEase (Ease.OutBounce);

	}

//	void WhoVotedCorrect (){
//	
//		questionText.text = "WHO GUESSED WHO THE DUPE WAS?";
//		questionObj.transform.DOLocalMoveX(0, 1.0f).SetEase (Ease.OutBounce);
//		dupeStatusObj.transform.DOLocalMoveX (-1000, 1.0f);
//
//	
//	}

	void GiveDupeGuessPoints (){
	
		if (WhoYouVoted (redPos [0]) == myRoom.dupeNum) {
			GivePoints (1, 1, 0);
		}

		if (WhoYouVoted (bluePos [0]) == myRoom.dupeNum) {
			GivePoints (2, 1, 0);
		}

		if (WhoYouVoted (greenPos [0]) == myRoom.dupeNum) {
			GivePoints (3, 1, 0);
		}

		if (WhoYouVoted (orangePos [0]) == myRoom.dupeNum) {
			GivePoints (4, 1, 0);
		}
	
	}

	void RevealDupeGuess(){
	
		if (dupeTie == true) {
			nameText.text = "Noboby!? It's a TIE!!!";
		} else {
			nameText.text = players[dupeGuessed - 1].text;
		}

		//Vector3 neg90 = new Vector3 (0, 0, -90);
		//questionRotation.transform.DORotate (neg90, 1.0f).SetEase (Ease.OutBounce);
		nameObj.transform.DOLocalMoveX (0, 1.0f).SetEase (Ease.OutBounce);
	
	}

	void RevealDupeStatus (){

		string dupeName = players[myRoom.dupeNum-1].text;
		string caughtString = "WE CAUGHT THE DUPE";
		string escapeString = "THE DUPE GOT AWAY";

		if (myRoom.dupeNum == myRoom.myColor) {
			caughtString = "THEY CAUGHT YOU!!!";
			escapeString = "YOU GOT AWAY WITH IT";
		}

		if (dupeTie == true) {
			dupeStatusText.text = escapeString;
		} else if (dupeGuessed == myRoom.dupeNum) {
			dupeStatusText.text = caughtString;
		} else  {
			dupeStatusText.text = escapeString;
		}

		Invoke ("GiveOutDupePoints", 1.0f);
		//Vector3 pos90 = new Vector3 (0, 0, 90);
		dupeStatusObj.transform.DOLocalMoveX (0, 1.0f).SetEase (Ease.OutBounce);
		questionObj.transform.DOLocalMoveX (1000, 1.0f);
		nameObj.transform.DOLocalMoveX (1000, 1.0f);

	}

	void GiveOutDupePoints (){
		
		if (dupeTie == true) {
			GivePoints (myRoom.dupeNum, 3, 1);
		} else if (dupeGuessed == myRoom.dupeNum) {
			GivePointsEveryoneBut (myRoom.dupeNum, 2);
		} else  {
			GivePointsEveryoneBut (myRoom.dupeNum, 2);
		}

		Invoke ("StartSecondAward", 1.5f);
	
	}

	void StartSecondAward(){
	
		DestroyVotes ();

		award2Winner = AwardWinner (FindWinner (1));
		//Debug.Log (myRoom.players [award2Winner - 1]);

		if (myRoom.awardNum > 0) {

			questionText.text = "THE MONKEY ARTIST GOES TO...";
			if (award2Winner == 0) {
				nameText.text = "Noboby!? It's a TIE!!!";
			} else {
				nameText.text = myRoom.players [award2Winner - 1];
			}

		}

		questionObj.transform.DOLocalMoveX (0, 1.0f).SetEase(Ease.InBounce);
		dupeStatusObj.transform.DOLocalMoveX (-1000, 1.0f);
		Invoke ("RevealSecondVotes", 2.0f);
			
	}

	void RevealSecondVotes(){

		int awardNum = myRoom.awardNum;
		int dupeNum = myRoom.dupeNum;

		if (dupeNum != 1) {
			GameObject awardVoteRed = Instantiate (voteFab, redPos [1], Quaternion.identity);
			awardVoteRed.GetComponent<VoteFabScript> ().SetupSecondVote (1, awardNum,0);
			awardVoteRed.GetComponent<VoteFabScript> ().CheckColor ();
		}

		if (dupeNum != 2) {
			GameObject awardVoteBlue = Instantiate (voteFab, bluePos [1], Quaternion.identity);
			awardVoteBlue.GetComponent<VoteFabScript> ().SetupSecondVote (2, awardNum,0);
			awardVoteBlue.GetComponent<VoteFabScript> ().CheckColor ();
		}

		if (dupeNum != 3) {
			GameObject awardVoteGreen = Instantiate (voteFab, greenPos [1], Quaternion.identity);
			awardVoteGreen.GetComponent<VoteFabScript> ().SetupSecondVote (3, awardNum,0);
			awardVoteGreen.GetComponent<VoteFabScript> ().CheckColor ();
		}

		if (dupeNum != 4) {
			GameObject awardVoteOrange = Instantiate (voteFab, orangePos [1], Quaternion.identity);
			awardVoteOrange.GetComponent<VoteFabScript> ().SetupSecondVote (4, awardNum,0);
			awardVoteOrange.GetComponent<VoteFabScript> ().CheckColor ();
		}


		//Vector3 neg90 = new Vector3 (0, 0, -90);
		nameObj.transform.DOLocalMoveX (0, 1.0f).SetEase (Ease.OutBounce).OnComplete(GiveOutAward2Points);

	}

	void GiveOutAward2Points (){

		if (award2Winner != 0) {
			GivePoints (award2Winner, -2, 1);
		}

		Invoke ("StartThirdAward", 2.5f);

	}

	void StartThirdAward(){

		DestroyVotes ();

		award3Winner = AwardWinner (FindWinner (2));

		if (award3Winner == 0) {
			nameText.text = "Noboby!? It's a TIE!!!";
		} else {
			nameText.text = myRoom.players [award3Winner - 1];
		}

		if (myRoom.dupeCaught == "o") {
			award3QText.text = "WHO HERE WON CAPTAIN OBVIOUS?";
		} else {
			award3QText.text = "WHO HERE DREW TOO VAGUE?";
		}

		award3Q.transform.DOLocalMoveX (0, 1.0f).SetEase(Ease.InBounce);
		questionObj.transform.DOLocalMoveX (-1000, 1.0f);
		nameObj.transform.DOLocalMoveX (-1000, 1.0f);
		Invoke ("RevealThirdVotes", 2.0f);

	}

	void RevealThirdVotes(){

		string awardNum = myRoom.dupeCaught;
		int dupeNum = myRoom.dupeNum;

		if (dupeNum != 1) {
			GameObject awardVoteRed = Instantiate (voteFab, redPos [2], Quaternion.identity);
			awardVoteRed.GetComponent<VoteFabScript> ().SetupThirdVote (1, awardNum,0);
			awardVoteRed.GetComponent<VoteFabScript> ().CheckColor ();
		}

		if (dupeNum != 2) {
			GameObject awardVoteBlue = Instantiate (voteFab, bluePos [2], Quaternion.identity);
			awardVoteBlue.GetComponent<VoteFabScript> ().SetupThirdVote (2, awardNum,0);
			awardVoteBlue.GetComponent<VoteFabScript> ().CheckColor ();
		}

		if (dupeNum != 3) {
			GameObject awardVoteGreen = Instantiate (voteFab, greenPos [2], Quaternion.identity);
			awardVoteGreen.GetComponent<VoteFabScript> ().SetupThirdVote (3, awardNum,0);
			awardVoteGreen.GetComponent<VoteFabScript> ().CheckColor ();
		}

		if (dupeNum != 4) {
			GameObject awardVoteOrange = Instantiate (voteFab, orangePos [2], Quaternion.identity);
			awardVoteOrange.GetComponent<VoteFabScript> ().SetupThirdVote (4, awardNum,0);
			awardVoteOrange.GetComponent<VoteFabScript> ().CheckColor ();
		}

		Vector3 farLeft = new Vector3 (-1000, nameObj.transform.position.y, nameObj.transform.position.z);
		nameObj.transform.position = farLeft;
		nameObj.transform.DOLocalMoveX (0, 1.0f).SetEase (Ease.OutBounce).OnComplete(GiveOutAward3Points);

	}

	void GiveOutAward3Points (){

		if (award3Winner != 0) {
			GivePoints (award3Winner, -2, 1);
		}

		Invoke ("StartDupeGuessReveal", 1.5f);

	}

	void StartDupeGuessReveal(){

		DestroyVotes ();

		finale.GetComponent<Text> ().text = "AND FINALLY... WHAT'D THE DUPE GUESS???";
		guess.GetComponent<Text> ().text = dupeGuess;

		finale.transform.DOLocalMoveX (0, 1.0f).SetEase (Ease.OutBounce);
		award3Q.transform.DOLocalMoveX (-1000, 1.0f);
		nameObj.transform.DOLocalMoveX (-1000, 1.0f);
		Invoke ("RevealDupeSubjectGuess", 3.5f);

	}

	void RevealDupeSubjectGuess (){

		guess.transform.DOLocalMoveX (0, 1.0f).SetEase (Ease.OutBounce);
		finale.transform.DOLocalMoveX (1000, 1.0f);
		Invoke ("RevealResult", 2.5f);
	}

	void RevealResult (){
	
		if (dupeGuess == myRoom.rightword) {
		
			finale.GetComponent<Text> ().text = "RIGHTO!";

		} else {
			finale.GetComponent<Text> ().text = "WRONG!";
		}
	
		finale.transform.DOLocalMoveX (0, 1.0f).SetEase (Ease.OutBounce);
		guess.transform.DOLocalMoveX (-1000, 1.0f);
		Invoke ("GiveFinalePoints", 2.5f);
	}

	void GiveFinalePoints (){

		if (dupeGuess == myRoom.rightword) {

			GivePoints (myRoom.dupeNum, 2, 1);

		} else {
			GivePointsEveryoneBut (myRoom.dupeNum, 2);
			dupeStatusText.text = "THE TRUE SUBJECT WAS " + myRoom.rightword; 
			finale.transform.DOLocalMoveX (1000, 1.0f);
			dupeStatusObj.transform.DOLocalMoveX (0, 1.0f);
		}

		Invoke ("EndTheRound", 2.5f);

	}


	void EndTheRound (){

		Destroy(myRoom.gameObject);

		roomMan.GetComponent<RoomManager> ().CurtainsIn ();
		roomMan.GetComponent<RoomManager> ().cameFromScoring = true;
		Invoke ("ReallyEndRound", 2.0f);

	}

	void ReallyEndRound (){
	
		int myScore = 0;

		if (myColor == 1) {
			myScore = redScore;
		} else if (myColor == 2) {
			myScore = blueScore;
		} else if (myColor == 3) {
			myScore = greenScore;
		} else if (myColor == 4) {
			myScore = orangeScore;
		}

		string roomIDstring = "|[ID]" + myRoomID.ToString ();

		UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();

		string roomsString = userAccount.activeRooms;
		string currentRoom = myRoom.roomID + "/";

		roomsString = roomsString.Replace (currentRoom, string.Empty);

		if (roomsString.Length < 5) {
		
			roomsString = string.Empty;

		}

		userAccount.activeRooms = roomsString;

		roomMan.GetComponent<RoomManager>().SendTheScore (myScore, myColor, roomIDstring, roomsString);
		RoomManager.instance.cameFromTurnBased=true;
		SceneManager.LoadScene ("Lobby Menu");
	
	}

	//play animation = 1
	void GivePoints (int playerNum, int points, int animation) {
	
		if (playerNum == 1) {
			redScore = redScore + points;
			redScoreText.text = redScore.ToString ();
			if (animation == 1) {
				redFlash.SetActive (true);
				Invoke ("TurnOffRed", 1.5f);
			}
		} else if (playerNum == 2) {
			blueScore = blueScore + points;
			blueScoreText.text = blueScore.ToString ();
			if (animation == 1) {
				blueFlash.SetActive (true);
				Invoke ("TurnOffBlue", 1.5f);
			}
		} else if (playerNum == 3) {
			greenScore = greenScore + points;
			greenScoreText.text = greenScore.ToString ();
			if (animation == 1) {
				greenFlash.SetActive (true);
				Invoke ("TurnOffGreen", 1.5f);
			}
		} else if (playerNum == 4) {
			orangeScore = orangeScore + points;
			orangeScoreText.text = orangeScore.ToString ();
			if (animation == 1) {
				orangeFlash.SetActive (true);
				Invoke ("TurnOffOrange", 1.5f);
			}
		}
	
	}

	void GivePointsEveryoneBut (int playerNum, int points) {

		if (playerNum == 1) {
			blueScore = blueScore + points;
			blueScoreText.text = blueScore.ToString ();
			blueFlash.SetActive (true);
			Invoke ("TurnOffBlue", 1.5f);
			greenScore = greenScore + points;
			greenScoreText.text = greenScore.ToString ();
			greenFlash.SetActive (true);
			Invoke ("TurnOffGreen", 1.5f);
			orangeScore = orangeScore + points;
			orangeScoreText.text = orangeScore.ToString ();
			orangeFlash.SetActive (true);
			Invoke ("TurnOffOrange", 1.5f);
		} else if (playerNum == 2) {
			redScore = redScore + points;
			redScoreText.text = redScore.ToString ();
			redFlash.SetActive (true);
			Invoke ("TurnOffRed", 1.5f);
			greenScore = greenScore + points;
			greenScoreText.text = greenScore.ToString ();
			greenFlash.SetActive (true);
			Invoke ("TurnOffGreen", 1.5f);
			orangeScore = orangeScore + points;
			orangeScoreText.text = orangeScore.ToString ();
			orangeFlash.SetActive (true);
			Invoke ("TurnOffOrange", 1.5f);
		} else if (playerNum == 3) {
			redScore = redScore + points;
			redScoreText.text = redScore.ToString ();
			redFlash.SetActive (true);
			Invoke ("TurnOffRed", 1.5f);
			blueScore = blueScore + points;
			blueScoreText.text = blueScore.ToString ();
			blueFlash.SetActive (true);
			Invoke ("TurnOffBlue", 1.5f);
			orangeScore = orangeScore + points;
			orangeScoreText.text = orangeScore.ToString ();
			orangeFlash.SetActive (true);
			Invoke ("TurnOffOrange", 1.5f);
		} else if (playerNum == 4) {
			redScore = redScore + points;
			redScoreText.text = redScore.ToString ();
			redFlash.SetActive (true);
			Invoke ("TurnOffRed", 1.5f);
			blueScore = blueScore + points;
			blueScoreText.text = blueScore.ToString ();
			blueFlash.SetActive (true);
			Invoke ("TurnOffBlue", 1.5f);
			greenScore = greenScore + points;
			greenScoreText.text = greenScore.ToString ();
			greenFlash.SetActive (true);
			Invoke ("TurnOffGreen", 1.5f);

		}

	}

	void TurnOffRed(){
		redFlash.SetActive (false);
	}
	void TurnOffBlue(){
		blueFlash.SetActive (false);
	}
	void TurnOffGreen(){
		greenFlash.SetActive (false);
	}
	void TurnOffOrange(){
		orangeFlash.SetActive (false);
	}

	void DestroyVotes(){
	
		GameObject[] votes = GameObject.FindGameObjectsWithTag ("Vote");
		foreach (GameObject vote in votes) {
			Destroy (vote);
		}
	
	}
		
}
