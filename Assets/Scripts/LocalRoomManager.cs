using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using DatabaseControl;
using UnityEngine.SceneManagement;

public class LocalRoomManager : MonoBehaviour {

	TurnRoomScript myRoom;
	public CameraScript cameraScript;
	public LineSpawnerScipt lineSpawn;
	public GameObject brushPrefabX;
	public GameObject brushPrefabY;
	public GameObject brushHolder;
	public Text subject;
	public GameObject blackLine;
	public GameObject whiteLine;
	public Color[] pColors;
	public Color blackBackground;
	public Color whiteMatOrig;
	public Image backGroundDraw;

	public GameObject readyButtons;
	public GameObject drawButtons;
	public GameObject theDrawButton;

	string myLineString;

	public Sprite redDone;
	public Sprite blueDone;
	public Sprite greenDone;
	public Sprite orangeDone;

	public Image doneButton;

	public Sprite redUndo;
	public Sprite blueUndo;
	public Sprite greenUndo;
	public Sprite orangeUndo;

	public Image undoButton;
	public Material blackMat;
	public Material whiteMat;

	GameObject roomMan;
	//public GameObject frameMessage;
	//Vector2 offScreen;

	bool readyToAdvance = false;
	bool okayToClick = false;

	public RectTransform bottomPanel;
	Vector3 panelScreen;
	Vector3 panelScreenOff;

	public bool tutorialMode;
	bool readyToMove;

	UserAccountManagerScript userAccount;

	public Image timerColor1;
	public Image timerColor2;
	public Image timerRing;
	public Text timerText1;
	public Text timerText2;
	float timerFloat;
	int timerInt;
	string timerString;
	bool timer;
	float timerStart;
	public GameObject unlimitedSign;

	public RectTransform brushSign;
	public Text signText;
	float signStartPos;
	float signOffPos;

	bool signIsUp = false;
	bool panelHasMoved = false;
	public SpriteRenderer blackScreen;

	public float shakeInt;

	public GameObject youTheDupe;
	public SpriteRenderer drawingBlocker;
	public SpriteRenderer youTheDupeText;

	bool gameEndMissedBrush = false;

	// Use this for initialization
	void Start () {

		Invoke ("ShakeButton", 1.5f);
		Invoke ("OpenCurtains", .5f);

		panelScreen = bottomPanel.anchoredPosition;
		panelScreenOff = new Vector3 (panelScreen.x, panelScreen.y - 500, panelScreen.z);

		brushSign.gameObject.SetActive (true);
		signStartPos = brushSign.anchoredPosition.y;
		signOffPos = signStartPos + 1300;
		brushSign.DOAnchorPosY (signOffPos, 0.1f);


		if (tutorialMode == true) {
			
			return;
		}

		FindMyRoom ();

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.K)) {

			EDAnimations.Instance.BurstDisappear (youTheDupe);

		}

		if (timer == true) {
			timerInt = Mathf.RoundToInt (timerFloat);
			timerFloat -= Time.deltaTime;
			timerString = timerInt.ToString ();
			if (timerInt < 10) {
				timerString = "0" + timerString;
			}
			timerText1.text = timerString[0].ToString();
			timerText2.text = timerString [1].ToString ();
			timerRing.fillAmount = timerFloat / timerStart;
			if (timerFloat < .05f) {
				timer = false;
				timerFloat = 0;
				timerInt = 0;
				timerRing.fillAmount = 0;
				TimerIsUp ();
			}
		}
	}

	void ShakeButton (){
	
		if (tutorialMode == true) {

			return;
		}
			
		Vector3 punchSize = new Vector3 (.5f, .5f, .5f);
		theDrawButton.transform.DOPunchScale (punchSize, 1.0f,10,.01f).SetDelay(3.0f).SetId("buttshake").OnComplete(ShakeButton);		

	} 

	void OpenCurtains(){
		roomMan.GetComponent<RoomManager> ().CurtainsOut();
	}

	void FindMyRoom (){
	
		roomMan = GameObject.FindGameObjectWithTag ("Room Manager");
		
		if (roomMan == null) {
			Debug.Log ("not logged in");
			return;
		}

		Transform roomHolder = GameObject.FindGameObjectWithTag ("Room Holder").transform;

		TurnRoomScript[] rooms = roomHolder.GetComponentsInChildren<TurnRoomScript> ();
		foreach (TurnRoomScript room in rooms) {

			if (room.activeRoom == true) {
				myRoom = room;
			}
		}

		DrawGrounding ();

		Camera.main.GetComponent<CameraScript> ().ZoomIn (myRoom.myActualColor);

		if (myRoom.dupeNum == myRoom.myColor) {

			if (myRoom.mode [1] == true) {
				subject.text = "???????????";
			} else {
				subject.text = myRoom.wrongword;
			}

		} else {
			subject.text = myRoom.rightword;
		}

		if (myRoom.myActualColor == 1) {

			doneButton.sprite = redDone;
			undoButton.sprite = redUndo;

		} else if (myRoom.myActualColor == 2) {

			doneButton.sprite = blueDone;
			undoButton.sprite = blueUndo;

		} else if (myRoom.myActualColor == 3) {

			doneButton.sprite = greenDone;
			undoButton.sprite = greenUndo;

		} else if (myRoom.myActualColor == 4) {

			doneButton.sprite = orangeDone;
			undoButton.sprite = orangeUndo;

		}

		backGroundDraw.color = pColors [myRoom.myActualColor - 1];
		timerColor1.color = pColors [myRoom.myActualColor - 1];
		timerColor2.color = pColors [myRoom.myActualColor - 1];

		userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();

		string roomsString = userAccount.activeRooms;

		string roomSearch = roomsString;
		bool foundRoom = false;
//		Debug.Log (roomSearch);

		if (roomSearch.Length > 0) {
			
			string[] roomSplit = roomSearch.Split ('/');
			string curRoom = myRoom.roomID.ToString ();

			for (int i = 0; i < roomSplit.Length; i++) {

				if (curRoom == roomSplit [i]) {
					foundRoom = true;
				}

			}
		}
		if (foundRoom == false) {

			if (roomsString == "") {
				roomsString = myRoom.roomID.ToString ();
				userAccount.activeRooms = roomsString;
			} else {
				roomsString = roomsString + "/" + myRoom.roomID.ToString ();
				userAccount.activeRooms = roomsString;
			}
		} else {
			Debug.Log ("Already logged room");
		}

		lineSpawn.GetColor (myRoom.myActualColor);

		if (myRoom.privateRoom == true) {
			if (myRoom.portraits [myRoom.myColor - 1] == "") {
				StartCoroutine (sendPlayerData ());
			}
		}

		if (myRoom.time [0] == true) {
			unlimitedSign.SetActive (true);
			timerText1.text = "";
			timerText2.text = "";
		} else if (myRoom.time [1] == true) {
			timerFloat = 15;
			timerStart = 15;
			timerText1.text = "1";
			timerText2.text = "5";
		} else if (myRoom.time [2] == true) {
			timerFloat = 45;
			timerStart = 45;
			timerText1.text = "4";
			timerText2.text = "5";
		} else if (myRoom.time [3] == true) {
			timerFloat = 60;
			timerStart = 60;
			timerText1.text = "6";
			timerText2.text = "0";
		} else {
			unlimitedSign.SetActive (true);
			timerText1.text = "";
			timerText2.text = "";
		}

		if (myRoom.time [0] == false) {
			Invoke ("TimedGameStartAlready", 10.0f);
		}

		if (myRoom.method [1] == true) {
			lineSpawn.TurnItClear ();
			blackScreen.color = blackBackground;
			drawingBlocker.color = blackBackground;
		}

		if (myRoom.mode [1] == true && myRoom.dupeNum == myRoom.myColor) {
			drawingBlocker.gameObject.SetActive (true);
			youTheDupe.SetActive (true);
			youTheDupeText.color = pColors [myRoom.myActualColor - 1];
			Invoke ("HideTheDupeSign", 4.0f);
		}

		Invoke ("ReadyToMove", 2.0f);

	}

	void TimedGameStartAlready(){
		if (panelHasMoved == false) {
			MoveDownReady();
		}
	}

	void HideTheDupeSign (){
		Invoke ("HideTheScreenBlocker", 1.0f);
		EDAnimations.Instance.BurstDisappear (youTheDupe);
	}

	void HideTheScreenBlocker (){
		Color clearColor = new Vector4 (1, 1, 1, 0);
		drawingBlocker.DOColor (clearColor, .75f);
	}


	IEnumerator sendPlayerData (){
	
		string roomIdString = myRoom.roomID.ToString();
		string myPlayerNum = myRoom.myColor.ToString();

		string URL = "http://dupesite.000webhostapp.com/addNotificationPortraits.php";

		WWWForm form = new WWWForm ();
		form.AddField ("idPost", roomIdString);
		form.AddField ("notIdPost", userAccount.notificationId);
		form.AddField ("portraitPost", userAccount.selfPortrait);
		form.AddField ("mySpotPost", myPlayerNum);

		WWW www = new WWW (URL, form);
		yield return www;

		Debug.Log ("Sent my pic and id:" + www.text);

	}

	void ReadyToMove(){
		readyToMove = true;
	}

	public void MoveDownReady(){
		panelHasMoved = true;
		bottomPanel.DOAnchorPos (panelScreenOff, .5f);

		if (readyToMove == true) {
			MoveToSection ();
		} else {
			Invoke ("MoveToSection", 2.0f);
		}
	}

	public void MoveToSection () {

		cameraScript.MoveToSection ();
		//Color clear = new Color (1, 1, 1, 0);
		Invoke ("HideLines", 2.5f);

	}

	void HideLines (){
		if (myRoom.method[1] == false) {
			blackMat.DOColor (Color.white, 1.0f).OnComplete (DestroyLines);
		} else {
			whiteMat.DOColor (blackBackground, 1.0f).OnComplete (DestroyLines);
		}
	}

	void DestroyLines (){
		
		if (myRoom.time [0] == false) {
			timer = true;
		}

		if (myRoom.method [2] == true) {
			Camera.main.gameObject.GetComponent<CameraShake> ().StartWiggle ();
		}

		GameObject[] lines = GameObject.FindGameObjectsWithTag ("Line");

		foreach (GameObject line in lines) {
			Destroy (line);
		}

		Color grey = new Color (.2f, .2f, .2f, 1.0f);
		blackMat.color = grey;
		whiteMat.color = whiteMatOrig;
	
	}

	public void StartGame(){
	
		DOTween.Kill ("buttshake");

		readyButtons.SetActive (false);
		drawButtons.SetActive (true);

		bottomPanel.DOAnchorPos (panelScreen, 1.0f);

		Invoke ("LoadBrushes", .7f);
	


	}

	public void MoveUpPanel(){

		Debug.Log ("move up p");

		bottomPanel.DOAnchorPos (panelScreen, 1.0f);

	}

	void DrawGrounding () {

		if (myRoom.method [1] == true) {
			blackLine = whiteLine;
		}
		Color grey = new Color (.2f, .2f, .2f, 1.0f);
		blackMat.color = grey;
		

		string[] lines = myRoom.grounding.Split ('$');

		foreach (string line in lines) {

			GameObject lineGo = Instantiate (blackLine);
			LineRenderer lineRend = lineGo.GetComponent <LineRenderer> ();

			string[] points = line.Split ('@');

			lineRend.positionCount = points.Length;

			for (int i = 0; i < points.Length; i++) {

				string[] vectArray = points[i].Split(',');

				Vector3 tempVect = new Vector3(
					float.Parse(vectArray[0]),
					float.Parse(vectArray[1]),
					0);
				lineRend.SetPosition (i, tempVect);

			}

		}

	}

	void LoadBrushes (){
	
		int myColor = myRoom.myActualColor;
		Invoke ("OkayToClick", 1.5f);

		for (int i = 0; i < myRoom.brushes.Length; i++) {

			Vector3 brushPos = myRoom.brushes [i];
			Vector3 newBrushPos;

			if (myRoom.brushes [i].x == 0) {

				if (myColor == 1 || myColor == 4) {
					newBrushPos = new Vector3 (brushPos.x + 1, brushPos.y, brushPos.z);
				} else {
					newBrushPos = new Vector3 (brushPos.x - 1, brushPos.y, brushPos.z);
				}

				GameObject newBrush = (GameObject)Instantiate (brushPrefabY, newBrushPos, Quaternion.identity);
				if (myRoom.method [2] == true) {
					newBrush.GetComponent<BrushScript> ().TurnOnHandle (myColor);
				}
				newBrush.transform.SetParent (brushHolder.transform, false);
				newBrush.transform.DOLocalMoveX (0, 1.0f);

			} else {

				if (myColor == 1 || myColor == 2) {
					newBrushPos = new Vector3 (brushPos.x, brushPos.y - 1, brushPos.z);
				} else {
					newBrushPos = new Vector3 (brushPos.x, brushPos.y + 1, brushPos.z);
				}

				GameObject newBrush = (GameObject)Instantiate (brushPrefabX, newBrushPos, Quaternion.identity);
				newBrush.transform.SetParent (brushHolder.transform, false);
				newBrush.transform.DOLocalMoveY (0, 1.0f);
				if (myRoom.method [2] == true) {
					newBrush.GetComponent<BrushScript> ().TurnOnHandle (myColor);
				}
			}
		}
	}

	void OkayToClick (){
		okayToClick = true;
	}

	public void DoneDrawingButton () {
	
		if (okayToClick == false) {
			return;
		}

		okayToClick = false; 
		Invoke ("OkayToClick", 2.5f);



		if (CheckBrushes() == true) {
			CollectYourLineData ();
			bottomPanel.DOAnchorPos (panelScreenOff, .5f);
		} else {
			StillHaveBrushes ();
		}
	
	}

	public void StillHaveBrushes(){

		signIsUp = true;

		signText.text = "YOU HAVE TO DRAW ON ALL THE BRUSHES";
		brushSign.DOAnchorPosY (signStartPos, 1.0f).SetEase (Ease.OutBounce);
		Invoke ("MoveOff", 2.5f);

	}

	void MissedABrushSign(){
	
		signText.text = "YOU MISSED A BRUSH!\n-1 POINT";
		brushSign.DOAnchorPosY (signStartPos, 1.0f).SetEase (Ease.OutBounce);
		//Invoke ("MoveOff", 2.5f);

	}

	void MoveOff() {

		brushSign.DOAnchorPosY (signOffPos, 1.0f).OnComplete (OkForNewSign);

	}

	void OkForNewSign(){
		okayToClick = true;
		signIsUp = false;

	}

	void TimerIsUp(){
	
		lineSpawn.dontDraw = true;

		if (CheckBrushes () == true) {
			CollectYourLineData ();
		} else {
			CollectYourLineData ();
			if (signIsUp == false) {
				MissedABrushSign ();
				gameEndMissedBrush = true;
			} else {
				Invoke ("MissedABrushSign", 2.1f);
				gameEndMissedBrush = true;
			}
		}
	}

	bool CheckBrushes(){
	
		readyToAdvance = true;
		int brushCount = brushHolder.transform.childCount;
		int myColor = myRoom.myActualColor;

		float xMax;
		float xMin;
		float yMax;
		float yMin;

		if (myColor == 1) {
			xMax = 0;
			xMin = -10;
			yMax = 10;
			yMin = 0;
		} else if (myColor == 2) {
			xMax = 10;
			xMin = 0;
			yMax = 10;
			yMin = 0;
		} else if (myColor == 3) {
			xMax = 10;
			xMin = 0;
			yMax = 0;
			yMin = -10;
		} else if (myColor == 4) {
			xMax = 0;
			xMin = -10;
			yMax = 0;
			yMin = -10;
		} else {
			xMax = 0;
			xMin = -10;
			yMax = 10;
			yMin = 0;
		}

		for (int i = 0; i < brushCount; i++) {

			Vector3 brushPos = brushHolder.transform.GetChild (i).transform.position;
			if (brushPos.y == 0 && brushPos.x > xMin && brushPos.x < xMax) {
				readyToAdvance = false;
			} else if (brushPos.x == 0 && brushPos.y > yMin && brushPos.y < yMax){
				readyToAdvance = false;
			}
		}

		if (readyToAdvance == true) {
			return true;
		} else {
			return false;
		}
	
	}
				

	void CollectYourLineData () {
		
		roomMan.GetComponent<RoomManager> ().TurnOffSign ();
	
		GameObject[] lines = GameObject.FindGameObjectsWithTag ("Line");

		myLineString = "[MYCOLOR]" + myRoom.myActualColor.ToString () + ":";

		foreach (GameObject line in lines) {

			LineRenderer lineRend = line.GetComponent<LineRenderer> ();
			int lineAmount = lineRend.positionCount;

			for (int i = 0; i < lineAmount; i++) {

				Vector2 point = lineRend.GetPosition (i);


				myLineString = myLineString + point.ToString ("F2") + "@";

				if (i == lineAmount - 1) {

					string[] charsToRemove = new string[] { "(", ")", " "};
					foreach (string character in charsToRemove)
					{
						myLineString = myLineString.Replace(character, string.Empty);
					}


					myLineString = myLineString.Replace("0.00", "0");
					myLineString = myLineString.Replace("-0.", "-.");
					myLineString = myLineString.Replace("0.", ".");
					myLineString = myLineString.Replace("++", "+");
					myLineString = myLineString.Replace(":+", ":");

				}

			}

			myLineString = myLineString.TrimEnd('@');
			myLineString = myLineString + "+";
		}
		myLineString = myLineString.TrimEnd('+');
		myLineString = myLineString + ":";

		GameObject[] dots = GameObject.FindGameObjectsWithTag ("Dot");

		foreach (GameObject dot in dots) {

			LineRenderer lineRend = dot.GetComponent<LineRenderer> ();
			int lineAmount = lineRend.positionCount;

			for (int i = 0; i < lineAmount; i++) {

				Vector2 point = lineRend.GetPosition (i);

				myLineString = myLineString + point.ToString ("F2") + "@";

				if (i == lineAmount - 1) {

					string[] charsToRemove = new string[] { "(", ")", " "};
					foreach (string character in charsToRemove)
					{
						myLineString = myLineString.Replace(character, string.Empty);
					}

					myLineString = myLineString.Replace("0.00", "0");
					myLineString = myLineString.Replace("-0.", "-.");
					myLineString = myLineString.Replace("0.", ".");

				}

			}

			myLineString = myLineString.TrimEnd('@');
			myLineString = myLineString + "+";
		}

		myLineString = myLineString.TrimEnd('+');

		if (CheckBrushes () == true) {
			myLineString = myLineString + ":0$";
		} else {
			myLineString = myLineString + ":1$";
		}

		if (myRoom.method [2] == true) {
			Camera.main.gameObject.GetComponent<CameraShake> ().StopWiggle ();
		}

		StartCoroutine (doneDrawing(myRoom.roomID, myLineString));

	}

	void CloseCurtains(){
		roomMan.GetComponent<RoomManager> ().CurtainsIn ();
		Invoke ("GoToLobby", 1.0f);
	}


	IEnumerator doneDrawing (int roomID, string drawingArray){

		string roomIdString = roomID.ToString();
		string myPlayerNum = myRoom.myColor.ToString ();

		string URL = "http://dupesite.000webhostapp.com/storeDrawing.php";

		WWWForm form = new WWWForm ();
		form.AddField ("roomIdPost", roomIdString);
		form.AddField ("playerNumPost", myPlayerNum);
		form.AddField ("drawingPost", drawingArray);

		WWW www = new WWW (URL, form);
		yield return www;

		Debug.Log ("Drawing return:" + www.text);

		myRoom.activeRoom = false;
		myRoom.status = "waiting";
		myRoom.statusNum = 1;

		float closingTime = 0.0f;
		if (gameEndMissedBrush == true) {
			closingTime = 3.0f;
		}
			
		if (myRoom.time [0] == true) {
			CloseCurtains ();
		} else {
			Invoke ("CloseCurtains", closingTime);
		}

		RoomManager.instance.cameFromTurnBased=true;
		RoomManager.instance.amIFirstDone=true;

	}

	void GoToLobby(){
	
		SceneManager.LoadScene ("Lobby Menu");

	}


}
