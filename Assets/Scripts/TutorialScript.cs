using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TutorialScript : MonoBehaviour {

	public Text introText;
	public Text intro2Text;
	public LobbyMenu lobbyMenu;
	public RoomManager roomMan;
	bool readyStartFourLegged;
	bool readyJoin;
	string introWords1 = "Welcome to Exquisite Dupe!";
	string introWords2 = "In this contest of wits you are competing against players across the globe to become a true artist";
	string introWords3 = "Right now you have no paintings in progress. To start, click \"join\" below";

	string intro2Words1 = "Each category is a collection of subjects which have a shape or theme in common";
	string intro2Words2 = "Click on " + "\"" + "Four Legged Animals" + "\"" + " to be assigned a subject and begin painting!";

	string intro3Words1 = "Nice work! Now we just wait for the other players to finish";
	string intro3Words2 = "That was fast. Click on \"vote\" to continue";

	public RectTransform bubbleOne;
	public RectTransform bubbleTwo;
	public RectTransform joinButton;
	public RectTransform fourLegged;

	public GameObject bubble;
	public TurnGameStatus status;
	bool readyVote;
	bool bubbleDone;

	public bool menu1;
	public bool menu2;

	// Use this for initialization
	void Start () {

		if (menu1 == true) {
			introText.text = introWords1;
			intro2Text.text = intro2Words1;
			Invoke ("OpenCurtains", 1.0f);
			InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
		}

		roomMan = GameObject.FindGameObjectWithTag ("Room Manager").GetComponent<RoomManager>();

		if (menu2 == true) {
			Invoke ("OpenCurtains", 1.0f);
			introText.text = intro3Words1;
			Invoke("OffBubble", 5.0f);
		}

	}
	
	// Update is called once per frame
	void Update () {

//		if (Input.GetKeyDown (KeyCode.W)) {
//		
//			bubbleOne.DOShakeRotation (1.0f, 20, 10);
//		
//		}

//		if (Input.GetKeyDown (KeyCode.E)) {
//
//			bubbleOne.DOShakeScale(1.0f, .2f, 10);
//
//		}


	}

	void BubbleOneShake (){
		
		bubbleOne.DOShakeScale(1.0f, .2f, 10);
	
	}

	void BubbleTwoShake (){

		bubbleTwo.DOShakeScale(1.0f, .2f, 10);

	}

	void JoinButtonShake (){

		joinButton.DOShakeScale(1.0f, .2f, 10);

	}

	void FourLeggedShake (){

		fourLegged.DOShakeScale(1.0f, .2f, 10);

	}

	public void IntroBubble () {
	
		if (introText.text == introWords1) {
			introText.text = introWords2;
			CancelInvoke ();
			InvokeRepeating ("BubbleOneShake", 4.0f, 3.0f);
		} else if (introText.text == introWords2) {
			CancelInvoke ();
			InvokeRepeating ("JoinButtonShake", 1.0f, 1.8f);
			introText.text = introWords3;
			readyJoin = true;
		}
	
	}

	public void Intro2Bubble () {

		if (intro2Text.text == intro2Words1) {
			CancelInvoke ();
			InvokeRepeating ("FourLeggedShake", .5f, 1.8f);
			intro2Text.text = intro2Words2;
			readyStartFourLegged = true;
		} 

	}

	public void JoinAPainting (){
		if (readyJoin == true) {
			lobbyMenu.NewTurnBased ();
			CancelInvoke ();
			InvokeRepeating ("BubbleTwoShake", 5.0f, 3.0f);
		}
	}

	public void StartFourLegged (){
		Debug.Log ("hitasdf");
		if (readyStartFourLegged == true) {
			CancelInvoke ();
			roomMan.CurtainsIn ();

			Invoke ("StartNextScene", 1.0f);
		}
	}

	void StartNextScene(){
	
		SceneManager.LoadScene ("Tutorial Based Room");

	}

	void OpenCurtains() {
	
		roomMan.CurtainsOut ();
	
	}

	public void OffBubble(){
		if (bubbleDone == false) {
			bubbleDone = true;
			bubble.SetActive (false);
			Invoke ("OnBubble", 3.2f);
			Invoke ("OnVote", 2.0f);
		}
	
	}

	void OnVote(){
	
		status.PhaseTwoReady ();

	}

	void OnBubble(){
		introText.text = intro3Words2;
		bubble.SetActive (true);
		readyVote = true;
		InvokeRepeating ("JoinButtonShake", .5f, 4.0f);
	
	}

	public void StartVoteTutorial (){
	
		Debug.Log ("button");

		if (readyVote == true) {
		
			roomMan.CurtainsIn ();
			Debug.Log ("start next phase");
			Invoke ("StartVoteTutorial1", 1.5f);

		}

	}

	void StartVoteTutorial1 (){
	
		SceneManager.LoadScene ("Tutorial Turn Based Voting");
	
	}
}
