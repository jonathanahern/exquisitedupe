using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour {

	string subject;
	LocalTurnVoting localTurn;

	public void SubjectGuessed(){

		subject = transform.GetChild (0).GetComponent<Text>().text;
		localTurn = GameObject.FindGameObjectWithTag ("Local Room").GetComponent<LocalTurnVoting> ();
		localTurn.GuessSubmitted (subject);

	}
}
