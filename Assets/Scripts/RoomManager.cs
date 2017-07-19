using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DatabaseControl;
using UnityEngine.UI;

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

	string[]roomIds = new string[5];

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
	private static string PLAYERSREADY_SYM = "[PLAYERSREADY]";
	private static string CATEGORY_SYM = "[CATEGORY]";
	private static string STATUS_SYM = "[STATUS]";
	private static string DRAWING_SYM = "[DRAWING]";
	private static string PLAYERS_SYM = "[PLAYERS]";
	private static string VOTEPOS_SYM = "[VOTEPOS]";
	private static string DUPEGUESS_SYM = "[DUPEGUESS]";

	private string[] words = new string[12];
	private string[] brushes = new string[10];

	public bool cameFromTurnBased;

	public string[] roomNum = new string[5];
	public string[] playersReady = new string[5];

	GameObject statusHolder;
	GameObject statusLoad;

	public Text frameText;

	void Start (){

		GetRooms ();
		//StartCoroutine (getRoomData(room));

	}

	void Update () {

		if (Input.GetKeyDown (KeyCode.J)) {
			UpdateTurnRooms ();
		}

	}

	void GetRooms (){
	
		//roomIds = new string [] { "0", "0", "0", "0", "0"};
		UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();

		string roomsString = userAccount.activeRooms;
		username = userAccount.loggedInUsername;

		//Debug.Log (roomsString);

		if (roomsString == "") {

			frameText.text = "Nothin happenin";
			return;
		}

		roomsString = roomsString.Substring (ID_SYM.Length);
		roomsString = roomsString.TrimEnd ('/');

		Debug.Log (roomsString);

		string[] rooms = roomsString.Split ('/');



		for (int i = 0; i < rooms.Length; i++) {

			string roomId = "|[ID]" + rooms[i];
			Debug.Log ("ROOOOOM: " + roomId);
			StartCoroutine (getRoomData(roomId));

		}

	}

	IEnumerator getRoomData (string roomID){

		IEnumerator e = DCP.RunCS ("turnRooms", "GetRoomData", new string[1] {roomID});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		returnText = returnText.TrimStart ('|');
		returnText = returnText.TrimEnd ('^');

		Debug.Log ("All Data:" + returnText);

		string[] pieces = returnText.Split ('^');


		for (int i = 0; i < pieces.Length; i++) {

			int goNum = -1;

			if (pieces.Length - 1 == i){
				goNum = 1;
			}

			CreateRoom ("junk", pieces[i], goNum);

		}

	}


	public void CreateRoom(string roomType, string roomId, int startRoom){

		GameObject newRoom = (GameObject)Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
		newRoom.transform.SetParent (gameObject.transform, false);
		TurnRoomScript roomScript = newRoom.GetComponent<TurnRoomScript> ();

		Debug.Log ("StartRoom" + startRoom);

		if (startRoom == 0) {
			
			roomScript.activeRoom = true;
			roomScript.roomType = roomType;

		}

		Debug.Log ("Room ID: " + roomId);

		string[] pieces = roomId.Split('|');

		foreach (string piece in pieces) {

			if (piece.StartsWith (COLOR_SYM)) {

				int color = int.Parse (piece.Substring (COLOR_SYM.Length));

				roomScript.myColor = color - 4;

			} else if (piece.StartsWith (ID_SYM)) {

				roomScript.roomID = int.Parse(piece.Substring (ID_SYM.Length));


			} else if (piece.StartsWith (WORDS_SYM)) {
				
				string wordsWhole = piece.Substring (WORDS_SYM.Length);

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

			} else if (piece.StartsWith (PLAYERS_SYM)) {

				string playersWhole = piece.Substring (PLAYERS_SYM.Length);
				string[] players = playersWhole.Split('/');

				roomScript.players = new string[players.Length];

				for (int i = 0; i < players.Length; i++) {

					Debug.Log (username + players [i]);
					roomScript.players [i] = players [i];

					if (players[i] == username) {
						roomScript.myColor = i + 1;
					}

				}
					
			} else if (piece.StartsWith (CATEGORY_SYM)) {

				string category = piece.Substring (CATEGORY_SYM.Length);
				roomScript.roomType = category;
			}

			else if (piece.StartsWith (STATUS_SYM)) {

				int myColor = roomScript.myColor;

				string stringID = roomScript.myColor.ToString ();

				string status = piece.Substring (STATUS_SYM.Length);

				roomScript.status = "waiting...";

				roomScript.statusNum = 0;

				Debug.Log ("Status: " + status + ", StringID: " + stringID);

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

				//Debug.Log ("letter id: " + stringIdLetter);
					
				if (status.Contains ("a") && status.Contains ("b") && status.Contains ("c") && status.Contains ("d")) {
					roomScript.statusNum = 4;
				} else if (status.Contains(stringIdLetter)){
					roomScript.statusNum = 3;
				} else if (status.Contains ("1") && status.Contains ("2") && status.Contains ("3") && status.Contains ("4")) {
					roomScript.statusNum = 2;
				} else if (status.Contains(stringID)){
					roomScript.statusNum = 1;
				}

				if (status.Contains ("x")) {

					roomScript.dupeCaught = "x";

				} else if (status.Contains ("o")) {

					roomScript.dupeCaught = "o";

				} else {
				
					roomScript.dupeCaught = "n";

				}
		
			}

			else if (piece.StartsWith (DRAWING_SYM)) {

				string drawingString = piece.Substring (DRAWING_SYM.Length);
				Debug.Log ("Drawing: " + drawingString);

				roomScript.drawings = drawingString;

			}

			else if (piece.StartsWith (VOTEPOS_SYM)) {

				string votePoses = piece.Substring (VOTEPOS_SYM.Length);

				Debug.Log ("Vote Poses: " + votePoses);

				votePoses = votePoses.TrimEnd ('@');
				roomScript.votePoses = votePoses;

			} else if (piece.StartsWith (DUPEGUESS_SYM)) {

				string dupeGuess = piece.Substring (DUPEGUESS_SYM.Length);

				Debug.Log ("Dupe Guess: " + dupeGuess);

				roomScript.dupeGuess = dupeGuess;

			}
				
		}

		if (startRoom == 0) {
			SceneManager.LoadScene ("Turn Based Room");
		} else if (startRoom == 1) {
			UpdateTurnRoomsFromLogin (roomScript.roomID);
			lobbyMenu.TurnBasedClicked ();

		}

	}
		

	public void UpdateTurnRoomsFromLogin(int statusRoomId){



		if (statusHolder == null) {
			statusHolder = GameObject.FindGameObjectWithTag ("Status Holder");
		}
		rooms = new int[5];

		int children = transform.childCount;
		//Debug.Log (children);


		for (int i = 0; i < children; ++i){
			TurnRoomScript turnRoom = transform.GetChild (i).GetComponent<TurnRoomScript>();

			if (turnRoom.roomID == statusRoomId) {
			
				GameObject roomStatus = Instantiate (statusPrefab);
				roomStatus.transform.SetParent (statusHolder.transform, false);
				Debug.Log (turnRoom.votePoses);
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
			} 
		}

		if (statusLoad == null) {
			statusLoad = GameObject.FindGameObjectWithTag ("Status Load");
		}

		statusLoad.SetActive (false);

	}

	//Instantiate status objs, match their status to rooms, create array of room Id #s
	public void UpdateTurnRooms(){
		cameFromTurnBased = false;
		if (statusHolder == null) {
			statusHolder = GameObject.FindGameObjectWithTag ("Status Holder");
		}
		rooms = new int[5];

		int children = transform.childCount;
		for (int i = 0; i < children; ++i){
			GameObject roomStatus = Instantiate (statusPrefab);
			roomStatus.transform.SetParent (statusHolder.transform, false);
			TurnRoomScript turnRoom = transform.GetChild (i).GetComponent<TurnRoomScript>();
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

			rooms [i] = turnRoom.roomID;
		}

		Invoke ("StatusUpdate", 1.0f);

	}

	//send room IDs to server
	public void StatusUpdate(){

		string roomID1;
		string roomID2;
		string roomID3;
		string roomID4;
		string roomID5;

		roomID1 = "|[ID]" + rooms [0].ToString();
		roomID2 = "|[ID]" + rooms [1].ToString();
		roomID3 = "|[ID]" + rooms [2].ToString();
		roomID4 = "|[ID]" + rooms [3].ToString();
		roomID5 = "|[ID]" + rooms [4].ToString();

		Debug.Log ("ROOM 1: " + roomID1);

		StartCoroutine (statusUpdateCheck(roomID1,roomID2, roomID3, roomID4, roomID5));
	
	}

	//returns ids with players checked in and drawings.
	IEnumerator statusUpdateCheck (string roomID1, string roomID2, string roomID3, string roomID4, string roomID5){

		IEnumerator e = DCP.RunCS ("turnRooms", "UpdateStatus", new string[5] {roomID1,roomID2,roomID3,roomID4,roomID5});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		returnText = returnText.TrimStart ('|');

		Debug.Log ("Returned Status:" + returnText);

		if (returnText != "Error") {
		
			BreakDownStatus (returnText);

		} 

	}

	//checks status
	void BreakDownStatus (string returned){
	
		string[] roomsData = returned.Split ('|');

		foreach (string roomData in roomsData) {

			string[] roomPiece = roomData.Split('/');

			string idString = roomPiece[0].Substring (ID_SYM.Length);
			string playersReadyString = roomPiece[1].Substring (PLAYERSREADY_SYM.Length);
			string drawingString = roomPiece [2];
			drawingString = drawingString.TrimEnd('$');

			UpdateRoom (idString, playersReadyString, drawingString);

		}

		Invoke ("UpdateStatusObjects", 1.0f);

	}

	void UpdateRoom (string roomId, string status, string drawing){
	
		int roomInt = int.Parse(roomId);
		int children = transform.childCount;
		string myColorNumNow;


		for (int i = 0; i < children; ++i){
			
			TurnRoomScript turnRoom = transform.GetChild (i).GetComponent<TurnRoomScript>();

			if (turnRoom.roomID == roomInt) {
				turnRoom.drawings = drawing;
				myColorNumNow = turnRoom.myColor.ToString ();

				string stringIdLetter;

				if (turnRoom.myColor == 1) {
					stringIdLetter = "a";
				} else if (turnRoom.myColor == 2) {
					stringIdLetter = "b";
				} else if (turnRoom.myColor == 3) {
					stringIdLetter = "c";
				} else if (turnRoom.myColor == 4) {
					stringIdLetter = "d";
				} else {
					stringIdLetter = "z";
				}

				Debug.Log ("myID: " + stringIdLetter + " myColor: " + myColorNumNow + " status: " + status + " RoomId: " + roomInt);


				if (status.Contains ("a") && status.Contains ("b") && status.Contains ("c") && status.Contains ("d")) {

					turnRoom.statusNum = 4;

				} else if (status.Contains(stringIdLetter)){

					Debug.Log ("MAKE IT 3");
					turnRoom.statusNum = 3;

				} else if (status.Contains ("1") && status.Contains ("2") && status.Contains ("3") && status.Contains ("4")) {
					
					Debug.Log ("MAKE IT 2");
					turnRoom.statusNum = 2;

				} else if (status.Contains(myColorNumNow)){

					turnRoom.statusNum = 1;

				}

			}

		}

		UpdateStatusObjects ();
	
	}

	//Updates status objects to their room's status
	void UpdateStatusObjects (){

		if (statusHolder == null) {
			statusHolder = GameObject.FindGameObjectWithTag ("Status Holder");
		}

		for (int i = 0; i < statusHolder.transform.childCount; ++i){

			Destroy (statusHolder.transform.GetChild (i).gameObject);

		}
			
		int children = transform.childCount;

		for (int i = 0; i < children; ++i){
			
			TurnRoomScript turnRoom = transform.GetChild (i).GetComponent<TurnRoomScript>();
			GameObject roomStatus = Instantiate (statusPrefab);
			roomStatus.transform.SetParent (statusHolder.transform, false);
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

		}

		if (statusLoad == null) {
			statusLoad = GameObject.FindGameObjectWithTag ("Status Load");
		}

		statusLoad.SetActive (false);

	}
}
