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
	public string[] players;
	public string[] playersNotId;
	public int dupeNum;
	public string rightword;
	public string wrongword;
	public int awardNum;
	public int myColor;
	public string grounding;

	public string drawings;
	public string votePoses;
	public string dupeGuess;

	public bool activeRoom;
	public bool activeVoteRoom;
	public bool activeScoreRoom;

	public string dupeCaught;
	public bool privateRoom;
	public int roundNum;
	public int colorMod;
	public int myActualColor;
	public string statusServer;
	public bool needsToSendAlert = false;


		
}
