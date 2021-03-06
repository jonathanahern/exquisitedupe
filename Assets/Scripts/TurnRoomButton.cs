﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseControl;

public class TurnRoomButton : MonoBehaviour {

	public Text roomType;
	public string roomTypeString;
	public string words;
	public string brushes;
	public string grounding;
	private string fate;
	bool alreadyClicked;
	public string description;
	public Color catColor;

	public bool privateRoom;

	public Sprite[] plaque;
	public Image plaqueImage;
	public Text descriptionText;

	// Use this for initialization
	void Start () {

		//Bear/Cow/Skunk/Horse/Pig/Rhino/Lion/Dog/Camel/Gerbil/Sloth/Cat
		//		-1.78f,0/1.33f,0/0,2.44f/0,-1.05f  ;

		//Confetti/Vomit/Bomb Blast/Messy Bed/Snake Pit/Rain Storm/Wheat Field/Spaghetti/Landfill/Tornado/Scrambled Eggs/Forest Fire
		//0,1.3f/0,6.7f/0,10.3f/0,-1.68f/0,-10.06f/2.24f,0/5.94f,0/-2.61f,0/-4.68f,0

	}
	
	// Update is called once per frame
	void Update () {

//				if (Input.GetKeyDown (KeyCode.S)) {
//
//			TurnRoomClicked ();
//
//				}

	}

	public void SetupButton (int buttonOrder){
		Debug.Log ("order: " + buttonOrder);
		plaqueImage.sprite = plaque [buttonOrder];
		float rotAngle = Random.Range(-1.2f, 1.1f);
//		if (buttonOrder == 0) {
//			rotAngle = -1.2f;
//		} else if (buttonOrder == 1){
//			rotAngle = 1.03f;
//		} else {
//			rotAngle = -1.33f;
//		}
		RectTransform rectTran = GetComponent<RectTransform> ();
		rectTran.rotation = Quaternion.Euler(0,0, rotAngle);

		plaqueImage.color = catColor;
		descriptionText.text = description;


	}

	public void TurnRoomClicked(){

		GameObject roomMan = GameObject.FindGameObjectWithTag ("Room Manager");
		RoomManager roomManScript = roomMan.GetComponent<RoomManager> ();
		roomManScript.CurtainsIn();
		Debug.Log ("Curtains shake from button");
		roomManScript.StartingNewRoom ();

		if (roomManScript.refreshing == true) {
			Debug.Log ("No Start");
			Invoke ("TurnRoomClicked", 1.0f);
		} else {
			StartThePainting ();
		}
	}

	public void PopulateButton(string roomTypeString, string wordsString, string brushesString, string groundingString){
		roomType.text = roomTypeString;
		words = wordsString;
		brushes = brushesString;
		grounding = groundingString;
	}

	void StartThePainting (){

		if (alreadyClicked == true) {
			return;
		}
		alreadyClicked = true;
		Invoke ("BackToFalse", 1.0f);

		int dupeNum = Random.Range (1, 5);
		int rightWord = Random.Range (1, 11);
		int wrongWord = Random.Range (1, 11);
		int colorMod = Random.Range (0, 4);
		//Debug.Log ("From colMod: " + colorMod);

		while(rightWord == wrongWord)
		{
			wrongWord = Random.Range (1, 11);
		}

		int awardNum = Random.Range (1, 3);

		string[] wordsSplit = words.Split ('/');
		string newWords = "";

		for (int i = 0; i < 10; i++) {

			newWords = newWords + wordsSplit [i] + "/";

		}

		newWords = newWords.TrimEnd ('/');

		fate = "|[WORDS]" + newWords + "|[BRUSHES]" + brushes + "|" + grounding + "|[FATE]" + dupeNum + "/" + rightWord + "/" + wrongWord + "/" + awardNum + "/" + colorMod + "/0/0/0";

		//Debug.Log ("From butt: " + fate);

		string roomToSend = "v2" + roomType.text;

		if (privateRoom == true) {
			roomToSend = "v2" + roomToSend;
		}

		string r = (Mathf.Round(catColor.r * 255)).ToString();
		string g = (Mathf.Round(catColor.g * 255)).ToString();
		string b = (Mathf.Round(catColor.b * 255)).ToString();

		string catColorString = r + "/" + g + "/" + b + "$" + description;

		UserAccountManagerScript.instance.TurnRoomSearch(roomToSend, fate, gameObject, catColorString);

		//LobbyMenu.instance.LoadingScreenFromNewCats ();
	
	}

	void BackToFalse () {
		alreadyClicked = false;
	}

	public void NextPrivatePainting(){
	
		int dupeNum = Random.Range (1, 5);
		int rightWord = Random.Range (1, 11);
		int wrongWord = Random.Range (1, 11);
		int colorMod = Random.Range (0, 4);

		while(rightWord == wrongWord)
		{
			wrongWord = Random.Range (1, 11);
		}

		int awardNum = Random.Range (1, 3);

		string[] wordsSplit = words.Split ('/');
		string newWords = "";

		for (int i = 0; i < 10; i++) {

			newWords = newWords + wordsSplit [i] + "/";

		}

		newWords = newWords.TrimEnd ('/');

		fate = "|[WORDS]" + newWords + "|[BRUSHES]" + brushes + "|" + grounding + "|[FATE]" + dupeNum + "/" + rightWord + "/" + wrongWord + "/" + awardNum + "/" + colorMod;

		Debug.Log ("From butt: " + fate + words);

		string roomToSend = roomType.text;

		if (privateRoom == true) {
			roomToSend = "v2" + roomToSend;
		}

		string r = (Mathf.Round(catColor.r * 255)).ToString();
		string g = (Mathf.Round(catColor.g * 255)).ToString();
		string b = (Mathf.Round(catColor.b * 255)).ToString();

		string catColorString = r + "/" + g + "/" + b + "$" + description;

		UserAccountManagerScript.instance.TurnRoomSearch(roomToSend, fate, gameObject, catColorString);
	
	}


}
