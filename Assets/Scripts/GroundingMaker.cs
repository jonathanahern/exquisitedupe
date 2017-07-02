using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundingMaker : MonoBehaviour {

	GameObject[] lines;
	string myLineString;
	public GameObject blackLine;

	private static string GROUNDING_SYM = "[GROUNDING]";



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.U)) {

			ConvertLinesToString ();
		}

		if (Input.GetKeyDown (KeyCode.I)) {

			ConvertStringToLines ();
		}

	}

	void ConvertLinesToString (){

		lines = GameObject.FindGameObjectsWithTag ("Line");

		myLineString = GROUNDING_SYM;

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
			myLineString = myLineString + "|";
		}
		myLineString = myLineString.TrimEnd('|');
		Debug.Log (myLineString);
		foreach (GameObject line in lines) {
			Destroy (line);
		}
	
	}

	void ConvertStringToLines (){
	
		string linesWhole = myLineString.Substring (GROUNDING_SYM.Length);
		string[] lines = linesWhole.Split ('|');

		foreach (string line in lines) {

			GameObject lineGo = Instantiate (blackLine);
			LineRenderer lineRend = lineGo.GetComponent <LineRenderer> ();

			string[] points = line.Split ('@');

			lineRend.numPositions = points.Length;

			for (int i = 0; i < points.Length; i++) {
				
				string[] vectArray = points[i].Split(',');

				Vector3 tempVect = new Vector3(
									float.Parse(vectArray[0]),
									float.Parse(vectArray[1]),
									0);
				lineRend.SetPosition (i, tempVect);

			}

		}
	
	}

}

