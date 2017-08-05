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

	public GameObject wordsPanel;

	private static string ROOMTYPE_SYM = "[ROOMTYPE]";
	private static string WORDS_SYM = "[WORDS]";
	private static string BRUSHES_SYM = "[BRUSHES]";
	private static string GROUNDING_SYM = "[GROUNDING]";

	public void CollectAllData(){
	
		CollectYourLineDataForCat ();
		CollectWords ();
		CollectRoomType ();
		CollectBrushes ();
		StartCoroutine (sendAllDataToServer());

	}

	void CollectYourLineDataForCat () {

		GameObject[] lines = GameObject.FindGameObjectsWithTag ("Line");

		myLineString = "";

		foreach (GameObject line in lines) {

			LineRenderer lineRend = line.GetComponent<LineRenderer> ();
			int lineAmount = lineRend.numPositions;

			for (int i = 0; i < lineAmount; i++) {

				Vector2 point = lineRend.GetPosition (i);


				myLineString = myLineString + point.ToString ("F2") + "@";

				if (i == lineAmount - 1) {

					string[] charsToRemove = new string[] { "(", ")" };
					foreach (string character in charsToRemove)
					{
						myLineString = myLineString.Replace(character, string.Empty);
					}

				}

			}

			myLineString = myLineString.TrimEnd('@');
			myLineString = myLineString + "$";

		}

		myLineString = myLineString.TrimEnd('$');
		myLineString = "|" + GROUNDING_SYM + myLineString;


	}

	void CollectWords(){
	
		for (int i = 0; i < words.Length; i++) {

			myWordsString = myWordsString + words [i].text + "/";

		}

		myWordsString = myWordsString.TrimEnd ('/');
		myWordsString = "|" + WORDS_SYM + myWordsString;

	}

	void CollectRoomType (){
	
		myRoomType = roomType.text;
		myRoomType = ROOMTYPE_SYM + myRoomType;
	
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
		myBrushesString = "|" + BRUSHES_SYM + myBrushesString;

	}

	IEnumerator sendAllDataToServer (){

		IEnumerator e = DCP.RunCS ("categories", "AddCategory", new string[4] {myLineString, myRoomType, myWordsString, myBrushesString});

		Debug.Log (myLineString + "&&&" + myRoomType + "&&&" +  myWordsString + "&&&" + myBrushesString);

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		Debug.Log ("Cat Added:" + returnText);

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

}
