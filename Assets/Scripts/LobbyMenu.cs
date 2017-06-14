using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenu : MonoBehaviour {

	public GameObject mainButtons;
	public GameObject turnBasedButtons;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TurnBasedClicked(){
	
		mainButtons.SetActive (false);
		turnBasedButtons.SetActive (true);
	
	}
}
