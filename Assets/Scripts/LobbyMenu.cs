using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

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
		if (roomMan.cameFromTurnBased == true) {
			TurnBasedClicked ();
			roomMan.UpdateTurnRooms();
		}

		if (roomMan.cameFromScoring == false) {
			highScores.gameObject.GetComponent<HighScoreScript> ().UpdateTheScore ();
		} else {
			GoToHighScores ();
			roomMan.cameFromScoring = false;
			UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();
			string roomsString = userAccount.activeRooms;

			if (roomsString.Length < 5) {
			
				if (frameText == null) {

					frameText = GameObject.FindGameObjectWithTag ("Frame Text");

				}

				frameText.GetComponent<Text>().text = "Nothin happenin";

			}

		}

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.C)) {
		
			FiveRoomsOnly ();
		
		}
		
	}

	public void TurnBasedClicked(){
		
		centerMainButts.DOLocalRotate (oneEighty, 1.0f).SetEase(Ease.OutQuad);
		centerTurnButts.DOLocalRotate (zeroCounter, 1.0f).SetEase(Ease.OutQuad);

		//DOTween.To(()=> center.GetComponent<RectTransform>().rotation, x=> center.GetComponent<RectTransform>().rotation = x, 180, 1.0f);

		//mainButtons.SetActive (false);
		//turnBasedButtons.SetActive (true);
	
	}

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

	public void RefreshList (){

		if (okToClick == false) {
			return;
		}

		okToClick = false;

		Invoke ("OkToClickAgain", 6.0f);

		statusLoad.SetActive (true);
		roomMan.GetRooms ();
	
	}

	void OkToClickAgain (){
	
		okToClick = true;
	
	}

}
