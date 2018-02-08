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

	public GameObject refreshingScreen;
	public Text loadScreenWords;

	public GameObject loadingAnimationCenter;
	public bool tutorialMode = false;
	int roomCount;
	public AnimationCurve moveItOut;
	public AnimationCurve moveItIn;

	public Text playersName;
	public Transform inviteHolder;
	public GameObject selectPlayerPrefab;
	public GameObject selectPlayerLoading;
	public GameObject selectCategoriesLoading;
	public Transform categoryPrivateHolder;
	public GameObject categoryPrivatePrefab;

	public Text selectText;
	public int selectCount;
	//public string[] artistsSelected;
	public List<string> artistsSelected;
	bool wasAtZeroInvites = false;

	public Text selectCatsText;
	public int catsCount;
//	public string[] catsSelected;
	public List<string> catsSelected;
	bool wasAtZeroCats = false;

	public Toggle[] gameOption;
	public string optionString;
	public List<TurnRoomButton> gameData;
	public List<string> fateData;
	List<int> playerBag;
	public List<int> invitationNums;

	UserAccountManagerScript userAccount;
	public GameObject invitationPrefab;
	public Transform invitationHolder;
	public RectTransform invitationPos;

	string inviteMessage = " has challenged you to a contest of wits and stupid art!";

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

		selectCount = 3;
		selectText.text = "SELECT " + selectCount + " ARTISTS";
		artistsSelected = new List<string> ();

		catsCount = 4;
		selectCatsText.text = "SELECT " + catsCount + " CATEGORIES";
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
			frameText.GetComponent<Text> ().text = "YOU ART NOT A PART\nOF ANY PAINTINGS";
			loadingAnimationCenter.SetActive (false);
		} else {
			//Debug.Log (roomCount);
			for (int i = 0; i < roomCount; i++) {
				//Debug.Log (roomArray [i]);
				roomMan.UpdateTurnRoomsFromLogin (int.Parse (roomArray [i]));
			}

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
		if (roomCount > 4) {
			FiveRoomsOnly ();
			return;
		}
	
		newCats.DOLocalMoveX (0, 1.5f).SetEase(moveItIn);
		centerTurnButts.DOLocalMoveX (startPos * -1.0f, 1.5f).SetEase(moveItOut);

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
		addFriendsRect.DOAnchorPosY (0, 1.5f).SetEase(moveItIn);
		startNewMain.DOAnchorPosY (-2100, 1.2f).SetEase(moveItOut);

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

	//From Add Cats
	public void GoToGameOptionsScreen() {

		if (okToClick == false) {
			return;
		}

		if (catsSelected.Count < 4 ) {
			return;
		}

		okToClick = false;

		Invoke ("OkToClickAgain", 1.5f);

		addCategories.DOAnchorPosY (-2100, 1.2f).SetEase(moveItOut);
		gameOptionsScreen.DOAnchorPosY (0, 1.5f).SetEase(moveItIn);

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

	void CreateFriendsList(){

		for (int i = 0; i < roomMan.otherPlayers.Count; i++) {

			GameObject playerNameObj = Instantiate (selectPlayerPrefab, inviteHolder);
			playerNameObj.GetComponent<SelectPlayerScript> ().InsertName (roomMan.otherPlayers [i]);

		}

		selectPlayerLoading.SetActive (false);
	
	}

	void CreateCategoriesList(){

		Transform catHolder = GameObject.FindGameObjectWithTag ("Category Holder").transform;

		for (int i = 0; i < catHolder.childCount; i++) {

			string catName = catHolder.GetChild (i).GetComponent<TurnRoomButton> ().roomTypeString;

			GameObject catObjSelect = Instantiate (categoryPrivatePrefab, categoryPrivateHolder);
			catObjSelect.GetComponent<SelectPlayerScript> ().InsertName (catName);

		}

		selectCategoriesLoading.SetActive (false);

	}

	public void BackToStartNewGame(){
	
		addFriendsRect.DOAnchorPosY (2100, 1.2f).SetEase(moveItIn);
		startNewMain.DOAnchorPosY (0, 1.5f).SetEase(moveItIn);
	
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

	public void ArtistSelected (string name){

		selectText.text = "SELECT " + selectCount + " ARTISTS";
		Debug.Log (name + " " + (selectCount));
		artistsSelected.Add(name);

		if (selectCount == 0) {
			wasAtZeroInvites = true;
			DeactivateSelection (inviteHolder);
		}

	}

	public void ArtistSubtracted (string name){

		selectText.text = "SELECT " + selectCount + " ARTISTS";
		artistsSelected.Remove (name);

		if (wasAtZeroInvites == true) {
			wasAtZeroInvites = false;
			ReactivateSelection (inviteHolder);
		}


	}

	public void CatSelected (string name){

		selectCatsText.text = "SELECT " + catsCount + " CATEGORIES";
		catsSelected.Add(name);

		if (catsCount == 0) {
			wasAtZeroCats = true;
			DeactivateSelection (categoryPrivateHolder);
		}

	}

	public void CatSubtracted (string name){

		selectCatsText.text = "SELECT " + catsCount + " CATEGORIES";

		catsSelected.Remove (name);

		if (wasAtZeroCats == true) {
			wasAtZeroCats = false;
			ReactivateSelection (categoryPrivateHolder);
		}


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

		fateData = new List<string> ();

		playerBag = new List<int> ();
		playerBag.Add (1);
		playerBag.Add (2);
		playerBag.Add (3);
		playerBag.Add (4);

		int playerRemove = Random.Range (1, 5);
		int secondaryNum = Random.Range (1, 5);
		int randoNum = Random.Range (0, 2);
		if (randoNum == 0) {
			playerBag.Remove (playerRemove);
			playerBag.Add (secondaryNum);
		}

		int count = playerBag.Count;
		int last = count - 1;
		for (int i = 0; i < last; ++i) {
			int r = Random.Range(i, count);
			int tmp = playerBag[i];
			playerBag[i] = playerBag[r];
			playerBag[r] = tmp;
		}

		List<int> finalGameOptions;
		finalGameOptions = new List<int>();
	
		for (int i = 0; i < gameOption.Length; i++) {

			if (gameOption[i].isOn == true){
				int toggleNum = int.Parse(gameOption[i].gameObject.name);
				finalGameOptions.Add(toggleNum);
			}

		}

		int[] finalOptions = finalGameOptions.ToArray ();
	
		gameData = new List<TurnRoomButton>();

		for (int r = 0; r < catsSelected.Count; r++) {

			for (int t = 0; t < roomMan.buttonHolder.childCount; t++) {

				TurnRoomButton currentButt = roomMan.buttonHolder.GetChild (t).GetComponent<TurnRoomButton> ();
				if (currentButt.roomTypeString == catsSelected [r]) {
					PrivateRoomMaker (currentButt, finalOptions);
				}

			}

		}

		StartCoroutine(createPrivateRooms());
	}

	void PrivateRoomMaker(TurnRoomButton turnRoom, int[] finalOptions){
		
		int dupeNum = playerBag [0];
		playerBag.RemoveAt (0);
		int rightWord = Random.Range (1, 11);
		int wrongWord = Random.Range (1, 11);
		int colorMod = Random.Range (0, 4);

		while(rightWord == wrongWord)
		{
			wrongWord = Random.Range (1, 11);
		}

		int awardNum = Random.Range (1, 3);

		string modeString;
	
		if (finalOptions [2] == 8) {
			int rando = Random.Range (5, 8);
			modeString = rando.ToString ();
		} else {
			modeString = finalOptions [2].ToString ();
		}

		optionString = "/" + finalOptions [0].ToString () + "/" + finalOptions [1].ToString () + "/" + modeString;


		string fate = "|[WORDS]" + turnRoom.words + "|[BRUSHES]" + turnRoom.brushes + "|" + turnRoom.grounding + "|[FATE]" + dupeNum + "/" + rightWord + "/" + wrongWord + "/" + awardNum + "/" + colorMod + optionString;

		fateData.Add (fate);

	}
	 
	//|[WORDS]Marilyn Monroe/Angelina Jolie/Julia Roberts/Scarlett Johansson/Jennifer Lawrence/Kristen Stewart/Oprah/Jennifer Aniston/Kim Kardashian/Betty White|[BRUSHES]0.0, -2.2/0.0, 2.7/1.5, 0.0/-1.5, 0.0/0.0, -1.3|[GROUNDING]-.2|[FATE]1/1/5/1/2

	IEnumerator createPrivateRooms (){

		string playerName = UserAccountManagerScript.instance.loggedInUsername;
		string notificationId = UserAccountManagerScript.instance.notificationId;
		string selfPortrait = UserAccountManagerScript.instance.selfPortrait;
		string URL = "http://dupesite.000webhostapp.com/createPrivateRooms.php";

		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", playerName);
		form.AddField ("notIdPost", notificationId);
		form.AddField ("portraitPost", selfPortrait);
		form.AddField ("playerPost2", artistsSelected[0]);
		form.AddField ("playerPost3", artistsSelected[1]);
		form.AddField ("playerPost4", artistsSelected[2]);

		form.AddField ("catPost1", "v2" + catsSelected[0]);
		form.AddField ("catPost2", "v2" + catsSelected[1]);
		form.AddField ("catPost3", "v2" + catsSelected[2]);
		form.AddField ("catPost4", "v2" + catsSelected[3]);

		form.AddField ("fatePost1", fateData[0]);
		form.AddField ("fatePost2", fateData[1]);
		form.AddField ("fatePost3", fateData[2]);
		form.AddField ("fatePost4", fateData[3]);

		WWW www = new WWW (URL, form);
		yield return www;

		Debug.Log (www.text);
		//RoomManager.instance.CreateRoom (roomType, room.id, room.fate, room.playerNum, -2);

		string[] rooms = www.text.Split ('|');

		string privateData = UserAccountManagerScript.instance.selfPortrait;

		RoomManager.instance.CreateRoom (catsSelected[0], rooms[0], fateData[0], "1", privateData, -2);
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

}
