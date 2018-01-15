using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPlayerScript : MonoBehaviour {

	public Text theName;
	public bool choosen;

	public Toggle onOff;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddToPlayerList (bool newValue){

		LobbyMenu lobby = GameObject.FindGameObjectWithTag ("Lobby Menu").GetComponent<LobbyMenu> ();
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

	public void TurnOffToggle(){
		onOff.interactable = false;
	}

	public void TurnOnToggle(){
		onOff.interactable = true;
	}

	public void AddToCategoryList (bool newValue){

		LobbyMenu lobby = GameObject.FindGameObjectWithTag ("Lobby Menu").GetComponent<LobbyMenu> ();

		choosen = newValue;

		if (newValue == false) {
			++lobby.catsCount;
			lobby.CatSubtracted (theName.text);

		} else {
			--lobby.catsCount;
			lobby.CatSelected (theName.text);
		}

	}
}

