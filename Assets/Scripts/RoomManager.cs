using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DatabaseControl;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

[Serializable]
public class RoomCreated
{
	public bool success;
	public string error;
	public string id;
	public string fate;
	public string playerNum;
}

[Serializable]
public class FullRoomData
{
	public bool success;
	public string error;
	public string category;
	public string fate;
	public string player1;
	public string notId1;
	public string portrait1;
	public string status1;
	public string drawing1;
	public string vote1;
	public string player2;
	public string notId2;
	public string portrait2;
	public string status2;
	public string drawing2;
	public string vote2;
	public string player3;
	public string notId3;
	public string portrait3;
	public string status3;
	public string drawing3;
	public string vote3;
	public string player4;
	public string notId4;
	public string portrait4;
	public string status4;
	public string drawing4;
	public string vote4;
	public string caughtEscape;
	public string privateGroup;
	public string score;

}

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
	public bool amIFirstDone = false;
	public bool cameFromScoring = false;
	public bool startingNew = false;
	public bool cameFromTutorial = false;
	public bool cameFromPrivate = false;

	public string[] roomNum = new string[5];
	public string[] playersReady = new string[5];

	GameObject statusHolder;
	GameObject statusLoad;

	public GameObject frameText;

	HighScoreScript highscore;
	public RectTransform leftCurtain;
	public RectTransform rightCurtain;
	public RectTransform centerCurtain;
	public Transform roomHolder;
	public Transform buttonHolder;
	int tempColor;
	string tempID;

	public List<GameObject> categoryButtons;
	public List<string> acceptableStrings;
	public List<string> bestStrings;
	public List<string> removeStrings;

	public List<int> currentRooms;
	public List<string> otherPlayers;

	int roomTotal;
	public bool buttonsReady = false;
	public bool roomsReady = false;
	public bool curtainMoving = false;

	GameObject tempRoom;

	public bool refreshing = false;
	public bool noRooms = false;

	public bool tutorialMode;

	string testString;

	public TurnRoomButton[] privateCat;
	public TurnRoomButton[] afterDarkCats;
	private string[] catNames;
	public GameObject sign;
	public Text categoryName;

	public AnimationCurve bigJitter;

	public bool afterDark = false;

	public int pointsToAddTemp;
	public int playerColorTemp;
	public string roomIDstringTemp;
	public string currentRoomsTemp;

	public bool beenToLobby = false;
	bool refreshingPlayers = false;

	public int nextRoomPrivate;

	void Start (){

		if (tutorialMode == true) {
			return;
		}

		categoryButtons = new List<GameObject>();
		acceptableStrings = new List<string>();
		bestStrings = new List<string>();

		catNames = new string[privateCat.Length];

		for (int i = 0; i < catNames.Length; i++) {

			catNames [i] = privateCat [i].roomTypeString;

		}

	}

	void Update () {

		if (Input.GetKeyDown (KeyCode.R)) {

			CurtainsIn ();
//			Vector2 newStretch = new Vector2 (leftCurtain.sizeDelta.x + 5, leftCurtain.sizeDelta.y);
//			leftCurtain.DOSizeDelta (newStretch, 1.0f).SetEase(bigJitter);
//			leftCurtain.DOShakeRotation (1.0f, 5, 10, 90);
//
//			Vector2 newStretchRight = new Vector2 (rightCurtain.sizeDelta.x + 5, rightCurtain.sizeDelta.y);
//			rightCurtain.DOSizeDelta (newStretchRight, 1.0f).SetEase(bigJitter);
//			rightCurtain.DOShakeRotation (1.0f, 5, 10, 90);

		}

		if (Input.GetKeyDown (KeyCode.S)) {
			CurtainsOut ();
		}

	}

	public void InitialLobbySetup(){
		beenToLobby = true;
		Invoke ("CurtainsOut", .5f);
		lobbyMenu = GameObject.FindGameObjectWithTag ("Lobby Menu").GetComponent<LobbyMenu> ();
		GetRooms ();
		StartCoroutine (getAllPlayerNames());
		if (buttonHolder.childCount < 1) {
			lobbyMenu.GetAllCategories ();
		}
		
	
	}

//	public void AfterDarkMode (){
//	
//		afterDark = true;
//
//		Camera.main.backgroundColor = Color.black;
//
//		catNames = new string[afterDarkCats.Length];
//
//		for (int i = 0; i < catNames.Length; i++) {
//
//			catNames [i] = afterDarkCats [i].roomTypeString;
//
//		}
//
//	}

	public void RefreshPlayerNames(){
		otherPlayers.Clear ();
		refreshingPlayers = true;
		StartCoroutine (getAllPlayerNames());


	}
		
	IEnumerator getAllPlayerNames (){

		string URL = "http://dupesite.000webhostapp.com/getAllPlayerNames.php";

		WWWForm form = new WWWForm ();

		WWW www = new WWW (URL, form);
		yield return www;

		string returnText = www.text;
		returnText = returnText.Replace("\n", "");
		returnText = returnText.TrimEnd ('|');
		//Debug.Log ("Everyone: " + returnText);
		otherPlayers = new List<string>();
		string[] names = returnText.Split ('|');
		for (int i = 0; i < names.Length; i++) {
			otherPlayers.Add(names[i]);
		}
		otherPlayers.Remove (username);
		otherPlayers.Sort ();
		if (refreshingPlayers == true) {
			lobbyMenu = GameObject.FindGameObjectWithTag ("Lobby Menu").GetComponent<LobbyMenu> ();
			lobbyMenu.CreateFriendsList ();
			refreshingPlayers = false;
		}

	}

	public void GetRooms (){

		if (startingNew == true) {
			return;
		}

		refreshing = true;

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
//			frameText.GetComponent<Text> ().text = "YOU ART NOT A PART\nOF ANY PAINTINGS";
//			GameObject loadAnimation = GameObject.FindGameObjectWithTag ("Load Animation");
//			loadAnimation.SetActive (false);
			roomTotal = 0;
			roomsReady = true;
			refreshing = false;
			FindEmptyRooms ();
			return;
		} else {
			noRooms = false;
		}

		roomTotal = roomsString.Split ('/').Length;

		//Debug.Log (roomsString);

		string[] rooms = roomsString.Split ('/');

		for (int i = 0; i < rooms.Length; i++) {

			currentRooms.Add (int.Parse (rooms [i]));
			string roomId = rooms[i];
			StartCoroutine (getRoomData(roomId));

		}

	}
		
	void RetryRoomGrab (string roomID, int updateOrNot){

	//	StartCoroutine (getRoomData(roomID, updateOrNot));

	}

	public void CreateRoomFromInvite (int roomID, string messageToDelete){
	
		StartCoroutine (getRoomData(roomID.ToString()));
		StartCoroutine (deleteMessage(messageToDelete));
	
	}

	IEnumerator deleteMessage (string message){

		string URL = "http://dupesite.000webhostapp.com/deleteOldMessage.php";

		Debug.Log ("To delete: " + message);

		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", username);
		form.AddField ("oldMessagePost", message);

		WWW www = new WWW (URL, form);
		yield return www;

		Debug.Log (www.text);

	}

	public void CreateNextPrivateRoom (){
		string roomID = nextRoomPrivate.ToString ();
		nextRoomPrivate = 0;
		StartCoroutine (getRoomData(roomID));
	}

	// 0 for status update, 1 to create room
	IEnumerator getRoomData (string roomID){

		string URL = "http://dupesite.000webhostapp.com/getAllRoomData.php";

		WWWForm form = new WWWForm ();
		form.AddField ("idPost", roomID);

		WWW www = new WWW (URL, form);
		yield return www;

		FullRoomData room = JsonUtility.FromJson<FullRoomData> (www.text);

		//Debug.Log (room.category);

		CreateRoomFull (room, roomID);
	}

	IEnumerator getRoomDataForUpdate (string roomID){

		string URL = "http://dupesite.000webhostapp.com/getAllRoomData.php";

		WWWForm form = new WWWForm ();
		form.AddField ("idPost", roomID);

		WWW www = new WWW (URL, form);
		yield return www;

		FullRoomData room = JsonUtility.FromJson<FullRoomData> (www.text);

		//Debug.Log (room.category);

		UpdateOldRoom (int.Parse(roomID), room);
		//CreateRoomFull (room, roomID);
	}



//		string returnText = www.text;
//
//
//		returnText = returnText.TrimStart ('|');
//		returnText = returnText.TrimEnd ('^');
//
//		if (returnText.Length < 4) {
//			Debug.Log ("loading error");
//			GetRooms ();
//			yield break;
//		}
//
//		string[] roomSplits = returnText.Split ('|');
//		bool matches = false;
//		int roomIDSingle = 0;
//
//		foreach (string roomerSplit in roomSplits) {
//
//			if (roomerSplit.StartsWith (ID_SYM)) {
//
//				string retrievedID = roomerSplit.Substring (ID_SYM.Length);
//				roomIDSingle = int.Parse(retrievedID);
//				retrievedID = "|[ID]" + retrievedID;
//
//				if (retrievedID == roomID) {
//					matches = true;
//				}
//
//			} 
//
//		}
//
//		if (matches == false) {
//		
//			RetryRoomGrab (roomID, updateOrNot);
//			yield break;
//		}
//
//
//		string[] pieces = returnText.Split ('^');
//
//		for (int i = 0; i < pieces.Length; i++) {
//
//			int goNum = 0;
//
//			if (updateOrNot == 1) {
//				//temp CreateRoom ("junk", pieces [i], goNum);
//			} else {
//				UpdateOldRoom (pieces [i], roomIDSingle);
//			}
//		}

//	}

	void UpdateOldRoom(int roomIDSingle, FullRoomData roomData){
		
		TurnRoomScript roomScript = roomHolder.GetChild (0).GetComponent<TurnRoomScript> ();

		TurnRoomScript[] rooms = roomHolder.GetComponentsInChildren<TurnRoomScript> ();
		foreach (TurnRoomScript room in rooms) {
		
			if (room.roomID == roomIDSingle) {
				roomScript = room;
			}
		
		}
			
		roomScript.roomID = roomIDSingle;
		string newFate = roomData.fate;
		newFate = newFate.TrimStart ('|');
		string[] pieces = newFate.Split('|');

		foreach (string piece in pieces) {

			if (piece.StartsWith (WORDS_SYM)) {
				string wordsWhole = piece.Substring (WORDS_SYM.Length);
				words = wordsWhole.Split ('/');
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

					string[] vectArray = brushes [i].Split (',');
					// store as a Vector2
					Vector3 tempVect = new Vector3 (
						float.Parse (vectArray [0]),
						float.Parse (vectArray [1]),
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

				roomScript.time [int.Parse(fate [5])] = true;
				roomScript.method [int.Parse(fate [6])] = true;
				roomScript.mode [int.Parse(fate [7])] = true;

			}
		}

		if (username == null) {
			UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();
			username = userAccount.loggedInUsername;
		}
		int myColor = 0;
		roomScript.players = new string[4];
		roomScript.players [0] = roomData.player1;
		roomScript.players [1] = roomData.player2;
		roomScript.players [2] = roomData.player3;
		roomScript.players [3] = roomData.player4;
		for (int i = 0; i < 4; i++) {
			if (roomScript.players [i] == username) {
				myColor = i + 1;
			}
		}
		roomScript.playersNotId = new string[4];
		roomScript.playersNotId [0] = roomData.notId1;
		roomScript.playersNotId [1] = roomData.notId2;
		roomScript.playersNotId [2] = roomData.notId3;
		roomScript.playersNotId [3] = roomData.notId4;

		roomScript.portraits = new string[4];
		roomScript.portraits [0] = roomData.portrait1;
		roomScript.portraits [1] = roomData.portrait2;
		roomScript.portraits [2] = roomData.portrait3;
		roomScript.portraits [3] = roomData.portrait4;

		roomScript.myColor = myColor;
		string realCat = roomData.category;
		realCat = realCat.TrimStart('v');
		realCat = realCat.TrimStart('2');
		roomScript.roomType = realCat;
		string newdrawing = roomData.drawing1 + roomData.drawing2 + roomData.drawing3 + roomData.drawing4;
		roomScript.drawings = newdrawing;

		if (roomData.privateGroup == "Public") {
			roomScript.privateRoom = false;
		} else {
			roomScript.privateRoom = true;
			roomScript.nextRoom = int.Parse (roomData.privateGroup);

			string[] scoreData = roomData.score.Split (':');
			string[] scoresString = scoreData[0].Split ('/');
			for (int i = 0; i < scoresString.Length; i++) {
				roomScript.scores [i] = int.Parse (scoresString [i]);
			}
			Debug.Log (scoreData [1]);
			roomScript.roundNum = int.Parse(scoreData [1]);
		}

		roomScript.statusServer = roomData.status1 + roomData.status2 + roomData.status3 + roomData.status4 + roomData.caughtEscape;
		roomScript.dupeCaught = roomData.caughtEscape;

		if (roomScript.dupeNum == 1) {
			roomScript.votePoses =	roomData.vote2 + roomData.vote3 + roomData.vote4;
			roomScript.dupeGuess = roomData.vote1;
		} else if (roomScript.dupeNum == 2) {
			roomScript.votePoses =	roomData.vote1 + roomData.vote3 + roomData.vote4;
			roomScript.dupeGuess = roomData.vote2;
		} else if (roomScript.dupeNum == 3) {
			roomScript.votePoses =	roomData.vote1 + roomData.vote2 + roomData.vote4;
			roomScript.dupeGuess = roomData.vote3;
		} else if (roomScript.dupeNum == 4) {
			roomScript.votePoses =	roomData.vote1 + roomData.vote2 + roomData.vote3;
			roomScript.dupeGuess = roomData.vote4;
		}

		int dupeColor = roomScript.dupeNum;
		dupeColor = dupeColor + roomScript.colorMod;

		if (dupeColor > 4) {
			dupeColor = dupeColor - 4;
		}

		roomScript.dupeColor = dupeColor;

		string stringID = roomScript.myColor.ToString ();
		string stringIdLetter;

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

		//Debug.Log ("status server: " + roomScript.statusServer);
		//Debug.Log ("stringid: " + stringID);

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

		if (roomScript.needsToSendAlert == true) {

			string message;

			if (roomScript.statusNum == 4) {
				message = "It's time to score!";
			} else if (roomScript.statusNum == 2) {
				message = "It's time to vote!";
			} else {
				message = "Come play!";
			}

			SendNotification (message, roomScript);

		}


		for (int i = 0; i < statusHolder.transform.childCount; i++) {

			TurnGameStatus tempScript = statusHolder.transform.GetChild (i).GetComponent<TurnGameStatus> ();

			if (tempScript.roomId == roomIDSingle) {

				tempScript.StatusUpdated (roomScript.statusNum);
			}

		}

	}

	void SendNotification (string message, TurnRoomScript roomScript){
	
		List<string> notIds;
		notIds = new List<string>();

		for (int i = 0; i < 4; i++) {
			if (i != roomScript.myColor - 1 && roomScript.playersNotId[i] != "Nothing") {
				notIds.Add(roomScript.playersNotId[i]);
			}
		}

		if (notIds.Count > 0) {
			Debug.Log ("Send alert to: " + notIds [0] + ". That makes: " + notIds.Count);
			var notification = new Dictionary<string, object> ();
			notification ["contents"] = new Dictionary<string, string> () { { "en", message } };

			if (notIds.Count == 1) {
				notification ["include_player_ids"] = new List<string> () { notIds [0] };
			} else if (notIds.Count == 2) {
				notification ["include_player_ids"] = new List<string> () { notIds [0], notIds [1] };
			} else if (notIds.Count == 3) {
				notification ["include_player_ids"] = new List<string> () { notIds [0], notIds [1], notIds [2] };
			}

			OneSignal.PostNotification (notification, (responseSuccess) => {
				Debug.Log ("Notification posted successful!");
			}, (responseFailure) => {
				Debug.Log ("Notification failed to post");
			});
		} else {
			Debug.Log("No notIds found");
		}

		roomScript.needsToSendAlert = false;
	
	}


	//roomMan.CreateRoom (roomType, room.id, room.fate, room.playerNum, "", -2);
	//RoomManager.instance.CreateRoom (roomType, room.id, room.fate, room.playerNum, -2);
	public void CreateRoom(string roomType, string roomId, string newFate, string myPlayerNum, string privateData, int startRoom){

		//Debug.Log ("New Room: " + roomId);
		GameObject newRoom = (GameObject)Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
		newRoom.transform.SetParent (roomHolder, false);
		TurnRoomScript roomScript = newRoom.GetComponent<TurnRoomScript> ();
		tempRoom = newRoom;

		//Debug.Log ("New room's id: " + roomId);
		roomScript.roomID = int.Parse(roomId);
		newFate = newFate.TrimStart ('|');
		string[] pieces = newFate.Split('|');

		foreach (string piece in pieces) {
		
			if (piece.StartsWith (WORDS_SYM)) {
				string wordsWhole = piece.Substring (WORDS_SYM.Length);
				words = wordsWhole.Split ('/');
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

					string[] vectArray = brushes [i].Split (',');
					// store as a Vector2
					Vector3 tempVect = new Vector3 (
					float.Parse (vectArray [0]),
					float.Parse (vectArray [1]),
					0);
					roomScript.brushes [i] = tempVect;

				}
			} else if (piece.StartsWith (FATE_SYM)) {
				Debug.Log (piece);
				string fateWhole = piece.Substring (FATE_SYM.Length);

				string[] fate = fateWhole.Split('/');

				roomScript.dupeNum = int.Parse(fate [0]);

				int rightword = int.Parse (fate [1]);
				roomScript.rightword = words [rightword-1];

				int wrongword = int.Parse (fate [2]);
				roomScript.wrongword = words [wrongword-1];

				roomScript.awardNum = int.Parse(fate [3]);

				roomScript.colorMod = int.Parse(fate [4]);

				roomScript.time [int.Parse(fate [5])] = true;
				roomScript.method [int.Parse(fate [6])] = true;
				roomScript.mode [int.Parse(fate [7])] = true;

			}
		}
			

		roomScript.myColor = int.Parse(myPlayerNum);
		string realCat = roomType;
		realCat = realCat.TrimStart('v');
		realCat = realCat.TrimStart('2');
		roomScript.roomType = realCat;
		int tempColor = roomScript.myColor;
		tempColor = tempColor + roomScript.colorMod;

		if (tempColor > 4) {
			tempColor = tempColor - 4;
		}

		roomScript.myActualColor = tempColor;
		roomScript.activeRoom = true;

		if (privateData != "") {
			roomScript.portraits = new string[4];
			roomScript.portraits [0] = privateData;
		}

		string serverSlot = myPlayerNum + "player";
			
		if (username == null) {
			UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();
			username = userAccount.loggedInUsername;
		}

		StartCoroutine (doubleCheckRoom(roomId, serverSlot, username, roomType));
			
	}

	void CreateRoomFull (FullRoomData roomData, string roomId){
	
		//Debug.Log ("New Room Full: " + roomId);
		GameObject newRoom = (GameObject)Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
		newRoom.transform.SetParent (roomHolder, false);
		TurnRoomScript roomScript = newRoom.GetComponent<TurnRoomScript> ();
		tempRoom = newRoom;

		//Debug.Log ("New room's id: " + roomId);
		roomScript.roomID = int.Parse(roomId);
		string newFate = roomData.fate;
		newFate = newFate.TrimStart ('|');
		string[] pieces = newFate.Split('|');

		foreach (string piece in pieces) {

			if (piece.StartsWith (WORDS_SYM)) {
				string wordsWhole = piece.Substring (WORDS_SYM.Length);
				words = wordsWhole.Split ('/');
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

					string[] vectArray = brushes [i].Split (',');
					// store as a Vector2
					Vector3 tempVect = new Vector3 (
						float.Parse (vectArray [0]),
						float.Parse (vectArray [1]),
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

				roomScript.time [int.Parse(fate [5])] = true;
				roomScript.method [int.Parse(fate [6])] = true;
				roomScript.mode [int.Parse(fate [7])] = true;

			}
		}



		if (username == null) {
			UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();
			username = userAccount.loggedInUsername;
		}
		int myColor = 0;
		roomScript.players = new string[4];
		roomScript.players [0] = roomData.player1;
		roomScript.players [1] = roomData.player2;
		roomScript.players [2] = roomData.player3;
		roomScript.players [3] = roomData.player4;
		for (int i = 0; i < 4; i++) {
			if (roomScript.players [i] == username) {
				myColor = i + 1;
			}
		}

		roomScript.playersNotId = new string[4];
		roomScript.playersNotId [0] = roomData.notId1;
		roomScript.playersNotId [1] = roomData.notId2;
		roomScript.playersNotId [2] = roomData.notId3;
		roomScript.playersNotId [3] = roomData.notId4;

		roomScript.portraits = new string[4];
		roomScript.portraits [0] = roomData.portrait1;
		roomScript.portraits [1] = roomData.portrait2;
		roomScript.portraits [2] = roomData.portrait3;
		roomScript.portraits [3] = roomData.portrait4;

		roomScript.myColor = myColor;
		string realCat = roomData.category;
		realCat = realCat.TrimStart('v');
		realCat = realCat.TrimStart('2');
		roomScript.roomType = realCat;
		int tempColor = roomScript.myColor;
		tempColor = tempColor + roomScript.colorMod;
		if (tempColor > 4) {
			tempColor = tempColor - 4;
		}
		roomScript.myActualColor = tempColor;

		string newdrawing = roomData.drawing1 + roomData.drawing2 + roomData.drawing3 + roomData.drawing4;
		roomScript.drawings = newdrawing;

		if (roomData.privateGroup == "Public") {
			roomScript.privateRoom = false;
		} else {
			roomScript.privateRoom = true;
			roomScript.nextRoom = int.Parse (roomData.privateGroup);

			string[] scoreData = roomData.score.Split (':');
			string[] scoresString = scoreData[0].Split ('/');
			for (int i = 0; i < scoresString.Length; i++) {
				roomScript.scores [i] = int.Parse (scoresString [i]);
			}
			Debug.Log (scoreData [1]);
			roomScript.roundNum = int.Parse(scoreData [1]);


		}

		roomScript.status = "waiting...";
		roomScript.statusNum = 0;
		roomScript.statusServer = roomData.status1 + roomData.status2 + roomData.status3 + roomData.status4 + roomData.caughtEscape;
		roomScript.dupeCaught = roomData.caughtEscape;

		if (roomScript.dupeNum == 1) {
			roomScript.votePoses =	roomData.vote2 + roomData.vote3 + roomData.vote4;
			roomScript.dupeGuess = roomData.vote1;
		} else if (roomScript.dupeNum == 2) {
			roomScript.votePoses =	roomData.vote1 + roomData.vote3 + roomData.vote4;
			roomScript.dupeGuess = roomData.vote2;
		} else if (roomScript.dupeNum == 3) {
			roomScript.votePoses =	roomData.vote1 + roomData.vote2 + roomData.vote4;
			roomScript.dupeGuess = roomData.vote3;
		} else if (roomScript.dupeNum == 4) {
			roomScript.votePoses =	roomData.vote1 + roomData.vote2 + roomData.vote3;
			roomScript.dupeGuess = roomData.vote4;
		}

		int dupeColor = roomScript.dupeNum;

		dupeColor = dupeColor + roomScript.colorMod;
		if (dupeColor > 4) {
			dupeColor = dupeColor - 4;
		}

		roomScript.dupeColor = dupeColor;

		string stringID = roomScript.myColor.ToString ();
		string stringIdLetter;

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

		//Debug.Log ("status server: " + roomScript.statusServer + " & my num is " + stringID);

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

		UpdateTurnRoomsFromLogin (roomScript.roomID);
		lobbyMenu = GameObject.FindGameObjectWithTag ("Lobby Menu").GetComponent<LobbyMenu> ();

		if (roomHolder.childCount > roomTotal - 1) {
			roomsReady = true;
			refreshing = false;
			if (lobbyMenu.turnHolder.childCount < 2) {
				FindEmptyRooms ();
			}
				
		}

	}
		

	void RetryDoubleCheck (string roomIDstring, string myColor, string usernameToSend, string roomTypeCheck){
	
		StartCoroutine (doubleCheckRoom(roomIDstring, myColor, usernameToSend, roomTypeCheck));

	}

	IEnumerator doubleCheckRoom (string roomIDstring, string myColor, string usernameToSend, string roomTypeCheck ){

		string URL = "http://dupesite.000webhostapp.com/doubleCheckRoom.php";

		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", usernameToSend);
		form.AddField ("playerNumPost", myColor);
		form.AddField ("roomIdPost", roomIDstring);
		form.AddField ("categoryPost", roomTypeCheck);

		WWW www = new WWW (URL, form);
		yield return www;

		string returnText = www.text;

		if (returnText == "Good") {
			userAccount.StoreRoom(roomIDstring);
			Invoke("DelayedNewRoom", 1.0f);

		} 
			
	}

	void DelayedNewRoom(){
		
		noRooms = false;
		SceneManager.LoadScene ("Turn Based Room");
	
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

				//Debug.Log ("status num creation: " + turnRoom.statusNum);

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
					status.roundNumber.text = "ROUND " + turnRoom.roundNum.ToString();

				} else {
					
					status.roundNumber.text = "";
					//status.MakeItSolo();
				
				}
			} 
		}

		if (statusLoad == null) {
			statusLoad = GameObject.FindGameObjectWithTag ("Status Load");
		}

		statusLoad.SetActive (false);

	}
		
	public void SendTheScore (int pointsToAdd, int playerColor, string roomIDstring, string currentRooms){
	
//		Debug.Log ("Stuff: " + pointsToAdd + " " + playerColor + " " + currentRooms);

		pointsToAddTemp = pointsToAdd;
		playerColorTemp = playerColor;
		roomIDstringTemp = roomIDstring;
		currentRoomsTemp = currentRooms;

		//string points = pointsToAdd.ToString ();
		//string currentRooms = "[ID]";
		//int children = roomHolder.childCount;
		//tempColor = playerColor;
		//tempID = roomIDstring;

//		if (username == null) {
//			userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();
//			username = userAccount.loggedInUsername;
//		}


//		StartCoroutine (updateHighScore());
//		StartCoroutine (statusUpdateScoring(roomIDstring, playerColor.ToString()));


	}

	public void SendTheScoreToServer (){
	
		StartCoroutine (updateHighScore ());

	}

	IEnumerator updateHighScore (){

		string URL = "http://dupesite.000webhostapp.com/updateHighScore.php";

		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", username);
		form.AddField ("pointsPost", pointsToAddTemp);
		form.AddField ("currentRoomsPost", currentRoomsTemp);
		form.AddField ("playerColorPost", playerColorTemp);
		form.AddField ("idPost", roomIDstringTemp);

		WWW www = new WWW (URL, form);
		yield return www;

		string returnText = www.text;

		Debug.Log (returnText);


		if (highscore == null) {

			highscore = GameObject.FindGameObjectWithTag ("High Score").GetComponent<HighScoreScript> ();

		} 

		highscore.TranslateToHighScoreList (returnText);


//		IEnumerator e = DCP.RunCS ("accounts", "UpdateHighScore", new string[3] {points,username,currentRooms});
//
//		while (e.MoveNext ()) {
//			yield return e.Current;
//		}
//
//		string returnText = e.Current as string;

		//returnText = returnText.TrimStart ('|');

		Debug.Log ("HighScore List:" + returnText);

//		if (returnText.Length < 2) {
//			SendTheScore (int.Parse(points), tempColor, tempID, currentRooms);
//			yield break;
//		}



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

		string URL = "http://dupesite.000webhostapp.com/findEmpties.php";

		WWW www = new WWW (URL);
		yield return www;

		string returnText = www.text;
//
//		Debug.Log (returnText);


		//	Debug.Log ("STUFF: " + roomIDstring + myColor);

//		IEnumerator e = DCP.RunCS ("turnRooms", "FindEmptyRooms");
//
//		while (e.MoveNext ()) {
//			yield return e.Current;
//		}
//
//		string returnText = e.Current as string;
//
//		Debug.Log ("Rooms needing:" + returnText);
//
//		if (returnText == "") {
//		
//			FindEmptyRooms ();
//			yield break;
//		
//		}

		catsWanted = new string[0];
		returnText = returnText.Replace("\n", "");
		returnText = returnText.TrimEnd ('|');
		//returnText = returnText.Substring (CATEGORIES_SYM.Length);
		catsWanted = returnText.Split('|');

		Debug.Log (returnText);

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

				string testableCat = "v2" + acceptableStrings [i];

				if (testableCat == catsWanted [t]) {
				
					bestStrings.Add (acceptableStrings [i]);

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
				//Debug.Log ("ButtonCount: " + buttCount + " I Count: " + i);
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
		//Debug.Log ("CURTAINS IN");

		Invoke ("ShakeCurtains", .5f);
		rightCurtain.DOAnchorPos (Vector2.zero, .7f).SetEase (Ease.InQuart);
		leftCurtain.DOAnchorPos (Vector2.zero, .7f).SetEase (Ease.InQuart);
		centerCurtain.DOAnchorPos (Vector2.zero, .7f).SetEase (Ease.InQuart);
		curtainMoving = true;
	
	}

	public void CurtainsOut(){

		Vector2 rightPos = new Vector2 (950, 0);
		Vector2 leftPos = new Vector2 (-950, 0);
		Vector2 centerPos = new Vector2 (0, 1400);

		rightCurtain.DOAnchorPos (rightPos, .75f).SetEase (Ease.InExpo);
		leftCurtain.DOAnchorPos (leftPos, .75f).SetEase (Ease.InExpo);
		centerCurtain.DOAnchorPos (centerPos, .75f).SetEase (Ease.InExpo).OnComplete(BackToNormalCurtains);
		curtainMoving = false;

	}

	void ShakeCurtains() {

		//Debug.Log ("SHAKIN IN");

		Vector2 newStretch = new Vector2 (leftCurtain.sizeDelta.x + 2, leftCurtain.sizeDelta.y);
		leftCurtain.DOSizeDelta (newStretch, 1.0f).SetEase(bigJitter);
		//leftCurtain.DOShakeRotation (1.0f, 5, 10, 90);

		Vector2 newStretchRight = new Vector2 (rightCurtain.sizeDelta.x + 2, rightCurtain.sizeDelta.y);
		rightCurtain.DOSizeDelta (newStretchRight, 1.0f).SetEase(bigJitter);
		//rightCurtain.DOShakeRotation (1.0f, 5, 10, 90);

		Vector2 newStretchCenter = new Vector2 (centerCurtain.sizeDelta.x, centerCurtain.sizeDelta.y +1);
		centerCurtain.DOSizeDelta (newStretchCenter, .8f).SetEase(bigJitter);

	}

	void BackToNormalCurtains(){

		Vector2 oldStretch = new Vector2 (leftCurtain.sizeDelta.x - 2, leftCurtain.sizeDelta.y);
		Vector2 oldStretchRight = new Vector2 (rightCurtain.sizeDelta.x - 2, rightCurtain.sizeDelta.y);

		leftCurtain.sizeDelta = oldStretch;
		rightCurtain.sizeDelta = oldStretchRight;
	
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
			string roomIdString = roomID.ToString();
			StartCoroutine (getStatusData(roomIdString));

		}

	}
		

	IEnumerator getStatusData (string roomIdString){

		string URL = "http://dupesite.000webhostapp.com/getStatusReport.php";

		WWWForm form = new WWWForm ();
		form.AddField ("idPost", roomIdString);

		WWW www = new WWW (URL, form);
		yield return www;

		//Debug.Log (roomIdString + ": " + www.text);

		UpdateStatusObject (www.text, roomIdString);

	}

	void UpdateStatusObject (string statusInfo, string roomIDString){

		//string[] pieces = statusInfo.Split ('|'); 

		int roomID = int.Parse (roomIDString);

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

		if (statusInfo.Contains ("a") && statusInfo.Contains ("b") && statusInfo.Contains ("c") && statusInfo.Contains ("d")) {
			statusNum = 4;
		} else if (statusInfo.Contains (stringIdLetter)) {
			statusNum = 3;
		} else if (statusInfo.Contains ("1") && statusInfo.Contains ("2") && statusInfo.Contains ("3") && statusInfo.Contains ("4")) {
			statusNum = 2;
		} else if (statusInfo.Contains (stringIdNum)) {
			statusNum = 1;
		} else {
			statusNum = 0;
		}

		//Debug.Log (statusNum);

		Invoke ("RefreshingTurnOff", 1.0f);

		if (roomScript.statusNum == statusNum) {
			return;
		}

		UpdateOneRoom (roomIDString);

		if (statusHolder == null) {
			statusHolder = GameObject.FindGameObjectWithTag ("Status Holder");
		}

		childCount = statusHolder.transform.childCount;

		for (int i = 0; i < childCount; i++) {

			TurnGameStatus tempScript = statusHolder.transform.GetChild (i).GetComponent<TurnGameStatus> ();

			if (tempScript.roomId == roomID) {
		
				tempScript.NewStatus (statusNum);
			}

		}

		if (amIFirstDone == true) {

			roomScript.needsToSendAlert = true;
			amIFirstDone = false;
		
		}
			
	}

	void RefreshingTurnOff (){
	
		Scene scene = SceneManager.GetActiveScene ();
		if (scene.name != "Lobby Menu") {
			return;
		}

		if (lobbyMenu == null) {
			lobbyMenu = GameObject.FindGameObjectWithTag ("Lobby Menu").GetComponent<LobbyMenu> ();
		}

		if (lobbyMenu != null) {
			lobbyMenu.GetComponent<LobbyMenu> ().refreshingScreen.SetActive (false);
		}

	}

// 0 for status update, 1 to create room
	void UpdateOneRoom (string stringID){
		
		StartCoroutine (getRoomDataForUpdate(stringID));

	}

	public void GiveMeDeath(){
		CurtainsOut ();
		//Invoke ("Death",2.0f);
	}

	void Death(){
		//Destroy(gameObject);
	}

//	public void StartNewPrivateGame(){
//	
//		//PlayerPrefs.SetInt (currentRoundLoc, 1);
//		if (afterDark == false) {
//			categoryName.text = privateCat [0].roomTypeString;
//			privateCat [0].TurnRoomClicked ();
//		} else {
//			categoryName.text = afterDarkCats [0].roomTypeString;
//			afterDarkCats [0].TurnRoomClicked ();
//
//		}
//
//	}
		
//	public void StartNextPrivateRound(string lastRoom){
//
//		int lastRound = 0;
//
//		for (int i = 0; i < catNames.Length; i++) {
//			if (lastRoom == catNames [i]) {
//				lastRound = i;
//			}
//		}
//
//		sign.SetActive (true);
//
//		if (afterDark == false) {
//			categoryName.text = privateCat [lastRound + 1].roomTypeString;
//			privateCat [lastRound + 1].NextPrivatePainting ();
//		} else {
//			categoryName.text = afterDarkCats [lastRound + 1].roomTypeString;
//			afterDarkCats [lastRound + 1].NextPrivatePainting ();
//		}
//	}

}

//how to tell if you were last one drawing...