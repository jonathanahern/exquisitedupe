using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DatabaseControl;
using UnityEngine.UI;

public class ServerManagement : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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

}
