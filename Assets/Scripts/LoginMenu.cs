//This script controls the UI in the Database Control (Free) demo scene
//It uses database control to login, register and send and recieve data

using UnityEngine;
using System; //allows string.Split to be used with SplitStringOptions.none
using System.Collections;
using DatabaseControl;//This line is always needed for any C# script using the database control requests. See PDF documentation for more information
//use 'import DatabaseControl;' if you are using JS


public class LoginMenu : MonoBehaviour {
	////These variables are set in the Inspector:
	
	//they are enabled and disabled to show and hide the different parts of the UI
	public GameObject login_object;
	public GameObject register_object;
	public GameObject loading_object;
	
	//these are the login input fields:
	public UnityEngine.UI.InputField input_login_username;
	public UnityEngine.UI.InputField input_login_password;
	
	//these are the register input fields:
	public UnityEngine.UI.InputField input_register_username;
	public UnityEngine.UI.InputField input_register_password;
	public UnityEngine.UI.InputField input_register_confirmPassword;
	
	//red error UI Texts:
	public UnityEngine.UI.Text login_error;
	public UnityEngine.UI.Text register_error;

	string userNameLocation = "userNameLocation";
	string passwordLocation = "passwordLocation";

	string databaseName = "";
	//bool canRunSequences = false;

	////These variables cannot be set in the Inspector:
	
	//the part of UI currently being shown
	// 0 = login, 1 = register, 2 = logged in, 3 = loading
	int part = 0;
	//scene starts showing login

	bool isDatabaseSetup = false;

	void Start () {

		string storedUsername = PlayerPrefs.GetString (userNameLocation);

		if (storedUsername != string.Empty) {
		
			input_login_username.text = storedUsername;
			input_login_password.text = PlayerPrefs.GetString (passwordLocation);
		
		}


		//Gets the databaseName as it was setup through the editor
		GameObject linkObj = GameObject.Find("Link");
		if (linkObj == null)
		{
			Debug.LogError("DCP Error: Cannot find the link object in the scene so scripts running Command Sequences don't know the database name");
		} else
		{
			DCP_Demos_LinkDatabaseName linkScript = linkObj.gameObject.GetComponent<DCP_Demos_LinkDatabaseName>() as DCP_Demos_LinkDatabaseName;
			if (linkScript == null)
			{
				Debug.LogError("DCP Error: Cannot find the link script on link object so scripts running Command Sequences don't know the database name");
			} else
			{
				if (linkScript.databaseName == "")
				{
					Debug.LogError("DCP Error: This demo scene has not been setup. Please setup the demo scene in the Setup window before use. Widnow>Database Control Pro>Setup Window");
				} else
				{
					databaseName = linkScript.databaseName;
					//canRunSequences = true;
					isDatabaseSetup = true;
				}
			}
		}

		//this checks whether the database is setup. It is used to prevent errors for users who try to use the demos
		//without having setup a database.
		//You don't need to use this bool as it will work without it as long as the database has been setup
//		TextAsset datafile = Resources.Load ("data") as TextAsset;
//		string[] splitdatafile = datafile.text.Split (new string[] { "-" }, StringSplitOptions.None);
//		if (splitdatafile [0] == "0") {
//			isDatabaseSetup = false;
//			Debug.Log ("These demos will not work out of the box. You need to setup a database first for it to work. Please read the Setup section of the PDF for more information");
//		} else {
//			isDatabaseSetup = true;
//		}

		//sets error Texts string to blank
		blankErrors();
	}

	void Update () {

		if (isDatabaseSetup == true) {

			//enables and disables the defferent objects to show correct part
			if (part == 0) {
				login_object.gameObject.SetActive (true);
				register_object.gameObject.SetActive (false);
				loading_object.gameObject.SetActive (false);
			}
			if (part == 1) {
				login_object.gameObject.SetActive (false);
				register_object.gameObject.SetActive (true);
				loading_object.gameObject.SetActive (false);
			}
			if (part == 2) {
				//We are logged in to new scene, hopefully
			}
			if (part == 3) {
				login_object.gameObject.SetActive (false);
				register_object.gameObject.SetActive (false);
				loading_object.gameObject.SetActive (true);
			}
		

		}
		
	}

	void blankErrors () {
		//blanks all error texts when part is changed e.g. login > Register
		login_error.text = "";
		register_error.text = "";
	}
	
	public void login_Register_Button () { //called when the 'Register' button on the login part is pressed
		part = 1; //show register UI
		blankErrors();
	}
	
	public void register_Back_Button () { //called when the 'Back' button on the register part is pressed
		part = 0; //goes back to showing login UI
		blankErrors();
	}
	
	public void data_LogOut_Button () { //called when the 'Log Out' button on the data part is pressed
		part = 0; //goes back to showing login UI

		UserAccountManagerScript.instance.LogOut ();

		blankErrors();
	}

	public void login_login_Button () { //called when the 'Login' button on the login part is pressed

		if (isDatabaseSetup == true) {
		
			//check fields aren't blank
			if ((input_login_username.text != "") && (input_login_password.text != "")) {
			
				//check fields don't contain '-' (if they do, login request will return with error and take longer)
				if ((input_login_username.text.Contains ("-")) || (input_login_password.text.Contains ("-"))) {
					//string contains "-" so return error
					login_error.text = "Unsupported Symbol '-'";
					input_login_password.text = ""; //blank password field
				} else {
					//ready to send request
					StartCoroutine (sendLoginRequest (input_login_username.text, input_login_password.text)); //calls function to send login request
					part = 3; //show 'loading...'
				}
			
			} else {
				//one of the fields is blank so return error
				login_error.text = "Field Blank!";
				input_login_password.text = ""; //blank password field
			}
		
		}
		
	}

	void RetryLoginRequest(string username, string password){

		StartCoroutine(sendLoginRequest(username, password));
	
	}
	
	IEnumerator sendLoginRequest (string username, string password) {

		if (isDatabaseSetup == true) {
		
			IEnumerator e = DCP.RunCS (databaseName, "Login", new string[2] { username, password });

			while (e.MoveNext ()) {
				yield return e.Current;
			}

			string returnText = e.Current as string;

			Debug.Log (returnText);

			if (returnText == "") {
			
				RetryLoginRequest (username, password);
				yield break;

			}

			string[] returnBroken = returnText.Split ('|');
			string success = returnBroken [0];

			//Debug.Log("SUCCESSa" + returnBroken[0]);

			if (success == "Success") {

				//Debug.Log("SUCCESSb" + returnBroken[1]);

				//Password was correct
				blankErrors ();
				part = 2; //show logged in UI
			
				//blank username field
				input_login_username.text = ""; //password field is blanked at the end of this function, even when error is returned
		
				PlayerPrefs.SetString (userNameLocation, username);
				PlayerPrefs.SetString (passwordLocation, password);

				UserAccountManagerScript.instance.LogIn (username, password, returnBroken[1], 1);

			} else {
				
			if (returnText == "UserError") {
				//Account with username not found in database
				login_error.text = "Username not found";
				part = 0; //back to login UI
				}
			if (returnText == "PassError") {
				//Account with username found, but password incorrect
				part = 0; //back to login UI
				login_error.text = "Incorrect Password";
			}
//			if (returnText == "ContainsUnsupportedSymbol") {
//				//One of the parameters contained a - symbol
//				part = 0; //back to login UI
//				login_error.text = "Unsupported Symbol '-'";
//			}
//			else {
//				//Account Not Created, another error occurred
//				part = 0; //back to login UI
//				login_error.text = "Database Error. Try again later.";
//			}
		}
			//blank password field
			input_login_password.text = "";

		}
	}

	public void register_register_Button () { //called when the 'Register' button on the register part is pressed

		if (isDatabaseSetup == true) {
		
			//check fields aren't blank
			if ((input_register_username.text != "") && (input_register_password.text != "") && (input_register_confirmPassword.text != "")) {
			
				//check username is longer than 4 characters
				if (input_register_username.text.Length > 4) {
				
					//check password is longer than 6 characters
					if (input_register_password.text.Length > 3) {
					
						//check passwords are the same
						if (input_register_password.text == input_register_confirmPassword.text) {
						
							if ((input_register_username.text.Contains ("-")) || (input_register_password.text.Contains ("-")) || (input_register_confirmPassword.text.Contains ("-"))) {
							
								//string contains "-" so return error
								register_error.text = "Unsupported Symbol '-'";
								input_login_password.text = ""; //blank password field
								input_register_confirmPassword.text = "";
							
							} else {
							
								//ready to send request
								StartCoroutine (sendRegisterRequest (input_register_username.text, input_register_password.text, "Hello World!")); //calls function to send register request
								part = 3; //show 'loading...'
							}
						
						} else {
							//return passwords don't match error
							register_error.text = "Passwords don't match!";
							input_register_password.text = ""; //blank password fields
							input_register_confirmPassword.text = "";
						}
					
					} else {
						//return password too short error
						register_error.text = "Password too Short";
						input_register_password.text = ""; //blank password fields
						input_register_confirmPassword.text = "";
					}
				
				} else {
					//return username too short error
					register_error.text = "Username too Short";
					input_register_password.text = ""; //blank password fields
					input_register_confirmPassword.text = "";
				}
			
			} else {
				//one of the fields is blank so return error
				register_error.text = "Field Blank!";
				input_register_password.text = ""; //blank password fields
				input_register_confirmPassword.text = "";
			}

		}
		
	}
	
	IEnumerator sendRegisterRequest (string username, string password, string data) {

		if (isDatabaseSetup == true) {

			IEnumerator ee = DCP.RunCS(databaseName, "Register", new string[2] { username, password });
			while(ee.MoveNext()) {
				yield return ee.Current;
			}
			//WWW returnedd = ee.Current as WWW;

			string returnText = ee.Current as string;

			if (returnText == "Success") {
				//Account created successfully

				PlayerPrefs.SetString (userNameLocation, username);
				PlayerPrefs.SetString (passwordLocation, password);

				blankErrors();
				//part = 2; //show logged in UI
				//blank username field
				input_register_username.text = ""; //password field is blanked at the end of this function, even when error is returned
				
		UserAccountManagerScript.instance.LogIn (username, password, "", 0);

			} else if (returnText == "username in use") {
				//Account Not Created due to username being used on another Account
				part = 1;
				register_error.text = "Username Unavailable. Try another.";
			} else {
			
				register_error.text = "Error";
			
			}
//			if (returnText == "ContainsUnsupportedSymbol") {
//				//Account Not Created as one of the parameters contained a - symbol
//				part = 1;
//				register_error.text = "Unsupported Symbol '-'";
//			}
//			if (returnText == "Error") {
//				//Account Not Created, another error occurred
//				part = 1;
//				login_error.text = "Database Error. Try again later.";
//			}
			
			input_register_password.text = "";
			input_register_confirmPassword.text = "";

		}
	}
}
	
