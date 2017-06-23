using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	private static string ID_SYM = "[ID]";
	private static string WORDS_SYM = "[WORDS]";
	private static string BRUSHES_SYM = "[BRUSHES]";
	private static string FATE_SYM = "[FATE]";

	private string[] words = new string[12];
	private string[] brushes = new string[10];

	public void CreateRoom(string roomType, string roomId){


//		string roomType;
//		int roomID;
//		string [] words = new string [12];
//		Vector3[] brushes = new Vector3 [10];
//		public int dupeNum;
//		public string rightword;
//		public string wrongword;
//		public int awardNum;

		GameObject newRoom = (GameObject)Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
		newRoom.transform.SetParent (gameObject.transform, false);
		TurnRoomScript roomScript = newRoom.GetComponent<TurnRoomScript> ();

		roomScript.roomType = roomType;

		Debug.Log ("Room ID: " + roomId);

		string[] pieces = roomId.Split('|');

		//Debug.Log ("piece one: " + pieces [0]);
		//Debug.Log ("piece two: " + pieces [1]);


		foreach (string piece in pieces) {

			if (piece.StartsWith (ID_SYM)) {

				//Debug.Log (piece);

				roomScript.roomID = int.Parse(piece.Substring (ID_SYM.Length));


			} else if (piece.StartsWith (WORDS_SYM)) {
				
				string wordsWhole = piece.Substring (WORDS_SYM.Length);

				words = wordsWhole.Split('/');

				//Debug.Log ("length" + words.Length);

				for (int i = 0; i < words.Length; i++) {

					roomScript.words [i] = words [i];
					//Debug.Log ("wordy: " + words[i]);

				}

			} else if (piece.StartsWith (BRUSHES_SYM)) {

				string brushesWhole = piece.Substring (BRUSHES_SYM.Length);

				brushes = brushesWhole.Split ('/');
				roomScript.brushes = new Vector3[brushes.Length];

				for (int i = 0; i < brushes.Length; i++) {

					string[] vectArray = brushes[i].Split(',');
					Debug.Log (vectArray[0]);

					// store as a Vector2
					Vector3 tempVect = new Vector3(
						float.Parse(vectArray[0]),
						float.Parse(vectArray[1]),
						0);

					roomScript.brushes [i] = tempVect;

				}

				//roomScript.brushes = piece.Substring (BRUSHES_SYM.Length);

			} else if (piece.StartsWith (FATE_SYM)) {

				string fateWhole = piece.Substring (FATE_SYM.Length);

				//Debug.Log ("fate whole: " + fateWhole);

				string[] fate = fateWhole.Split('/');

				roomScript.dupeNum = int.Parse(fate [0]);
				//Debug.Log ("dupeNum: " + roomScript.dupeNum);

				int rightword = int.Parse (fate [1]);
				roomScript.rightword = words [rightword];
				//Debug.Log ("rightword: " + roomScript.rightword);

				int wrongword = int.Parse (fate [2]);
				roomScript.wrongword = words [wrongword];

				roomScript.awardNum = int.Parse(fate [3]);

			}

		}

	}

}
