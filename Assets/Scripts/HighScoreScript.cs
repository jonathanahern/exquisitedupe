using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreScript : MonoBehaviour {

	public List<string> scoreList;
	public GameObject scoreObjPrefab;
	public Transform content;


	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// 4$jonathan|3$jonathan|6$jonathan|3$jonathan|

	public void TranslateToHighScoreList (string scores) {

		scoreList = new List<string> ();

		scores = scores.TrimEnd ('|');
		string[] scoreArray = scores.Split('|');
		foreach (string score in scoreArray) {
			scoreList.Add (score);
		}
		scoreList.Sort ();
		scoreList.Reverse ();

		for (int i = 0; i < scoreList.Count; i++) {

			string score;
			string playerName;

			string[] pieces = scoreList [i].Split ('$');

			score = pieces [0];
			playerName = pieces [1];

			GameObject scoreObj = Instantiate (scoreObjPrefab);
			scoreObj.transform.SetParent (content, false);
			scoreObj.transform.GetChild (1).gameObject.GetComponent<Text> ().text = score;
			scoreObj.transform.GetChild (0).gameObject.GetComponent<Text> ().text = playerName;

		}

	}


}
