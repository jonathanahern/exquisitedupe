using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using DatabaseControl;

public class LocalRoomManager : MonoBehaviour {

	TurnRoomScript myRoom;
	public LineSpawnerScipt lineSpawn;
	public GameObject brushPrefabX;
	public GameObject brushPrefabY;
	public GameObject brushHolder;
	public Text subject;

	// Use this for initialization
	void Start () {

		FindMyRoom ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FindMyRoom (){
	
		GameObject roomMan = GameObject.FindGameObjectWithTag ("Room Manager");

		if (roomMan == null) {
			Debug.Log ("not logged in");
			return;
		}

		TurnRoomScript[] rooms = roomMan.transform.GetComponentsInChildren<TurnRoomScript> ();
		foreach (TurnRoomScript room in rooms) {

			if (room.activeRoom == true) {
				myRoom = room;
			}
		}

		Camera.main.GetComponent<CameraScript> ().ZoomIn (myRoom.myColor);

		if (myRoom.dupeNum == myRoom.myColor) {
			subject.text = myRoom.wrongword;
		} else {
			subject.text = myRoom.rightword;
		}

	}


	public void StartGame(){
	
		LoadBrushes ();
		lineSpawn.GetColor(myRoom.myColor);


	}

	void LoadBrushes (){
	

		for (int i = 0; i < myRoom.brushes.Length; i++) {
			
			if (myRoom.brushes [i].x == 0) {
				GameObject newBrush = (GameObject)Instantiate (brushPrefabY, myRoom.brushes[i], Quaternion.identity);
				newBrush.transform.SetParent (brushHolder.transform, false);
			} else {
				GameObject newBrush = (GameObject)Instantiate (brushPrefabX, myRoom.brushes[i], Quaternion.identity);
				newBrush.transform.SetParent (brushHolder.transform, false);
			
			}
		}
	}
}

