﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using DatabaseControl;
using UnityEngine.SceneManagement;

public class LobbyMenu : MonoBehaviour {

	public static LobbyMenu instance;
	bool okToClick = true;

	public GameObject mainButtons;
	public RectTransform loadScreen;

	public RectTransform loginName;
	public RectTransform centerMainButts;
	public RectTransform newCats;
	private float startPos;
	public RectTransform centerTurnButts;
	public RectTransform highScores;

	Vector3 zeroCounter;
	Vector3 oneEighty;

	public GameObject frameMessage;
	Vector2 offScreen;

	public Transform statusHolder;
	GameObject statusLoad;

	RoomManager roomMan;
	public GameObject frameText;
	public GameObject tutorialWords;
	public GameObject fiveRooms;

	public GameObject turnButtonObj;
	public Transform turnHolder;
	Transform catHolder;
	public GameObject loadingText;

	private static string ROOMTYPE_SYM = "[ROOMTYPE]";
	private static string WORDS_SYM = "[WORDS]";
	private static string BRUSHES_SYM = "[BRUSHES]";
	private static string GROUNDING_SYM = "[GROUNDING]";

	public GameObject refreshingScreen;
	public Text loadScreenWords;

	public GameObject loadingAnimationCenter;
	public bool tutorialMode = false;
	int roomCount;
	public AnimationCurve moveItOut;
	public AnimationCurve moveItIn;

	// Use this for initialization
	void Awake () {

		instance = this;

	}

	void Start(){

		statusLoad = GameObject.FindGameObjectWithTag ("Status Load");
		oneEighty = new Vector3 (0, 0, 180.0f);
		zeroCounter = new Vector3 (0, 0, 360.0f);
		startPos = newCats.position.x;
		roomMan = GameObject.FindGameObjectWithTag ("Room Manager").GetComponent<RoomManager> ();

//		if (roomMan.cameFromTutorial == true) {
//			GetAllCategories ();
//			roomMan.cameFromTutorial = false;
//		}

		if (tutorialMode == true) {
			return;
		}

		//roomMan.roomsReady = false;
		if (roomMan.cameFromTurnBased == true) {
			Debug.Log ("came From turn based");
			TurnBasedClicked ();
			Invoke ("CurtainsOpen", 1.0f);
			roomMan.FindEmptyRooms ();
			roomMan.startingNew = false;
			roomMan.UpdateStatus ();
			//roomMan.DropOffButtons ();
		}



		UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();

		if (userAccount.firstLogin == true) {
			Invoke ("AskForTutorial", 1.0f);
			userAccount.firstLogin = false;
		}

		string roomsString = userAccount.activeRooms;
		string[] roomArray = roomsString.Split ('/');
		roomCount = roomArray.Length;
		if (userAccount.activeRooms == "") {
			roomCount = 0;
		}
		//Debug.Log (roomCount);

		if (roomMan.cameFromScoring == false) {
			//highScores.gameObject.GetComponent<HighScoreScript> ().UpdateTheScore ();
		} else {
			GoToHighScores ();
			roomMan.SendTheScoreToServer ();
			roomMan.cameFromScoring = false;

		}
			
		if (roomCount < 1) {
			roomMan.noRooms = true;
			if (frameText == null) {
				frameText = GameObject.FindGameObjectWithTag ("Frame Text");
			}
			frameText.GetComponent<Text> ().text = "YOU ART NOT APART\nOF ANY PAINTINGS";
			loadingAnimationCenter.SetActive (false);
		} else {
			//Debug.Log (roomCount);
			for (int i = 0; i < roomCount; i++) {
				//Debug.Log (roomArray [i]);
				roomMan.UpdateTurnRoomsFromLogin (int.Parse (roomArray [i]));
			}

		}

		InvokeRepeating ("AutoUpdateRooms", 1.0f, 10.0f);

	}
	
	// Update is called once per frame
	void Update () {

//		if (Input.GetKeyDown (KeyCode.C)) {
//		
//			GameObject buttonObj = Instantiate (turnButtonObj,offScreen,Quaternion.identity);
//			buttonObj.transform.SetParent (roomMan.buttonHolder, false);
//			GameObject buttonObj1 = Instantiate (turnButtonObj,offScreen,Quaternion.identity);
//			buttonObj1.transform.SetParent (roomMan.buttonHolder, false);
//			GameObject buttonObj2 = Instantiate (turnButtonObj,offScreen,Quaternion.identity);
//			buttonObj2.transform.SetParent (roomMan.buttonHolder, false);
//		
//		}
		
	}

	void CurtainsOpen (){
	

		roomMan.GetComponent<RoomManager> ().CurtainsOut();
	
	}

	void AskForTutorial(){

		tutorialWords.SetActive (true);
		fiveRooms.SetActive (false);
		offScreen = frameMessage.transform.position;
		frameMessage.transform.DOLocalMoveY(0,1.0f).SetEase (Ease.OutBounce);

	}

	public void SendUpTutorial(){
	
		frameMessage.transform.DOLocalMoveY(offScreen.y,1.0f);

	}

	void AutoUpdateRooms (){
		if (roomMan.noRooms == true) {
			return;
		}
		//Debug.Log ("TURN ON");
		refreshingScreen.SetActive (true);
		//loadScreenWords.text = "refreshing...";
		roomMan.UpdateStatus ();

	}

	public void TurnBasedClicked(){
		
		centerMainButts.DOLocalRotate (oneEighty, 1.0f).SetEase(Ease.OutQuad);
		centerTurnButts.DOLocalRotate (zeroCounter, 1.0f).SetEase(Ease.OutQuad);

		//DOTween.To(()=> center.GetComponent<RectTransform>().rotation, x=> center.GetComponent<RectTransform>().rotation = x, 180, 1.0f);

		//mainButtons.SetActive (false);
		//turnBasedButtons.SetActive (true);
	
	}

	void OnApplicationFocus(bool isFocused){

		if (tutorialMode == true) {
			return;
		}

		if (isFocused) {
			Debug.Log ("asdfasdf");
			CancelInvoke ();
			Invoke ("OkToClickAgain", 1.0f);

			if (roomMan.refreshing == true) {
				InvokeRepeating ("AutoUpdateRooms", 10.0f, 10.0f);
				//Invoke ("SendLate", 5.0f);
			} else {
				InvokeRepeating ("AutoUpdateRooms", 2.0f, 10.0f);
			}
		}

	}

//	void SendLate (){
//	
//		roomMan.ComingIntoFocus ();
//	
//	}

	public void NewTurnBased() {

		if (okToClick == false) {
			return;
		}

		okToClick = false;

		Invoke ("OkToClickAgain", 2.5f);
		if (roomCount > 4) {
			FiveRoomsOnly ();
			return;
		}
	
		newCats.DOLocalMoveX (0, 1.5f).SetEase(moveItIn);
		centerTurnButts.DOLocalMoveX (startPos * -1.0f, 1.5f).SetEase(moveItOut);

	}

	public void NewCatsOffScreen(){
	
		newCats.DOLocalMoveX (startPos, 1.5f).SetEase(moveItOut);
		centerTurnButts.DOLocalMoveX (0, 1.5f).SetEase(moveItIn);
	
	}

	public void LoadingScreenFromNewCats (){
	
		loginName.DOLocalMoveX (startPos, 2.0f);
		newCats.DOLocalMoveX (startPos, 2.0f);
		loadScreen.DOLocalMoveY (0, 2.0f).SetEase(Ease.OutBounce);
	
	}

	public void LoadingScreenAbort(){
	
		loadScreen.DOLocalMoveY (startPos*-1.0f, 2.0f).SetEase(Ease.OutBounce);
		centerTurnButts.DOLocalMoveX (0, 2.0f).SetEase(Ease.OutBounce);
	
	}

	public void GoToHighScores(){

		if (okToClick == false) {
			return;
		}

		okToClick = false;

		Invoke ("OkToClickAgain", 2.5f);

		highScores.DOLocalMoveX (0, 1.5f).SetEase(moveItIn);
		centerTurnButts.DOLocalMoveX (startPos, 1.5f).SetEase (moveItOut);


	}

	public void BackToMainMenu(){

		centerMainButts.DOLocalRotate (zeroCounter, 1.0f).SetEase(Ease.OutQuad);
		centerTurnButts.DOLocalRotate (oneEighty, 1.0f).SetEase(Ease.OutQuad);


	}

	public void BackToMainTurnMenu(){

		highScores.DOLocalMoveX (startPos * -1, 1.5f).SetEase(moveItOut);
		centerTurnButts.DOLocalMoveX (0, 1.5f).SetEase(moveItIn);


	}

	public void FiveRoomsOnly(){
		tutorialWords.SetActive (false);
		fiveRooms.SetActive (true);
		offScreen = frameMessage.transform.position;

		frameMessage.transform.DOLocalMoveY(0,1.0f).SetEase (Ease.OutBounce);
		Invoke ("MoveOff", 2.5f);
	}

	void MoveOff() {

		frameMessage.transform.DOLocalMoveY(offScreen.y * -1, 1.0f).OnComplete (MoveBack);

	}

	void MoveBack(){

		frameMessage.transform.DOLocalMoveY(offScreen.y, 0.0f);

	}

//	public void RefreshList (){
//
//		if (okToClick == false) {
//			return;
//		}
//
//		okToClick = false;
//
//		Invoke ("OkToClickAgain", 6.0f);
//
//		statusLoad.SetActive (true);
//		roomMan.roomsReady = false;
//		roomMan.GetRooms ();
//	
//	}

	void OkToClickAgain (){
	
		okToClick = true;
	
	}

	public void GetAllCategories (){

		StartCoroutine (getAllCategories());

	}

	IEnumerator getAllCategories (){

		string getCatsURL = "http://dupesite.000webhostapp.com/getCats.php";

		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", "fakeUser");

		WWW www = new WWW (getCatsURL, form);
		yield return www;

		//Debug.Log (www.text);


//		IEnumerator e = DCP.RunCS ("categories", "GetCategories2");
//
//		while (e.MoveNext ()) {
//			yield return e.Current;
//		}
//
//		string returnText = e.Current as string;
//
//		if (returnText == "") {
//			Debug.Log ("not cats returned");
//
//			GetAllCategories ();
//			yield break;
//		
//		}
//			
		CreateCatButtons (www.text);

	}

	void CreateCatButtons (string totalCats) {

		totalCats = totalCats.Replace("\n", "");

//		string newCatString = totalCats.Substring(totalCats.Length-6);
//		newCatString = "&" + newCatString + "&";
//		Debug.Log (newCatString);

		totalCats = totalCats.TrimEnd ('^');

		string[] totalCat = totalCats.Split ('^');

		foreach (string cat in totalCat) {


			//Debug.Log (cat);

			Vector3 offScreen = new Vector3 (2000,2000, 0);
			GameObject buttonObj = Instantiate (turnButtonObj,offScreen,Quaternion.identity, roomMan.buttonHolder);

//			buttonObj.GetComponent<RectTransform>().localScale = new Vector3 (1,1,1);

			roomMan.categoryButtons.Add (buttonObj);
			TurnRoomButton turnButt = buttonObj.GetComponent<TurnRoomButton> ();
			string[] item = cat.Split ('|');
			//Debug.Log (item[0]);
			for (int i = 0; i < item.Length; i++) {

				if (item [i].StartsWith (ROOMTYPE_SYM)) {

					turnButt.roomTypeString = item [i].Substring (ROOMTYPE_SYM.Length);

				} else if (item [i].StartsWith (WORDS_SYM)) {

					turnButt.words = item [i].Substring (WORDS_SYM.Length);

				} else if (item [i].StartsWith (BRUSHES_SYM)) {

					turnButt.brushes = item [i].Substring (BRUSHES_SYM.Length);

				} else if (item [i].StartsWith (GROUNDING_SYM)) {

					turnButt.grounding = item [i];

				}

			}

			turnButt.roomType.text = turnButt.roomTypeString;

		
		}
		if (roomMan == null) {
			roomMan = GameObject.FindGameObjectWithTag ("Room Manager").GetComponent<RoomManager> ();
		}

		roomMan.buttonsReady = true;
		roomMan.FindEmptyRooms ();

	}
		
	public void PlaceOptimalButtons(GameObject[] turnButtons){

		for (int i = 0; i < turnButtons.Length; i++) {

			if (turnHolder.childCount < 3){

				GameObject buutonObj = Instantiate(turnButtons[i], turnHolder);
				buutonObj.GetComponent<RectTransform>().localScale = new Vector3 (1,1,1);

		}

	}

		loadingText.SetActive (false);

	}

	public void LoadTutorial (){
	
		Invoke ("StartTutorial", 2.0f);
		roomMan.CurtainsIn ();

	}

	void StartTutorial () {
	
		SceneManager.LoadScene ("Tutorial Lobby Menu");
	}

	public void StartPrivateGame (){
	
		UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();
		string roomsString = userAccount.activeRooms;

		if (roomsString.Length > 3) {
			return;
		}

		roomMan = GameObject.FindGameObjectWithTag ("Room Manager").GetComponent<RoomManager> ();
		roomMan.StartNewPrivateGame ();
		roomMan.TurnOnSign ();
	
	}

	public void SignOut(){
		GameObject roomManagerObj = GameObject.FindGameObjectWithTag ("Room Manager");
		Destroy (roomManagerObj);
		UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();
		userAccount.activeRooms = string.Empty;
		userAccount.loggedInUsername = string.Empty;

		SceneManager.LoadScene ("Login");

	}

//	public void StartNextPrivateRound (){
//
//		roomMan = GameObject.FindGameObjectWithTag ("Room Manager").GetComponent<RoomManager> ();
//		roomMan.StartNextPrivateRound ();
//		roomMan.TurnOnSign ();
//
//	}



}
