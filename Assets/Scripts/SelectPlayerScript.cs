﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPlayerScript : MonoBehaviour {

	public Text description;
	public Image buttColor;
	public Text theName;
	public Text catCount;
	int catCountInt = 0;
	public bool choosen;
	public RectTransform buttonRect;

	public Toggle onOff;
	LobbyMenu lobby;

	// Use this for initialization
	void Start () {
		lobby = GameObject.FindGameObjectWithTag ("Lobby Menu").GetComponent<LobbyMenu> ();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddToPlayerList (bool newValue){

		lobby = GameObject.FindGameObjectWithTag ("Lobby Menu").GetComponent<LobbyMenu> ();
//		if (lobby.selectCount < 1) {
//			return;
//		}
		choosen = newValue;

		if (newValue == false) {
			Debug.Log ("False!" + lobby.selectCount);
			++lobby.selectCount;
			lobby.ArtistSubtracted (theName.text);

		} else {
			Debug.Log ("True!" + lobby.selectCount);
			--lobby.selectCount;
			lobby.ArtistSelected (theName.text);
		}

	}

	public void InsertName(string name){
	
		theName.text = name;

	}

	public void InsertCategory(string nameString, string descriptionString, Color catColor){

		theName.text = nameString;
		buttColor.color = catColor;
		description.text = descriptionString;

		float randomAngle = Random.Range (-2.8f, 2.0f);
		buttonRect.rotation = Quaternion.Euler(0,0, randomAngle);

	}

	public void TurnOffToggle(){
		onOff.interactable = false;
	}

	public void TurnOnToggle(){
		onOff.interactable = true;
	}

//	public void AddToCategoryList (bool newValue){
//
//		LobbyMenu lobby = GameObject.FindGameObjectWithTag ("Lobby Menu").GetComponent<LobbyMenu> ();
//
//		choosen = newValue;
//
//		if (newValue == false) {
//			++lobby.catsCount;
//			lobby.CatSubtracted (theName.text);
//
//		} else {
//			--lobby.catsCount;
//			lobby.CatSelected (theName.text);
//		}
//
//	}

	public void PlusButton (){

		if (lobby.catsSelected.Count > 7) {
			lobby.LimitToEight ();
			return;
		}

		catCountInt = catCountInt + 1;
		catCount.text = catCountInt.ToString ();
		lobby.CatSelected (theName.text);
	}

	public void ResetToZero(){
		catCountInt = 0;
		catCount.text = catCountInt.ToString ();
	}

//	public void MinusButton (){
//
//		if (catCountInt < 1) {
//			return;
//		}
//
//		lobby.CatSubtracted (theName.text);
//
//		catCountInt = catCountInt - 1;
//		catCount.text = catCountInt.ToString ();
//	}

}

