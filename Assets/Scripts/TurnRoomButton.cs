using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseControl;

public class TurnRoomButton : MonoBehaviour {

	public Text roomType;
	public string words;
	public string brushes;
	private string fate;

	// Use this for initialization
	void Start () {

		//Bear/Cow/Skunk/Horse/Pig/Rhino/Lion/Dog/Camel/Gerbil/Sloth/Cat
		//		-8.05f,0/6.33f,0/0,5.69f/0,-1.8f  ;

		//Confetti/Vomit/Bomb Blast/Messy Bed/Snake Pit/Rain Storm/Wheat Field/Spaghetti/Landfill/Tornado/Scrambled Eggs/Forest Fire
		//0,1.3f/0,6.7f/0,10.3f/0,-1.68f/0,-10.06f/2.24f,0/5.94f,0/-2.61f,0/-4.68f,0

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TurnRoomClicked(){

		int dupeNum = Random.Range (1, 5);
		int rightWord = Random.Range (1, 13);
		int wrongWord = Random.Range (1, 13);

		while(rightWord == wrongWord)
		{
			wrongWord = Random.Range (1, 13);
		}

		int awardNum = Random.Range (1, 3);

		fate = "|[WORDS]" + words + "|[BRUSHES]" + brushes + "|[FATE]" + dupeNum + "/" + rightWord + "/" + wrongWord + "/" + awardNum;

		UserAccountManagerScript.instance.TurnRoomSearch(roomType.text, fate);

		LobbyMenu.instance.LoadingScreenFromNewCats ();



	}

}
