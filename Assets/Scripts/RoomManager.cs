using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DatabaseControl;
using UnityEngine.UI;
using DG.Tweening;

public class RoomManager : MonoBehaviour {

	public static RoomManager instance;


	void Awake () {

		if (instance != null) {

			Destroy (gameObject);
			return;

		}

		instance = this;
		DontDestroyOnLoad (this);

	}

	public LobbyMenu lobbyMenu; 
	UserAccountManagerScript userAccount;

	public string username;
	public GameObject roomPrefab;
	public GameObject statusPrefab;
	public int[] rooms = new int[5];
	private static string ID_SYM = "[ID]";
	private static string WORDS_SYM = "[WORDS]";
	private static string BRUSHES_SYM = "[BRUSHES]";
	private static string FATE_SYM = "[FATE]";
	private static string COLOR_SYM = "[COLOR]";
	private static string GROUNDING_SYM = "[GROUNDING]";
	//private static string PLAYERSREADY_SYM = "[PLAYERSREADY]";
	private static string CATEGORY_SYM = "[CATEGORY]";
	private static string STATUS_SYM = "[STATUS]";
	private static string DRAWING_SYM = "[DRAWING]";
	private static string PLAYERS_SYM = "[PLAYERS]";
	private static string VOTEPOS_SYM = "[VOTEPOS]";
	private static string DUPEGUESS_SYM = "[DUPEGUESS]";
	private static string CATEGORIES_SYM = "[CATEGORIES]";

	public string[] catsWanted;
	public string[] catsPlaying;

	private string[] words = new string[12];
	private string[] brushes = new string[10];

	public bool cameFromTurnBased;
	public bool cameFromScoring = false;
	public bool startingNew = false;
	public bool cameFromTutorial = false;

	public string[] roomNum = new string[5];
	public string[] playersReady = new string[5];

	GameObject statusHolder;
	GameObject statusLoad;

	public GameObject frameText;

	HighScoreScript highscore;
	public GameObject leftCurtain;
	public GameObject rightCurtain;
	public GameObject centerCurtain;
	public Transform roomHolder;
	public Transform buttonHolder;
	int tempColor;
	string tempID;

	public List<GameObject> categoryButtons;
	public List<string> acceptableStrings;
	public List<string> bestStrings;
	public List<string> removeStrings;

	public List<int> currentRooms;

	int roomTotal;
	public bool buttonsReady = false;
	public bool roomsReady = false;

	GameObject tempRoom;

	public bool refreshing = false;
	public bool noRooms = false;

	public bool tutorialMode;

	string testString;

	public TurnRoomButton[] privateCat;
	private string[] catNames;
	public GameObject sign;
	public Text categoryName;

	void Start (){

		if (tutorialMode == true) {
			return;
		}

		categoryButtons = new List<GameObject>();
		acceptableStrings = new List<string>();
		bestStrings = new List<string>();

		GetRooms ();

		if (lobbyMenu == null) {
			lobbyMenu = GameObject.FindGameObjectWithTag ("Lobby Menu").GetComponent<LobbyMenu> ();
		}

		if (buttonHolder.childCount < 1) {
			lobbyMenu.GetAllCategories ();
		}

		catNames = new string[privateCat.Length];

		for (int i = 0; i < catNames.Length; i++) {

			catNames [i] = privateCat [i].roomTypeString;

		}

	}

	void Update () {

		if (Input.GetKeyDown (KeyCode.R)) {
			
		}
//
//		if (Input.GetKeyDown (KeyCode.S)) {
//			CurtainsOut ();
//		}

	}

	public void GetRooms (){

		if (startingNew == true) {
			return;
		}

		refreshing = true;

		if (statusHolder == null) {
			statusHolder = GameObject.FindGameObjectWithTag ("Status Holder");
		}

		int childCount = roomHolder.transform.childCount;
		int statusCount = statusHolder.transform.childCount;

		if (childCount > 0) {
			for (int i = 0; i < childCount; i++) {
				Destroy(roomHolder.transform.GetChild(i).gameObject);
			}
		}

		if (statusCount > 0) {
			for (int i = 0; i < statusCount; i++) {
				Destroy(statusHolder.transform.GetChild(i).gameObject);
			}
		}

		//roomIds = new string [] { "0", "0", "0", "0", "0"};
		userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();

		string roomsString = userAccount.activeRooms;
		username = userAccount.loggedInUsername;

		//Debug.Log ("roomsString " + roomsString);

		if (roomsString == string.Empty) {

			//Debug.Log ("roomsString2 " + roomsString);

			if (frameText == null) {
			
				frameText = GameObject.FindGameObjectWithTag ("Frame Text");

			}
			noRooms = true;
			frameText.GetComponent<Text> ().text = "You are not apart\nof any paintings";
			GameObject loadAnimation = GameObject.FindGameObjectWithTag ("Load Animation");
			loadAnimation.SetActive (false);
			roomTotal = 0;
			roomsReady = true;
			refreshing = false;
			FindEmptyRooms ();
			return;
		} else {
			noRooms = false;
		}

		roomsString = roomsString.Substring (ID_SYM.Length);
		roomsString = roomsString.TrimEnd ('/');

		roomTotal = roomsString.Split ('/').Length;

		//Debug.Log (roomsString);

		string[] rooms = roomsString.Split ('/');

		for (int i = 0; i < rooms.Length; i++) {

			currentRooms.Add (int.Parse (rooms [i]));
			string roomId = "|[ID]" + rooms[i];
			//Debug.Log ("ROOOOOM: " + roomId);
			//string update = UPDATE_SYM + "Nonupdate";
			// 0 for status update, 1 to create room
			StartCoroutine (getRoomData(roomId, 1));

		}

	}

	void RetryRoomGrab (string roomID, int updateOrNot){

		StartCoroutine (getRoomData(roomID, updateOrNot));

	}

	// 0 for status update, 1 to create room
	IEnumerator getRoomData (string roomID, int updateOrNot){

		IEnumerator e = DCP.RunCS ("turnRooms", "GetRoomData", new string[1] {roomID});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("All Data:" + returnText);

		returnText = returnText.TrimStart ('|');
		returnText = returnText.TrimEnd ('^');

		if (returnText.Length < 4) {
			Debug.Log ("loading error");
			GetRooms ();
			yield break;
		}

		string[] roomSplits = returnText.Split ('|');
		bool matches = false;
		int roomIDSingle = 0;

		foreach (string roomerSplit in roomSplits) {

			if (roomerSplit.StartsWith (ID_SYM)) {

				string retrievedID = roomerSplit.Substring (ID_SYM.Length);
				roomIDSingle = int.Parse(retrievedID);
				retrievedID = "|[ID]" + retrievedID;

				if (retrievedID == roomID) {
					matches = true;
				}

			} 

		}

		if (matches == false) {
		
			RetryRoomGrab (roomID, updateOrNot);
			yield break;
		}


		string[] pieces = returnText.Split ('^');

		for (int i = 0; i < pieces.Length; i++) {

			int goNum = 0;

			if (updateOrNot == 1) {
				CreateRoom ("junk", pieces [i], goNum);
			} else {
				UpdateOldRoom (pieces [i], roomIDSingle);
			}
		}

	}

	void UpdateOldRoom(string roomInfoWhole, int roomIDSingle){
		
		TurnRoomScript myRoom = roomHolder.GetChild (0).GetComponent<TurnRoomScript> ();

		TurnRoomScript[] rooms = roomHolder.GetComponentsInChildren<TurnRoomScript> ();
		foreach (TurnRoomScript room in rooms) {
		
			if (room.roomID == roomIDSingle) {
				myRoom = room;
			}
		
		}

		string[] pieces = roomInfoWhole.Split('|');

		foreach (string piece in pieces) {

			if (piece.StartsWith (PLAYERS_SYM)) {

				string playersWhole = piece.Substring (PLAYERS_SYM.Length);
				string[] players = playersWhole.Split('/');

				myRoom.players = new string[players.Length];
				int colorMod = myRoom.colorMod;

				for (int i = 0; i < players.Length; i++) {

					if (players[i] == username) {
						Debug.Log ("colormod CHECK!: " + myRoom.colorMod.ToString());
						myRoom.myColor = i + 1;
					}
				
				}



				if (players.Length > 3) {
					for (int i = 0; i < players.Length; i++) {

						int playerPos = i - colorMod;

						if (playerPos < 0) {
							playerPos = playerPos + 4;
						}

						myRoom.players [i] = players [playerPos];
					}
				}

			} else if (piece.StartsWith (STATUS_SYM)) {
				
				string status = piece.Substring (STATUS_SYM.Length);

				if (status.Contains ("x")) {

					myRoom.dupeCaught = "x";

				} else if (status.Contains ("o")) {

					myRoom.dupeCaught = "o";

				} else {

					myRoom.dupeCaught = "n";

				}

			}

			else if (piece.StartsWith (DRAWING_SYM)) {

				string drawingString = piece.Substring (DRAWING_SYM.Length);
				//Debug.Log ("Drawing: " + drawingString);

				myRoom.drawings = drawingString;

			}

			else if (piece.StartsWith (VOTEPOS_SYM)) {

				string votePoses = piece.Substring (VOTEPOS_SYM.Length);

				//Debug.Log ("Vote Poses: " + votePoses);

				votePoses = votePoses.TrimEnd ('@');
				myRoom.votePoses = votePoses;

			}

			else if (piece.StartsWith (DUPEGUESS_SYM)) {

				string dupeGuess = piece.Substring (DUPEGUESS_SYM.Length);

				//Debug.Log ("Dupe Guess: " + dupeGuess);

				myRoom.dupeGuess = dupeGuess;

			}
		}



		for (int i = 0; i < statusHolder.transform.childCount; i++) {

			TurnGameStatus tempScript = statusHolder.transform.GetChild (i).GetComponent<TurnGameStatus> ();

			if (tempScript.roomId == roomIDSingle) {

				tempScript.StatusUpdated ();
			}

		}

	}

	public void CreateRoom(string roomType, string roomId, int startRoom){

		GameObject newRoom = (GameObject)Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
		newRoom.transform.SetParent (roomHolder, false);
		TurnRoomScript roomScript = newRoom.GetComponent<TurnRoomScript> ();
		tempRoom = newRoom;

		Debug.Log ("All: " + roomId);

		string[] pieces = roomId.Split('|');

		foreach (string piece in pieces) {

			if (piece.StartsWith (COLOR_SYM)) {

				int color = int.Parse (piece.Substring (COLOR_SYM.Length));

				Debug.Log ("colormod2: " + roomScript.colorMod.ToString());

				roomScript.myColor = color - 4;

			} else if (piece.StartsWith (ID_SYM)) {

				roomScript.roomID = int.Parse(piece.Substring (ID_SYM.Length));

			} else if (piece.StartsWith (WORDS_SYM)) {
				
				string wordsWhole = piece.Substring (WORDS_SYM.Length);

				//Debug.Log (wordsWhole);

				words = wordsWhole.Split('/');

				for (int i = 0; i < words.Length; i++) {

					roomScript.words [i] = words [i];

				}

			} else if (piece.StartsWith (GROUNDING_SYM)) {

				roomScript.grounding = piece.Substring (GROUNDING_SYM.Length);

			} else if (piece.StartsWith (BRUSHES_SYM)) {

				string brushesWhole = piece.Substring (BRUSHES_SYM.Length);

				brushes = brushesWhole.Split ('/');
				roomScript.brushes = new Vector3[brushes.Length];

				for (int i = 0; i < brushes.Length; i++) {

					string[] vectArray = brushes[i].Split(',');
					//Debug.Log (vectArray[0]);

					// store as a Vector2
					Vector3 tempVect = new Vector3(
						float.Parse(vectArray[0]),
						float.Parse(vectArray[1]),
						0);

					roomScript.brushes [i] = tempVect;

				}

			} else if (piece.StartsWith (FATE_SYM)) {

				string fateWhole = piece.Substring (FATE_SYM.Length);

				string[] fate = fateWhole.Split('/');

				roomScript.dupeNum = int.Parse(fate [0]);

				int rightword = int.Parse (fate [1]);
				roomScript.rightword = words [rightword-1];

				int wrongword = int.Parse (fate [2]);
				roomScript.wrongword = words [wrongword-1];

				roomScript.awardNum = int.Parse(fate [3]);

				roomScript.colorMod = int.Parse(fate [4]);
				Debug.Log ("colormod3: " + roomScript.colorMod.ToString());

			} else if (piece.StartsWith (PLAYERS_SYM)) {
				
				string playersWhole = piece.Substring (PLAYERS_SYM.Length);
				Debug.Log (playersWhole + " SDFSDF");
				string[] players = playersWhole.Split('/');

				roomScript.players = new string[players.Length];

				int colorMod = roomScript.colorMod;

				for (int i = 0; i < players.Length; i++) {

					if (players[i] == username) {
						Debug.Log ("colormod: " + roomScript.colorMod.ToString());
						roomScript.myColor = i + 1;
					}

				}

				if (players.Length > 3) {
					for (int i = 0; i < players.Length; i++) {
						        
						int playerPos = i - colorMod;

						if (playerPos < 0) {
							playerPos = playerPos + 4;
						}

						roomScript.players [i] = players [playerPos];
					}
				}
					
			} else if (piece.StartsWith (CATEGORY_SYM)) {

				string category = piece.Substring (CATEGORY_SYM.Length);

				Debug.Log ("It did happen! " + category);

				if (category.StartsWith ("abcde")) {
				
					category = category.Substring (5);
					roomScript.privateRoom = true;
				}

				roomScript.roomType = category;
			}

			else if (piece.StartsWith (STATUS_SYM)) {

				string status = piece.Substring (STATUS_SYM.Length);
				roomScript.status = "waiting...";
				roomScript.statusNum = 0;
				roomScript.statusServer = status;

			}

			else if (piece.StartsWith (DRAWING_SYM)) {

				string drawingString = piece.Substring (DRAWING_SYM.Length);
				//Debug.Log ("Drawing: " + drawingString);

				roomScript.drawings = drawingString;

			}

			else if (piece.StartsWith (VOTEPOS_SYM)) {

				string votePoses = piece.Substring (VOTEPOS_SYM.Length);

				//Debug.Log ("Vote Poses: " + votePoses);

				votePoses = votePoses.TrimEnd ('@');
				roomScript.votePoses = votePoses;

			} else if (piece.StartsWith (DUPEGUESS_SYM)) {

				string dupeGuess = piece.Substring (DUPEGUESS_SYM.Length);

				//Debug.Log ("Dupe Guess: " + dupeGuess);

				roomScript.dupeGuess = dupeGuess;

			}

		}

		int tempColor = roomScript.myColor;
		tempColor = tempColor + roomScript.colorMod;
		if (tempColor > 4) {
			tempColor = tempColor - 4;
		}
		roomScript.myActualColor = tempColor;


		int myColor = roomScript.myActualColor;
		string stringID = roomScript.myActualColor.ToString ();
		string stringIdLetter;

		Debug.Log ("Color: " + roomScript.statusServer + " " + roomScript.myActualColor);

		if (myColor == 1) {
			stringIdLetter = "a";
		} else if (myColor == 2) {
			stringIdLetter = "b";
		} else if (myColor == 3) {
			stringIdLetter = "c";
		} else if (myColor == 4) {
			stringIdLetter = "d";
		} else {
			stringIdLetter = "z";
		}

		if (roomScript.statusServer.Contains ("a") && roomScript.statusServer.Contains ("b") && roomScript.statusServer.Contains ("c") && roomScript.statusServer.Contains ("d")) {
			roomScript.statusNum = 4;
		} else if (roomScript.statusServer.Contains(stringIdLetter)){
			roomScript.statusNum = 3;
		} else if (roomScript.statusServer.Contains ("1") && roomScript.statusServer.Contains ("2") && roomScript.statusServer.Contains ("3") && roomScript.statusServer.Contains ("4")) {
			roomScript.statusNum = 2;
		} else if (roomScript.statusServer.Contains(stringID)){
			roomScript.statusNum = 1;
		}

		if (roomScript.statusServer.Contains ("x")) {
			roomScript.dupeCaught = "x";
		} else if (roomScript.statusServer.Contains ("o")) {
			roomScript.dupeCaught = "o";
		} else {
			roomScript.dupeCaught = "n";
		}


		if (roomType.StartsWith ("abcde")) {

			roomScript.privateRoom = true;

		}

		if (startRoom == -2) {

			int serverSlot = roomScript.myColor + 4;
			Debug.Log ("server Slot" + serverSlot.ToString ());
			string roomIDstring = "|[ID]" + roomScript.roomID.ToString();

			if (username == null) {
				UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();
				username = userAccount.loggedInUsername;
			}

			StartCoroutine (doubleCheckRoom(roomIDstring, serverSlot.ToString(), username, roomType));
			roomScript.activeRoom = true;
			roomScript.roomType = roomType;
			CurtainsIn ();
			//SceneManager.LoadScene ("Turn Based Room");

		} else {
			
			UpdateTurnRoomsFromLogin (roomScript.roomID);
			lobbyMenu = GameObject.FindGameObjectWithTag ("Lobby Menu").GetComponent<LobbyMenu> ();
			lobbyMenu.TurnBasedClicked ();

			//Debug.Log ("room holder: " + roomHolder.childCount.ToString() + " roomTotal: " + roomTotal.ToString());

			if (roomHolder.childCount > roomTotal - 1) {
				roomsReady = true;
				refreshing = false;
				if (lobbyMenu.turnHolder.childCount < 2) {
					FindEmptyRooms ();
				}
					
			}

		}
			
	}
		

	void RetryDoubleCheck (string roomIDstring, string myColor, string usernameToSend, string roomTypeCheck){
	
		StartCoroutine (doubleCheckRoom(roomIDstring, myColor, usernameToSend, roomTypeCheck));

	}

	IEnumerator doubleCheckRoom (string roomIDstring, string myColor, string usernameToSend, string roomTypeCheck ){
	
		Debug.Log ("Submitted room: " + roomTypeCheck + " & " + roomIDstring);

		IEnumerator e = DCP.RunCS ("turnRooms", "DoubleCheckRoom", new string[4] {roomIDstring, myColor, usernameToSend, roomTypeCheck});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("UpdatedRoom:" + returnText);

		if (returnText == "") {
		
			RetryDoubleCheck (roomIDstring, myColor, usernameToSend, roomTypeCheck);
			Debug.Log ("BLLLAAANNNK");
			yield break;
	
		}

		if (returnText == "Good") {

			string roomIDslash = roomIDstring.Substring (ID_SYM.Length + 1) + "/";

			userAccount.StoreRoom(roomIDslash);
			SceneManager.LoadScene ("Turn Based Room");
		
		} else if (returnText == "New Room") {

			Destroy (tempRoom);
			userAccount.RedoRoomSearch ();

		} else {

			Destroy (tempRoom);

			string roomId;
			string[] fates = returnText.Split ('|');
			foreach (string data in fates) {

				if (data.StartsWith ("[ID]")){

					roomId = data.Substring ("[ID]".Length);
					roomId = roomId + "/";

				}

			}

			CreateRoom (roomTypeCheck, returnText, -2);

		}
			
	}

	public void UpdateTurnRoomsFromLogin(int statusRoomId){


		if (statusHolder == null) {
			statusHolder = GameObject.FindGameObjectWithTag ("Status Holder");
		}

		//rooms = new int[5];

		int children = roomHolder.childCount;
		//Debug.Log ("kid count: " + children);


		for (int i = 0; i < children; ++i){
			TurnRoomScript turnRoom = roomHolder.GetChild (i).GetComponent<TurnRoomScript>();

			if (turnRoom.roomID == statusRoomId) {
				


				GameObject roomStatus = Instantiate (statusPrefab);
				roomStatus.transform.SetParent (statusHolder.transform, false);
				//Debug.Log ("Found: " + statusRoomId);
				TurnGameStatus status = roomStatus.GetComponent<TurnGameStatus> ();
				status.roomId = turnRoom.roomID;
				status.categoryName.text = turnRoom.roomType;
				status.gameStatus.text = turnRoom.status;

				if (turnRoom.statusNum == 0) {
					status.PhaseOneReady ();
				} else if (turnRoom.statusNum == 1) {
					status.PhaseOneDone ();
				} else if (turnRoom.statusNum == 2) {
					status.PhaseTwoReady ();
				} else if (turnRoom.statusNum == 3) {
					status.PhaseTwoDone ();
				} else if (turnRoom.statusNum == 4) {
					status.PhaseThreeReady ();
				}

				if (turnRoom.privateRoom == true) {
					int roundNumber = 1;	
					for (int t = 0; t < privateCat.Length; t++) {


						if (privateCat[t].roomTypeString == turnRoom.roomType) {
							roundNumber = t + 1;

						}

					}

					string roundString = "";

					if (roundNumber == 1) {
						roundString = "ROUND ONE";
						turnRoom.roundNum = 1;
					} else if (roundNumber == 2) {
						roundString = "ROUND TWO";
						turnRoom.roundNum = 2;
					} else if (roundNumber == 3) {
						roundString = "ROUND THREE";
						turnRoom.roundNum = 3;
					} else if (roundNumber == 4) {
						roundString = "ROUND FOUR";
						turnRoom.roundNum = 4;
					}

					status.roundNumber.text = roundString;

				}
			} 
		}

		if (statusLoad == null) {
			statusLoad = GameObject.FindGameObjectWithTag ("Status Load");
		}

		statusLoad.SetActive (false);

	}
		
	public void SendTheScore (int pointsToAdd, int playerColor, string roomIDstring, string currentRooms){
	
		string points = pointsToAdd.ToString ();
		//string currentRooms = "[ID]";
		int children = roomHolder.childCount;
		tempColor = playerColor;
		tempID = roomIDstring;

		if (username == null) {
			userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();
			username = userAccount.loggedInUsername;
		}


		StartCoroutine (updateHighScore(points, username, currentRooms));
		StartCoroutine (statusUpdateScoring(roomIDstring, playerColor.ToString()));


	}

	IEnumerator updateHighScore (string points, string username, string currentRooms){

		IEnumerator e = DCP.RunCS ("accounts", "UpdateHighScore", new string[3] {points,username,currentRooms});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		//returnText = returnText.TrimStart ('|');

		Debug.Log ("HighScore List:" + returnText);

		if (returnText.Length < 2) {
			SendTheScore (int.Parse(points), tempColor, tempID, currentRooms);
			yield break;
		}

		if (highscore == null) {

			highscore = GameObject.FindGameObjectWithTag ("High Score").GetComponent<HighScoreScript> ();

		} 

		highscore.TranslateToHighScoreList (returnText);

	}

	IEnumerator statusUpdateScoring (string roomIDstring, string myColor){

	//	Debug.Log ("STUFF: " + roomIDstring + myColor);

		IEnumerator e = DCP.RunCS ("turnRooms", "StatusUpdateScoring", new string[2] {roomIDstring, myColor});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;
		Debug.Log ("UpdatedScoring:" + returnText);

	}

	public void FindEmptyRooms(){

		if (roomsReady == true && buttonsReady == true) {
			StartCoroutine (findEmptyRooms ());
		}
		
	}

	IEnumerator findEmptyRooms (){

		//	Debug.Log ("STUFF: " + roomIDstring + myColor);

		IEnumerator e = DCP.RunCS ("turnRooms", "FindEmptyRooms");

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("Rooms needing:" + returnText);

		if (returnText == "") {
		
			FindEmptyRooms ();
			yield break;
		
		}

		catsWanted = new string[0];

		returnText = returnText.TrimEnd ('|');
		returnText = returnText.Substring (CATEGORIES_SYM.Length);
		catsWanted = returnText.Split('|');

		LoadUpTurnButtons ();

	}

	void LoadUpTurnButtons(){

		Scene scene = SceneManager.GetActiveScene ();
		if (scene.name != "Lobby Menu") {
			return;
		}

		acceptableStrings = new List<string>();
		bestStrings = new List<string>();

		for (int i = 0; i < categoryButtons.Count; i++) {

			string catName = categoryButtons [i].GetComponent<TurnRoomButton> ().roomTypeString;
			acceptableStrings.Add (catName);

		}

		int childCount = roomHolder.childCount;
		//Debug.Log (childCount);

		catsPlaying = new string[childCount];

		for (int i = 0; i < childCount; i++) {

			string catName = roomHolder.GetChild (i).GetComponent<TurnRoomScript> ().roomType;
			catsPlaying[i] = catName;
			//Debug.Log ("Cat Playing: " + catName);

		}

		int acceptCount = acceptableStrings.Count;
		removeStrings = new List<string>();

		for (int i = 0; i < acceptCount; i++) {

			for (int t = 0; t < catsPlaying.Length; t++) {

				if (acceptableStrings [i] == catsPlaying [t]) {

					//Debug.Log (catsPlaying [t]);
					removeStrings.Add (catsPlaying [t]);

				}

			}
				
		}

		int removeCount = removeStrings.Count;

		for (int i = 0; i < removeCount; i++) {
			
			acceptableStrings.Remove (removeStrings [i]);

		}

		int listCount = acceptableStrings.Count;

		for (int i = 0; i < listCount; i++) {

			for (int t = 0; t < catsWanted.Length; t++) {

				if (acceptableStrings [i] == catsWanted [t]) {
				
					bestStrings.Add (catsWanted [t]);

				}

			}
				
		}

		bestStrings.Sort ();
		int bestTotal = bestStrings.Count - 1;

		if (bestTotal > 0) {

			for (int i = 0; i < bestTotal; i++) {

				int num = bestTotal - i;

				if (bestStrings [num] == bestStrings [num - 1]) {
			
					bestStrings.RemoveAt (num);

				}
			}
		}


		for (int i = 0; i < bestStrings.Count; i++) {

			acceptableStrings.Remove (bestStrings[i]);

		}

		int bestCount = bestStrings.Count;
		int stringsNeeded = 3 - bestCount;
		if (stringsNeeded < 1) {
			stringsNeeded = 0;
		}

		for (int i = 0; i < acceptableStrings.Count; i++) {
			string temp = acceptableStrings[i];
			int randomIndex = Random.Range(i, acceptableStrings.Count);
			acceptableStrings[i] = acceptableStrings[randomIndex];
			acceptableStrings[randomIndex] = temp;
		}

		for (int i = 0; i < stringsNeeded; i++) {

			bestStrings.Add (acceptableStrings [i]);

		}

		GameObject[] buttons;
		buttons = new GameObject[3];
		int buttCount = 0;

		for (int i = 0; i < categoryButtons.Count; i++) {

			GameObject turnButton = categoryButtons [i];
			string buttonName = turnButton.GetComponent<TurnRoomButton> ().roomTypeString;
			if (buttonName == bestStrings [0] || buttonName == bestStrings [1] || buttonName == bestStrings [2]) {
				buttons [buttCount] = categoryButtons[i];
				buttCount++;
			}
				
		}

		if (lobbyMenu == null) {
			lobbyMenu = GameObject.FindGameObjectWithTag ("Lobby Menu").GetComponent<LobbyMenu> ();
		}

		if (lobbyMenu != null) {
			lobbyMenu.PlaceOptimalButtons (buttons);
		}
	}

	public void StartingNewRoom(){
	
		startingNew = true;
	
	}

	public void CurtainsIn(){

		rightCurtain.GetComponent<RectTransform> ().DOAnchorPos (Vector2.zero, 1.0f).SetEase (Ease.OutExpo);
		leftCurtain.GetComponent<RectTransform> ().DOAnchorPos (Vector2.zero, 1.0f).SetEase (Ease.OutExpo);
		centerCurtain.GetComponent<RectTransform> ().DOAnchorPos (Vector2.zero, 1.0f).SetEase (Ease.OutExpo);
	
	}

	public void CurtainsOut(){

		Vector2 rightPos = new Vector2 (1300, 0);
		Vector2 leftPos = new Vector2 (-1300, 0);
		Vector2 centerPos = new Vector2 (0, 1400);

		rightCurtain.GetComponent<RectTransform> ().DOAnchorPos (rightPos, 1.0f).SetEase (Ease.InExpo);
		leftCurtain.GetComponent<RectTransform> ().DOAnchorPos (leftPos, 1.0f).SetEase (Ease.InExpo);
		centerCurtain.GetComponent<RectTransform> ().DOAnchorPos (centerPos, 1.0f).SetEase (Ease.InExpo);

	}

	public void TurnOffSign(){
	
		sign.SetActive (false);
	
	}

	public void TurnOnSign(){

		sign.SetActive (true);

	}

	public void UpdateStatus(){
	
		int childCount = roomHolder.childCount;
		
		for (int i = 0; i < childCount; i++) {
		
			int roomID = roomHolder.GetChild (i).gameObject.GetComponent<TurnRoomScript> ().roomID;
			string roomIdString = "|[ID]" + roomID.ToString();
			StartCoroutine (getStatusData(roomIdString));

		}
	
	}

	IEnumerator getStatusData (string roomIdString){

		IEnumerator e = DCP.RunCS ("turnRooms", "UpdateStatus", new string[1] { roomIdString });

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		//Debug.Log (returnText);

		if (returnText.Contains("[ID]")) {
			UpdateStatusObject (returnText);
		}

	}

	void UpdateStatusObject (string statusInfo){

		string[] pieces = statusInfo.Split ('|'); 

		string roomIDstring = pieces [1];
		string status = pieces [0];
		
		roomIDstring = roomIDstring.Substring (ID_SYM.Length);
		int roomID = int.Parse (roomIDstring);

		int myColor = 0;

		int childCount = roomHolder.childCount;
		int statusNum = 1;

		TurnRoomScript roomScript = roomHolder.GetChild (0).GetComponent<TurnRoomScript> ();

		for (int i = 0; i < childCount; i++) {

			TurnRoomScript tempRoomScript = roomHolder.GetChild (i).GetComponent<TurnRoomScript> ();

			if (tempRoomScript.roomID == roomID) {

				myColor = tempRoomScript.myActualColor;
				roomScript = tempRoomScript;

			}

		}

		string stringIdLetter;
		string stringIdNum = myColor.ToString ();

		if (myColor == 1) {
			stringIdLetter = "a";
		} else if (myColor == 2) {
			stringIdLetter = "b";
		} else if (myColor == 3) {
			stringIdLetter = "c";
		} else if (myColor == 4) {
			stringIdLetter = "d";
		} else {
			stringIdLetter = "z";
		}

		//Debug.Log ("letter id: " + stringIdLetter);

		if (status.Contains ("a") && status.Contains ("b") && status.Contains ("c") && status.Contains ("d")) {
			statusNum = 4;
		} else if (status.Contains (stringIdLetter)) {
			statusNum = 3;
		} else if (status.Contains ("1") && status.Contains ("2") && status.Contains ("3") && status.Contains ("4")) {
			statusNum = 2;
		} else if (status.Contains (stringIdNum)) {
			statusNum = 1;
		} else {
			statusNum = 0;
		}

		//Debug.Log (statusNum);

		Invoke ("RefreshingTurnOff", 2.0f);

		if (roomScript.statusNum == statusNum) {
			return;
		}

		UpdateOneRoom (roomIDstring);

		childCount = statusHolder.transform.childCount;

		for (int i = 0; i < childCount; i++) {

			TurnGameStatus tempScript = statusHolder.transform.GetChild (i).GetComponent<TurnGameStatus> ();

			if (tempScript.roomId == roomID) {
		
				tempScript.NewStatus (statusNum);
			}

		}
			
	}

	void RefreshingTurnOff (){
	
		if (lobbyMenu == null) {
			lobbyMenu = GameObject.FindGameObjectWithTag ("Lobby Menu").GetComponent<LobbyMenu> ();
		}

		if (lobbyMenu != null) {
			lobbyMenu.GetComponent<LobbyMenu> ().refreshingScreen.SetActive (false);
		}

	}

// 0 for status update, 1 to create room
	void UpdateOneRoom (string stringID){
		string roomID = "|[ID]" + stringID;
//		string update = UPDATE_SYM + "Update";
		StartCoroutine (getRoomData(roomID,0));

	}

	public void GiveMeDeath(){
		CurtainsOut ();
		//Invoke ("Death",2.0f);
	}

	void Death(){
		//Destroy(gameObject);
	}

	public void StartNewPrivateGame(){
	
		//PlayerPrefs.SetInt (currentRoundLoc, 1);
		categoryName.text = privateCat [0].roomTypeString;
		privateCat [0].TurnRoomClicked ();

	}
		
	public void StartNextPrivateRound(string lastRoom){

		int lastRound = 0;

		for (int i = 0; i < catNames.Length; i++) {
			if (lastRoom == catNames [i]) {
				lastRound = i;
			}
		}

		sign.SetActive (true);
		categoryName.text = privateCat [lastRound + 1].roomTypeString;
		privateCat [lastRound + 1].NextPrivatePainting();

	}

}