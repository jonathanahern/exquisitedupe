using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenu : MonoBehaviour {

	public static LobbyMenu instance;

	public GameObject mainButtons;
	public GameObject turnBasedButtons;
	public GameObject loadScreen;

	// Use this for initialization
	void Awake () {

		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TurnBasedClicked(){
	
		mainButtons.SetActive (false);
		turnBasedButtons.SetActive (true);
	
	}

	public void LoadingScreen (){
	
		loadScreen.SetActive (true);
		turnBasedButtons.SetActive (false);
	
	}

}
