using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PhaseTwoReady () {

		doneVoting.SetActive (true);
		gameStatus.text = "VOTE!";
		button.color = ready;

	}

}
