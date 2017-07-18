﻿using System.Collections;
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
	bool phaseThreeReady;

	// Use this for initialization
	void Start () {
		
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

	//3
	public void PhaseTwoDone(){

		phaseThreeReady = false;
		doneDrawing.SetActive (true);
		doneDrawingCheck.SetActive (true);
		doneVoting.SetActive (true);
		doneVotingCheck.SetActive (true);
		gameStatus.text = "waiting...";
		button.color = wait;

	}

	//4
	public void PhaseThreeReady(){

		phaseThreeReady = true;
		doneDrawing.SetActive (true);
		doneDrawingCheck.SetActive (true);
		doneVoting.SetActive (true);
		doneVotingCheck.SetActive (true);
		doneAwards.SetActive (true);
		gameStatus.text = "SCORE IT!";
		button.color = ready;

	}

	public void StartNextPhase(){

		if (phaseTwoReady == true) {
			EnterPhaseTwo ();
			phaseTwoReady = false;
		} else if (phaseOneReady == true) {
			EnterPhaseOne ();
			phaseOneReady = false;
		} else if (phaseThreeReady == true) {
			EnterPhaseThree ();
			phaseThreeReady = false;
		}

	}

	void EnterPhaseThree (){

		GameObject roomMan = GameObject.FindGameObjectWithTag ("Room Manager");

		int children = roomMan.transform.childCount;

		Debug.Log (roomMan.name + children);

		for (int i = 0; i < children; ++i){

			TurnRoomScript turnRoom = roomMan.transform.GetChild (i).GetComponent<TurnRoomScript>();

			if (turnRoom.roomID == roomId) {
				turnRoom.activeScoreRoom = true;
				i = children;
			}

		}

		SceneManager.LoadScene ("Turn Based Scoring");

	}


	void EnterPhaseTwo (){
	
		GameObject roomMan = GameObject.FindGameObjectWithTag ("Room Manager");

		int children = roomMan.transform.childCount;

		Debug.Log (roomMan.name + children);

		for (int i = 0; i < children; ++i){
			
			TurnRoomScript turnRoom = roomMan.transform.GetChild (i).GetComponent<TurnRoomScript>();

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
