using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseControl;

public class TurnRoomScript : MonoBehaviour {

	public string roomType;
	public string status;
	public int statusNum;
	public int roomID;
	public string [] words = new string [12];
	public Vector3[] brushes;
	public int dupeNum;
	public string rightword;
	public string wrongword;
	public int awardNum;
	public int myColor;
	public string grounding;

	public bool activeRoom;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Q)) {

		
		}
		
	}


		
}
