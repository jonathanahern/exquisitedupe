using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinCategory : MonoBehaviour {

	string CreateUserURL = "http://dupesite.000webhostapp.com/createJoin.php";

	public string myName;
	public string myCat;

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.J)) {

			StartCoroutine (CreateJoin(myName, myCat));

		}

	}

	IEnumerator CreateJoin(string myName, string myCat){

		WWWForm form = new WWWForm ();
		form.AddField ("usernamePost", myName);
		form.AddField ("categoryPost", myCat);

		WWW www = new WWW (CreateUserURL, form);
		yield return www;

		Debug.Log (www.text);

	}

}
