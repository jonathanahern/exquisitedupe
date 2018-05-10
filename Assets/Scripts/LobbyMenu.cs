using System.Collections;
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
	public RectTransform addFriendsRect;
	public RectTransform startNewMain;
	public RectTransform addCategories;
	public RectTransform gameOptionsScreen;

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
	private static string OTHER_SYM = "[OTHER]";

	public GameObject refreshingScreen;
	public Text loadScreenWords;

	//public GameObject loadingAnimationCenter;
	public bool tutorialMode = false;
	int roomCount;
	public AnimationCurve moveItOut;
	public AnimationCurve moveItIn;
	public AnimationCurve scaleBounce;

	public Text playersName;
	public Transform inviteHolder;
	public GameObject selectPlayerPrefab;
	public GameObject selectPlayerLoading;
	public GameObject selectCategoriesLoading;
	public Transform categoryPrivateHolder;
	public GameObject categoryPrivatePrefab;
	public GameObject scroller;

	public int selectCount;
	//public string[] artistsSelected;
	public List<string> artistsSelected;
	bool wasAtZeroInvites = false;

	public Text selectCatsText;
	//public int catsCount;
//	public string[] catsSelected;
	public List<string> catsSelected;

	public Toggle[] gameOption;
	public string optionString;
	//public List<TurnRoomButton> gameData;
	public List<string> fateData;
	List<string>catColors;
	List<int> playerBag;
	List<int> modeBag;
	List<int> dupeNums;
	public List<int> invitationNums;

	UserAccountManagerScript userAccount;
	public GameObject invitationPrefab;
	public Transform invitationHolder;
	public RectTransform invitationPos;

	public RectTransform buttonPalette;
	float paletteStartPos;
	public RectTransform nextButtonNG;
	public RectTransform backButtonNG;
	float backButtonNGStartPos;
	public GameObject nextText;
	public GameObject nextButton;
	public GameObject sendInvitesButton;
	public AnimationCurve signRotate;


	public Text roundCount;
	public RectTransform eightLimit;

	int stepNG = 0;
	WWWForm privateForm;

	//string inviteMessage = " has challenged you to a contest of wits and stupid art!";

	// Use this for initialization
	void Awake () {

		instance = this;

	}

	void Start(){

		paletteStartPos = buttonPalette.anchoredPosition.y;
		backButtonNGStartPos = backButtonNG.anchoredPosition.y;
		backButtonNG.DOAnchorPosY (backButtonNGStartPos - 600, 0.0f);
		statusLoad = GameObject.FindGameObjectWithTag ("Status Load");
		oneEighty = new Vector3 (0, 0, 180.0f);
		zeroCounter = new Vector3 (0, 0, 360.0f);
		startPos = newCats.position.x;
		roomMan = GameObject.FindGameObjectWithTag ("Room Manager").GetComponent<RoomManager> ();

		selectCount = 3;
		artistsSelected = new List<string> ();

		playerBag = new List<int> ();
		modeBag = new List<int> ();
		catColors = new List<string> ();

		//catsCount = 4;
		//selectCatsText.text = "SELECT " + catsCount + " CATEGORIES";
		catsSelected = new List<string> ();

		invitationNums = new List<int> ();

		if (roomMan.beenToLobby == false) {
			roomMan.InitialLobbySetup ();
		}

		if (tutorialMode == true) {
			return;
		}

		if (roomMan.cameFromTurnBased == true) {
			Debug.Log ("came From turn based");
			TurnBasedClicked ();
			Invoke ("CurtainsOpen", 1.0f);
			roomMan.FindEmptyRooms ();
			roomMan.startingNew = false;
			roomMan.UpdateStatus ();
			StartCoroutine (getMessageData());
			Invoke ("TurnOffFirstDone", 3.0f);

		}



		userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();

//		if (userAccount.firstLogin == true) {
//			Invoke ("AskForTutorial", 1.0f);
//			userAccount.firstLogin = false;
//		}

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
			frameText.GetComponent<Text> ().text = "YOU ART NOT A PART OF ANY PAINTINGS";
			refreshingScreen.SetActive (false);
		} else {
			//Debug.Log (roomCount);
			for (int i = 0; i < roomCount; i++) {
				//Debug.Log (roomArray [i]);
				roomMan.UpdateTurnRoomsFromLogin (int.Parse (roomArray [i]));
			}

		}

		if (roomCount > 5) {
			scroller.SetActive (true);
		}

		if (roomMan.cameFromPrivate == true) {
			TurnBasedClicked ();
			Invoke ("CurtainsOpen", 1.0f);
			roomMan.FindEmptyRooms ();
			roomMan.startingNew = false;
			roomMan.UpdateStatus ();
			StartCoroutine (getMessageData());
			roomMan.amIFirstDone = false;
			roomMan.CreateNextPrivateRoom ();
		}

		InvokeRepeating ("AutoUpdateRooms", 1.0f, 10.0f);

	}

	void TurnOffFirstDone (){
	
		roomMan.amIFirstDone = false;
	
	}

	public void SendAHello (){
		
		StartCoroutine (getPlayerNotId ());
	
	}

	IEnumerator getPlayerNotId (){

		string getPlayerNotIdURL = "http://dupesite.000webhostapp.com/getPlayerNotId.php";

		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", playersName.text);

		WWW www = new WWW (getPlayerNotIdURL, form);
		yield return www;

		Debug.Log (www.text);

		SendTheMessage (www.text);
	}

	void SendTheMessage (string userNotId){

		if (userNotId == "Not Found") {
			return;
		}
	
				// Just an example userId, use your own or get it the devices by calling OneSignal.GetIdsAvailable
				
		var notification = new Dictionary<string, object> ();
		notification ["contents"] = new Dictionary<string, string> () { { "en", "Hey punky" } };

		notification ["include_player_ids"] = new List<string> () { userNotId };
		// Example of scheduling a notification in the future.
		//notification ["send_after"] = System.DateTime.Now.ToUniversalTime ().AddSeconds (5).ToString ("U");

		OneSignal.PostNotification (notification, (responseSuccess) => {
			Debug.Log("Notification posted successful! Delayed by about 30 secounds to give you time to press the home button to see a notification vs an in-app alert.");
		}, (responseFailure) => {
			Debug.Log("Notification failed to post");
		});

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
		StartCoroutine (getMessageData());
		if (roomMan.noRooms == true) {
			Debug.Log ("no rooms found so no updates");
			return;
		}
		refreshingScreen.SetActive (true);
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

		Invoke ("OkToClickAgain", 1.5f);
//		if (roomCount > 4) {
//			FiveRoomsOnly ();
//			return;
//		}
		stepNG = 1;
		HideLobbyButtons ();
		newCats.DOAnchorPosX (0, 1.5f).SetEase(moveItIn);
		centerTurnButts.DOAnchorPosX (-1200, 1.5f).SetEase(moveItOut);
		Invoke ("BringInBackButtonNG", .5f);

	}

	public void GoToAddFriendsScreen() {

		if (okToClick == false) {
			return;
		}

		okToClick = false;

		Invoke ("OkToClickAgain", 1.5f);

		if (inviteHolder.childCount < 1) {
			CreateFriendsList ();
		}
		addFriendsRect.DOAnchorPosX (0, 1.5f).SetEase(moveItIn);
		startNewMain.DOAnchorPosX (-1600, 1.2f).SetEase(moveItOut);

	}

	public void GoToAddCategoriesScreen() {

		if (okToClick == false) {
			return;
		}

		if (artistsSelected.Count < 3 ) {
			return;
		}

		okToClick = false;

		Invoke ("OkToClickAgain", 1.5f);

		CreateCategoriesList ();
		addCategories.DOAnchorPosY (0, 1.5f).SetEase(moveItIn);
		addFriendsRect.DOAnchorPosY (-2100, 1.2f).SetEase(moveItOut);

	}

	//From Add Cats
	public void GoBackToAddFriendsScreen() {

		if (okToClick == false) {
			return;
		}

		okToClick = false;

		Invoke ("OkToClickAgain", 1.5f);

		addCategories.DOAnchorPosY (2100, 1.2f).SetEase(moveItOut);
		addFriendsRect.DOAnchorPosY (0, 1.5f).SetEase(moveItIn);

	}

	//From Main Start Screen
	public void GoToGameOptionsScreen() {

		if (okToClick == false) {
			return;
		}

//		if (catsSelected.Count < 4 ) {
//			return;
//		}

		okToClick = false;

		Invoke ("OkToClickAgain", 1.5f);
		stepNG = 2;
		startNewMain.DOAnchorPosX (-1600, 1.2f).SetEase(moveItOut);
		gameOptionsScreen.DOAnchorPosX (0, 1.5f).SetEase(moveItIn);
		nextButtonNG.DOAnchorPosY(backButtonNGStartPos, 1.0f).SetEase (Ease.OutBounce);


	}

	//From Add Cats
	public void GoBackToCategoriesScreen() {

		if (okToClick == false) {
			return;
		}
		okToClick = false;
		Invoke ("OkToClickAgain", 1.5f);

		addCategories.DOAnchorPosY (0, 1.5f).SetEase(moveItIn);
		gameOptionsScreen.DOAnchorPosY (2100, 1.2f).SetEase(moveItOut);

	}

	public void RefreshCreateFriendsList (){

		if (selectCount != 3) {
			selectCount = 3;
			artistsSelected.Clear ();

			if (wasAtZeroInvites == true) {
				wasAtZeroInvites = false;
				ReactivateSelection (inviteHolder);
				FlipSignToBack ();
			}
		}

		selectPlayerLoading.SetActive (true);
		for (int i = 0; i < inviteHolder.childCount; i++) {
			Destroy (inviteHolder.GetChild (i).gameObject);
		}
		roomMan.RefreshPlayerNames ();

	}

	public void CreateFriendsList(){

		for (int i = 0; i < roomMan.otherPlayers.Count; i++) {

			GameObject playerNameObj = Instantiate (selectPlayerPrefab, inviteHolder);
			playerNameObj.GetComponent<SelectPlayerScript> ().InsertName (roomMan.otherPlayers [i]);

		}

		selectPlayerLoading.SetActive (false);
	
	}

	void CreateCategoriesList(){

		Transform catHolder = GameObject.FindGameObjectWithTag ("Category Holder").transform;

		for (int i = 0; i < catHolder.childCount; i++) {

			TurnRoomButton turnButt = catHolder.GetChild (i).GetComponent<TurnRoomButton> ();
			GameObject catObjSelect = Instantiate (categoryPrivatePrefab, categoryPrivateHolder);
			catObjSelect.GetComponent<SelectPlayerScript> ().InsertCategory (turnButt.roomTypeString, turnButt.description,turnButt.catColor);

		}

		selectCategoriesLoading.SetActive (false);

	}

	public void BackToStartNewGame(){
	
		addFriendsRect.DOAnchorPosY (2100, 1.2f).SetEase(moveItIn);
		startNewMain.DOAnchorPosY (0, 1.5f).SetEase(moveItIn);
	
	}


	public void BackButtonNG(){

		if (okToClick == false) {
			return;
		}
		okToClick = false;
		Invoke ("OkToClickAgain", 1.25f);


		if (stepNG == 1) {
			newCats.DOLocalMoveX (startPos, 1.5f).SetEase (moveItOut);
			centerTurnButts.DOLocalMoveX (0, 1.5f).SetEase (moveItIn);
			backButtonNG.DOAnchorPosY (backButtonNGStartPos - 600, 1.0f);
			Invoke ("BringBackLobbyButtons", .5f);
		
		} else if (stepNG == 2) {
			startNewMain.DOAnchorPosX (0, 1.5f).SetEase(moveItIn);
			gameOptionsScreen.DOAnchorPosX (1600, 1.2f).SetEase(moveItOut);
			nextButtonNG.DOAnchorPosY(backButtonNGStartPos - 600, 1.0f);
			stepNG = 1;
		} else if (stepNG == 3) {
			gameOptionsScreen.DOAnchorPosX (0, 1.5f).SetEase(moveItIn);
			addCategories.DOAnchorPosX (1600, 1.2f).SetEase(moveItOut);
			stepNG = 2;
		} else if (stepNG == 4) {
			addCategories.DOAnchorPosX (0, 1.5f).SetEase(moveItIn);
			addFriendsRect.DOAnchorPosX (1600, 1.2f).SetEase(moveItOut);
			stepNG = 3;
			if (selectCount == 0) {
				FlipSignBackToNornal ();
			} else {
				FlipSignToFront ();
			}

		}
	
	}

	public void NextButtonNG(){

		if (okToClick == false) {
			return;
		}
		okToClick = false;
		Invoke ("OkToClickAgain", 1.25f);

		if (stepNG == 2) {

			if (categoryPrivateHolder.childCount < 1) {
				CreateCategoriesList ();
			}

			gameOptionsScreen.DOAnchorPosX (-1600, 1.2f).SetEase (moveItOut);
			addCategories.DOAnchorPosX (0, 1.5f).SetEase (moveItIn);
			stepNG = 3;

		} else if (stepNG == 3) {

			if (catsSelected.Count < 1 ) {
					return;
			}

			if (selectCount == 0) {
				FlipSignToFrontDone ();
			} else {
				FlipSignToBack ();
			}

			stepNG = 4;
			addCategories.DOAnchorPosX (-1600, 1.2f).SetEase(moveItOut);
			addFriendsRect.DOAnchorPosX (0, 1.5f).SetEase(moveItIn);

			if (inviteHolder.childCount < 1) {
				CreateFriendsList ();
			}

		} else if (stepNG == 4) {

			Debug.Log ("start the game");

		}

	}

	void FlipSignToBack(){
		Vector3 oneEighty = new Vector3 (0, 180, 0);
		nextButtonNG.DORotate (oneEighty, 1.0f).SetEase(signRotate);
		Invoke ("TurnIntoText", .71f);
	}

	void FlipSignToFront(){
		nextButtonNG.DORotate (Vector3.zero, 1.0f).SetEase(signRotate);
		Invoke ("TurnIntoButton", .71f);
	}

	void FlipSignToFrontWithDoneButton(){
		nextButtonNG.DORotate (Vector3.zero, 1.0f).SetEase(signRotate);
		Invoke ("TurnIntoDoneButton", .71f);
	}

	void FlipSignBackToNornal (){
		Vector3 oneEighty = new Vector3 (0, 180, 0);
		nextButtonNG.DORotate (oneEighty, 1.0f).SetEase(signRotate);
		Invoke ("TurnIntoText", .71f);
		Invoke ("FlipSignToFront", 1.0f);
	}

	void FlipSignToFrontDone (){
		Vector3 oneEighty = new Vector3 (0, 180, 0);
		nextButtonNG.DORotate (oneEighty, 1.0f).SetEase(signRotate);
		Invoke ("TurnIntoText", .71f);
		Invoke ("FlipSignToFrontWithDoneButton", 1.0f);
	}

	void FlipSign(){
		Vector3 oneEighty = new Vector3 (0, 180, 0);
		nextButtonNG.DORotate (oneEighty, 1.0f).SetEase(signRotate);
		Invoke ("TurnIntoText", .71f);
	}

	void TurnIntoText (){
		nextButton.SetActive (false);
		sendInvitesButton.SetActive (false);
		nextText.SetActive (true);
	}

	void TurnIntoButton (){
		nextButton.SetActive (true);
		nextText.SetActive (false);
	}

	void TurnIntoDoneButton (){
		sendInvitesButton.SetActive (true);
		nextText.SetActive (false);
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

//	public void FiveRoomsOnly(){
//		tutorialWords.SetActive (false);
//		fiveRooms.SetActive (true);
//		offScreen = frameMessage.transform.position;
//
//		frameMessage.transform.DOLocalMoveY(0,1.0f).SetEase (Ease.OutBounce);
//		Invoke ("MoveOff", 2.5f);
//	}

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

				} else if (item [i].StartsWith (OTHER_SYM)) {

					//Debug.Log ("returned: " + item [i]);

					string other = item[i].Substring (OTHER_SYM.Length);
					string[] otherSplit = other.Split ('$');
					string[] colorSplit = otherSplit [0].Split ('/');
					float r = float.Parse (colorSplit [0]) / 255.0f;
					float g = float.Parse (colorSplit [1]) / 255.0f;
					float b = float.Parse (colorSplit [2]) / 255.0f;

					Color catColor = new Color (r,g,b);
					turnButt.description = otherSplit[1];
					turnButt.catColor = catColor;

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

				GameObject buttonObj = Instantiate(turnButtons[i], turnHolder);
				buttonObj.GetComponent<RectTransform>().localScale = new Vector3 (1,1,1);
				buttonObj.GetComponent<TurnRoomButton> ().SetupButton (i);

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

//	public void StartPrivateGame (){
//	
//		UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();
//		string roomsString = userAccount.activeRooms;
//
//		if (roomsString.Length > 3) {
//			return;
//		}
//
//		roomMan = GameObject.FindGameObjectWithTag ("Room Manager").GetComponent<RoomManager> ();
//		roomMan.StartNewPrivateGame ();
//		roomMan.TurnOnSign ();
//	
//	}

	public void SignOut(){
		GameObject roomManagerObj = GameObject.FindGameObjectWithTag ("Room Manager");
		Destroy (roomManagerObj);
		UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();
		userAccount.activeRooms = string.Empty;
		userAccount.loggedInUsername = string.Empty;

		SceneManager.LoadScene ("Login");

	}

	public void ArtistSelected (string name){

		artistsSelected.Add(name);

		if (selectCount == 0) {
			FlipSignToFrontWithDoneButton ();
			wasAtZeroInvites = true;
			DeactivateSelection (inviteHolder);

		}

	}

	public void ArtistSubtracted (string name){

		artistsSelected.Remove (name);

		if (wasAtZeroInvites == true) {
			wasAtZeroInvites = false;
			ReactivateSelection (inviteHolder);
			FlipSignToBack ();
		}


	}

	public void CatSelected (string name){

		catsSelected.Add(name);
		roundCount.text = catsSelected.Count.ToString ();

	}

	public void UndoCatsSelected(){
	
		catsSelected.Clear ();
		roundCount.text = catsSelected.Count.ToString ();

		int childCount = categoryPrivateHolder.childCount;
		for (int i = 0; i < childCount; i++) {
			SelectPlayerScript selectScript = categoryPrivateHolder.GetChild(i).GetComponent<SelectPlayerScript>();
			selectScript.ResetToZero ();
		}


	}

	public void CatSubtracted (string name){

		//selectCatsText.text = "SELECT " + catsCount + " CATEGORIES";

		catsSelected.Remove (name);

//		if (wasAtZeroCats == true) {
//			wasAtZeroCats = false;
//			ReactivateSelection (categoryPrivateHolder);
//		}

		roundCount.text = catsSelected.Count.ToString ();


	}

	public void LimitToEight(){

		eightLimit.DOScale (Vector3.one, .75f).SetEase (scaleBounce);
		if (eightLimit.localScale.x > .5f) {
			Vector3 punchSize = new Vector3 (.4f, .4f, .4f);
			eightLimit.DOPunchScale (punchSize, 1.0f, 10, .01f).OnComplete (BackToOneScale);
		}

	}

	void BackToOneScale(){
		eightLimit.DOScale (Vector3.one, .1f);
	}

	void DeactivateSelection(Transform theHolder){
	
		int childCount = theHolder.childCount;
		for (int i = 0; i < childCount; i++) {
			SelectPlayerScript selectScript = theHolder.GetChild(i).GetComponent<SelectPlayerScript>();
			if (selectScript.choosen == false) {
				selectScript.TurnOffToggle ();
			}
		}
	}

	void ReactivateSelection(Transform theHolder){

		int childCount = theHolder.childCount;
		for (int i = 0; i < childCount; i++) {
			SelectPlayerScript selectScript = theHolder.GetChild(i).GetComponent<SelectPlayerScript>();
			if (selectScript.choosen == false) {
				selectScript.TurnOnToggle ();
			}
		}
	}

	public void LaunchPrivateGame (){

		RoomManager.instance.CurtainsIn ();
		int roundCount = catsSelected.Count;

		privateForm = new WWWForm ();

//		for (int i = 0; i < roundCount; ++i) {
//			int r = Random.Range(i, roundCount);
//			string tmp = catsSelected[i];
//			catsSelected[i] = catsSelected[r];
//			catsSelected[r] = tmp;
//		}

		for (int i = roundCount - 1; i > 0; i--) {
			int r = Random.Range(0,i);
			string tmp = catsSelected[i];
			catsSelected[i] = catsSelected[r];
			catsSelected[r] = tmp;
		}

		for (int i = 0; i < catsSelected.Count; i++) {
			catsSelected [i] = "v2" + catsSelected [i];
			privateForm.AddField ("catPost[]", catsSelected[i]);
		}

		dupeNums  = new List<int> ();
		playerBag.Clear ();

		for (int i = 0; i < roundCount; i++) {
			int newNum;
			if (playerBag.Count == 0) {
				ResetPlayerBag ();
			}
			int randomNum = Random.Range (0, playerBag.Count);
			newNum = playerBag [randomNum];
			playerBag.RemoveAt (randomNum);
			dupeNums.Add(newNum);
		}

		for (int i = roundCount - 1; i > 0; i--) {
			int r = Random.Range(0,i);
			int tmp = dupeNums[i];
			dupeNums[i] = dupeNums[r];
			dupeNums[r] = tmp;
		}

		fateData = new List<string> ();

		List<int> finalGameOptions;
		finalGameOptions = new List<int>();
	
		for (int i = 0; i < gameOption.Length; i++) {

			if (gameOption[i].isOn == true){
				int toggleNum = int.Parse(gameOption[i].gameObject.name);
				finalGameOptions.Add(toggleNum);
			}

		}

		int[] finalOptions = finalGameOptions.ToArray ();
	
		//gameData = new List<TurnRoomButton>();
		//Debug.Log ("got here 1st");
		for (int r = 0; r < catsSelected.Count; r++) {
			//Debug.Log ("got here");
			for (int t = 0; t < roomMan.buttonHolder.childCount; t++) {
				TurnRoomButton currentButt = roomMan.buttonHolder.GetChild (t).GetComponent<TurnRoomButton> ();
				//Debug.Log (currentButt.roomTypeString + " == " + catsSelected [r]);
				if ("v2" + currentButt.roomTypeString == catsSelected [r]) {
					//Debug.Log ("got here3");
					PrivateRoomMaker (currentButt, finalOptions);
				}

			}

		}

		StartCoroutine(createPrivateRooms());
	}

	void ResetPlayerBag(){
		playerBag = new List<int> ();
		playerBag.Add (1);
		playerBag.Add (2);
		playerBag.Add (3);
		playerBag.Add (4);

	}

	void ResetModeBag(){
		modeBag = new List<int> ();
		modeBag.Add (0);
		modeBag.Add (1);
		modeBag.Add (2);
	}

	void PrivateRoomMaker(TurnRoomButton turnRoom, int[] finalOptions){
		
		int dupeNum = dupeNums[0];
		dupeNums.RemoveAt (0);
		int rightWord = Random.Range (1, 11);
		int wrongWord = Random.Range (1, 11);
		int colorMod = Random.Range (0, 4);

		while(rightWord == wrongWord)
		{
			wrongWord = Random.Range (1, 11);
		}

		int awardNum = Random.Range (1, 3);

		string modeString;
	
		if (finalOptions [2] == 3) {
			if (modeBag.Count == 0) {
				ResetModeBag ();
			}
			int rando = Random.Range (0, 4);
			modeString = modeBag[rando].ToString ();
			modeBag.RemoveAt (rando);
		} else {
			modeString = finalOptions [2].ToString ();
		}

		optionString = "/" + finalOptions [0].ToString () + "/" + finalOptions [1].ToString () + "/" + modeString;


		string fate = "|[WORDS]" + turnRoom.words + "|[BRUSHES]" + turnRoom.brushes + "|" + turnRoom.grounding + "|[FATE]" + dupeNum + "/" + rightWord + "/" + wrongWord + "/" + awardNum + "/" + colorMod + optionString;

		string r = (Mathf.Round(turnRoom.catColor.r * 255)).ToString();
		string g = (Mathf.Round(turnRoom.catColor.g * 255)).ToString();
		string b = (Mathf.Round(turnRoom.catColor.b * 255)).ToString();

		string catColorString = r + "/" + g + "/" + b + "$" + turnRoom.description;

		catColors.Add (catColorString);

		privateForm.AddField ("fatePost[]", fate);
		privateForm.AddField ("catColorPost[]", catColorString);
		//Debug.Log ("fate added");
		fateData.Add (fate);

	}
	 
	//|[WORDS]Marilyn Monroe/Angelina Jolie/Julia Roberts/Scarlett Johansson/Jennifer Lawrence/Kristen Stewart/Oprah/Jennifer Aniston/Kim Kardashian/Betty White|[BRUSHES]0.0, -2.2/0.0, 2.7/1.5, 0.0/-1.5, 0.0/0.0, -1.3|[GROUNDING]-.2|[FATE]1/1/5/1/2

	IEnumerator createPrivateRooms (){

		string playerName = UserAccountManagerScript.instance.loggedInUsername;
		string notificationId = UserAccountManagerScript.instance.notificationId;
		string selfPortrait = UserAccountManagerScript.instance.selfPortrait;
		string URL = "http://dupesite.000webhostapp.com/createPrivateRooms.php";

		//WWWForm form = new WWWForm ();
		privateForm.AddField ("usernamePost", playerName);
		privateForm.AddField ("notIdPost", notificationId);
		privateForm.AddField ("portraitPost", selfPortrait);
		privateForm.AddField ("playerPost2", artistsSelected[0]);
		privateForm.AddField ("playerPost3", artistsSelected[1]);
		privateForm.AddField ("playerPost4", artistsSelected[2]);

		//privateForm.AddField ("roundNumPost", catsSelected.Count);


		//form.AddField ("catPost[]", catsSelected.ToArray);
		//form.AddField ("fatePost[]", fateData.ToArray);

		WWW www = new WWW (URL, privateForm);
		yield return www;


		//RoomManager.instance.CreateRoom (roomType, room.id, room.fate, room.playerNum, -2);

		string returnText = www.text;
		returnText = returnText.TrimStart ('|');
		Debug.Log ("Returned text: " + returnText);


		string[] rooms = returnText.Split ('|');

		//string privateData = UserAccountManagerScript.instance.selfPortrait;

		RoomManager.instance.CreateRoom (catsSelected[0], rooms[0], fateData[0], catColors[0], "1", selfPortrait, -2);
		string message = playerName + "|" + rooms[0] + "$";
		UserAccountManagerScript.instance.SendMessageToPlayersBox (message, artistsSelected.ToArray ());

	}

	IEnumerator getMessageData (){

		string URL = "http://dupesite.000webhostapp.com/lookForNewMessage.php";

		if (userAccount == null) {
			userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();
		}

		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", userAccount.loggedInUsername);

		WWW www = new WWW (URL, form);
		yield return www;

		string returnText = www.text;
		returnText = returnText.TrimEnd ('$');

		Debug.Log ("Message: " + www.text);

		if (returnText != "Error" && returnText.Length>1) {

			string[] messages = returnText.Split ('$');

			for (int i = 0; i < messages.Length; i++) {

				string[] messageParts = messages[i].Split ('|');
				int roomNum = int.Parse (messageParts [1]);
	
				if (invitationNums.Contains (roomNum) == false) {
					CreateInvitation (messageParts [0], roomNum);
					invitationNums.Add (roomNum);
				}
			}
		}
	}

	void CreateInvitation (string message, int roomNum){
		
		GameObject invitation = Instantiate (invitationPrefab, invitationPos.position, Quaternion.identity, invitationHolder);
		invitation.GetComponent<InvitationScript> ().SetupInvitation (message, roomNum, this);

	}

	void HideLobbyButtons(){
		buttonPalette.DOAnchorPosY (paletteStartPos - 800, .5f).SetEase (Ease.OutBack);
	}

	void BringBackLobbyButtons(){
		buttonPalette.DOAnchorPosY (paletteStartPos, .5f).SetEase (moveItIn);
	}

	void BringInBackButtonNG(){
		backButtonNG.DOAnchorPosY(backButtonNGStartPos, 1.0f).SetEase (Ease.OutBounce);
	}

}
