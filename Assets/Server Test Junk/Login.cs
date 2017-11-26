using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour {

	string LoginURL = "http://dupesite.000webhostapp.com/Login.php";

	public string inputUsername;
	public string inputPassword;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.L)) {
			StartCoroutine(LoginToDB (inputUsername, inputPassword));
		}
		
	}

	IEnumerator LoginToDB(string username, string password){
	
		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", username);
		form.AddField ("passwordPost", password);

		WWW www = new WWW (LoginURL, form);
		yield return www;

		Debug.Log (www.text);

	}
}
