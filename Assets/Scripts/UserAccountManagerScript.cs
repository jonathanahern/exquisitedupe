using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseControl;
using UnityEngine.SceneManagement;

public class UserAccountManagerScript : MonoBehaviour {

	public static UserAccountManagerScript instance;



	void Awake () {
	
		if (instance != null) {
		
			Destroy (gameObject);
			return;
		
		}

		instance = this;
		DontDestroyOnLoad (this);
	
	
	}


	public static string LoggedIn_Username { get; protected set;} //stores username once logged in
	private static string LoggedIn_Password =""; //stores password once logged in
	public static string LoggedIn_Data { get; protected set;} 

	public static bool IsLoggedIn { get; protected set;}

	public string loggedInSceneName = "Lobby";
	public string loggedOutSceneName = "Login";

	public string databaseName;
	public string loggedInUsername;

	string roomId;
	public string activeRooms;

	public void LogOut(){
	
		LoggedIn_Username = "";
		LoggedIn_Password = "";

		IsLoggedIn = false;

		Debug.Log ("User out");
		SceneManager.LoadScene (loggedOutSceneName);
	
	}

	public void AddName(string username){
	
		LoggedIn_Username = username;
	
	}

	public void LogIn(string username, string password, string rooms){

		LoggedIn_Username = username;
		LoggedIn_Password = password;
		loggedInUsername = username;

		activeRooms = rooms;

		IsLoggedIn = true;

		Debug.Log ("User in as: " + username);

		SceneManager.LoadScene (loggedInSceneName);

	}

	public void SendData (string databaseName, string data) { 

		//called when the 'Send Data' button on the data part is pressed
		if (IsLoggedIn) {
			//ready to send request
			StartCoroutine (sendSendDataRequest (databaseName, LoggedIn_Username, LoggedIn_Password, data)); //calls function to send: send data request

		}
	}

	IEnumerator sendSendDataRequest(string database, string username, string password, string data) {

		databaseName = database;

		IEnumerator e = DCP.RunCS(databaseName, "Set Data", new string[3] { username, password, data });
		//IEnumerator eee = DC.SetUserData (username, password, data);

		while (e.MoveNext()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		if (returnText == "Success")
		{
			//set data was successful
//			loadingStuff.MoveBack(); // Fade out loading ui and fade in the data ui
//			yield return new WaitForSeconds(0.5f);
//			dataStuff.MoveForward();
			Debug.Log ("Data sent.");
		}
		else
		{
			//Either error with server or error with user's username and password so logout
//			loadingStuff.MoveBack(); // Fade out loading ui and fade in the login ui
//			yield return new WaitForSeconds(0.5f);
//			loginStuff.MoveForward();
			Debug.Log ("Data Upload Error. Could be a server error. To check try again, if problem still occurs, contact us.");
		}


//		WWW returneddd = e.Current as WWW;
//		if (returneddd.text == "ContainsUnsupportedSymbol") {
//			//One of the parameters contained a - symbol
//			Debug.Log ("Data Upload Error. Could be a server error. To check try again, if problem still occurs, contact us.");
//		}
//		if (returneddd.text == "Error") {
//			//Error occurred. For more information of the error, DC.Login could
//			//be used with the same username and password
//			Debug.Log ("Data Upload Error: Contains Unsupported Symbol '-'");
//		}

	}

	public void GetData () { //called when the 'Get Data' button on the data part is pressed

		if (IsLoggedIn) {
			//ready to send request
			StartCoroutine (sendGetDataRequest (LoggedIn_Username, LoggedIn_Password)); //calls function to send get data request
	
		}
	}

	IEnumerator sendGetDataRequest(string username, string password) {

		string returnText = "ERROR";

		IEnumerator eeee = DCP.RunCS(databaseName, "Get Data", new string[2] { username, password });

//		IEnumerator eeee = DC.GetUserData (username, password);
		while (eeee.MoveNext()) {
			yield return eeee.Current;
		}

		returnText = eeee.Current as string;
		LoggedIn_Data = returnText;
		//With this sequence we will assume there are no errors as the data string could be anything (even the word 'Error')
//		Data_LoadedData.text = returnText; // << shows the retrieved data on ui text
//
//		if (returnText == "Error") {
//			//Error occurred. For more information of the error, DC.Login could
//			//be used with the same username and password
//			Debug.Log ("Data Upload Error. Could be a server error. To check try again, if problem still occurs, contact us.");
//		} else {
//			if (returnedddd.text == "ContainsUnsupportedSymbol") {
//				//One of the parameters contained a - symbol
//				Debug.Log ("Get Data Error: Contains Unsupported Symbol '-'");
//			} else {
//				//Data received in returned.text variable
//				string DataRecieved = returnedddd.text;
//				data = DataRecieved;
//			}
//		}
//
//		LoggedIn_Data = returnText;

	}

	public void TurnRoomSearch(string roomType, string fate){
		
		StartCoroutine (turnRoom(roomType, LoggedIn_Username, fate));

	}

	IEnumerator turnRoom (string roomType, string playerName, string fate){

		IEnumerator e = DCP.RunCS ("turnRooms", "JoinCreateRoom", new string[3] { roomType, playerName, fate});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("HERE?" + returnText);

		string[] fates = returnText.Split ('|');

		foreach (string data in fates) {

			if (data.StartsWith ("[ID]")){

				roomId = data.Substring ("[ID]".Length);
				roomId = roomId + "/";

			}
			
		}

		Debug.Log ("UserName: " + LoggedIn_Username);
		Debug.Log ("roomId: " + roomId);

		StartCoroutine (storeRoomId(LoggedIn_Username, roomId));

		RoomManager.instance.CreateRoom (roomType, returnText, 0);

	}

	IEnumerator storeRoomId (string username, string roomId){
	
		IEnumerator e = DCP.RunCS ("accounts", "StoreRoomId", new string[2] { username, roomId });

			while (e.MoveNext ()) {
				yield return e.Current;
			}

		string returnText = e.Current as string;
			
		//activeRooms = returnText; 	

		Debug.Log ("Stored: " + returnText);
	
	}

}
