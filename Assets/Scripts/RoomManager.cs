using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
	private static string ID_SYM = "[ID]";
	private static string WORDS_SYM = "[WORDS]";
	private static string BRUSHES_SYM = "[BRUSHES]";
	private static string FATE_SYM = "[FATE]";
	private static string COLOR_SYM = "[COLOR]";
	private static string GROUNDING_SYM = "[GROUNDING]";

	private string[] words = new string[12];
	private string[] brushes = new string[10];

	public bool cameFromTurnBased;

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
				roomScript.rightword = words [rightword];

				int wrongword = int.Parse (fate [2]);
				roomScript.wrongword = words [wrongword];

				roomScript.awardNum = int.Parse(fate [3]);

			}
				

		}

		SceneManager.LoadScene ("Turn Based Room");

	}

	public void UpdateTurnRooms(){
		cameFromTurnBased = false;
		GameObject statusHolder = GameObject.FindGameObjectWithTag ("Status Holder");

		int children = transform.childCount;
		for (int i = 0; i < children; ++i){
			GameObject roomStatus = Instantiate (statusPrefab);
			roomStatus.transform.SetParent (statusHolder.transform, false);
			TurnRoomScript turnRoom = transform.GetChild (i).GetComponent<TurnRoomScript>();
			TurnGameStatus status = roomStatus.GetComponent<TurnGameStatus> ();
			status.categoryName.text = turnRoom.roomType;
			status.gameStatus.text = turnRoom.status;
			if (turnRoom.statusNum == 2) {
			
				status.doneDrawing.SetActive (true);
				status.doneDrawingCheck.SetActive (true);
			
			}

		}
			
	}

}
