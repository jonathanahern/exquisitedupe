using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LocalRoomManager : MonoBehaviour {

	TurnRoomScript myRoom;
	public GameObject brushPrefabX;
	public GameObject brushPrefabY;
	public GameObject brushHolder;

	// Use this for initialization
	void Start () {

		FindMyRoom ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FindMyRoom (){
	
		GameObject roomMan = GameObject.FindGameObjectWithTag ("Room Manager");
		TurnRoomScript[] rooms = roomMan.transform.GetComponentsInChildren<TurnRoomScript> ();
		foreach (TurnRoomScript room in rooms) {

			if (room.activeRoom == true) {
				myRoom = room;
			}
		}
		Camera.main.GetComponent<CameraScript> ().ZoomIn (myRoom.myColor);

	}


	public void StartGame(){
	
		LoadBrushes ();

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

