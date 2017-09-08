using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using DG.Tweening;

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

	string intro1Words1 = "What a brilliant creature! It's a masterpiece!";
	string intro1Words2 = "But one player has received the wrong subject";
	string intro1Words3 = "Who messed up my painting? Find the dupe!";
	string intro1Words4;
	string intro1Words5 = "If a majority voted for the dupe, the true artists will score 2 points";
	string intro1Words6 = "We caught the dupe! 2 points for all true artists";
	string intro1Words7 = "Let's see if the dupe can guess the right subject";
	string intro1Words8 = "The dupe guessed right! No points for you!";
	string intro1Words9 = "Next time, don't draw so obviously";

	string intro2Words1 = "You missed the dupe!";
	string intro2Words2 = "Could anyone else find the dupe in this ambiguous being?";
	string intro2Words3 = "Nope! Nobody figured it out. It's one big tie";
	string intro2Words4 = "The dupe scores big and you get nothing";
	string intro2Words5 = "To win Exquisite Dupe you must either leave subtle clues for your fellow artists...";
	string intro2Words6 = "...or misdirect the dupe with red herrings.";
	string intro2Words7 = "That is all young artist. Go and paint me a masterpiece!";

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

	// Use this for initialization
	void Start () {

		if (drawing1 == true) {
			dupeText.text = intro1Words1;
			InvokeRepeating ("BubbleOneShake", 5.0f, 3.0f);
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

		if (Input.GetKeyDown (KeyCode.C)) {
			DupeLooking ();
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

	public void DupeBubble () {

		if (dupeText.text == intro1Words1) {
			dupeText.text = intro1Words2;
			CancelInvoke ();
			InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
		} else if (dupeText.text == intro1Words2) {
			dupeText.text = intro1Words3;
			CancelInvoke ();
			InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
		} else if (dupeText.text == intro1Words3) {
			CancelInvoke ();
			tutorialDupe.SetActive (false);
			myVote.WiggleStart ();
			SendUpVote ();
		} else if (dupeText.text == intro1Words4) {
			CancelInvoke ();
			dupeText.text = intro1Words5;
			Invoke ("RevealVotes", 4.0f);
		} else if (dupeText.text == intro1Words6) {
			dupeText.text = intro1Words7;

			foreach (GameObject voteThing in votes) {
				voteThing.SetActive (false);
			}

			GameObject vote = GameObject.FindGameObjectWithTag ("Vote");
			Destroy (vote);
			CancelInvoke ();
			Invoke ("TurnOffTutorialDupe", 4.0f);

		} else if (dupeText.text == intro1Words8) {
			dupeText.text = intro1Words9;
			CancelInvoke ();
			InvokeRepeating ("BubbleOneShake", 4.0f, 3.0f);
		} else if (dupeText.text == intro1Words9) {

			roomMan.CurtainsIn ();
			Invoke ("EndScene", 4.0f);

		}

	}

	public void DupeBubble2 () {

		if (dupeText.text == intro2Words1) {
			CancelInvoke ();
			dupeText.text = intro2Words2;
			Invoke ("RevealVotes2", 3.5f);
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

	void EndScene (){
	
		SceneManager.LoadScene ("Tutorial Based Room 2");
	
	}

	void CloseCurtains() {
		roomMan.CurtainsIn ();
		roomMan.cameFromTutorial = true;
		Invoke ("EndTutorial", 2.5f);
	}

	void EndTutorial (){
		roomMan.cameFromTutorial = true;
		SceneManager.LoadScene ("Lobby Menu");

	}

	void SendUpVote(){
	
		LocalTurnVoting localMan = GameObject.FindGameObjectWithTag ("Local Room").GetComponent<LocalTurnVoting> ();
		localMan.MoveUpSign ();
		localMan.MoveUpPedestal ();

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
			Invoke ("AfterVote", 2.5f);
		} else {
			Invoke ("AfterVote2", 2.5f);
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
		InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
	
	}

	void TurnOffTutorialDupe (){
	
		tutorialDupe.SetActive (false);
		Invoke ("StartDupeGuess", 2.0f);
	}

	void StartDupeGuess (){
	
		LocalTurnVoting localMan = GameObject.FindGameObjectWithTag ("Local Room").GetComponent<LocalTurnVoting> ();
		localMan.LaunchDupeGuess ();
		sliderVal.value = 1;
		Invoke ("DupeLooking", 3.5f);

	}

	void DupeLooking (){
		DOTween.To(()=> sliderVal.value, x=> sliderVal.value = x, 0, 2.0f).OnComplete(BackUp);
	}

	void BackUp (){
		DOTween.To(()=> sliderVal.value, x=> sliderVal.value = x, .95f, 1.2f).OnComplete(ToElephant);
	}

	void ToElephant(){
		DOTween.To(()=> sliderVal.value, x=> sliderVal.value = x, .44f, .8f).OnComplete(SelectElephant);
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

		Invoke ("RevealPoints2", 2.0f);

	}

	void RevealPoints2(){

		dupeText.text = intro2Words3;
		CancelInvoke ();
		InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);

	}

}
