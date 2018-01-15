using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvitationScript : MonoBehaviour {

	public Text messageText;
	public int roomNum;
	LobbyMenu lobbyMenu;
	string playerSent;

	public void SetupInvitation(string message, int roomSent, LobbyMenu theScript){

		roomNum = roomSent;
		messageText.text = message + " has challenged you to a contest of wits and stupid art!";
		lobbyMenu = theScript;
		playerSent = message;
	}

	public void AcceptInvite (){
	
		UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();
		string roomsString = userAccount.activeRooms;

		if (roomsString == "") {
			roomsString = roomNum.ToString ();
			userAccount.activeRooms = roomsString;
		} else {
			roomsString = roomsString + "/" + roomNum.ToString ();
			userAccount.activeRooms = roomsString;
		}

		string message = playerSent + "|" + roomNum.ToString () + "$";
		RoomManager.instance.CreateRoomFromInvite (roomNum, message);
		userAccount.StoreRoom (roomNum.ToString ());
	
		//lobbyMenu.invitationNums.Remove (roomNum);
		Destroy(gameObject);
	}

}
