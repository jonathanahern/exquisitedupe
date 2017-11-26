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
	string introWords1 = "Welcome to the Exquisite Dupe tutorial!";
	//string introWords2 = "In this contest of wits you are competing against players across the globe to become a true artist";
	string introWords3 = "You have no paintings in progress";

	//string intro2Words1 = "Each category is a collection of subjects which have a shape or theme in common";
	string intro2Words2 = "Each category is a collection of subjects with a shape or theme in common";

	string intro3Words1 = "Nice work! Now wait for the other players to finish";
	string intro3Words2 = "That was fast. Press \"vote\" to continue";

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

	public RectTransform fingerOne;
	Vector2 offscreenPosF1;
	Vector2 screenPosF1;

	public RectTransform fingerTwo;
	Vector2 offscreenPosF2;
	Vector2 screenPosF2;

	public RectTransform fingerThree;
	Vector2 offscreenPosF3;
	Vector2 screenPosF3;

	public RectTransform fingerFour;
	Vector2 offscreenPosF4;
	Vector2 screenPosF4;

	// Use this for initialization
	void Start () {

		if (menu1 == true) {
			screenPosF1 = fingerOne.anchoredPosition;
			offscreenPosF1 = new Vector2 (screenPosF1.x + 600, screenPosF1.y - 400);
			fingerOne.anchoredPosition = offscreenPosF1;

			screenPosF2 = fingerTwo.anchoredPosition;
			offscreenPosF2 = new Vector2 (screenPosF2.x, screenPosF2.y - 400);
			fingerTwo.anchoredPosition = offscreenPosF2;

			screenPosF3 = fingerThree.anchoredPosition;
			offscreenPosF3 = new Vector2 (screenPosF3.x - 600, screenPosF3.y - 150);
			fingerThree.anchoredPosition = offscreenPosF3;

			introText.text = introWords1;
			intro2Text.text = intro2Words2;

			Invoke ("OpenCurtains", 1.0f);
			Invoke ("MoveFingerOneIn", 4.0f);
			InvokeRepeating ("BubbleOneShake", 3.0f, 3.0f);
		}

		roomMan = GameObject.FindGameObjectWithTag ("Room Manager").GetComponent<RoomManager>();

		if (menu2 == true) {
			Invoke ("OpenCurtains", 1.0f);
			introText.text = intro3Words1;
			Invoke("OffBubble", 5.0f);

			screenPosF4 = fingerFour.anchoredPosition;
			offscreenPosF4 = new Vector2 (screenPosF4.x + 575, screenPosF4.y - 225);
			fingerFour.anchoredPosition = offscreenPosF4;
		}

	}

	void Awake (){
		roomMan = GameObject.FindGameObjectWithTag ("Room Manager").GetComponent<RoomManager> ();
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

		if (Input.GetMouseButtonDown(0) && roomMan.curtainMoving == false)
		{

			if (roomMan == null) {
				return;
			}

			if (introText.text == introWords1) {
				CancelInvoke ();
				DOTween.Kill ("finger1");
				MoveFingerOneOut ();
				InvokeRepeating ("JoinButtonShake", 1.0f, 1.8f);
				Invoke ("MoveFingerTwoIn", 2.0f);
				introText.text = introWords3;
				readyJoin = true;
			}

		}


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
	
		Debug.Log ("CLICKEDEDED");

		if (introText.text == introWords1) {
			CancelInvoke ();
			InvokeRepeating ("JoinButtonShake", 1.0f, 1.8f);
			introText.text = introWords3;
			readyJoin = true;
		}
	
	}

//	public void Intro2Bubble () {
//
//		if (intro2Text.text == intro2Words1) {
//			CancelInvoke ();
//			InvokeRepeating ("FourLeggedShake", .5f, 1.8f);
//			intro2Text.text = intro2Words2;
//			readyStartFourLegged = true;
//		} 
//
//	}

	public void JoinAPainting (){
		if (readyJoin == true) {
			DOTween.Kill ("finger2");
			MoveFingerTwoOut ();
			lobbyMenu.NewTurnBased ();
			CancelInvoke ();
			InvokeRepeating ("FourLeggedShake", 1.5f, 1.8f);
			intro2Text.text = intro2Words2;
			readyStartFourLegged = true;
			Invoke ("MoveFingerThreeIn", 3.0f);


		}
	}

	public void StartFourLegged (){
		Debug.Log ("hitasdf");
		if (readyStartFourLegged == true) {
			CancelInvoke ();
			roomMan.CurtainsIn ();
			readyStartFourLegged = false;
			DOTween.Kill ("finger3");
			MoveFingerThreeOut ();

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
			Invoke ("MoveFingerFourIn", 4.75f);
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
			readyVote = false;
			Debug.Log ("start next phase");
			Invoke ("StartVoteTutorial1", 1.5f);

		}

	}

	void StartVoteTutorial1 (){
	
		SceneManager.LoadScene ("Tutorial Turn Based Voting");
	
	}

	void MoveFingerOneIn(){
	
		fingerOne.DOAnchorPos (screenPosF1, .6f).SetId ("finger1").OnComplete(PressDownF1);
		Invoke ("MoveFingerOneOut", 2.0f);
	
	}

	void PressDownF1(){
		Vector3 smaller = new Vector3 (-.15f, -.15f, -.15f);
		fingerOne.DOPunchScale (smaller, .5f, 1, .01f);
	}

	void MoveFingerOneOut(){

		fingerOne.DOAnchorPos (offscreenPosF1, .6f);

	}

	void MoveFingerTwoIn(){

		fingerTwo.DOAnchorPos (screenPosF2, .6f).SetId ("finger2").OnComplete(PressDownF2);
		Invoke ("MoveFingerTwoOut", 2.0f);

	}

	void PressDownF2(){
		Vector3 smaller = new Vector3 (-.15f, -.15f, -.15f);
		fingerTwo.DOPunchScale (smaller, .5f, 1, .01f);
	}

	void MoveFingerTwoOut(){

		fingerTwo.DOAnchorPos (offscreenPosF2, .6f);

	}

	void MoveFingerThreeIn(){

		fingerThree.DOAnchorPos (screenPosF3, .6f).SetId ("finger3").OnComplete(PressDownF3);
		Invoke ("MoveFingerThreeOut", 2.0f);

	}

	void PressDownF3(){
		Vector3 smaller = new Vector3 (-.15f, -.15f, -.15f);
		fingerThree.DOPunchScale (smaller, .5f, 1, .01f);
	}

	void MoveFingerThreeOut(){

		fingerThree.DOAnchorPos (offscreenPosF3, .6f);

	}

	void MoveFingerFourIn(){

		fingerFour.DOAnchorPos (screenPosF4, .6f).SetId ("finger4").OnComplete(PressDownF4);
		Invoke ("MoveFingerFourOut", 1.8f);

	}

	void PressDownF4(){
		Vector3 smaller = new Vector3 (-.15f, -.15f, -.15f);
		fingerFour.DOPunchScale (smaller, .5f, 1, .01f);
	}

	void MoveFingerFourOut(){

		fingerFour.DOAnchorPos (offscreenPosF4, .6f);

	}

}
