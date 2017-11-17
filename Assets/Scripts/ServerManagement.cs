using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DatabaseControl;
using UnityEngine.UI;

using System.IO;

public class ServerManagement : MonoBehaviour {

	public Text roomID;

	public Text roomIDtoDelete;
	public Text roomIDs;
	public Text username;
	public Text catName;
	public Text inOut;
	public Text title;

//	public Text roomList;
//	public Text usernameAlterList;

	private static string DRAWING_SYM = "[DRAWING]";
	private static string MYCOLOR_SYM = "[MYCOLOR]";

	public GameObject redLine;
	public GameObject blueLine;
	public GameObject greenLine;
	public GameObject orangeLine;

	public GameObject redDot;
	public GameObject blueDot;
	public GameObject greenDot;
	public GameObject orangeDot;

	string shrinkString;
	string drawingTemp;

	string filePath;

	// Use this for initialization
	void Start () {

		filePath = Application.dataPath + "/PaintingArrays.txt";

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Alpha1)) {
		
			GameObject[] lines = GameObject.FindGameObjectsWithTag ("Line");
			foreach (GameObject item in lines) {
				Destroy (item);
			}

			GameObject[] dots = GameObject.FindGameObjectsWithTag ("Dot");
			foreach (GameObject item in dots) {
				Destroy (item);
			}
		
		}
		
	}

	public void BackToZero (){

		StartCoroutine (resetScoresZero());

	}

	IEnumerator resetScoresZero (){

		IEnumerator e = DCP.RunCS ("accounts", "ResetScoresZero");

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("Back To Zero?:" + returnText);

	}

//	public void AlterRoomList (){
//
//		StartCoroutine (alterRoomList());
//
//	}
//
//	IEnumerator alterRoomList (){
//
//		IEnumerator e = DCP.RunCS ("accounts", "AlterRoomList",new string[2] {roomList,usernameAlterList});
//
//		while (e.MoveNext ()) {
//			yield return e.Current;
//		}
//
//		string returnText = e.Current as string;
//
//		Debug.Log ("Altered To:" + returnText);
//
//	}

	public void GrabDrawing () {

		string roomIDstring = "|[ID]" + roomID.text;
		Debug.Log (roomIDstring);

		StartCoroutine (grabDrawing(roomIDstring));

	}

	IEnumerator grabDrawing (string roomID){

		IEnumerator e = DCP.RunCS ("turnRooms", "GrabDrawing",new string[1] {roomID});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("painting:" + returnText);

		if (returnText == "No Drawing") {
			yield break;
		}

		//string drawingString = returnText.Substring (DRAWING_SYM.Length);

		shrinkString = returnText;

		CreateDrawing (returnText);


	}

	void CreateDrawing (string drawing){

		drawing = drawing.TrimEnd ('$');

		drawingTemp = drawing + "|";

		string[] drawingInfos = drawing.Split ('$');

		foreach (string drawingInfo in drawingInfos) {

			string drawingString;
			string colorNum;
			string dotsString = "";

			string[] drawings = drawingInfo.Split (':');

			colorNum = drawings[0].Substring (MYCOLOR_SYM.Length);

			drawingString = drawings [1];
			dotsString = drawings [2];

			DrawLine (colorNum, drawingString, dotsString);


		}

	}

	void DrawLine (string colorNum, string drawing, string dotsString) {

		GameObject lineFab = new GameObject();
		GameObject dotFab = new GameObject();

		if (colorNum == "1") {
			lineFab = redLine;
			dotFab = redDot;
		} else if (colorNum == "2") {
			lineFab = blueLine;
			dotFab = blueDot;
		} else if (colorNum == "3") {
			lineFab = greenLine;
			dotFab = greenDot;
		} else if (colorNum == "4") {
			lineFab = orangeLine;
			dotFab = orangeDot;
		}

//		drawing = drawing.TrimStart ('+');
//		drawing = drawing.Replace ("++", "+");

		string[] lines = drawing.Split ('+');
		//Debug.Log (drawing);
		foreach (string line in lines) {

			GameObject lineGo = Instantiate (lineFab);
			LineRenderer lineRend = lineGo.GetComponent <LineRenderer> ();

			string[] points = line.Split ('@');

			lineRend.positionCount = points.Length;

			for (int i = 0; i < points.Length; i++) {
				//Debug.Log (points [i]);
				string[] vectArray = points [i].Split (',');
				Vector3 tempVect = new Vector3 (
					float.Parse (vectArray [0]),
					float.Parse (vectArray [1]),
					0);
				//Debug.Log (tempVect);

				lineRend.SetPosition (i, tempVect);

			}
		}

		if (dotsString == "") {
			return;
		}

		string[] dots = dotsString.Split ('+');

		foreach (string dot in dots) {

			GameObject lineGo = Instantiate (dotFab);
			LineRenderer lineRend = lineGo.GetComponent <LineRenderer> ();

			string[] points = dot.Split ('@');

			lineRend.positionCount = points.Length;

			for (int i = 0; i < points.Length; i++) {

				string[] vectArray = points [i].Split (',');

				Vector3 tempVect = new Vector3 (
					float.Parse (vectArray [0]),
					float.Parse (vectArray [1]),
					0);
				lineRend.SetPosition (i, tempVect);

			}
		}
	}

	public void ResetRooms (){

		Debug.Log ("Reset");

		StartCoroutine (resetRooms());
		StartCoroutine (resetEverything());

	}

	public void ChangeRoomNums (){

		Debug.Log ("Reset");


		StartCoroutine (changeRooms());

	}

	IEnumerator resetRooms (){

		IEnumerator e = DCP.RunCS ("accounts", "ResetRooms");

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("Cleared Rooms?:" + returnText);

	}

	IEnumerator resetEverything (){

		IEnumerator ee = DCP.RunCS ("turnRooms", "ClearEverything");

		while (ee.MoveNext ()) {
			yield return ee.Current;
		}

		string returnText = ee.Current as string;

		Debug.Log ("Cleared Everything?:" + returnText);

	}

	IEnumerator changeRooms (){

		string usernameString = username.text;
		string roomIDstring = roomIDs.text;

		IEnumerator e = DCP.RunCS ("accounts", "ChangeRooms", new string[2] {usernameString, roomIDstring});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("Changed Rooms?:" + returnText);

	}

	public void ShrinkRoom (){

		Debug.Log ("SHRUNK!");

		string roomIDstring = "|[ID]" + roomID.text;

//		string[] charsToRemove = new string[] { "(", ")", " "};
//		foreach (string character in charsToRemove)
//		{
//			shrinkString = shrinkString.Replace(character, string.Empty);
//		}
//
//		shrinkString = shrinkString.Replace("0.00", "0");
//		shrinkString = shrinkString.Replace("-0.", "-.");
//		shrinkString = shrinkString.Replace("0.", ".");

//		shrinkString = shrinkString.Replace("-0.", "-.");
//		shrinkString = shrinkString.Replace("0@", "@");
//		shrinkString = shrinkString.Replace("0.", ".");
//		shrinkString = shrinkString.Replace("0,", ",");
//
//		shrinkString = shrinkString.Replace("1.,", "1,");
//		shrinkString = shrinkString.Replace("2.,", "2,");
//		shrinkString = shrinkString.Replace(".,", "0,");
//		shrinkString = shrinkString.Replace(",.0@", ",0@");
//		shrinkString = shrinkString.Replace("1.0", "1,");
//		shrinkString = shrinkString.Replace("2.0", "2,");
//		shrinkString = shrinkString.Replace(".0,", "0,");
//		shrinkString = shrinkString.Replace(",.@", ",0@");
//

//		1., turn into 1,
//		2., turn into 2,
//			., turn into 0,
//			,.0@ turn into ,0@
//			1.0, turn into 1,
//			2.0, Turn into 2,
//			.0, turn into 0,
//			,.@ turn into ,0@

		shrinkString = shrinkString.Replace("1:+", "1:");
		shrinkString = shrinkString.Replace("++", "+");

		StartCoroutine (shrinkRoom(roomIDstring));

	}

	IEnumerator shrinkRoom (string roomIDstring){

		IEnumerator e = DCP.RunCS ("turnRooms", "ShrinkRoom", new string[2] {roomIDstring,shrinkString});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("Shrunk?:" + returnText);

	}

	public void DeleteRoom () {

		string roomIDDelete = "|[ID]" + roomIDtoDelete.text;
		Debug.Log (roomIDDelete);

		StartCoroutine (deleteRoom(roomIDDelete));

	}

	IEnumerator deleteRoom (string roomIDDelete){

		IEnumerator e = DCP.RunCS ("turnRooms", "DeleteRoom", new string[1] {roomIDDelete});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("Deleted?:" + returnText);

	}

	public void ChangeInOut () {

		StartCoroutine (changeInOut());

	}

	IEnumerator changeInOut (){

		string catString = "[ROOMTYPE]" + catName.text;
		string intOutString = inOut.text;

		Debug.Log ("sent: " + catString);

		IEnumerator e = DCP.RunCS ("categories", "AddInOutStat", new string[2] {catString, intOutString});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("Changed?:" + returnText);

	}

	public void StoreDrawing(){
	
		//Debug.Log (filePath);

		StreamWriter sWriter;

		if (!File.Exists (filePath)) {
		
			sWriter = File.CreateText (filePath);

		} else {
		
			sWriter = new StreamWriter (filePath, append: true);
		
		}

		drawingTemp = title.text + "^" + drawingTemp;

		sWriter.Write (drawingTemp);

		sWriter.Close ();
	
	}

	public void RestoreDrawing(){

		//Debug.Log (filePath);

		StreamReader sReader = new StreamReader(filePath);

		string allData = sReader.ReadToEnd ();

		sReader.Close ();

		allData = allData.TrimEnd ('|');

		string[] splitDrawings = allData.Split ('|');

		for (int i = 0; i < splitDrawings.Length; i++) {

			string[] splitTitle = splitDrawings[i].Split ('^');

			if (splitTitle [0] == title.text) {
				
				CreateDrawing (splitTitle[1]);

			}



		}

	}


}
