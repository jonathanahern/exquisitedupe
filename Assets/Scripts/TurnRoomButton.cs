using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseControl;

public class TurnRoomButton : MonoBehaviour {

	public Text roomType;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TurnRoomClicked(){
		UserAccountManagerScript.instance.TurnRoomSearch(roomType.text);
	}

}
