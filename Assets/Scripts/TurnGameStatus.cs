using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TurnGameStatus : MonoBehaviour {

	public int roomId;

	public Text categoryName;
	public Text gameStatus;
	public Image button;

	public Color wait;
	public Color ready;

	public GameObject doneDrawing;
	public GameObject doneDrawingCheck;
	public GameObject doneVoting;
	public GameObject doneVotingCheck;
	public GameObject doneAwards;

	bool phaseOneReady;
	bool phaseTwoReady;
	//bool phaseThreeReady;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//0
	public void PhaseOneReady () {

		phaseOneReady = true;
		doneDrawing.SetActive (true);
		gameStatus.text = "DRAW!";
		button.color = ready;

	}

	//1
	public void PhaseOneDone () {

		phaseOneReady = false;
		doneDrawing.SetActive (true);
		doneDrawingCheck.SetActive (true);
		gameStatus.text = "waiting...";
		button.color = wait;

	}

	//2
	public void PhaseTwoReady () {

		phaseTwoReady = true;
		doneVoting.SetActive (true);
		doneDrawing.SetActive (true);
		doneDrawingCheck.SetActive (true);
		gameStatus.text = "VOTE!";
		button.color = ready;

	}

	public void StartNextPhase(){

		if (phaseTwoReady == true) {
			EnterPhaseTwo ();
			phaseTwoReady = false;
		} else if (phaseOneReady == true) {
			EnterPhaseOne ();
			phaseOneReady = false;

		}

	}

	void EnterPhaseTwo (){
	
		GameObject roomMan = GameObject.FindGameObjectWithTag ("Room Manager");

		int children = roomMan.transform.childCount;

		Debug.Log (roomMan.name + children);

		for (int i = 0; i < children; ++i){
			
			TurnRoomScript turnRoom = roomMan.transform.GetChild (i).GetComponent<TurnRoomScript>();

//			Debug.Log (turnRoom.roomID);
//			Debug.Log (roomId);

			if (turnRoom.roomID == roomId) {
				turnRoom.activeVoteRoom = true;
				i = children;
			}

		}

		SceneManager.LoadScene ("Turn Based Voting");

	}

	void EnterPhaseOne (){

		GameObject roomMan = GameObject.FindGameObjectWithTag ("Room Manager");

		int children = roomMan.transform.childCount;

		Debug.Log (roomMan.name + children);

		for (int i = 0; i < children; ++i){

			TurnRoomScript turnRoom = roomMan.transform.GetChild (i).GetComponent<TurnRoomScript>();

			if (turnRoom.roomID == roomId) {
				turnRoom.activeRoom = true;
				i = children;
			}

		}

		SceneManager.LoadScene ("Turn Based Room");

	}

}
