using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInserter : MonoBehaviour {

	string CreateUserURL = "http://dupesite.000webhostapp.com/UserData.php";

	public string inputUsername;
	public string inputPassword;
	public string inputEmail;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Space)) {
		
			CreateUser (inputUsername, inputPassword, inputEmail);
		
		}

	}

	public void CreateUser(string username, string password, string email){
	
		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", username);
		form.AddField ("passwordPost", password);
		form.AddField ("emailPost", email);
	
		WWW www = new WWW (CreateUserURL, form);

	}
}
