//This script controls the UI in the Database Control (Free) demo scene
//It uses database control to login, register and send and recieve data

using UnityEngine;
using System; //allows string.Split to be used with SplitStringOptions.none
using System.Collections;
using DatabaseControl;//This line is always needed for any C# script using the database control requests. See PDF documentation for more information
//use 'import DatabaseControl;' if you are using JS
using DG.Tweening;

public class LoginMenu : MonoBehaviour {
	
	//these are the login input fields:
	public UnityEngine.UI.InputField input_login_username;
	public UnityEngine.UI.InputField input_login_password;
	
	//these are the register input fields:
	public UnityEngine.UI.InputField input_register_password;
	public UnityEngine.UI.InputField input_register_confirmPassword;
	
	//red error UI Texts:
	public UnityEngine.UI.Text login_error;
	public UnityEngine.UI.Text register_error;

	string userNameLocation = "userNameLocation";
	string passwordLocation = "passwordLocation";

	public GameObject notEnough;
	public GameObject noFunnies;
	public GameObject enterButton;
	public GameObject loadingHolder;
	public GameObject backButton;
	public GameObject registerButton;


	void Start () {

		string storedUsername = PlayerPrefs.GetString (userNameLocation);

		if (storedUsername != string.Empty) {
		
			input_login_username.text = storedUsername;
			input_login_password.text = PlayerPrefs.GetString (passwordLocation);
		
		}

		blankErrors();
	}

	void Update () {

		if (Input.GetKeyDown (KeyCode.D)) {

			//GoFromEnterToLoading ();

		}
		
	}

	void blankErrors () {
		//blanks all error texts when part is changed e.g. login > Register
		login_error.text = "";
		register_error.text = "";
	}
	


	public void login_login_Button () { //called when the 'Login' button on the login part is pressed
		
			//check fields aren't blank
			if (input_login_username.text != ""){// && (input_login_password.text != "")) {
			
				//check fields don't contain '-' (if they do, login request will return with error and take longer)
				if ((input_login_username.text.Contains ("-")) || (input_login_username.text.Contains ("|")) || (input_login_username.text.Contains ("/")) || (input_login_username.text.Contains (",")) || (input_login_username.text.Contains ("$")) || (input_login_username.text.Contains ("@")) || (input_login_username.text.Contains ("^")) || (input_login_username.text.Contains ("&"))) {
					//string contains "-" so return error
					//login_error.text = "Unsupported Symbol '-'";
					noFunnies.SetActive(true);
					Vector3 punchSize = new Vector3 (15, 15, 15);
					enterButton.transform.DOPunchPosition (punchSize,.5f, 10, 1);
					//Vector3 fullRotation = new Vector3 (0, 0, 360);
					//enterButton.transform.DOLocalRotate (fullRotation, 1.5f,RotateMode.FastBeyond360).SetEase (Ease.InOutFlash);
					input_login_password.text = ""; //blank password field
				} else if (input_login_username.text.Length < 3){
					notEnough.SetActive (true);
					//Vector3 fullRotation = new Vector3 (0, 0, 360);
					Vector3 punchSize = new Vector3 (15, 15, 15);
					enterButton.transform.DOPunchPosition (punchSize,.5f, 10, 1);
					//enterButton.transform.DOLocalRotate (fullRotation, 1.5f,RotateMode.FastBeyond360).SetEase (Ease.InOutFlash);
				} else {
					//ready to send request
					StartCoroutine (sendLoginRequest (input_login_username.text, input_login_password.text)); //calls function to send login request
					//part = 3; //show 'loading...'
					GoFromEnterToLoading();
				}
			
			} else {
				//one of the fields is blank so return error
				login_error.text = "Field Blank!";
				input_login_password.text = ""; //blank password field
			}
		
		
	}

	void RetryLoginRequest(string username, string password){

		StartCoroutine(sendLoginRequest(username, password));
	
	}
	
	IEnumerator sendLoginRequest (string username, string password) {


			Debug.Log ("Sent login request for: " + username);

			string URL = "http://dupesite.000webhostapp.com/loginRequest.php";

			WWWForm form = new WWWForm ();
			form.AddField ("usernamePost", username);

			WWW www = new WWW (URL, form);
			yield return www;

			string returnText = www.text;
			returnText = returnText.Replace("\n", "");
			Debug.Log ("Web returned: " + returnText);

//			if (returnText == "") {
//				RetryLoginRequest (username, password);
//				yield break;
//			}
//
//			if (returnText.Contains("#")) {
//				RetryLoginRequest (username, password);
//				Debug.Log("Weird bug happened");
//				yield break;
//
//			}

				string[] returnBroken = returnText.Split ('|');
				string success = returnBroken [0];

			//Debug.Log("SUCCESSa" + returnBroken[0]);

			if (success == "Success") {

				//Debug.Log("SUCCESSb" + returnBroken[1]);

				//Password was correct
				blankErrors ();
				//part = 2; //show logged in UI
			
				//blank username field
				//input_login_username.text = ""; //password field is blanked at the end of this function, even when error is returned
		
				PlayerPrefs.SetString (userNameLocation, username);
				PlayerPrefs.SetString (passwordLocation, password);

				//Debug.Log ("Paintings: " + returnBroken [1]);

				UserAccountManagerScript.instance.LogIn (username, password, returnBroken[1], returnBroken[2], returnBroken[3], 1);

			} else {
				
			if (returnText == "Not Found") {
				//Account with username not found in database
				login_error.text = "Name not found. Register this username?";
				blankErrors();
				GoToRegisterButtonsFromLoading ();
				//part = 1; //back to register UI
				register_error.text = "Name not found. Register this username?";
				input_login_username.text = username; //blank password field
				input_register_password.text = password;
				}
			if (returnText == "PassError") {
				//Account with username found, but password incorrect
				//part = 0; //back to login UI
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

	public void register_register_Button () { //called when the 'Register' button on the register part is pressed

			//check fields aren't blank
		if ((input_login_username.text != "")){// && (input_register_password.text != "")) {// && (input_register_confirmPassword.text != "")) {
			
				//check username is longer than 2 characters
			if (input_login_username.text.Length > 2) {
				
					//check password is longer than 6 characters
					if (input_register_password.text.Length > -1) {
					
						//check passwords are the same jonathan changed this so so confirm needed
						if (input_register_password.text == input_register_password.text) {
						
						if ((input_login_username.text.Contains ("-")) || (input_login_username.text.Contains ("|")) || (input_login_username.text.Contains ("/"))|| (input_login_username.text.Contains ("$"))|| (input_login_username.text.Contains ("^"))|| (input_login_username.text.Contains ("@"))) {
							
								noFunnies.SetActive (true);
								//string contains "-" so return error
								//register_error.text = "Unsupported Symbol";
								input_login_password.text = ""; //blank password field
								//input_register_confirmPassword.text = "";
								register_error.text = "";
							
							} else {
							blankErrors ();
								//ready to send request
							StartCoroutine (sendRegisterRequest (input_login_username.text, input_register_password.text, "Hello World!")); //calls function to send register request
							noFunnies.SetActive(false);
							notEnough.SetActive(false);
							HideRegisterButtons ();
							//part = 3; //show 'loading...'

							}
						
						} else {
							//return passwords don't match error
							register_error.text = "Passwords don't match!";
							input_register_password.text = ""; //blank password fields
							//input_register_confirmPassword.text = "";
						}
					
					} else {
						//return password too short error
						register_error.text = "Password too Short";
						input_register_password.text = ""; //blank password fields
						//input_register_confirmPassword.text = "";
					}
				
				} else {
					//return username too short error
					//register_error.text = "Username too Short";
					notEnough.SetActive (true);
					input_register_password.text = ""; //blank password fields
					//input_register_confirmPassword.text = "";
					register_error.text = "";
				}
			
			} else {
				//one of the fields is blank so return error
				register_error.text = "Field Blank!";
				input_register_password.text = ""; //blank password fields
				//input_register_confirmPassword.text = "";
			}

	}
	
	IEnumerator sendRegisterRequest (string username, string password, string data) {

		string URL = "http://dupesite.000webhostapp.com/registerRequest.php";
		string notId = UserAccountManagerScript.instance.notificationId;

		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", username);
		form.AddField ("notIdPost",notId);

		WWW www = new WWW (URL, form);
		yield return www;

		string returnText = www.text;
		returnText = returnText.Replace("\n", "");
		Debug.Log (returnText);

			if (returnText == "Success") {
				//Account created successfully

				PlayerPrefs.SetString (userNameLocation, username);
				PlayerPrefs.SetString (passwordLocation, password);
				blankErrors();
				
			UserAccountManagerScript.instance.LogIn (username, password, "", notId, "", 0);

			} else if (returnText == "Already exists") {
				//Account Not Created due to username being used on another Account
				//part = 1;
				GoToRegisterButtonsFromLoading();
				register_error.text = "Username Unavailable. Try another.";
			} else {
				GoToRegisterButtonsFromLoading ();
				register_error.text = "Error";
			
			}

//			if (returnText == "Error") {
//				//Account Not Created, another error occurred
//				part = 1;
//				login_error.text = "Database Error. Try again later.";
//			}
			
			input_register_password.text = "";
			input_register_confirmPassword.text = "";

		
	}

	void GoFromEnterToLoading (){
		notEnough.SetActive(false);
		noFunnies.SetActive(false);
		enterButton.transform.DOScale (Vector3.zero, .4f).SetEase(Ease.InBack).OnComplete(BringUpLoading);

	}

	void BringUpLoading (){
		loadingHolder.SetActive (true);
		loadingHolder.transform.DOScale (Vector3.one, .4f).SetId("Loading").SetEase(Ease.OutBack);
	
	}

	void GoToRegisterButtonsFromLoading(){
		DOTween.Kill ("Loading");
		loadingHolder.transform.DOScale (Vector3.zero, .4f).SetEase(Ease.InBack).OnComplete(BringUpRegisterButtons);
	}

	void BringUpRegisterButtons(){
		loadingHolder.SetActive (false);
		registerButton.transform.DOScale (Vector3.one, .4f).SetEase(Ease.OutBack);
		backButton.transform.DOScale (Vector3.one, .4f).SetEase(Ease.OutBack);
	}

	void HideRegisterButtons(){
		registerButton.transform.DOScale (Vector3.zero, .4f).SetEase(Ease.InBack);
		backButton.transform.DOScale (Vector3.zero, .4f).SetEase(Ease.InBack).OnComplete(BringUpLoading);
	}

	public void BackButton(){
		registerButton.transform.DOScale (Vector3.zero, .4f).SetEase(Ease.InBack);
		backButton.transform.DOScale (Vector3.zero, .4f).SetEase(Ease.InBack).OnComplete(BringUpEnter);
		blankErrors();
		notEnough.SetActive(false);
		noFunnies.SetActive(false);
	}

	void BringUpEnter(){
		enterButton.transform.DOScale (Vector3.one, .4f).SetEase(Ease.OutBack);
	}

}
	
