using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DatabaseControl;

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

	private string[] words = new string[12];
	private string[] brushes = new string[10];

	public bool cameFromTurnBased;

	public string[] roomNum = new string[5];
	public string[] playersReady = new string[5];

	GameObject statusHolder;

	void Start (){


	}

	public void CreateRoom(string roomType, string roomId){

		GameObject newRoom = (GameObject)Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
		newRoom.transform.SetParent (gameObject.transform, false);
		TurnRoomScript roomScript = newRoom.GetComponent<TurnRoomScript> ();

		roomScript.activeRoom = true;
		roomScript.roomType = roomType;

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

			}
				

		}

		SceneManager.LoadScene ("Turn Based Room");

	}

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
			if (turnRoom.statusNum == 2) {
			
				status.doneDrawing.SetActive (true);
				status.doneDrawingCheck.SetActive (true);
			
			}
			rooms [i] = turnRoom.roomID;

		}

		Invoke ("StatusUpdate", 3.0f);
			
	}

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

	IEnumerator statusUpdateCheck (string roomID1, string roomID2, string roomID3, string roomID4, string roomID5){

		IEnumerator e = DCP.RunCS ("turnRooms", "UpdateStatus", new string[5] {roomID1,roomID2,roomID3,roomID4,roomID5});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		returnText = returnText.TrimStart ('|');

		Debug.Log ("Returned:" + returnText);

//		string linesWhole = myLineString.Substring (GROUNDING_SYM.Length);
//		string[] lines = linesWhole.Split ('|');
//
//		foreach (string line in lines) {
//
//			GameObject lineGo = Instantiate (blackLine);
//			LineRenderer lineRend = lineGo.GetComponent <LineRenderer> ();
//
//			string[] points = line.Split ('@');
//
//			lineRend.numPositions = points.Length;
//
//			for (int i = 0; i < points.Length; i++) {
//
//				string[] vectArray = points[i].Split(',');
//
//				Vector3 tempVect = new Vector3(
//					float.Parse(vectArray[0]),
//					float.Parse(vectArray[1]),
//					0);
//				lineRend.SetPosition (i, tempVect);
//
//			}


		//need to know non zero roomIDs with column 4 nums
		//|[ID]3
		if (returnText != "Error") {
		
			LookForSets (returnText);

		} 

	}


	void LookForSets (string returned){
	
		roomNum = new string[5];
		playersReady = new string[5];

		string[] roomsDone = returned.Split ('|');

		for (int i = 0; i < roomsDone.Length; i++) {

			string[] roomInfo = returned.Split ('$');


			roomNum[i] = roomInfo[0].Substring (ID_SYM.Length);
			playersReady[i] = roomInfo[1].Substring (PLAYERSREADY_SYM.Length);


		}

		for (int i = 0; i < roomNum.Length; i++) {

			if (playersReady[i] == null) {
				return;
			}


			if (playersReady[i].Contains ("1") && playersReady [i].Contains ("2") && playersReady [i].Contains ("3") && playersReady [i].Contains ("4")) {
			
				UpdateStatusObject (roomNum[i]);
			
			}

		}

	}

	void UpdateStatusObject (string roomId){
	
		int roomInt = int.Parse(roomId);
		if (statusHolder == null) {
			statusHolder = GameObject.FindGameObjectWithTag ("Status Holder");
		}
		int childCount = statusHolder.transform.childCount;
		for (int i = 0; i < childCount; i++) {
			TurnGameStatus turnStat = statusHolder.transform.GetChild (i).GetComponent<TurnGameStatus>();
			if (turnStat.roomId == roomInt) {
			
				turnStat.PhaseTwoReady ();
			
			}
				
		}

	}

}
