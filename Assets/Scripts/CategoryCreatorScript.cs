using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryCreatorScript : MonoBehaviour {

	string myLineString;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CollectYourLineDataForCat () {

		GameObject[] lines = GameObject.FindGameObjectsWithTag ("Line");

		myLineString = "[GROUNDING]";

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

		Debug.Log (myLineString);

	}

}
