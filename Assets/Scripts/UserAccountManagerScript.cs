using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseControl;
using UnityEngine.SceneManagement;
using DG.Tweening;

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

	public GameObject messageBoard;
	LobbyMenu lobbyMenu;

	GameObject tempGameobject;
	public string notificationId;
	public string selfPortrait;

	void Start(){
		notificationId = "Nothing";
	}

	public void LogOut(){
	
		LoggedIn_Username = "";
		LoggedIn_Password = "";

		IsLoggedIn = false;

		Debug.Log ("User out");
		SceneManager.LoadScene (loggedOutSceneName);
	
	}

	void Update (){

		if (Input.GetKeyDown (KeyCode.H)) {
		
//			string asdf = "<!DOCTYPSRJSDNFKjnsDF";
//
//			if (asdf.StartsWith ("<!ds")) {
//				Debug.Log ("Worked");
//			} else {
//				Debug.Log ("Not working");
//			}


			//BackToLobbyError ();
		
		}


	}

	public void AddName(string username){
	
		LoggedIn_Username = username;
	
	}

	public void LogIn(string username, string password, string rooms, string notIdServer, string portrait, int firstGame){

		LoggedIn_Username = username;
		LoggedIn_Password = password;
		loggedInUsername = username;
		selfPortrait = portrait;

		if (notificationId == "Nothing" || notificationId == "") {
			notificationId = notIdServer;
		}

		activeRooms = rooms;

		IsLoggedIn = true;

		//Debug.Log ("User in as: " + username);
		RoomManager.instance.CurtainsIn();
		if (firstGame == 0) {
			Invoke ("LoadPortrait", 1.5f);
		} else {
			Invoke ("LoadLobby", 1.5f);
		}
	}

	void LoadPortrait(){
		SceneManager.LoadScene ("Portrait Creation");
	}

	void LoadLobby(){
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

	public void TurnRoomSearch(string roomType, string fate, GameObject buttonClicked){

		tempGameobject = buttonClicked;

		StartCoroutine (turnRoom(roomType, LoggedIn_Username, fate));

	}

	void RetryTurnRoomNothingFound(string tempRoomType, string tempPlayerName, string tempFate){

		StartCoroutine (turnRoom(tempRoomType, tempPlayerName, tempFate));

	}

	IEnumerator turnRoom (string roomType, string playerName, string fate){

		string URL = "http://dupesite.000webhostapp.com/createJoin.php";

		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", playerName);
		form.AddField ("categoryPost", roomType);
		form.AddField ("fatePost", fate);
		form.AddField ("notIdPost", notificationId);
		form.AddField ("portraitPost", selfPortrait);

		WWW www = new WWW (URL, form);
		yield return www;

		RoomCreated room = JsonUtility.FromJson<RoomCreated>(www.text);

		RoomManager.instance.CreateRoom (roomType, room.id, room.fate, room.playerNum, "", -2);

	}

	void RetryTurnRoom(string tempRoomType, string tempPlayerName, string tempFate){

		StartCoroutine (blankSearch(tempRoomType, tempPlayerName, tempFate));

	}

	IEnumerator blankSearch (string roomType, string playerName, string fate){

		IEnumerator e = DCP.RunCS ("turnRooms", "BlankSearch", new string[3] { roomType, playerName, fate});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		if (returnText == "") {

			Debug.Log ("Retrying new room again from blank search");
			RetryTurnRoom (roomType, playerName, fate);

			yield break;

		} else if (returnText == "Nothing Found"){
			RetryTurnRoomNothingFound (roomType, playerName, fate);
			yield break;
		}

		Debug.Log ("Found this: " + returnText);

		string[] fates = returnText.Split ('|');

		foreach (string data in fates) {

			if (data.StartsWith ("[ID]")){

				roomId = data.Substring ("[ID]".Length);
				roomId = roomId + "/";

			}

		}

		Debug.Log ("UserName: " + LoggedIn_Username);
		Debug.Log ("roomId: " + roomId);

		//temp RoomManager.instance.CreateRoom (roomType, returnText, -2);

	}

	public void StoreRoom (string roomId){

		StartCoroutine (storeRoomId (LoggedIn_Username, roomId));
	
	}

	IEnumerator storeRoomId (string username, string roomId){

		string URL = "http://dupesite.000webhostapp.com/storeRoom.php";

		WWWForm form = new WWWForm ();
		form.AddField ("newIdPost", roomId);
		form.AddField ("usernamePost", username);

		WWW www = new WWW (URL, form);
		yield return www;

		string returnText = www.text;

		Debug.Log ("Stored: " + www.text);
	
	}

//	public void StoreEditedRooms (string roomId){
//
//		StartCoroutine (storeEditedRoomIds(LoggedIn_Username, roomId));
//
//	}
//
//	IEnumerator storeEditedRoomIds (string username, string roomId){
//
//		IEnumerator e = DCP.RunCS ("accounts", "ReplaceRoomData", new string[2] { username, roomId });
//
//		while (e.MoveNext ()) {
//			yield return e.Current;
//		}
//
//		string returnText = e.Current as string;
//
//		//activeRooms = returnText; 	
//
//		Debug.Log ("Stored: " + returnText);
//
//	}

	void BackToLobbyError() {
	
		messageBoard.transform.DOLocalMoveY (1300, 0.0001f);
		messageBoard.transform.DOLocalMoveY (0, 1.0f).SetEase (Ease.OutBounce);
		Invoke ("TakeOffScreen", 2.0f);
	
	}

	void TakeOffScreen(){
	
		messageBoard.transform.DOLocalMoveY (-1300, 1.0f).SetEase (Ease.InQuad);
	}

	public void RedoRoomSearch (){
	
		tempGameobject.GetComponent<TurnRoomButton> ().TurnRoomClicked ();
	
	}

	public void SendMessageToPlayersBox (string message, string[] player){
	
		StartCoroutine(sendMessageBox (message, player));

	}

	IEnumerator sendMessageBox (string message, string[] player){
		string URL = "http://dupesite.000webhostapp.com/mailboxMessage.php";

		//Debug.Log ("Message thing: " + message);

		WWWForm form = new WWWForm ();
		form.AddField ("username1Post", player[0]);
		form.AddField ("username2Post", player[1]);
		form.AddField ("username3Post", player[2]);
		form.AddField ("messagePost", message);

		WWW www = new WWW (URL, form);
		yield return www;

		string returnText = www.text;

		Debug.Log ("Stored Message: " + www.text);

	}

}
