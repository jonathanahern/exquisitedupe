﻿using System.Collections;
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

	public GameObject[] scoreObj;

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
	public Text[] scores;
	bool gotOne = false;
	bool gotTwo = false;
	bool gotThree = false;
	bool gotFour = false;


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
	Vector2[] guessOnScreen;
	Vector2[] guessOffScreen;
	public Text dupesWord;
	public GameObject dupeDrew;

	Vector2 dupeDrewScreenPos;

	public Transform[] awardHolder;
	public GameObject awardPrefab;

	public List<int> votes;
	public List<int> winnersCircle;
	public List<int> votesNums;
	public int[] pointers;

	int[] monaPoints = {0,0,0,0};

	private static string MYCOLOR_SYM = "[MYCOLOR]";
	//bool privateScoring = false;

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
			//return;
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
	
		myColor = myRoom.myActualColor;
		myRoomID = myRoom.roomID;

		for (int i = 0; i < players.Length; i++) {

			players [i].text = myRoom.players [i];

		}

		guessOnScreen = new Vector2[guessObjs.Length];
		guessOffScreen = new Vector2[guessObjs.Length];

		for (int i = 0; i < guessObjs.Length; i++) {
			guessOnScreen[i] = guessObjs [i].GetComponent<RectTransform>().anchoredPosition;
		}

		guessOffScreen [0] = new Vector2 (guessOnScreen [0].x, guessOnScreen [0].y + 350);
		guessOffScreen [1] = new Vector2 (guessOnScreen [1].x-500, guessOnScreen [1].y);
		guessOffScreen [2] = new Vector2 (guessOnScreen [2].x+500, guessOnScreen [2].y);

		for (int i = 0; i < guessObjs.Length; i++) {
			guessObjs [i].GetComponent<RectTransform> ().anchoredPosition = guessOffScreen [i];
		}

		dupesWord.text = myRoom.wrongword;
		dupeDrewScreenPos = dupeDrew.GetComponent<RectTransform> ().anchoredPosition;
		Vector2 offScreenDupe = new Vector2 (dupeDrewScreenPos.x - 1500,dupeDrewScreenPos.y);
		dupeDrew.GetComponent<RectTransform> ().anchoredPosition = offScreenDupe;

		if (myRoom.privateRoom == true && myRoom.roundNum != 1) {

			for (int i = 0; i < players.Length; i++) {

				int curScore = PlayerPrefs.GetInt (players [i].text + "abcde");
				scores [i].text = curScore.ToString();

				if (i == 0) {
					redScore = curScore;
				} else if (i == 1) {
					blueScore = curScore;
				} else if (i == 2) {
					greenScore = curScore;
				} else if (i == 3) {
					orangeScore = curScore;
				}

			}

		}

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.T)) {
			
			//GivePointsEveryoneBut (3, -1);

		}

	}

	//3 $ 1.76, 3.07$-1.60, -1.84$2.00, -1.29 @ 4$1.22, -2.28$-1.23, -2.17$-0.99, 2.59 @ 1$1.18, 2.12$-1.22, -1.68$1.18, -1.73


	void GetVoteData (){
	
		string[] voteDataWhole = myRoom.votePoses.Split ('@');

		int guessCount = 0;

		foreach (string voteData in voteDataWhole) {

			string[] oneData = voteData.Split ('$');
			oneData [4] = CheckIfPass (oneData[4]);

			if (oneData [0] == "1" && gotOne==false) {
			
				gotOne = true;

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


			} else if (oneData [0] == "2" && gotTwo==false) {

				gotTwo = true;

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

			} else if (oneData [0] == "3" && gotThree==false) {

				gotThree = true;

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

			} else if (oneData [0] == "4" && gotFour==false) {

				gotFour = true;

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

	string GetWinnerNames (){

		string winnerNames = "";

		votesNums = new List<int>();

		votesNums.Add (WhoYouVoted (redPos [1]));
		votesNums.Add (WhoYouVoted (bluePos [1]));
		votesNums.Add (WhoYouVoted (greenPos [1]));
		votesNums.Add (WhoYouVoted (orangePos [1]));

		votesNums.Remove(0);
		votesNums.Sort ();

		pointers = new int[votesNums.Count];

		votesNums.CopyTo(pointers);

		int voteCount = votesNums.Count;

		for (int i = voteCount-1; i >= 1; i--) {
			Debug.Log ("COUNT " + i);
			if (votesNums [i] == votesNums [i - 1]) {
				votesNums.RemoveAt (i);
			}

		}

		voteCount = votesNums.Count;

		for (int i = 0; i < voteCount; i++) {

			winnerNames = winnerNames + players[votesNums [i] -1].text;
			if (i != voteCount - 1) {
				winnerNames = winnerNames + " and ";
			}

		}

		return winnerNames;
	
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
		dupeVoteRed.GetComponent<VoteFabScript> ().SetupDupeReveal(1);
		dupeVoteRed.GetComponent<VoteFabScript> ().CheckColor();

		GameObject dupeVoteBlue = Instantiate (voteFab, bluePos[0], Quaternion.identity);
		dupeVoteBlue.GetComponent<VoteFabScript> ().SetupDupeReveal(2);
		dupeVoteBlue.GetComponent<VoteFabScript> ().CheckColor();

		GameObject dupeVoteGreen = Instantiate (voteFab, greenPos[0], Quaternion.identity);
		dupeVoteGreen.GetComponent<VoteFabScript> ().SetupDupeReveal(3);
		dupeVoteGreen.GetComponent<VoteFabScript> ().CheckColor();

		GameObject dupeVoteOrange = Instantiate (voteFab, orangePos[0], Quaternion.identity);
		dupeVoteOrange.GetComponent<VoteFabScript> ().SetupDupeReveal(4);
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

		if (myRoom.dupeNum == myColor) {
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
			CreateDupeEscapeAwardIcon ();
		} else if (dupeGuessed == myRoom.dupeNum) {
			GivePointsEveryoneBut (myRoom.dupeNum, 2);
			CreateDupeCapturedAwardIcon ();
		} else if (dupeGuessed != myRoom.dupeNum) {
			GivePoints (myRoom.dupeNum, 3, 1);
			CreateDupeEscapeAwardIcon ();
		} else  {
			GivePointsEveryoneBut (myRoom.dupeNum, 1);
			CreateDupeCapturedAwardIcon ();
		}

		Invoke ("StartSecondAward", 2.0f);
	
	}

	void CreateDupeEscapeAwardIcon () {
	
		int dupeNum = myRoom.dupeNum;

		GameObject newIcon = Instantiate(awardPrefab,Vector3.zero,Quaternion.identity, awardHolder[dupeNum -1]);
		AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
		awardScript.SetupEscaped (dupeNum);
	
	}

	void CreateDupeCapturedAwardIcon(){

		int dupeNum = myRoom.dupeNum;

		if (dupeNum != 1) {
			GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [0]);
			AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
			awardScript.SetupCaptured (dupeNum);
		}

		if (dupeNum != 2) {
			GameObject newIcon2 = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [1]);
			AwardIconScript awardScript2 = newIcon2.GetComponent<AwardIconScript> ();
			awardScript2.SetupCaptured (dupeNum);
		}

		if (dupeNum != 3) {
			GameObject newIcon3 = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [2]);
			AwardIconScript awardScript3 = newIcon3.GetComponent<AwardIconScript> ();
			awardScript3.SetupCaptured (dupeNum);
		}

		if (dupeNum != 4) {
			GameObject newIcon4 = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [3]);
			AwardIconScript awardScript4 = newIcon4.GetComponent<AwardIconScript> ();
			awardScript4.SetupCaptured (dupeNum);
		}

	}

	void StartSecondAward(){
	
		DestroyVotes ();

		//award2Winner = AwardWinner (FindWinner (1));
		//Debug.Log (myRoom.players [award2Winner - 1]);

		if (myRoom.awardNum == 1) {

			award2Winner = AwardWinner (FindWinner (1));

			questionText.text = "THE MONKEY ARTIST GOES TO...";
			if (award2Winner == 0) {
				nameText.text = "Noboby!? It's a TIE!!!";
			} else {
				nameText.text = myRoom.players [award2Winner - 1];
			}

		} else if (myRoom.awardNum > 1) {

			string awardWinners = GetWinnerNames();
			questionText.text = "WHO RECEIVED A MONA MASTERPIECE?";
			nameText.text = awardWinners;

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

		if (myRoom.awardNum == 1) {
			
			if (award2Winner != 0) {
				GivePointsEveryoneButAndDupe (award2Winner, 1);
				AddSplatterToOne (1, 2);
			} else {
				AddSplatterToAll (myRoom.awardNum);
			}

		} else if (myRoom.awardNum > 1) {

			int lastNum = 100;
			int animate;

			for (int i = 0; i < pointers.Length; i++) {
				if (lastNum == pointers [i]) {
					animate = 0;
				} else {
					animate = 1;
				}

				monaPoints [pointers[i] - 1] = monaPoints [pointers[i] - 1] + 1;

				GivePoints (pointers [i], 1, animate);

				lastNum = pointers [i];
			}

		} 

		GiveMonaAwardIcons ();
		Invoke ("StartThirdAward", 3.0f);

	}

	void AddSplatterToAll(int awardNum){
	
		int dupeNum = myRoom.dupeNum;

		if (dupeNum != 1) {
			GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [0]);
			AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
			awardScript.SetupSplatter (1, awardNum);
		}

		if (dupeNum != 2) {
			GameObject newIcon2 = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [1]);
			AwardIconScript awardScript2 = newIcon2.GetComponent<AwardIconScript> ();
			awardScript2.SetupSplatter (2, awardNum);
		}

		if (dupeNum != 3) {
			GameObject newIcon3 = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [2]);
			AwardIconScript awardScript3 = newIcon3.GetComponent<AwardIconScript> ();
			awardScript3.SetupSplatter (3, awardNum);
		}

		if (dupeNum != 4) {
			GameObject newIcon4 = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [3]);
			AwardIconScript awardScript4 = newIcon4.GetComponent<AwardIconScript> ();
			awardScript4.SetupSplatter (4, awardNum);
		}
				
	}

	//1=monkey, 2=vague, 3=obvious
	void AddSplatterToOne(int awardNum, int awardPhase){

		int dupeNum = myRoom.dupeNum;
		int splatterWinner = AwardWinner (FindWinner (awardPhase));

		if (dupeNum != 1) {
			GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [0]);
			AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
			if (splatterWinner == 1) {
				awardScript.SetupSplatter (1, awardNum);
			} else {
				awardScript.SetupSplatterAvoider (awardNum);
			}
		}

		if (dupeNum != 2) {
			GameObject newIcon2 = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [1]);
			AwardIconScript awardScript2 = newIcon2.GetComponent<AwardIconScript> ();
			if (splatterWinner == 2) {
				awardScript2.SetupSplatter (2,awardNum);
			} else {
				awardScript2.SetupSplatterAvoider (awardNum);
			}
		}

		if (dupeNum != 3) {
			GameObject newIcon3 = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [2]);
			AwardIconScript awardScript3 = newIcon3.GetComponent<AwardIconScript> ();
			if (splatterWinner == 3) {
				awardScript3.SetupSplatter (3,awardNum);
			} else {
				awardScript3.SetupSplatterAvoider (awardNum);
			}
		}

		if (dupeNum != 4) {
			GameObject newIcon4 = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [3]);
			AwardIconScript awardScript4 = newIcon4.GetComponent<AwardIconScript> ();
			awardScript4.SetupMonkey (4);
			if (splatterWinner == 4) {
				awardScript4.SetupSplatter (4,awardNum);
			} else {
				awardScript4.SetupSplatterAvoider (awardNum);
			}
		}




	}

	void GiveMonaAwardIcons(){

		for (int i = 0; i < monaPoints.Length; i++) {
			
			if (monaPoints [i] != 0) {
				GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [i]);
				AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
				awardScript.SetupMona (monaPoints[i]);
			
			}


		}
	
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

		int awardNum;

		if (myRoom.dupeCaught == "o") {
			awardNum = 3;
		} else {
			awardNum = 2;
		}

		if (award3Winner != 0) {
			
			GivePointsEveryoneButAndDupe (award3Winner, 2);
			AddSplatterToOne (awardNum, 2);

		} else {
			AddSplatterToAll (awardNum);

		}

		Invoke ("StartNonDupeGuessReveal", 1.5f);

	}

	void StartNonDupeGuessReveal(){

		DestroyVotes ();

		finale.GetComponent<Text> ().text = "WHAT DID Y'ALL THINK THE DUPE DREW?";

		finale.transform.DOLocalMoveX (0, 1.0f).SetEase (Ease.OutBounce);
		award3Q.transform.DOLocalMoveX (-1000, 1.0f);
		nameObj.transform.DOLocalMoveX (-1000, 1.0f);
		Invoke ("MoveFinaleOver", 3.5f);

	}

	void MoveFinaleOver(){

		finale.transform.DOLocalMoveX (1000, 1.0f).SetEase (Ease.OutBounce).OnComplete(MoveInGuesses);

	}

	void MoveInGuesses(){
	
		for (int i = 0; i < guessObjs.Length; i++) {
			guessObjs [i].GetComponent<RectTransform> ().DOAnchorPos(guessOnScreen[i], .5f).SetEase(Ease.OutBounce);
		}

		Invoke ("MoveInDupeWord", 2.5f);
	
	}

	void MoveInDupeWord(){

		dupeDrew.GetComponent<RectTransform>().DOAnchorPos (dupeDrewScreenPos, 1.0f).SetEase (Ease.OutBounce);

		Invoke ("ScoreGuesses", 2.5f);

	}

	void ScoreGuesses (){


		for (int i = 0; i < guesserNames.Length; i++) {

			int playerNum = 0;

			for (int t = 0; t < players.Length; t++) {

				if (players [t].text == guesserNames [i].text) {
				
					playerNum = t +1;
				
				}

			}

			int points = 0;
			int animationNum;
			string editedGuess;

			editedGuess = nonDupeGuesses [i].text.TrimStart('"');
			editedGuess = editedGuess.TrimEnd('"');

			if (editedGuess == myRoom.wrongword) {
				points = 2;
				animationNum = 1;
			} else if (editedGuess == "heckifiknow") {
				points = 0;
				animationNum = 0;
			} else {
				points = -1;
				animationNum = 1;
			}

			GivePoints (playerNum, points, animationNum);

			if (points != 0) {
				SetupGuessIcon (playerNum, points);
			}

		}

		Invoke ("MoveOutGuesses", 3.5f);
			
	}

	void SetupGuessIcon (int playerNum, int points){
		
		GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [playerNum - 1]);
		AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
		awardScript.SetupGuess (points);

	}

	void MoveOutGuesses(){
	
		for (int i = 0; i < guessObjs.Length; i++) {
			guessObjs [i].GetComponent<RectTransform> ().DOAnchorPos(guessOffScreen[i], 1.0f);
		}
	
		dupeDrew.transform.DOLocalMoveX (-1000, 1.0f).SetEase (Ease.OutBounce);
		Invoke ("StartDupeGuessReveal", 1.0f);
	}

	void StartDupeGuessReveal(){

		finale.GetComponent<Text> ().text = "AND FINALLY... WHAT'D THE DUPE GUESS???";
		guess.GetComponent<Text> ().text = dupeGuess;

		finale.transform.DOLocalMoveX (0, 1.0f).SetEase (Ease.OutBounce);
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
			RightDupeGuess ();
			GivePoints (myRoom.dupeNum, 3, 1);

		} else {
			WrongDupeGuess ();
			GivePointsEveryoneBut (myRoom.dupeNum, 2);
			dupeStatusText.text = "THE TRUE SUBJECT WAS " + myRoom.rightword; 
			finale.transform.DOLocalMoveX (1000, 1.0f);
			dupeStatusObj.transform.DOLocalMoveX (0, 1.0f);

		}

		Invoke ("EndTheRound", 6.5f);

	}

	void RightDupeGuess(){
	
		int dupeNum = myRoom.dupeNum;

		GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [dupeNum -1]);
		AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
		awardScript.SetupDupeGuess (3,dupeNum);

	
	}

	void WrongDupeGuess(){
	
		int dupeNum = myRoom.dupeNum;

		if (dupeNum != 1) {
			GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [0]);
			AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
			awardScript.SetupDupeGuess (2,dupeNum);
		}

		if (dupeNum != 2) {
			GameObject newIcon2 = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [1]);
			AwardIconScript awardScript2 = newIcon2.GetComponent<AwardIconScript> ();
			awardScript2.SetupDupeGuess (2,dupeNum);
		}

		if (dupeNum != 3) {
			GameObject newIcon3 = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [2]);
			AwardIconScript awardScript3 = newIcon3.GetComponent<AwardIconScript> ();
			awardScript3.SetupDupeGuess (2,dupeNum);
		}

		if (dupeNum != 4) {
			GameObject newIcon4 = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [3]);
			AwardIconScript awardScript4 = newIcon4.GetComponent<AwardIconScript> ();
			awardScript4.SetupDupeGuess (2,dupeNum);
		}

	}

	void StoreScore (){
	
		//string roundNum = myRoom.roundNum.ToString();

		string roundLoc = myRoom.roundNum + "check" + "abcde";
		string doneString = PlayerPrefs.GetString (roundLoc);

		if (doneString == "Done") {
			return;
		}

		for (int i = 0; i < players.Length; i++) {

			string location = players [i].text + "abcde";
			int currentScore = PlayerPrefs.GetInt (location);
			int newScore = currentScore + int.Parse (scores [i].text);
			PlayerPrefs.SetInt (location, newScore);

		}

		PlayerPrefs.SetString (roundLoc, "Done");

	}

	void EndTheRound (){

		RoomManager roomManScript = roomMan.GetComponent<RoomManager> ();

		if (myRoom.privateRoom == true) {
			StoreScore ();
			string catName = myRoom.roomType;
			roomManScript.StartNextPrivateRound (catName);

			UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();

			string roomsString = userAccount.activeRooms;
			string currentRoom = myRoom.roomID + "/";

			roomsString = roomsString.Replace (currentRoom, string.Empty);

			if (roomsString.Length < 5) {
				roomsString = string.Empty;
			}

			userAccount.activeRooms = roomsString;
			userAccount.StoreEditedRooms (roomsString);

		} else {
			roomManScript.cameFromScoring = true;
			Invoke ("ReallyEndRoundPublic", 1.0f);
		}

		Destroy(myRoom.gameObject);

		roomManScript.CurtainsIn ();

	}

	void ReallyEndRoundPublic (){
	
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

		roomMan.GetComponent<RoomManager> ().SendTheScore (myScore, myColor, roomIDstring, roomsString);
		RoomManager.instance.cameFromTurnBased = true;
		SceneManager.LoadScene ("Lobby Menu");

	}

	//play animation = 1
	void GivePoints (int playerNum, int points, int animation) {
	
		if (playerNum == 1) {
			redScore = redScore + points;
			redScoreText.text = redScore.ToString ();
			if (animation == 1) {
				if (points > 0) {
					redFlash.SetActive (true);
					Invoke ("TurnOffRed", 1.5f);
				} else if (points < 0) {
					MinusPointsAnimation (1);
				}
			}
		} else if (playerNum == 2) {
			blueScore = blueScore + points;
			blueScoreText.text = blueScore.ToString ();
			if (animation == 1) {
				if (points > 0) {
					blueFlash.SetActive (true);
					Invoke ("TurnOffBlue", 1.5f);
				} else if (points < 0) {
					MinusPointsAnimation (2);
				}
			}
		} else if (playerNum == 3) {
			greenScore = greenScore + points;
			greenScoreText.text = greenScore.ToString ();
			if (animation == 1) {
				if (points > 0) {
					greenFlash.SetActive (true);
					Invoke ("TurnOffGreen", 1.5f);
				} else if (points < 0) {
					MinusPointsAnimation (3);
				}
			}
		} else if (playerNum == 4) {
			orangeScore = orangeScore + points;
			orangeScoreText.text = orangeScore.ToString ();
			if (animation == 1) {
				if (points > 0) {
					orangeFlash.SetActive (true);
					Invoke ("TurnOffOrange", 1.5f);
				} else if (points < 0) {
					MinusPointsAnimation (4);
				}
			}
		}
	
	}

	void GivePointsEveryoneBut (int playerNum, int points) {

		if (playerNum == 1) {
			blueScore = blueScore + points;
			blueScoreText.text = blueScore.ToString ();
			if (points > 0) {
				blueFlash.SetActive (true);
				Invoke ("TurnOffBlue", 1.5f);
			} else if (points < 0) {
				MinusPointsAnimation (2);
			}
			greenScore = greenScore + points;
			greenScoreText.text = greenScore.ToString ();
			if (points > 0) {
				greenFlash.SetActive (true);
				Invoke ("TurnOffGreen", 1.5f);
			} else if (points < 0) {
				MinusPointsAnimation (3);
			}
			orangeScore = orangeScore + points;
			orangeScoreText.text = orangeScore.ToString ();
			if (points > 0) {
				orangeFlash.SetActive (true);
				Invoke ("TurnOffOrange", 1.5f);
			} else if (points < 0) {
				MinusPointsAnimation (4);
			}
		} else if (playerNum == 2) {
			redScore = redScore + points;
			redScoreText.text = redScore.ToString ();
			if (points > 0) {
				redFlash.SetActive (true);
				Invoke ("TurnOffRed", 1.5f);
			} else if (points < 0) {
				MinusPointsAnimation (1);
			}
			greenScore = greenScore + points;
			greenScoreText.text = greenScore.ToString ();
			if (points > 0) {
				greenFlash.SetActive (true);
				Invoke ("TurnOffGreen", 1.5f);
			} else if (points < 0) {
				MinusPointsAnimation (3);
			}
			orangeScore = orangeScore + points;
			orangeScoreText.text = orangeScore.ToString ();
			if (points > 0) {
				orangeFlash.SetActive (true);
				Invoke ("TurnOffOrange", 1.5f);
			} else if (points < 0) {
				MinusPointsAnimation (4);
			}
		} else if (playerNum == 3) {
			redScore = redScore + points;
			redScoreText.text = redScore.ToString ();
			if (points > 0) {
				redFlash.SetActive (true);
				Invoke ("TurnOffRed", 1.5f);
			} else if (points < 0) {
				MinusPointsAnimation (1);
			}
			blueScore = blueScore + points;
			blueScoreText.text = blueScore.ToString ();
			if (points > 0) {
				blueFlash.SetActive (true);
				Invoke ("TurnOffBlue", 1.5f);
			} else if (points < 0) {
				MinusPointsAnimation (2);
			}
			orangeScore = orangeScore + points;
			orangeScoreText.text = orangeScore.ToString ();
			if (points > 0) {
				orangeFlash.SetActive (true);
				Invoke ("TurnOffOrange", 1.5f);
			} else if (points < 0) {
				MinusPointsAnimation (4);
			}
		} else if (playerNum == 4) {
			redScore = redScore + points;
			redScoreText.text = redScore.ToString ();
			if (points > 0) {
				redFlash.SetActive (true);
				Invoke ("TurnOffRed", 1.5f);
			} else if (points < 0) {
				MinusPointsAnimation (1);
			}
			blueScore = blueScore + points;
			blueScoreText.text = blueScore.ToString ();
			if (points > 0) {
				blueFlash.SetActive (true);
				Invoke ("TurnOffBlue", 1.5f);
			} else if (points < 0) {
				MinusPointsAnimation (2);
			}
			greenScore = greenScore + points;
			greenScoreText.text = greenScore.ToString ();
			if (points > 0) {
				greenFlash.SetActive (true);
				Invoke ("TurnOffGreen", 1.5f);
			} else if (points < 0) {
				MinusPointsAnimation (3);
			}

		}

	}

	void GivePointsEveryoneButAndDupe (int playerNum, int points) {

		int dupeNum = myRoom.dupeNum;

		if (playerNum == 1) {

			if (dupeNum != 2) {
				blueScore = blueScore + points;
				blueScoreText.text = blueScore.ToString ();
				if (points > 0) {
					blueFlash.SetActive (true);
					Invoke ("TurnOffBlue", 1.5f);
				} else if (points < 0) {
					MinusPointsAnimation (2);
				}
			}
			if (dupeNum != 3) {
				greenScore = greenScore + points;
				greenScoreText.text = greenScore.ToString ();
				if (points > 0) {
					greenFlash.SetActive (true);
					Invoke ("TurnOffGreen", 1.5f);
				} else if (points < 0) {
					MinusPointsAnimation (3);
				}
			}
			if (dupeNum != 4) {
				orangeScore = orangeScore + points;
				orangeScoreText.text = orangeScore.ToString ();
				if (points > 0) {
					orangeFlash.SetActive (true);
					Invoke ("TurnOffOrange", 1.5f);
				} else if (points < 0) {
					MinusPointsAnimation (4);
				}
			}
		} else if (playerNum == 2) {
			if (dupeNum != 1) {
				redScore = redScore + points;
				redScoreText.text = redScore.ToString ();
				if (points > 0) {
					redFlash.SetActive (true);
					Invoke ("TurnOffRed", 1.5f);
				} else if (points < 0) {
					MinusPointsAnimation (1);
				}
			}
			if (dupeNum != 3) {
				greenScore = greenScore + points;
				greenScoreText.text = greenScore.ToString ();
				if (points > 0) {
					greenFlash.SetActive (true);
					Invoke ("TurnOffGreen", 1.5f);
				} else if (points < 0) {
					MinusPointsAnimation (3);
				}
			}
			if (dupeNum != 4) {
				orangeScore = orangeScore + points;
				orangeScoreText.text = orangeScore.ToString ();
				if (points > 0) {
					orangeFlash.SetActive (true);
					Invoke ("TurnOffOrange", 1.5f);
				} else if (points < 0) {
					MinusPointsAnimation (4);
				}
			}
		} else if (playerNum == 3) {
			if (dupeNum != 1) {
				redScore = redScore + points;
				redScoreText.text = redScore.ToString ();
				if (points > 0) {
					redFlash.SetActive (true);
					Invoke ("TurnOffRed", 1.5f);
				} else if (points < 0) {
					MinusPointsAnimation (1);
				}
			}
			if (dupeNum != 2) {
				blueScore = blueScore + points;
				blueScoreText.text = blueScore.ToString ();
				if (points > 0) {
					blueFlash.SetActive (true);
					Invoke ("TurnOffBlue", 1.5f);
				} else if (points < 0) {
					MinusPointsAnimation (2);
				}
			}
			if (dupeNum != 4) {
				orangeScore = orangeScore + points;
				orangeScoreText.text = orangeScore.ToString ();
				if (points > 0) {
					orangeFlash.SetActive (true);
					Invoke ("TurnOffOrange", 1.5f);
				} else if (points < 0) {
					MinusPointsAnimation (4);
				}
			}
		} else if (playerNum == 4) {
			if (dupeNum != 1) {
				redScore = redScore + points;
				redScoreText.text = redScore.ToString ();
				if (points > 0) {
					redFlash.SetActive (true);
					Invoke ("TurnOffRed", 1.5f);
				} else if (points < 0) {
					MinusPointsAnimation (1);
				}
			}
			if (dupeNum != 2) {
				blueScore = blueScore + points;
				blueScoreText.text = blueScore.ToString ();
				if (points > 0) {
					blueFlash.SetActive (true);
					Invoke ("TurnOffBlue", 1.5f);
				} else if (points < 0) {
					MinusPointsAnimation (2);
				}
			}
			if (dupeNum != 3) {
				greenScore = greenScore + points;
				greenScoreText.text = greenScore.ToString ();
				if (points > 0) {
					greenFlash.SetActive (true);
					Invoke ("TurnOffGreen", 1.5f);
				} else if (points < 0) {
					MinusPointsAnimation (3);
				}
			}

		}

	}

	void MinusPointsAnimation (int playerNum){
		Vector3 threesixty = new Vector3 (0f, 0f, 1080f);
		scoreObj [playerNum - 1].transform.DOLocalRotate (threesixty, 1.5f,RotateMode.FastBeyond360).SetEase (Ease.InOutFlash);

//		Sequence mySequence = DOTween.Sequence();
//		// Add a movement tween at the beginning
//		mySequence.Append(SpriteRenderer.DOColor (redColor, .75f).SetEase (Ease.InOutBounce);
//		// Add a rotation tween as soon as the previous one is finished
//		mySequence.Append(transform.DORotate(new Vector3(0,180,0), 1));
//		
//		
//
//
//		Color current = scoreObj [playerNum - 1].GetComponent<SpriteRenderer> ().color;
//		Color redColor = Color.red;
//
//		scoreObj [playerNum - 1].GetComponent<SpriteRenderer> ().DOColor (redColor, .75f).SetEase (Ease.InOutBounce);

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
