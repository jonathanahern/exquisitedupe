using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TutorialVoteScript : MonoBehaviour {

	string drawing1Location = "drawing1Location";
	string elephantHead;

	public GameObject redLine;
	public GameObject blueLine;
	public GameObject greenLine;
	public GameObject orangeLine;

	public GameObject redDot;
	public GameObject blueDot;
	public GameObject greenDot;
	public GameObject orangeDot;

	private static string MYCOLOR_SYM = "[MYCOLOR]";

	public Text dupeText;
	public GameObject tutorialDupe;

	RoomManager roomMan;

	//string intro1Words1 = "What a brilliant creature! It's a masterpiece!";
	string intro1Words2 = "It's brilliant! But one player has received the wrong subject";
	string intro1Words3 = "Who messed up my painting? Find the dupe!";
	string intro1Words4;
	string intro1Words5 = "If a majority voted for the dupe, the true artists will score 2 points";
	string intro1Words6 = "We caught the dupe! 2 pts for all true artists";
	string intro1Words7 = "Let's see if the dupe can guess the right subject";
	string intro1Words8 = "The dupe guessed right! No points for you!";
	string intro1Words9 = "Next time, don't draw so obviously";

	string intro2Words1 = " You missed the dupe!";
	string intro2Words2 = "Can anyone find the dupe in this amorphous being?";
	string intro2Words3 = "Nope! It's one big tie";
	string intro2Words4 = "The dupe scores big and you get nothing!";
	string intro2Words5 = "You must leave clues for your fellow artists...";
	string intro2Words6 = "...or misdirect the dupe with red herrings.";
	string intro2Words7 = "That is all young artist. Go paint me a masterpiece!";

	int artistNum;

	public GameObject[] votes;
	public GameObject[] dupeVotes;

	public GameObject[] drawing2Votes;

	public Slider sliderVal;
	public RectTransform elephantButt;

	public bool drawing1;
	public bool drawing2;

	public RectTransform bubbleOne;
	public VoteFabScript myVote;

	public GameObject confettiBlast;
	public GameObject starBlast;

	public GameObject finger;
	Vector3 offScreenPos;
	Vector3 onScreenPos;
	Vector3 onFrameSPos;
	bool fingerMovement = false;

	void Awake (){
		roomMan = GameObject.FindGameObjectWithTag ("Room Manager").GetComponent<RoomManager> ();
	}

	// Use this for initialization
	void Start () {

		if (drawing1 == true) {
			dupeText.text = intro1Words2;
			InvokeRepeating ("BubbleOneShake", 5.0f, 3.0f);
			offScreenPos = finger.transform.position;
			onScreenPos = new Vector3(-3.3f,-5.7f,-.6f);
			onFrameSPos = new Vector3(-2.25f,-3.5f,-.6f);

		}

		roomMan = GameObject.FindGameObjectWithTag ("Room Manager").GetComponent<RoomManager> ();

		CreateDrawing ();

		if (drawing2 == true) {
			Invoke ("MoveUpPed", 2.0f);
			myVote.WiggleStart ();
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (fingerMovement == true && drawing1 == true) {
			
			if (myVote.gameObject.transform.parent == null) {
				fingerMovement = false;
				DOTween.Kill ("finger");
				MoveFingerOff ();
			}
				
		}


		if (Input.GetMouseButtonDown (0) && roomMan.curtainMoving == false) {
		
			if (roomMan == null) {
				return;
			}
				
			if (dupeText.text == intro1Words2) {
				dupeText.text = intro1Words3;
				CancelInvoke ();
				InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
			} else if (dupeText.text == intro1Words3) {
				CancelInvoke ();
				tutorialDupe.SetActive (false);
				myVote.WiggleStart ();
				dupeText.text = "";
				SendUpVote ();
			} else if (dupeText.text == intro1Words4) {
				CancelInvoke ();
				dupeText.text = intro1Words5;
				Invoke ("RevealVotes", 3.0f);
			} else if (dupeText.text == intro1Words6) {
				dupeText.text = intro1Words7;

				foreach (GameObject voteThing in votes) {
					voteThing.SetActive (false);
				}

				GameObject vote = GameObject.FindGameObjectWithTag ("Vote");
				Destroy (vote);
				CancelInvoke ();
				Invoke ("TurnOffTutorialDupe", 3.0f);

			} else if (dupeText.text == intro1Words8) {
				dupeText.text = intro1Words9;
				CancelInvoke ();
				InvokeRepeating ("BubbleOneShake", 4.0f, 3.0f);
			} else if (dupeText.text == intro1Words9) {
				dupeText.text = "";
				roomMan.CurtainsIn ();
				Invoke ("EndScene", 4.0f);

			} else if (dupeText.text == intro2Words1) {
				CancelInvoke ();
				dupeText.text = intro2Words2;
				Invoke ("RevealVotes2", 2.0f);
			} else if (dupeText.text == intro2Words3) {
				CancelInvoke ();
				InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
				dupeText.text = intro2Words4;
			} else if (dupeText.text == intro2Words4) {
				CancelInvoke ();
				InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
				dupeText.text = intro2Words5;
			} else if (dupeText.text == intro2Words5) {
				CancelInvoke ();
				InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
				dupeText.text = intro2Words6;
			} else if (dupeText.text == intro2Words6) {
				CancelInvoke ();
				dupeText.text = intro2Words7;
				Invoke ("CloseCurtains", 3.0f);
				roomMan.cameFromTurnBased = true;
				roomMan.tutorialMode = false;
			} 
		
		}

	}

	void BubbleOneShake (){

		bubbleOne.DOShakeScale(1.0f, .2f, 10);

	}

	void MoveUpPed(){

		LocalTurnVoting localMan = GameObject.FindGameObjectWithTag ("Local Room").GetComponent<LocalTurnVoting> ();
		localMan.MoveUpPedestal ();
		localMan.MoveUpSign ();
	}

	void CreateDrawing (){

		elephantHead = PlayerPrefs.GetString (drawing1Location);

		elephantHead = elephantHead.TrimEnd ('$');
		string[] drawingInfos = elephantHead.Split ('$');

		Debug.Log ("drawing: " + elephantHead);

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

		if (roomMan != null) {
			roomMan.CurtainsOut ();
		}
	}

//	public void DupeBubble () {
//
//		if (dupeText.text == intro1Words2) {
//			dupeText.text = intro1Words3;
//			CancelInvoke ();
//			InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
//		} else if (dupeText.text == intro1Words3) {
//			CancelInvoke ();
//			tutorialDupe.SetActive (false);
//			myVote.WiggleStart ();
//			SendUpVote ();
//		} else if (dupeText.text == intro1Words4) {
//			CancelInvoke ();
//			dupeText.text = intro1Words5;
//			Invoke ("RevealVotes", 3.0f);
//		} else if (dupeText.text == intro1Words6) {
//			dupeText.text = intro1Words7;
//
//			foreach (GameObject voteThing in votes) {
//				voteThing.SetActive (false);
//			}
//
//			GameObject vote = GameObject.FindGameObjectWithTag ("Vote");
//			Destroy (vote);
//			CancelInvoke ();
//			Invoke ("TurnOffTutorialDupe", 3.0f);
//
//		} else if (dupeText.text == intro1Words8) {
//			dupeText.text = intro1Words9;
//			CancelInvoke ();
//			InvokeRepeating ("BubbleOneShake", 4.0f, 3.0f);
//		} else if (dupeText.text == intro1Words9) {
//
//			roomMan.CurtainsIn ();
//			Invoke ("EndScene", 4.0f);
//
//		}
//
//	}

//	public void DupeBubble2 () {
//
//		 if (dupeText.text == intro2Words1) {
//			CancelInvoke ();
//			dupeText.text = intro2Words2;
//			Invoke ("RevealVotes2", 2.0f);
//		} else if (dupeText.text == intro2Words3) {
//			CancelInvoke ();
//			InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
//			dupeText.text = intro2Words4;
//		} else if (dupeText.text == intro2Words4) {
//			CancelInvoke ();
//			InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
//			dupeText.text = intro2Words5;
//		} else if (dupeText.text == intro2Words5) {
//			CancelInvoke ();
//			InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
//			dupeText.text = intro2Words6;
//		} else if (dupeText.text == intro2Words6) {
//			CancelInvoke ();
//			dupeText.text = intro2Words7;
//			Invoke ("CloseCurtains", 3.0f);
//			roomMan.cameFromTurnBased = true;
//			roomMan.tutorialMode = false;
//		} 
//
//	}

	void EndScene (){
	
		SceneManager.LoadScene ("Tutorial Based Room 2");
	
	}

	void CloseCurtains() {
		roomMan.CurtainsIn ();
		roomMan.cameFromTutorial = true;
		Invoke ("EndTutorial", 2.5f);
	}

	void EndTutorial (){
		SceneManager.LoadScene ("Lobby Menu");
	}

	void SendUpVote(){
	
		LocalTurnVoting localMan = GameObject.FindGameObjectWithTag ("Local Room").GetComponent<LocalTurnVoting> ();
		localMan.MoveUpSign ();
		localMan.MoveUpPedestal ();
		if (drawing1 == true) {
			Invoke ("MoveFingerIn", 1.5f);
			Debug.Log ("Send UP Vote");
		}

	}

	public void SubmitVote(){

		GameObject vote = GameObject.FindGameObjectWithTag ("Vote");
		vote.GetComponent<VoteFabScript> ().stuck = true;
		Vector3 pos = vote.transform.position;

		LocalTurnVoting localMan = GameObject.FindGameObjectWithTag ("Local Room").GetComponent<LocalTurnVoting> ();
		localMan.MoveDownSign ();
		localMan.MoveDownPedestal ();

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




		if (drawing1 == true) {
			Invoke ("AfterVote", 1.0f);
			if (artistNum == 3) {
				confettiBlast.SetActive (true);
			}
		} else {
			Invoke ("AfterVote2", 1.0f);
		}
	}

	void AfterVote (){

		//Debug.Log (artistNum);

		if (artistNum != 3) {
			intro1Words4 = "You missed the dupe!";
		} else {
			intro1Words4 = "You found the dupe!\n+1 point";
		}
			
		tutorialDupe.SetActive (true);
		dupeText.text = intro1Words4;
		CancelInvoke ();
		InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);

	}


	void AfterVote2 (){
		InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
		dupeText.text = intro2Words1;
		tutorialDupe.SetActive (true);

	}

	void RevealVotes () {

		if (artistNum != 2) {
			votes[2] = dupeVotes [0];
		} else {
			votes[2] = dupeVotes [1];
		}
	
		foreach (GameObject voteThing in votes) {

			voteThing.SetActive (true);

		}

		Invoke ("RevealPoints", 2.5f);
	
	}

	void RevealPoints(){
	
		dupeText.text = intro1Words6;
		starBlast.SetActive (true);
		InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
	
	}

	void TurnOffTutorialDupe (){
	
		tutorialDupe.SetActive (false);
		Invoke ("StartDupeGuess", .5f);
	}

	void StartDupeGuess (){
	
		LocalTurnVoting localMan = GameObject.FindGameObjectWithTag ("Local Room").GetComponent<LocalTurnVoting> ();
		localMan.LaunchDupeGuess ();
		sliderVal.value = 1;
		Invoke ("DupeLooking", 3.5f);

	}

	void DupeLooking (){
		DOTween.To(()=> sliderVal.value, x=> sliderVal.value = x, 0, 1.5f).OnComplete(BackUp);
	}

	void BackUp (){
		DOTween.To(()=> sliderVal.value, x=> sliderVal.value = x, .95f, .8f).OnComplete(ToElephant);
	}

	void ToElephant(){
		DOTween.To(()=> sliderVal.value, x=> sliderVal.value = x, .44f, .5f).OnComplete(SelectElephant);
	}

	void SelectElephant(){
		elephantButt.DOShakeAnchorPos (.75f,20,10);
		Invoke ("BackToMainView", 2.0f);
	}

	void BackToMainView (){
		
		LocalTurnVoting localMan = GameObject.FindGameObjectWithTag ("Local Room").GetComponent<LocalTurnVoting> ();
		localMan.MoveOutGuesser ();
		Invoke ("ClosingScreen", 2.0f);
	}

	void ClosingScreen(){
		CancelInvoke ();
		InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
		tutorialDupe.SetActive (true);
		dupeText.text = intro1Words8;

	}

	void RevealVotes2 () {

		drawing2Votes [artistNum - 1].SetActive (true);

		Invoke ("RevealPoints2", 1.0f);

	}

	void RevealPoints2(){

		dupeText.text = intro2Words3;
		CancelInvoke ();
		InvokeRepeating ("BubbleOneShake", 2.0f, 3.0f);

	}

	void MoveFingerIn(){
		fingerMovement = true;
		finger.transform.DOMove (onScreenPos, .75f).SetId ("finger").OnComplete(MoveDown);
	}

	void MoveDown(){
		Vector3 smaller = new Vector3 (.16f, .17f, 1);
		finger.transform.DOScale(smaller,.3f).SetEase(Ease.OutBounce).SetId ("finger").OnComplete(MoveFingerToFrame);
	}

	void MoveFingerToFrame(){
		finger.transform.DOMove (onFrameSPos, .75f).SetId ("finger").OnComplete(MoveUp);
	}

	void MoveUp(){
		Vector3 bigger = new Vector3 (.19f, .2f, 1);
		finger.transform.DOScale(bigger,.3f).SetEase(Ease.OutBounce).SetId ("finger").OnComplete(MoveFingerToLeft);
	}

	void MoveFingerToLeft(){
		fingerMovement = false;
		finger.transform.DOLocalMoveX (-7.3f, .75f).SetId ("finger").OnComplete(MoveFingerOff);
	}

	void MoveFingerOff(){
		fingerMovement = false;
		finger.transform.DOMove (offScreenPos, .75f).SetId ("finger");
	}



}
