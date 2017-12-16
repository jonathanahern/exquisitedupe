using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DatabaseControl;
using UnityEngine.SceneManagement;

public class LocalTurnScoring : MonoBehaviour {

	float speed;
	bool fastForward = false;

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
	public GameObject trueArtist;
	public GameObject congrats;
	public Text gameWinnerName;
	int winnerNumber;

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
	public GameObject[] dupeHats;
	public Image[] bars;
	public GameObject[] splatterOne;
	public GameObject[] splatterTwo;

	Vector2 dupeDrewScreenPos;

	public Transform[] awardHolder;
	public GameObject awardPrefab;

	public GameObject exitSign;

	public List<int> votes;
	public List<int> winnersCircle;
	public List<int> votesNums;
	public List<int> winnersGame;
	public int[] pointers;

	int[] monaPoints = {0,0,0,0};

	private static string MYCOLOR_SYM = "[MYCOLOR]";

	public GameObject signOne;
	public GameObject signTwo;
	public Image nextImage;
	public Sprite exitSprite;

	int stepNum = 0;

	public GameObject[] playerIcons;
	public GameObject starPrefab;
	GameObject[] starPoints;
	public Transform[] starPos;
	public GameObject starHolder;
	float abovePainting;
	float behindPainting;

	bool alreadyDupeGuessedIcon = false;
	bool alreadyDupeGivenIcon = false;

	List<int> starPeeps = new List<int>();
	List<int> monasAlready = new List<int>();

	public AnimationCurve splatterCurve;
	public AnimationCurve starEnd;
	public AnimationCurve wordBounce;
	//bool privateScoring = false;

	public Material[] lineMats;
	public Color[] regColors;

	public GameObject signs;

	// Use this for initialization
	void Start () {

		if (Camera.main.aspect < .5f) {
			//9.73
			Camera.main.GetComponent<Camera> ().orthographicSize = 11.8f;
			Vector3 signPos = new Vector3 (signs.transform.position.x, signs.transform.position.y - 1.25f, signs.transform.position.z);
			signs.transform.position = signPos;

		}

		for (int i = 0; i < lineMats.Length; i++) {
			lineMats[i].color = regColors[i];
		}

		starPoints = new GameObject[3];
		abovePainting = starHolder.transform.position.y;
		behindPainting = starHolder.transform.position.y - 1;
		starHolder.transform.DOMoveY (behindPainting, .1f);

		monasAlready = new List<int>();

		speed = .9f;

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

		dupeHats [myRoom.dupeNum - 1].SetActive (true);

		starPeeps = new List<int> ();
	
		if (myRoom.dupeNum == 1) {
			starPeeps.Add (3);
			starPeeps.Add (2);
			starPeeps.Add (1);
		} else if (myRoom.dupeNum == 2) {
			starPeeps.Add (0);
			starPeeps.Add (3);
			starPeeps.Add (2);
		} else if (myRoom.dupeNum == 3) {
			starPeeps.Add (0);
			starPeeps.Add (3);
			starPeeps.Add (1);
		} else if (myRoom.dupeNum == 4) {
			starPeeps.Add (0);
			starPeeps.Add (2);
			starPeeps.Add (1);
		} 

//		if (myRoom.privateRoom == true && myRoom.roundNum != 1) {
//
//			string myUsername = players [myRoom.myActualColor - 1].text;
//
//			for (int i = 0; i < players.Length; i++) {
//
//				string location = myUsername + players [i].text + "abcde";
//
//				int curScore = PlayerPrefs.GetInt (location);
//				scores [i].text = curScore.ToString();
//				Debug.Log (location);
//				Debug.Log ("Player " + myUsername + " scores " + curScore.ToString());
//				if (i == 0) {
//					redScore = curScore;
//				} else if (i == 1) {
//					blueScore = curScore;
//				} else if (i == 2) {
//					greenScore = curScore;
//				} else if (i == 3) {
//					orangeScore = curScore;
//				}
//
//			}
//
//		}

//		if (myRoom.privateRoom == true && myRoom.roundNum == 1) {
//			ClearPlayerPrefs ();
//		}

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.T)) {

			for (int i = 0; i < 3; i++) {

				starPoints[i] = Instantiate (starPrefab, starPos [i].position, Quaternion.identity, starPos[i]);

			}

			starHolder.transform.DOMoveY (abovePainting, .8f * speed).SetEase(wordBounce);

		}

		if (Input.GetKeyDown (KeyCode.Y)) {

		}

	}

	void MoveStarsUp(int starCount){

		for (int i = 0; i < 3; i++) {

			starPoints[i] = Instantiate (starPrefab, starPos [i].position, Quaternion.identity, starPos[i]);

		}

		starHolder.transform.DOMoveY (abovePainting, 1.2f * speed).SetEase(wordBounce);

	}


	//-1=mona 1=monkey, 2=vague, 3=obvious, 4=dupeCaught, 5=dupeEscape, 6=dupeGuessedRight, 7=dupeGuessedWrong
	//start with zero
	//0 don't score
	//1 score it
	void MoveTheStar(int playerNum, int starNum, float startTime, int scoreOrNot, int points, int awardNum){

		//Debug.Log (playerNum + " " + starNum);

		starPoints [starNum].GetComponent<SpriteRenderer> ().sortingOrder = 3;
		Vector3 fullRotation = new Vector3 (0,0,720);
//		Vector3 offscreen;
		Vector3 playerPos = playerIcons[playerNum].transform.position;



		int awardPhase;
		if (awardNum < 2) {
			awardPhase = 1;
		} else if (awardNum > 3) {
			awardPhase = 3;
		} else {
			awardPhase = 2;
		}

		starPoints [starNum].transform.DOMove (playerIcons [playerNum].transform.position, .7f * speed).SetDelay(startTime*speed).SetEase(Ease.InCirc).OnComplete(()=>ShrinkStar(starPoints[starNum], points, playerNum));
		starPoints [starNum].transform.DORotate (fullRotation, .7f * speed, RotateMode.FastBeyond360).SetDelay(startTime*speed).OnComplete(()=>GivePointsAndSplatter(playerNum,points,1,awardPhase,awardNum));

		//Sequence mySequence = DOTween.Sequence();
		//mySequence.Append(starPoints [starNum].transform.DOMove (playerIcons [playerNum].transform.position, .7f * speed)).SetEase(Ease.InCirc);
		//mySequence.Insert (0, starPoints [starNum].transform.DORotate (fullRotation, .7f * speed, RotateMode.FastBeyond360)).OnComplete(()=>GivePointsAndSplatter(playerNum,points,1,awardPhase,awardNum));
		//mySequence.Append (starPoints [starNum].transform.DOScale (Vector3.zero, .5f * speed).SetEase (starEnd));
//		if (scoreOrNot == 1) {
//			mySequence.Append (starPoints [starNum].transform.DOScale (Vector3.zero, .5f * speed).SetEase (starEnd));
//		} else {
//			mySequence.Append (starPoints [starNum].transform.DOMove (offscreen, .5f * speed).SetEase(Ease.OutCirc));
//			mySequence.Insert(0, starPoints[starNum].transform.DORotate (fullRotation, .5f * speed, RotateMode.FastBeyond360));
//		}
		//mySequence.PrependInterval(startTime * speed);

	}

	void ShrinkStar(GameObject star, int points, int playerNum){
		if (points > 0) {
			star.transform.DOScale (Vector3.zero, .5f * speed).SetEase (starEnd);
		} else {
		
			Vector3 offscreen;
			Vector3 playerPos = playerIcons[playerNum].transform.position;
			Vector3 fullRotation = new Vector3 (0,0,540);

			if (playerPos.x > 0) {
				offscreen = new Vector3 (playerPos.x + 4.0f, playerPos.y + 3.5f, playerPos.z);
			} else {
				offscreen = new Vector3 (playerPos.x - 4.0f, playerPos.y + 3.5f, playerPos.z);
			}

			star.transform.DOMove (offscreen, 1.5f * speed).SetEase(Ease.OutCirc);
			star.transform.DORotate (fullRotation, 1.5f * speed, RotateMode.FastBeyond360);
		
		}
	
	}

	void GivePointsAndSplatter (int playerNum, int points, int animation, int awardPhase, int awardNum){
	
		GivePoints(playerNum+1,points,1);
		if (awardNum == -1) {
			AddAwardIconToOne (awardNum, playerNum);
		} else if (awardPhase < 3) {
			AddSplatterToOne (awardNum, awardPhase, playerNum);
		} else {
			AddAwardIconToOne (awardNum, playerNum);
		}

	}

	void GetVoteData (){
		
		myRoom.votePoses = myRoom.votePoses.TrimEnd('@');
		string[] voteDataWhole = myRoom.votePoses.Split ('@');

		int guessCount = 0;

		foreach (string voteData in voteDataWhole) {
			Debug.Log (voteData);
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

		Invoke ("OpenCurtains", 1.5f * speed);
		Invoke ("BeginDupeReveal", 4 * speed);

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

	int DupeWinner (int[]playersWhoWin){
	
		if (playersWhoWin.Length == 1) {
			Debug.Log ("Dupe Winner: " + playersWhoWin [0]);
			return playersWhoWin [0];
		} else if (playersWhoWin.Length == 2) {

			if (playersWhoWin [0] == myRoom.dupeNum) {
				dupeTie = true;
				return playersWhoWin [1];
			} else {
				dupeTie = true;
				return playersWhoWin [0];
			}

		} else {
			dupeTie = true;
			return 0;
		}
	
	}

	int AwardWinner (int[]playersWhoWin){

		if (playersWhoWin.Length == 1) {

			//Debug.Log ("Award Winner: " + playersWhoWin [0]);

			return playersWhoWin [0];
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

	void BeginDupeReveal () {
		stepNum = 1;
		intro.transform.DOLocalMoveX (-1000, 1.0f * speed);
		questionObj.transform.DOLocalMoveX (0, 1.0f * speed).SetEase (wordBounce);
		MoveStarsUp (3);

		Invoke ("RevealDupeVotes", 2.0f * speed);

	
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
			nameText.text = "IT'S A TIE!!!";
		} else {
			nameText.text = players[dupeGuessed - 1].text;
		}

		//Vector3 neg90 = new Vector3 (0, 0, -90);
		nameObj.transform.DOLocalMoveX (0, 1.0f * speed).SetEase (wordBounce);
		Invoke ("RevealDupeGuess", 2.0f * speed);
	}
		

	void GiveDupeGuessPoints (){
	
		if (WhoYouVoted (redPos [0]) == myRoom.dupeNum) {
			GivePoints (1, 1, 0);
			GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [0]);
			AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
			awardScript.SetupDupeFound ();
		}

		if (WhoYouVoted (bluePos [0]) == myRoom.dupeNum) {
			GivePoints (2, 1, 0);
			GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [1]);
			AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
			awardScript.SetupDupeFound ();
		}

		if (WhoYouVoted (greenPos [0]) == myRoom.dupeNum) {
			GivePoints (3, 1, 0);
			GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [2]);
			AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
			awardScript.SetupDupeFound ();
		}

		if (WhoYouVoted (orangePos [0]) == myRoom.dupeNum) {
			GivePoints (4, 1, 0);
			GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [3]);
			AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
			awardScript.SetupDupeFound ();
		}
	
	}

	void RevealDupeGuess(){
	
		if (dupeTie == true) {
			nameText.text = "IT'S A TIE!!!";
		} else {
			nameText.text = players[dupeGuessed - 1].text;
			LockHimUp (dupeGuessed - 1);
		}

		//Vector3 neg90 = new Vector3 (0, 0, -90);
		//questionRotation.transform.DORotate (neg90, 1.0f).SetEase (Ease.OutBounce);
		nameObj.transform.DOLocalMoveX (0, 1.0f * speed).SetEase (wordBounce);
		Invoke ("RevealDupeStatus", 1.5f * speed);
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


		//Invoke ("GiveOutDupePoints", 1.0f * speed);
		Invoke ("SendOutDupeStars", 1.0f * speed);
		//Vector3 pos90 = new Vector3 (0, 0, 90);
		dupeStatusObj.transform.DOLocalMoveX (0, 1.0f * speed).SetEase (wordBounce);
		questionObj.transform.DOLocalMoveX (1000, 1.0f * speed);
		nameObj.transform.DOLocalMoveX (1000, 1.0f * speed);

	}

	//4 dupeCaught 5 dupeEscape
	void SendOutDupeStars (){
	
		int dupeNum = myRoom.dupeNum;
		float startTime = 0;
		//Debug.Log ("STARPEEP: " + starPeeps.Count);
		if (dupeGuessed == dupeNum) {

			for (int i = 0; i < 3; i++) {

				MoveTheStar (starPeeps [i], i, startTime, 1, 2, 4);
				startTime = startTime + .5f;

			}

		} else {
		
			for (int i = 0; i < 3; i++) {

				MoveTheStar (dupeNum-1, i, startTime, 1, 1, 5);
				startTime = startTime + .5f;

			}


		}

		stepNum = 2;

		if (fastForward == false) {
			Invoke ("CheckIfStillSlow", 3.5f * speed);
		} else {
			Invoke ("StartSecondAward", 4.5f * speed);
		}

	}

	void LockHimUp (int personCaught){
	
		Debug.Log ("Lock: " + personCaught);

		DOTween.To (() => bars [personCaught].fillAmount, x => bars [personCaught].fillAmount = x, 1, 1.1f * speed).SetEase (Ease.OutBounce);

	}

//	void GiveOutDupePoints (){
//		
//		if (dupeTie == true) {
//			GivePoints (myRoom.dupeNum, 3, 1);
//			CreateDupeEscapeAwardIcon ();
//		} else if (dupeGuessed == myRoom.dupeNum) {
//			GivePointsEveryoneBut (myRoom.dupeNum, 2);
//			CreateDupeCapturedAwardIcon ();
//		} else if (dupeGuessed != myRoom.dupeNum) {
//			GivePoints (myRoom.dupeNum, 3, 1);
//			CreateDupeEscapeAwardIcon ();
//		} else  {
//			GivePointsEveryoneBut (myRoom.dupeNum, 1);
//			CreateDupeCapturedAwardIcon ();
//		}
//
//		stepNum = 2;
//
//		if (fastForward == false) {
//			Invoke ("FlipSignTwoToNext", 1.5f * speed);
//		} else {
//			Invoke ("StartSecondAward", 2.5f * speed);
//		}
//			
//	}

//	void CreateDupeEscapeAwardIcon () {
//	
//		int dupeNum = myRoom.dupeNum;
//
//		GameObject newIcon = Instantiate(awardPrefab,Vector3.zero,Quaternion.identity, awardHolder[dupeNum -1]);
//		AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
//		awardScript.SetupEscaped (dupeNum);
//	
//	}

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
		starHolder.transform.DOMoveY (behindPainting, 0.0f);
		DestroyVotes ();

		//award2Winner = AwardWinner (FindWinner (1));
		//Debug.Log (myRoom.players [award2Winner - 1]);

		if (myRoom.awardNum == 1) {

			award2Winner = AwardWinner (FindWinner (1));

			questionText.text = "WHO GOT THE MOST MONKEY VOTES?";
			if (award2Winner == 0) {
				nameText.text = "IT'S A TIE!!!";
			} else {
				nameText.text = myRoom.players [award2Winner - 1];
			}

		} else if (myRoom.awardNum > 1) {

			string awardWinners = GetWinnerNames();
			questionText.text = "WHO RECEIVED A MONA MASTERPIECE?";
			nameText.text = awardWinners;

		}

		MoveStarsUp (3);

		questionObj.transform.DOLocalMoveX (0, 1.0f * speed).SetEase(wordBounce);
		dupeStatusObj.transform.DOLocalMoveX (-1000, 1.0f * speed);
		Invoke ("RevealSecondVotes", 2.0f * speed);
			
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

		if (awardNum == 1) {
			if (award2Winner != 0) {
				SplatterIcon (award2Winner, 1);
			} else {
				AddSplatterToAllIcons (myRoom.awardNum);
			}
		}

		nameObj.transform.DOLocalMoveX (0, 1.0f * speed).SetEase (wordBounce).OnComplete(SendOutStarsAward2);

	}

	void SendOutStarsAward2(){
	
		//int dupeNum = myRoom.dupeNum;
		float startTime = 0;
		int secondAwardNum;

		if (myRoom.awardNum == 1){
			secondAwardNum = 1;

			if (award2Winner == 0) {

				for (int i = 0; i < 3; i++) {

					MoveTheStar (starPeeps [i], i, startTime, 1, 0, secondAwardNum);
					startTime = startTime + .5f;

				}

			} else {

				for (int i = 0; i < 3; i++) {

					int scoreOrNot = 1;
					int points = 1;

					//Debug.Log(award2Winner + starPeeps[i] - 1

					if (award2Winner == starPeeps [i] + 1) {
						scoreOrNot = 0;
						points = 0;
					}

					MoveTheStar (starPeeps [i], i, startTime, scoreOrNot, points, secondAwardNum);
					startTime = startTime + .5f;

				}

			}

		} else {
			//mona
			secondAwardNum = -1;
			for (int i = 0; i < pointers.Length; i++) {

				MoveTheStar (pointers [i] - 1, i, startTime, 1, 1, -1);
				startTime = startTime + .5f;

			}

		}
			
		stepNum = 3;

		if (fastForward == false) {
			Invoke ("CheckIfStillSlow", 3.0f * speed);
		} else {
			Invoke ("StartThirdAward", 4.0f * speed);
		}
	
	}

//	void GiveOutAward2Points (){
//
//		if (myRoom.awardNum == 1) {
//			
//			if (award2Winner != 0) {
//				//GivePointsEveryoneButAndDupe (award2Winner, 1);
//				//AddSplatterToOne(1,1);
//				//SplatterIcon (award2Winner, 1);
//			} else {
//				AddSplatterToAll (myRoom.awardNum);
//			}
//
//		} else if (myRoom.awardNum > 1) {
//
//			int lastNum = 100;
//			int animate;
//
//			for (int i = 0; i < pointers.Length; i++) {
//				if (lastNum == pointers [i]) {
//					animate = 0;
//				} else {
//					animate = 1;
//				}
//
//				monaPoints [pointers[i] - 1] = monaPoints [pointers[i] - 1] + 1;
//				GivePoints (pointers [i], 1, animate);
//				lastNum = pointers [i];
//			}
//			GiveMonaAwardIcons ();
//
//		} 
//
//		stepNum = 3;
//
//		if (fastForward == false) {
//			Invoke ("FlipSignTwoToNext", 1.5f * speed);
//		} else {
//			Invoke ("StartThirdAward", 2.5f * speed);
//		}
//
//	}

	void SplatterIcon(int playerNum, int awardNum) {

		Vector3 fullSize = new Vector3 (4.0f, 4.0f, 4.0f);
		if (awardNum == 1) {
			splatterOne [playerNum - 1].SetActive (true);
			splatterOne [playerNum - 1].transform.DOScale (fullSize, .8f * speed).SetEase (splatterCurve);
		} else {
			splatterTwo [playerNum - 1].SetActive (true);
			splatterTwo [playerNum - 1].transform.DOScale (fullSize, .8f * speed).SetEase (splatterCurve);
		}

	}

	void AddSplatterToAllIcons(int awardNum){
		
		int dupeNum = myRoom.dupeNum;

		int awardPhase;

		if (awardNum == 1) {
			awardPhase = 1;
		} else {
			awardPhase = 2;
		}

		if (dupeNum != 1) {
			SplatterIcon (1,awardPhase);
		}
		if (dupeNum != 2) {
			SplatterIcon (2,awardPhase);
		}
		if (dupeNum != 3) {
			SplatterIcon (3,awardPhase);
		}
		if (dupeNum != 4) {
			SplatterIcon (4,awardPhase);
		}

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
	void AddSplatterToOne(int awardNum, int awardPhase, int playerNum){
		//Debug.Log ("SPLAT: " + playerNum);
		//int dupeNum = myRoom.dupeNum;
		int splatterWinner = AwardWinner (FindWinner (awardPhase));

		GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [playerNum]);
		AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
		if (splatterWinner == playerNum + 1 || splatterWinner == 0) {
			
			awardScript.SetupSplatter (playerNum+1, awardNum);

			if (awardPhase == 1) {
				Vector3 smallSize = new Vector3 (2.0f, 2.0f, 2.0f);
				splatterOne [playerNum].transform.DOScale (smallSize, .5f * speed).SetEase (splatterCurve);
			} else {
				Vector3 smallSize = new Vector3 (2.0f, 2.0f, 2.0f);
				splatterTwo [playerNum].transform.DOScale (smallSize, .5f * speed).SetEase (splatterCurve);
			}

		} else {
			awardScript.SetupSplatterAvoider (awardNum);
		}

	}

	void AddAwardIconToOne(int awardNum, int playerNum){

		if (awardNum == 4 && (myRoom.dupeNum == dupeGuessed)) {
			GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [playerNum]);
			AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
			awardScript.SetupCaptured (myRoom.dupeNum);
		} else if (awardNum == 4 && (myRoom.dupeNum != dupeGuessed)) {
			GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [playerNum]);
			AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
			awardScript.SetupCaptured (myRoom.dupeNum);
		} else if (awardNum == 6 && alreadyDupeGuessedIcon == false) {
			alreadyDupeGuessedIcon = true;
			int dupeNum = myRoom.dupeNum;
			GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [dupeNum -1]);
			AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
			awardScript.SetupDupeGuess (3,dupeNum);
		} else if (awardNum == 5 && alreadyDupeGivenIcon == false) {
			alreadyDupeGivenIcon = true;
			int dupeNum = myRoom.dupeNum;
			GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [dupeNum -1]);
			AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
			awardScript.SetupEscaped (dupeNum);
		} else if (awardNum == 7) {
			GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [playerNum]);
			AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
			awardScript.SetupDupeGuess (2,myRoom.dupeNum);
		} else if (awardNum == -1) {
			bool monaFound = false;
			//Debug.Log ("playersent: " + playerNum);
			for (int i = 0; i < monasAlready.Count; i++) {
				//Debug.Log ("monasalready: " + monasAlready[i]);
				if (monasAlready [i] == playerNum) {
					monaFound = true;
				}
			}
			if (monaFound == false) {
				GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [playerNum]);
				AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
				awardScript.SetupMona (1);
				monasAlready.Add (playerNum);
			} else {
				//Debug.Log ("MONA TRUE");
				AddToMonaAwardIcon (playerNum);
			}
		}

	}

	void AddToMonaAwardIcon (int playerNum){
		Debug.Log ("MONA TRUE: " + playerNum);
		int awardCount = awardHolder[playerNum].transform.childCount;
		Debug.Log ("CHILDS: " + awardCount);
		awardHolder [playerNum].transform.GetChild(awardCount-1).GetComponent<AwardIconScript> ().AddAPointToMona ();

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
		
		starHolder.transform.DOMoveY (behindPainting, 0.0f);
		MoveStarsUp (3);

		DestroyVotes ();


		award3Winner = AwardWinner (FindWinner (2));

		if (myRoom.dupeCaught == "o") {
			award3QText.text = "WHO GOT THE MOST CAPT OBVIOUS VOTES?";
		} else {
			award3QText.text = "WHO GOT THE MOST VAGUE PANTS VOTES?";
		}

		award3Q.transform.DOLocalMoveX (0, 1.0f * speed).SetEase(wordBounce);
		questionObj.transform.DOLocalMoveX (-1000, 1.0f * speed);
		nameObj.transform.DOLocalMoveX (-1000, 1.0f * speed);

		Invoke ("RevealThirdVotes", 2.0f * speed);

	}

	void RevealThirdVotes(){

		if (award3Winner == 0) {
			nameText.text = "IT'S A TIE!!!";
		} else {
			nameText.text = myRoom.players [award3Winner - 1];
		}

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
		nameObj.transform.DOLocalMoveX (0, 1.0f * speed).SetEase (wordBounce).OnComplete(GiveOutAward3Points);

	}

	void GiveOutAward3Points (){

		int awardNum;

		if (myRoom.dupeCaught == "o") {
			awardNum = 3;
		} else {
			awardNum = 2;
		}

		if (award3Winner != 0) {
			
			//GivePointsEveryoneButAndDupe (award3Winner, 2);
			SplatterIcon(award3Winner,awardNum);
			//AddSplatterToOne (awardNum, 2);

		} else {

			AddSplatterToAllIcons (awardNum);
			//AddSplatterToAll (awardNum);

		}

		Invoke ("SendOutAward3Stars", 2.5f * speed);

//		stepNum = 4;
//
//		if (fastForward == false) {
//			Invoke ("FlipSignTwoToNext", 1.5f * speed);
//		} else {
//			Invoke ("StartNonDupeGuessReveal", 2.5f * speed);
//		}

	}

	void SendOutAward3Stars (){
		
		//int dupeNum = myRoom.dupeNum;
		float startTime = 0;
		int thirdAwardNum;

		if (myRoom.dupeCaught == "o") {
			thirdAwardNum = 3;
		} else {
			thirdAwardNum = 2;
		}

		for (int i = 0; i < 3; i++) {

			int scoreOrNot = 1;
			int points = 2;

			//Debug.Log(award2Winner + starPeeps[i] - 1

			if (award3Winner == starPeeps [i] + 1) {
				scoreOrNot = 0;
				points = 0;
			} else if (award3Winner == 0) {
				scoreOrNot = 0;
				points = 0;
			}

			MoveTheStar (starPeeps[i], i, startTime, scoreOrNot, points, thirdAwardNum);
			startTime = startTime + .5f;

		}

		stepNum = 4;

		if (fastForward == false) {
			Invoke ("CheckIfStillSlow", 3.5f * speed);
		} else {
			Invoke ("StartNonDupeGuessReveal", 5.0f * speed);
		}

	}

	void StartNonDupeGuessReveal(){

		DestroyVotes ();
		starHolder.transform.DOMoveY (behindPainting, 0.0f);

		finale.GetComponent<Text> ().text = "WHAT DID Y'ALL THINK THE DUPE DREW?";

		finale.transform.DOLocalMoveX (0, 1.0f * speed).SetEase (wordBounce);
		award3Q.transform.DOLocalMoveX (-1000, 1.0f * speed);
		nameObj.transform.DOLocalMoveX (-1000, 1.0f * speed);
		Invoke ("MoveFinaleOver", 3.5f * speed);

	}

	void MoveFinaleOver(){

		finale.transform.DOLocalMoveX (1000, 1.0f * speed).OnComplete(MoveInGuesses);

	}

	void MoveInGuesses(){
	
		for (int i = 0; i < guessObjs.Length; i++) {
			guessObjs [i].GetComponent<RectTransform> ().DOAnchorPos(guessOnScreen[i], .5f * speed).SetEase(wordBounce);
		}

		Invoke ("MoveInDupeWord", 2.5f * speed);
	
	}

	void MoveInDupeWord(){

		dupeDrew.GetComponent<RectTransform>().DOAnchorPos (dupeDrewScreenPos, 1.0f * speed).SetEase (wordBounce);

		Invoke ("ScoreGuesses", 2.5f * speed);

	}

	void ScoreGuesses (){

		for (int i = 0; i < guesserNames.Length; i++) {

			int playerNum = 0;

			for (int t = 0; t < players.Length; t++) {

				if (players [t].text == guesserNames [i].text) {
				
					playerNum = t +1;
					Debug.Log ("Num: " + playerNum);
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

		stepNum = 5;

		if (fastForward == false) {
			Invoke ("CheckIfStillSlow", 1.5f * speed);
		} else {
			Invoke ("MoveOutGuesses", 2.5f * speed);
		}

	}

	void SetupGuessIcon (int playerNum, int points){

		Debug.Log ("asdf: " + playerNum);

		GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [playerNum - 1]);
		AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
		awardScript.SetupGuess (points);

	}

	void MoveOutGuesses(){
	
		starHolder.transform.DOMoveY (behindPainting, 0.0f);

		for (int i = 0; i < guessObjs.Length; i++) {
			guessObjs [i].GetComponent<RectTransform> ().DOAnchorPos(guessOffScreen[i], 1.0f * speed);
		}
	
		dupeDrew.transform.DOLocalMoveX (-1000, 1.0f * speed);

		Invoke ("StartDupeGuessReveal", 1.0f * speed);
	}

	void StartDupeGuessReveal(){

		MoveStarsUp (3);

		finale.GetComponent<Text> ().text = "AND FINALLY... WHAT'D THE DUPE GUESS???";
		guess.GetComponent<Text> ().text = dupeGuess;

		finale.transform.DOLocalMoveX (0, 1.0f * speed).SetEase (wordBounce);
		Invoke ("RevealDupeSubjectGuess", 3.5f * speed);

	}

	void RevealDupeSubjectGuess (){

		guess.transform.DOLocalMoveX (0, 1.0f * speed).SetEase (wordBounce);
		finale.transform.DOLocalMoveX (1000, 1.0f * speed);
		Invoke ("RevealResult", 2.5f * speed);
	}

	void RevealResult (){
	
		if (dupeGuess == myRoom.rightword) {
		
			finale.GetComponent<Text> ().text = "RIGHTO!";

		} else {
			finale.GetComponent<Text> ().text = "WRONG!";
		}
	
		finale.transform.DOLocalMoveX (0, 1.0f * speed).SetEase (wordBounce);
		guess.transform.DOLocalMoveX (-1000, 1.0f * speed);
		Invoke ("GiveFinalePoints", 2.5f * speed);
	}



	void GiveFinalePoints (){

		if (dupeGuess == myRoom.rightword) {
//			RightDupeGuess ();
//			GivePoints (myRoom.dupeNum, 3, 1);

			float startTime = 0;

			for (int i = 0; i < 3; i++) {

				MoveTheStar (myRoom.dupeNum - 1, i, startTime, 1, 1, 6);
				startTime = startTime + .5f;

			}

//			stepNum = 6;
//			nextImage.sprite = exitSprite;
			Invoke ("EndGameSequence", 2.5f * speed);



		} else {
			
			//GivePointsEveryoneBut (myRoom.dupeNum, 2);
			dupeStatusText.text = "THE TRUE SUBJECT WAS " + myRoom.rightword; 
			finale.transform.DOLocalMoveX (1000, 1.0f * speed);
			dupeStatusObj.transform.DOLocalMoveX (0, 1.0f * speed);

			Invoke ("WrongDupeGuess", 2.5f * speed);

		}

		//endRoundText.SetActive (true);



	}

//	void RightDupeGuess(){
//	
//		int dupeNum = myRoom.dupeNum;
//
//		GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [dupeNum -1]);
//		AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
//		awardScript.SetupDupeGuess (3,dupeNum);
//
//	
//	}

	void WrongDupeGuess(){
	
		float startTime = 0;

		for (int i = 0; i < 3; i++) {

			MoveTheStar (starPeeps[i], i, startTime, 1, 2, 7);
			startTime = startTime + .5f;

		}



	//	stepNum = 6;
//		nextImage.sprite = exitSprite;
		Invoke ("EndGameSequence", 2.5f * speed);

//		int dupeNum = myRoom.dupeNum;
//
//		if (dupeNum != 1) {
//			GameObject newIcon = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [0]);
//			AwardIconScript awardScript = newIcon.GetComponent<AwardIconScript> ();
//			awardScript.SetupDupeGuess (2,dupeNum);
//		}
//
//		if (dupeNum != 2) {
//			GameObject newIcon2 = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [1]);
//			AwardIconScript awardScript2 = newIcon2.GetComponent<AwardIconScript> ();
//			awardScript2.SetupDupeGuess (2,dupeNum);
//		}
//
//		if (dupeNum != 3) {
//			GameObject newIcon3 = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [2]);
//			AwardIconScript awardScript3 = newIcon3.GetComponent<AwardIconScript> ();
//			awardScript3.SetupDupeGuess (2,dupeNum);
//		}
//
//		if (dupeNum != 4) {
//			GameObject newIcon4 = Instantiate (awardPrefab, Vector3.zero, Quaternion.identity, awardHolder [3]);
//			AwardIconScript awardScript4 = newIcon4.GetComponent<AwardIconScript> ();
//			awardScript4.SetupDupeGuess (2,dupeNum);
//		}

	}

	void StoreScore (){

		string myUsername = players [myRoom.myActualColor - 1].text;

		for (int i = 0; i < players.Length; i++) {

			string location = myUsername + players [i].text + "abcde";
			//int currentScore = PlayerPrefs.GetInt (location);
			int newScore = int.Parse (scores [i].text);
			Debug.Log (location + ": " + newScore);
			PlayerPrefs.SetInt (location, newScore);

		}

	}

	public void EndTheRound (){

		RoomManager roomManScript = roomMan.GetComponent<RoomManager> ();

		if (myRoom.privateRoom == true) {

			if (myRoom.roundNum > 3) {
			
				EndGameSequence ();
				return;
			
			}

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



		roomManScript.CurtainsIn ();

	}

	void ReallyEndRoundPublic (){
	
		int myScore = 0;

		if (myRoom.myColor == 1) {
			myScore = redScore;
		} else if (myRoom.myColor == 2) {
			myScore = blueScore;
		} else if (myRoom.myColor == 3) {
			myScore = greenScore;
		} else if (myRoom.myColor == 4) {
			myScore = orangeScore;
		}

		string roomIDstring = myRoomID.ToString ();

		UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();

		string roomsString = userAccount.activeRooms;
		string currentRoom = myRoom.roomID.ToString();

		roomsString = roomsString.Replace (currentRoom, string.Empty);
		roomsString = roomsString.TrimEnd ('/');
		roomsString = roomsString.TrimStart ('/');
		roomsString = roomsString.Replace ("//", "/");

		if (roomsString.Length < 1) {
			roomsString = string.Empty;
		}

		userAccount.activeRooms = roomsString;

		Debug.Log ("finished room " + roomIDstring);
		roomMan.GetComponent<RoomManager> ().SendTheScore (myScore, myRoom.myColor, roomIDstring, roomsString);
		RoomManager.instance.cameFromTurnBased = true;
		Destroy(myRoom.gameObject);
		SceneManager.LoadScene ("Lobby Menu");

	}

	void EndGameSequence(){
	
		//ClearPlayerPrefs ();
		string loserText= "YOU'RE LAST. QUITE PATHETIC!";
		string midText = "YOU DIDN'T WIN OR LOSE, HOW MEDIOCRE OF YOU";
		string winText = "SOMETHING MUST BE WRONG, YOU WON ONE!";
		winnersGame = new List<int>();

		for (int i = 0; i < players.Length; i++) {
			int scoreString = int.Parse(scores [i].text);
			winnersGame.Add (scoreString);
		}

		winnersGame.Sort ();

		int myScore = int.Parse(scores[myRoom.myActualColor - 1].text);

		if (myScore == winnersGame [winnersGame.Count - 1]) {
			trueArtist.GetComponent<Text> ().text = winText;
		} else if (myScore == winnersGame [0]) {
			trueArtist.GetComponent<Text> ().text = loserText;
		} else {
			trueArtist.GetComponent<Text> ().text = midText;
		}

//		string winnersScore = winnersGame [winnersGame.Count - 1].ToString ();
//
//		for (int i = 0; i < scores.Length; i++) {
//		
//			if (scores [i].text == winnersScore) {
//				winnerNumber = i;
//			}
//		
//		}

//		gameWinnerName.text = players [winnerNumber].text;

		finale.transform.DOLocalMoveX (1000, 1.0f * speed);
		dupeStatusObj.transform.DOLocalMoveX (1000, 1.0f * speed);
		trueArtist.transform.DOLocalMoveX (0, 1.0f * speed).SetEase (wordBounce);
		//Invoke ("BringInWinner", 3.5f * speed);

		stepNum = 6;
		nextImage.sprite = exitSprite;
		Invoke ("FlipSignTwoToNext", 1.5f * speed);


	}

	void BringInWinner(){
		
		trueArtist.transform.DOLocalMoveX (-1000, 1.0f * speed);
		congrats.transform.DOLocalMoveX (0, 1.0f * speed).SetEase (wordBounce);
		exitSign.SetActive (true);

	}

//	public void ExitSign (){
//		
//		RoomManager roomManScript = roomMan.GetComponent<RoomManager> ();
//		UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();
//
//		string roomsString = userAccount.activeRooms;
//		string currentRoom = myRoom.roomID + "/";
//
//		roomsString = roomsString.Replace (currentRoom, string.Empty);
//
//		if (roomsString.Length < 5) {
//			roomsString = string.Empty;
//		}
//
//		userAccount.activeRooms = roomsString;
//		userAccount.StoreEditedRooms (roomsString);
//
//		Destroy(myRoom.gameObject);
//		roomManScript.CurtainsIn ();
//
//		Invoke ("LeavePrivateGame", 2.0f * speed);
//
//	}

//	void LeavePrivateGame(){
//	
//		RoomManager.instance.cameFromTurnBased = true;
//		SceneManager.LoadScene ("Lobby Menu");
//	
//	}

//	void ClearPlayerPrefs (){
//		
//		string myUsername = players [myRoom.myActualColor - 1].text;
//
//		for (int i = 0; i < players.Length; i++) {
//
//			string location = myUsername + players [i].text + "abcde";
//
//			Debug.Log ("Clearing: " + location);
//
//			PlayerPrefs.DeleteKey (location);
//
//		}
//			
//	}

	//play animation = 1
	void GivePoints (int playerNum, int points, int animation) {
	
		if (playerNum == 1) {
			redScore = redScore + points;
			redScoreText.text = redScore.ToString ();
			if (animation == 1) {
				if (points > 0) {
					redFlash.SetActive (true);
					Invoke ("TurnOffRed", 1.5f * speed);
				} else if (points < 1) {
					MinusPointsAnimation (1);
				}
			}
		} else if (playerNum == 2) {
			blueScore = blueScore + points;
			blueScoreText.text = blueScore.ToString ();
			if (animation == 1) {
				if (points > 0) {
					blueFlash.SetActive (true);
					Invoke ("TurnOffBlue", 1.5f * speed);
				} else if (points < 1) {
					MinusPointsAnimation (2);
				}
			}
		} else if (playerNum == 3) {
			greenScore = greenScore + points;
			greenScoreText.text = greenScore.ToString ();
			if (animation == 1) {
				if (points > 0) {
					greenFlash.SetActive (true);
					Invoke ("TurnOffGreen", 1.5f * speed);
				} else if (points < 1) {
					MinusPointsAnimation (3);
				}
			}
		} else if (playerNum == 4) {
			orangeScore = orangeScore + points;
			orangeScoreText.text = orangeScore.ToString ();
			if (animation == 1) {
				if (points > 0) {
					orangeFlash.SetActive (true);
					Invoke ("TurnOffOrange", 1.5f * speed);
				} else if (points < 1) {
					MinusPointsAnimation (4);
				}
			}
		}
	
	}

//	void GivePointsEveryoneBut (int playerNum, int points) {
//
//		if (playerNum == 1) {
//			blueScore = blueScore + points;
//			blueScoreText.text = blueScore.ToString ();
//			if (points > 0) {
//				blueFlash.SetActive (true);
//				Invoke ("TurnOffBlue", 1.5f * speed);
//			} else if (points < 0) {
//				MinusPointsAnimation (2);
//			}
//			greenScore = greenScore + points;
//			greenScoreText.text = greenScore.ToString ();
//			if (points > 0) {
//				greenFlash.SetActive (true);
//				Invoke ("TurnOffGreen", 1.5f * speed);
//			} else if (points < 0) {
//				MinusPointsAnimation (3);
//			}
//			orangeScore = orangeScore + points;
//			orangeScoreText.text = orangeScore.ToString ();
//			if (points > 0) {
//				orangeFlash.SetActive (true);
//				Invoke ("TurnOffOrange", 1.5f * speed);
//			} else if (points < 0) {
//				MinusPointsAnimation (4);
//			}
//		} else if (playerNum == 2) {
//			redScore = redScore + points;
//			redScoreText.text = redScore.ToString ();
//			if (points > 0) {
//				redFlash.SetActive (true);
//				Invoke ("TurnOffRed", 1.5f * speed);
//			} else if (points < 0) {
//				MinusPointsAnimation (1);
//			}
//			greenScore = greenScore + points;
//			greenScoreText.text = greenScore.ToString ();
//			if (points > 0) {
//				greenFlash.SetActive (true);
//				Invoke ("TurnOffGreen", 1.5f * speed);
//			} else if (points < 0) {
//				MinusPointsAnimation (3);
//			}
//			orangeScore = orangeScore + points;
//			orangeScoreText.text = orangeScore.ToString ();
//			if (points > 0) {
//				orangeFlash.SetActive (true);
//				Invoke ("TurnOffOrange", 1.5f * speed);
//			} else if (points < 0) {
//				MinusPointsAnimation (4);
//			}
//		} else if (playerNum == 3) {
//			redScore = redScore + points;
//			redScoreText.text = redScore.ToString ();
//			if (points > 0) {
//				redFlash.SetActive (true);
//				Invoke ("TurnOffRed", 1.5f * speed);
//			} else if (points < 0) {
//				MinusPointsAnimation (1);
//			}
//			blueScore = blueScore + points;
//			blueScoreText.text = blueScore.ToString ();
//			if (points > 0) {
//				blueFlash.SetActive (true);
//				Invoke ("TurnOffBlue", 1.5f * speed);
//			} else if (points < 0) {
//				MinusPointsAnimation (2);
//			}
//			orangeScore = orangeScore + points;
//			orangeScoreText.text = orangeScore.ToString ();
//			if (points > 0) {
//				orangeFlash.SetActive (true);
//				Invoke ("TurnOffOrange", 1.5f * speed);
//			} else if (points < 0) {
//				MinusPointsAnimation (4);
//			}
//		} else if (playerNum == 4) {
//			redScore = redScore + points;
//			redScoreText.text = redScore.ToString ();
//			if (points > 0) {
//				redFlash.SetActive (true);
//				Invoke ("TurnOffRed", 1.5f * speed);
//			} else if (points < 0) {
//				MinusPointsAnimation (1);
//			}
//			blueScore = blueScore + points;
//			blueScoreText.text = blueScore.ToString ();
//			if (points > 0) {
//				blueFlash.SetActive (true);
//				Invoke ("TurnOffBlue", 1.5f * speed);
//			} else if (points < 0) {
//				MinusPointsAnimation (2);
//			}
//			greenScore = greenScore + points;
//			greenScoreText.text = greenScore.ToString ();
//			if (points > 0) {
//				greenFlash.SetActive (true);
//				Invoke ("TurnOffGreen", 1.5f * speed);
//			} else if (points < 0) {
//				MinusPointsAnimation (3);
//			}
//
//		}
//
//	}

//	void GivePointsEveryoneButAndDupe (int playerNum, int points) {
//
//		int dupeNum = myRoom.dupeNum;
//
//		if (playerNum == 1) {
//
//			if (dupeNum != 2) {
//				blueScore = blueScore + points;
//				blueScoreText.text = blueScore.ToString ();
//				if (points > 0) {
//					blueFlash.SetActive (true);
//					Invoke ("TurnOffBlue", 1.5f * speed);
//				} else if (points < 0) {
//					MinusPointsAnimation (2);
//				}
//			}
//			if (dupeNum != 3) {
//				greenScore = greenScore + points;
//				greenScoreText.text = greenScore.ToString ();
//				if (points > 0) {
//					greenFlash.SetActive (true);
//					Invoke ("TurnOffGreen", 1.5f * speed);
//				} else if (points < 0) {
//					MinusPointsAnimation (3);
//				}
//			}
//			if (dupeNum != 4) {
//				orangeScore = orangeScore + points;
//				orangeScoreText.text = orangeScore.ToString ();
//				if (points > 0) {
//					orangeFlash.SetActive (true);
//					Invoke ("TurnOffOrange", 1.5f * speed);
//				} else if (points < 0) {
//					MinusPointsAnimation (4);
//				}
//			}
//		} else if (playerNum == 2) {
//			if (dupeNum != 1) {
//				redScore = redScore + points;
//				redScoreText.text = redScore.ToString ();
//				if (points > 0) {
//					redFlash.SetActive (true);
//					Invoke ("TurnOffRed", 1.5f * speed);
//				} else if (points < 0) {
//					MinusPointsAnimation (1);
//				}
//			}
//			if (dupeNum != 3) {
//				greenScore = greenScore + points;
//				greenScoreText.text = greenScore.ToString ();
//				if (points > 0) {
//					greenFlash.SetActive (true);
//					Invoke ("TurnOffGreen", 1.5f * speed);
//				} else if (points < 0) {
//					MinusPointsAnimation (3);
//				}
//			}
//			if (dupeNum != 4) {
//				orangeScore = orangeScore + points;
//				orangeScoreText.text = orangeScore.ToString ();
//				if (points > 0) {
//					orangeFlash.SetActive (true);
//					Invoke ("TurnOffOrange", 1.5f * speed);
//				} else if (points < 0) {
//					MinusPointsAnimation (4);
//				}
//			}
//		} else if (playerNum == 3) {
//			if (dupeNum != 1) {
//				redScore = redScore + points;
//				redScoreText.text = redScore.ToString ();
//				if (points > 0) {
//					redFlash.SetActive (true);
//					Invoke ("TurnOffRed", 1.5f * speed);
//				} else if (points < 0) {
//					MinusPointsAnimation (1);
//				}
//			}
//			if (dupeNum != 2) {
//				blueScore = blueScore + points;
//				blueScoreText.text = blueScore.ToString ();
//				if (points > 0) {
//					blueFlash.SetActive (true);
//					Invoke ("TurnOffBlue", 1.5f * speed);
//				} else if (points < 0) {
//					MinusPointsAnimation (2);
//				}
//			}
//			if (dupeNum != 4) {
//				orangeScore = orangeScore + points;
//				orangeScoreText.text = orangeScore.ToString ();
//				if (points > 0) {
//					orangeFlash.SetActive (true);
//					Invoke ("TurnOffOrange", 1.5f * speed);
//				} else if (points < 0) {
//					MinusPointsAnimation (4);
//				}
//			}
//		} else if (playerNum == 4) {
//			if (dupeNum != 1) {
//				redScore = redScore + points;
//				redScoreText.text = redScore.ToString ();
//				if (points > 0) {
//					redFlash.SetActive (true);
//					Invoke ("TurnOffRed", 1.5f * speed);
//				} else if (points < 0) {
//					MinusPointsAnimation (1);
//				}
//			}
//			if (dupeNum != 2) {
//				blueScore = blueScore + points;
//				blueScoreText.text = blueScore.ToString ();
//				if (points > 0) {
//					blueFlash.SetActive (true);
//					Invoke ("TurnOffBlue", 1.5f * speed);
//				} else if (points < 0) {
//					MinusPointsAnimation (2);
//				}
//			}
//			if (dupeNum != 3) {
//				greenScore = greenScore + points;
//				greenScoreText.text = greenScore.ToString ();
//				if (points > 0) {
//					greenFlash.SetActive (true);
//					Invoke ("TurnOffGreen", 1.5f * speed);
//				} else if (points < 0) {
//					MinusPointsAnimation (3);
//				}
//			}
//
//		}
//
//	}

	void MinusPointsAnimation (int playerNum){
		Vector3 threesixty = new Vector3 (0f, 0f, 1080f);
		scoreObj [playerNum - 1].transform.DOLocalRotate (threesixty, 1.5f * speed,RotateMode.FastBeyond360).SetEase (Ease.OutQuad);

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


		
	void FlipSignTwoToNext () {
		Vector3 oneEighty = new Vector3 (0, 180, 0);
		signTwo.transform.DORotate (oneEighty, 1.0f * speed);
	}

	void FlipSignTwoToWait () {
		signTwo.transform.DORotate (Vector3.zero, 1.0f * speed);
	}

	void FlipSignOneToPlay () {
		Vector3 oneEighty = new Vector3 (0, 180, 0);
		signOne.transform.DORotate (oneEighty, 1.0f * speed);
	}

	void FlipSignOneToFastForward () {
		signOne.transform.DORotate (Vector3.zero, 1.0f * speed);
	}
		
	public void FastForward () {
	
		fastForward = true;
		speed = .2f;
		FlipSignOneToPlay ();
		if (signTwo.transform.rotation.y > .9f) {
			NextButton ();
		} else {
			FlipSignTwoToWait ();
		}

	}

	public void PlayButton (){

		if (stepNum == 6) {
			EndTheRound ();
			return;
		}

		fastForward = false;
		speed = 1.0f;
		FlipSignOneToFastForward();

	}

	public void NextButton (){
		
		if (stepNum == 2) {
			StartSecondAward ();
			FlipSignTwoToWait ();
		} else if (stepNum == 3) {
			StartThirdAward ();
			FlipSignTwoToWait ();
		} else if (stepNum == 4) {
			StartNonDupeGuessReveal ();
			FlipSignTwoToWait ();
		} else if (stepNum == 5) {
			MoveOutGuesses ();
			FlipSignTwoToWait ();
		} else if (stepNum == 6) {
			EndTheRound ();
		}

	}

	void CheckIfStillSlow (){

		if (fastForward == false) {
			FlipSignTwoToNext ();
		} else {
			NextButton ();
		}

	}

}
