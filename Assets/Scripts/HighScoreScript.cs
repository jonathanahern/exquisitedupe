using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseControl;

public class HighScoreScript : MonoBehaviour {

	public List<string> scoreList;
	public List<GameObject> scoreObjList;
	public GameObject scoreObjPrefab;
	public Transform content;
	public GameObject loading;
	string username;

	bool okToClick = true;

	// Use this for initialization
	void Start () {


	}

	// 4$jonathan|3$jonathan|6$jonathan|3$jonathan|

	public void TranslateToHighScoreList (string scores) {

		scoreList = new List<string> ();
		scoreObjList = new List<GameObject> ();

		scores = scores.TrimEnd ('|');
		string[] scoreArray = scores.Split('|');
		foreach (string score in scoreArray) {
			scoreList.Add (score);
		}

		for (int i = 0; i < scoreList.Count; i++) {

			string score;
			string playerName;

			string[] pieces = scoreList [i].Split ('$');

			score = pieces [0];
			playerName = pieces [1];

			GameObject scoreObj = Instantiate (scoreObjPrefab);
			scoreObj.transform.SetParent (content, false);
			HighScoreObject highScore = scoreObj.GetComponent<HighScoreObject> ();
			highScore.score = int.Parse (score);
			highScore.playerName = playerName;
			scoreObj.transform.GetChild (1).gameObject.GetComponent<Text> ().text = score;
			scoreObj.transform.GetChild (0).gameObject.GetComponent<Text> ().text = playerName;

		}

		int childCount = content.childCount;

		for (int i = 0; i < childCount; i++) {

			scoreObjList.Add (content.GetChild (i).gameObject);
		}

		if (scoreObjList.Count > 0) {
			scoreObjList.Sort(delegate(GameObject a, GameObject b) {
				return (a.GetComponent<HighScoreObject>().score).CompareTo(b.GetComponent<HighScoreObject>().score);
			});
		}

		for (int i = 0; i < childCount; i++) {

			scoreObjList [i].transform.SetSiblingIndex (0);
		}

		loading.SetActive (false);

	}
		
	public void UpdateTheScore (){

		if (okToClick == false) {
			return;
		}

		okToClick = false;

		string points = "0";
		string currentRooms = "0";

		if (username == null) {
			UserAccountManagerScript userAccount = GameObject.FindGameObjectWithTag ("User Account Manager").GetComponent<UserAccountManagerScript> ();
			username = userAccount.loggedInUsername;
		}

		int contentCount = content.childCount;

		if (contentCount > 0) {
		
			for (int i = 0; i < contentCount; i++) {

				Destroy (content.GetChild (i).gameObject);

			}
		
		}

		loading.SetActive (true);

		StartCoroutine (updateHighScore(points, username, currentRooms));

	}

	IEnumerator updateHighScore (string points, string username, string currentRooms){

		IEnumerator e = DCP.RunCS ("accounts", "UpdateHighScore", new string[3] {points,username,currentRooms});

		while (e.MoveNext ()) {
			yield return e.Current;
		}

		string returnText = e.Current as string;

		//Debug.Log ("HighScore List:" + returnText);

		if (returnText.Length < 2) {
			UpdateTheScore ();
			yield break;
		}

		TranslateToHighScoreList (returnText);
		Invoke ("OkToClickAgain", 1.0f);

	}

	void OkToClickAgain (){

		okToClick = true;

	}

}
