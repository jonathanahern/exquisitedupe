using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserAccountLobby : MonoBehaviour {

	public Text usernameText;

	//public GameObject loadingScreen;

	// Use this for initialization
	void Start () {

		if (UserAccountManagerScript.IsLoggedIn)
		usernameText.text = UserAccountManagerScript.LoggedIn_Username;
		
	}

	public void LogOut () {
	
		if (UserAccountManagerScript.IsLoggedIn)
			UserAccountManagerScript.instance.LogOut ();
	
	}

}
