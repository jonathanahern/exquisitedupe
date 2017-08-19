using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using DatabaseControl;

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
		roomMan.roomsReady = false;
		if (roomMan.cameFromTurnBased == true) {
			TurnBasedClicked ();
			roomMan.CurtainsOut();
			//roomMan.FindEmptyRooms ();
			roomMan.startingNew = false;
			roomMan.GetRooms ();
			//roomMan.DropOffButtons ();
		}

		if (roomMan.cameFromScoring == false) {
			highScores.gameObject.GetComponent<HighScoreScript> ().UpdateTheScore ();
		} else {
			GoToHighScores ();
			roomMan.cameFromScoring = false;
			UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();
			string roomsString = userAccount.activeRooms;

			if (roomsString.Length < 5) {
				roomMan.noRooms = true;
				if (frameText == null) {

					frameText = GameObject.FindGameObjectWithTag ("Frame Text");

				}

				frameText.GetComponent<Text> ().text = "Nothin happenin";

			} else {
				
			}

		}

			InvokeRepeating ("AutoUpdateRooms", 20.0f, 20.0f);

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.C)) {
		
			GameObject buttonObj = Instantiate (turnButtonObj,offScreen,Quaternion.identity);
			buttonObj.transform.SetParent (roomMan.buttonHolder, false);
			GameObject buttonObj1 = Instantiate (turnButtonObj,offScreen,Quaternion.identity);
			buttonObj1.transform.SetParent (roomMan.buttonHolder, false);
			GameObject buttonObj2 = Instantiate (turnButtonObj,offScreen,Quaternion.identity);
			buttonObj2.transform.SetParent (roomMan.buttonHolder, false);
		
		}
		
	}

	void AutoUpdateRooms (){
		if (roomMan.noRooms == true) {
			return;
		}
		Debug.Log ("TURN ON");
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

		if (isFocused) {
			Debug.Log ("asdfasdf");
			CancelInvoke ();

			if (roomMan.refreshing == true) {
				InvokeRepeating ("AutoUpdateRooms", 12.0f, 20.0f);
				//Invoke ("SendLate", 5.0f);
			} else {
				InvokeRepeating ("AutoUpdateRooms", 2.0f, 20.0f);
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

		if (statusHolder.childCount > 4) {
			FiveRoomsOnly ();
			return;
		}
	
		newCats.DOLocalMoveX (0, 2.0f).SetEase(Ease.OutBounce);
		centerTurnButts.DOLocalMoveX (startPos * -1.0f, 2.0f).SetEase(Ease.OutBounce);

	}

	public void NewCatsOffScreen(){
	
		newCats.DOLocalMoveX (startPos, 2.0f);
		centerTurnButts.DOLocalMoveX (0, 2.0f).SetEase(Ease.OutBounce);
	
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

		highScores.DOLocalMoveX (0, 2.0f).SetEase(Ease.OutBounce);
		centerTurnButts.DOLocalMoveX (startPos, 2.0f);


	}

	public void BackToMainMenu(){

		centerMainButts.DOLocalRotate (zeroCounter, 1.0f).SetEase(Ease.OutQuad);
		centerTurnButts.DOLocalRotate (oneEighty, 1.0f).SetEase(Ease.OutQuad);


	}

	public void BackToMainTurnMenu(){

		highScores.DOLocalMoveX (startPos * -1, 2.0f);
		centerTurnButts.DOLocalMoveX (0, 2.0f).SetEase(Ease.OutBounce);


	}

	public void FiveRoomsOnly(){

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

		IEnumerator e = DCP.RunCS ("categories", "GetCategories");

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		CreateCatButtons (returnText);

	}

	void CreateCatButtons (string totalCats) {

		totalCats = totalCats.TrimEnd ('^');
		//Debug.Log (totalCats);
		string[] totalCat = totalCats.Split ('^');


		foreach (string cat in totalCat) {

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

//	public void DetachButtons(){
//
//		loadingText.SetActive (true);
//
//		int childCount = turnHolder.childCount;
//
//		for (int i = 0; i < childCount; i++) {
//
//			turnHolder.GetChild (0).transform.SetParent(null,false);
//
//		}
//
//		roomMan.TakeButtonsWith ();
//	}

}
