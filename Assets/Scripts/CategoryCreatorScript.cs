using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DatabaseControl;

public class CategoryCreatorScript : MonoBehaviour {

	string myLineString;
	string myRoomType;
	string myWordsString = string.Empty;
	string myBrushesString;

	public Text[] words;
	public Text roomType;
	public Text catName;

	public Text[] wordsAlter;
	public Text roomTypeAlter;
	public GameObject wordsPanelAlter;

	public GameObject wordsPanel;

	public Text newBrushesText;
	public Text newBrushesTextTitle;
	public GameObject brushesPanel;
	public GameObject blackLine;

	private static string ROOMTYPE_SYM = "[ROOMTYPE]";
	private static string WORDS_SYM = "[WORDS]";
	private static string BRUSHES_SYM = "[BRUSHES]";
	private static string GROUNDING_SYM = "[GROUNDING]";

	public string cat1;
	public string words1;
	public string brushes1;
	public string grounding1;

	string createCategoryURL = "http://dupesite.000webhostapp.com/insertNewCategory.php";

	void Update () {

		if (Input.GetKeyDown (KeyCode.A)) {
		}

	}

	IEnumerator createCat (){
	
		WWWForm form = new WWWForm ();
		form.AddField ("categoryPost", cat1);
		form.AddField ("wordsPost", words1);
		form.AddField ("brushesPost", brushes1);
		form.AddField ("groundingPost", grounding1);

		WWW w = new WWW (createCategoryURL, form);
		yield return w;

		Debug.Log (w.text);

	}

	public void CollectAllData(){
	
		CollectYourLineDataForCat ();
		CollectWords ();
		CollectRoomType ();
		CollectBrushes ();
		StartCoroutine (sendAllDataToServer());

	}

	public void AlterCategory(){

		myRoomType = string.Empty;
		myWordsString = string.Empty;

		CollectWordsAlter ();
		CollectRoomTypeAlter ();

		StartCoroutine (sendAlteredDataToServer());

	}

	void CollectYourLineDataForCat () {

		GameObject[] lines = GameObject.FindGameObjectsWithTag ("Line");

		myLineString = "";

		foreach (GameObject line in lines) {

			LineRenderer lineRend = line.GetComponent<LineRenderer> ();
			int lineAmount = lineRend.positionCount;

			for (int i = 0; i < lineAmount; i++) {

				Vector2 point = lineRend.GetPosition (i);

				myLineString = myLineString + point.ToString ("F3") + "@";

				if (i == lineAmount - 1) {

					string[] charsToRemove = new string[] { "(", ")", " "};
					foreach (string character in charsToRemove)
					{
						myLineString = myLineString.Replace(character, string.Empty);
					}

					myLineString = myLineString.Replace("0.000", "0");
					myLineString = myLineString.Replace("-0.", "-.");
					myLineString = myLineString.Replace("0.", ".");
					myLineString = myLineString.Replace("$$", "$");

				}

			}

			myLineString = myLineString.TrimEnd('@');
			myLineString = myLineString + "$";

		}

		myLineString = myLineString.TrimEnd('$');
		//myLineString = "|" + GROUNDING_SYM + myLineString;


	}

	void CollectWords(){
	
		for (int i = 0; i < words.Length; i++) {

			myWordsString = myWordsString + words [i].text + "/";

		}

		myWordsString = myWordsString.TrimEnd ('/');
		//myWordsString = "|" + WORDS_SYM + myWordsString;

	}

	void CollectWordsAlter(){

		for (int i = 0; i < wordsAlter.Length; i++) {

			myWordsString = myWordsString + wordsAlter [i].text + "/";

		}

		myWordsString = myWordsString.TrimEnd ('/');
		//myWordsString = "|" + WORDS_SYM + myWordsString;

	}

	void CollectRoomType (){
	
		myRoomType = roomType.text;
		//myRoomType = ROOMTYPE_SYM + myRoomType;
	
	}

	void CollectRoomTypeAlter (){

		myRoomType = roomTypeAlter.text;
		//myRoomType = ROOMTYPE_SYM + myRoomType;

	}

	void CollectBrushes (){
	
		GameObject[] brushes = GameObject.FindGameObjectsWithTag ("Brush Marker");

		for (int i = 0; i < brushes.Length; i++) {

			Vector2 pos = brushes [i].transform.position;
			string posString = pos.ToString ();
			posString = posString.TrimEnd (')');
			posString = posString.TrimStart ('(');

			myBrushesString = myBrushesString + posString + "/";

		}

		myBrushesString = myBrushesString.TrimEnd ('/');
		//myBrushesString = "|" + BRUSHES_SYM + myBrushesString;

	}

	IEnumerator sendAllDataToServer (){

		WWWForm form = new WWWForm ();
		form.AddField ("categoryPost", myRoomType);
		form.AddField ("wordsPost", myWordsString);
		form.AddField ("brushesPost", myBrushesString);
		form.AddField ("groundingPost", myLineString);

		WWW w = new WWW (createCategoryURL, form);
		yield return w;

//		IEnumerator e = DCP.RunCS ("categories", "AddCategory", new string[4] {myLineString, myRoomType, myWordsString, myBrushesString});
//
//		Debug.Log (myLineString + "&&&" + myRoomType + "&&&" +  myWordsString + "&&&" + myBrushesString);
//
//		while (e.MoveNext ()) {
//			yield return e.Current;
//		}
//
//		string returnText = e.Current as string;

		Debug.Log ("Cat Added:" + w.text);

	}

	IEnumerator sendAlteredDataToServer(){

		IEnumerator e = DCP.RunCS ("categories", "AlterCategory", new string[2] {myRoomType, myWordsString});

		Debug.Log (myRoomType + "&&&" +  myWordsString);

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("Cat Altered:" + returnText);
	}

	public void SendAlteredBrushes(){
	
		StartCoroutine (sendAlteredBrushes());
	
	}

	IEnumerator sendAlteredBrushes(){

		string myNewRoomType = ROOMTYPE_SYM + newBrushesTextTitle.text;
		string myAlteredBrushes = newBrushesText.text;

		IEnumerator e = DCP.RunCS ("categories", "AlterBrushes", new string[2] {myNewRoomType, myAlteredBrushes});

		Debug.Log (myNewRoomType + "&&&" +  myAlteredBrushes);

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("Cat Altered:" + returnText);
	}

	public void MoveAddWordsPanel(){

		RectTransform rectTran = wordsPanel.GetComponent<RectTransform>();
		float pos = rectTran.anchoredPosition.x;

		LineSpawnerScipt lineSpawn = GameObject.FindGameObjectWithTag ("Line Spawner").GetComponent<LineSpawnerScipt> ();

		Debug.Log (pos);

		if (pos > 1000) {
			lineSpawn.dontDraw = true;
			rectTran.DOAnchorPosX (0, 1.0f);
		} else {
			lineSpawn.dontDraw = false;
			rectTran.DOAnchorPosX (1500, 1.0f);
		}
	
	}

	public void MoveAddWordsPanelAlter(){

		RectTransform rectTran = wordsPanelAlter.GetComponent<RectTransform>();
		float pos = rectTran.anchoredPosition.x;

		LineSpawnerScipt lineSpawn = GameObject.FindGameObjectWithTag ("Line Spawner").GetComponent<LineSpawnerScipt> ();

		Debug.Log (pos);

		if (pos > 1000) {
			lineSpawn.dontDraw = true;
			rectTran.DOAnchorPosX (0, 1.0f);
		} else {
			lineSpawn.dontDraw = false;
			rectTran.DOAnchorPosX (1500, 1.0f);
		}

	}

	public void MoveAlterBrushesPanel(){

		RectTransform rectTran = brushesPanel.GetComponent<RectTransform>();
		float pos = rectTran.anchoredPosition.x;

		LineSpawnerScipt lineSpawn = GameObject.FindGameObjectWithTag ("Line Spawner").GetComponent<LineSpawnerScipt> ();

		Debug.Log (pos);

		if (pos > 1000) {
			lineSpawn.dontDraw = true;
			rectTran.DOAnchorPosX (0, 1.0f);
		} else {
			lineSpawn.dontDraw = false;
			rectTran.DOAnchorPosX (1500, 1.0f);
		}

	}

	public void ReturnDrawing (){
	
		StartCoroutine (getDrawing());

	}

	IEnumerator getDrawing(){

		string myNewRoomType = ROOMTYPE_SYM + catName.text;

		IEnumerator e = DCP.RunCS ("categories", "GetGrounding", new string[1] {myNewRoomType});

		Debug.Log (myNewRoomType);

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("Cat Altered:" + returnText);

		DrawLine (returnText);

	}
		

	void DrawLine (string drawing) {

		drawing = drawing.Substring (GROUNDING_SYM.Length + 1);

		GameObject lineFab = blackLine;

		string[] lines = drawing.Split ('$');
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
				
				lineRend.SetPosition (i, tempVect);

			}
		}
	
	}

}
