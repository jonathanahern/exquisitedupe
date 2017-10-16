using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

enum SpriteType { awardstroke, capObBlue, capObIcon, capObOrange, capObRed, dupeBlue, dupeCapturedBlue,
	dupeCapturedGreen, dupeCapturedOrange, dupeCapturedRed, dupeEscaped, dupeEscapedBlue,
	dupeEscapedGreen, dupeEscapedOrange, dupeEscapedRed, dupeGreen, dupeOrange, dupeRed, monaBlue,
	monaGreen, monaIcon, monaOrange, monaRed, monkeyBlack, monkeyBlue, monkeyGreen, monkeyOrange, monkeyRed,
	rightDupeGuessBlue, rightDupeGuessGreen, rightDupeGuessOrange, rightDupeGuessRed, rightGuess, splatter, splatter2,
	stamp, stampCircle, star, trophyIcon, vagueBlack, vagueBlue, vagueGreen, vagueOrange, vagueRed, wrongDupeGuessBlue,
	wrongDupeGuessGreen, wrongDupeGuessOrange, wrongDupeGuessRed, wrongGuess, xOut}

public class AwardIconScript : MonoBehaviour {

	[SerializeField]
	private SpriteAtlas atlas;

	public Image awardIcon;
	public Image pointsIcon;
	public Text pointsText;

	public Color red;
	public Color blue;
	public Color green;
	public Color orange;
	public Color yellow;

//	public Sprite star;
//	public Sprite splatter;
//
//	public Sprite escapeDupeRed;
//	public Sprite escapeDupeBlue;
//	public Sprite escapeDupeGreen;
//	public Sprite escapeDupeOrange;
//
//	public Sprite capturedDupeRed;
//	public Sprite capturedDupeBlue;
//	public Sprite capturedDupeGreen;
//	public Sprite capturedDupeOrange;
//
//	public Sprite rightDupeRed;
//	public Sprite rightDupeBlue;
//	public Sprite rightDupeGreen;
//	public Sprite rightDupeOrange;
//
//	public Sprite wrongDupeRed;
//	public Sprite wrongDupeBlue;
//	public Sprite wrongDupeGreen;
//	public Sprite wrongDupeOrange;
//
//	public Sprite monkeySprite;
//	public Sprite monaSprite;
//	public Sprite vagueSprite;
//	public Sprite obviousSprite;
//	public Sprite rightSprite;
//	public Sprite wrongSprite;
//	public Sprite trophySprite;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetupEscaped (int fromColor){
	
		if (fromColor == 1) {
			awardIcon.sprite = atlas.GetSprite ("dupeEscapedRed");
		} else if (fromColor == 2) {
			awardIcon.sprite = atlas.GetSprite ("dupeEscapedBlue");
		} else if (fromColor == 3) {
			awardIcon.sprite = atlas.GetSprite ("dupeEscapedGreen");
		} else if (fromColor == 4) {
			awardIcon.sprite = atlas.GetSprite ("dupeEscapedOrange");
		}

		pointsIcon.sprite = atlas.GetSprite ("star");
		pointsText.text = "3";
	
	}

	public void SetupCaptured (int dupeNum){

		if (dupeNum == 1) {
			awardIcon.sprite = atlas.GetSprite ("dupeCapturedRed");
		} else if (dupeNum == 2) {
			awardIcon.sprite = atlas.GetSprite ("dupeCapturedBlue");
		} else if (dupeNum == 3) {
			awardIcon.sprite = atlas.GetSprite ("dupeCapturedGreen");
		} else if (dupeNum == 4) {
			awardIcon.sprite = atlas.GetSprite ("dupeCapturedOrange");
		}

		pointsIcon.sprite = atlas.GetSprite ("star");
		pointsText.text = "2";

	}

	public void SetupDupeFound (){

		awardIcon.sprite = atlas.GetSprite ("trophyIcon");

		pointsIcon.sprite = atlas.GetSprite ("star");
		pointsText.text = "1";

	}

	public void SetupMonkey (int fromColor){

		awardIcon.sprite = atlas.GetSprite ("monkeyBlack");
		pointsIcon.sprite = atlas.GetSprite ("splatter2");

		if (fromColor == 1) {
			pointsIcon.color = red;
		} else if (fromColor == 2) {
			pointsIcon.color = blue;
		} else if (fromColor == 3) {
			pointsIcon.color = green;
		} else if (fromColor == 4) {
			pointsIcon.color = orange;
		}

		pointsText.text = "0";

	}

	public void SetupVague (int fromColor, int points){

		awardIcon.sprite = atlas.GetSprite ("vagueBlack");
		if (points == 0) {
			pointsIcon.sprite = atlas.GetSprite ("splatter2");
			pointsText.text = "0";

			if (fromColor == 1) {
				pointsIcon.color = red;
			} else if (fromColor == 2) {
				pointsIcon.color = blue;
			} else if (fromColor == 3) {
				pointsIcon.color = green;
			} else if (fromColor == 4) {
				pointsIcon.color = orange;
			}


		} else {
			
			pointsIcon.sprite  = atlas.GetSprite ("star");
			pointsText.text = "2";

		}

	}

	public void SetupMona (int points){

		pointsIcon.sprite = atlas.GetSprite ("star");
		pointsText.text = points.ToString();
		awardIcon.sprite = atlas.GetSprite ("monaIcon");

	}

	public void AddAPointToMona(){

		int oldNum = int.Parse (pointsText.text);
		int newNum = oldNum + 1;
		pointsText.text = newNum.ToString();
	
	}

	public void SetupSplatter(int fromColor, int awardNum){
	
		pointsIcon.sprite = atlas.GetSprite ("splatter2");
		pointsText.text = "0";

		if (fromColor == 1) {
			pointsIcon.color = red;
		} else if (fromColor == 2) {
			pointsIcon.color = blue;
		} else if (fromColor == 3) {
			pointsIcon.color = green;
		} else if (fromColor == 4) {
			pointsIcon.color = orange;
		}
	
		if (awardNum == 1) {
			awardIcon.sprite = atlas.GetSprite ("monkeyBlack");
		} else if (awardNum == 2) {
			awardIcon.sprite = atlas.GetSprite ("vagueBlack");
		} else if (awardNum == 3) {
			awardIcon.sprite = atlas.GetSprite ("capObIcon");
		}
	
	}

	public void SetupSplatterAvoider(int awardNum){
	
		pointsIcon.sprite = atlas.GetSprite ("star");
		pointsIcon.color = yellow;


		if (awardNum == 1) {
			awardIcon.sprite = atlas.GetSprite ("monkeyBlack");
			pointsText.text = "1";
		} else if (awardNum == 2) {
			awardIcon.sprite = atlas.GetSprite ("vagueBlack");
			pointsText.text = "2";
		} else if (awardNum == 3) {
			awardIcon.sprite = atlas.GetSprite ("capObIcon");
			pointsText.text = "2";
		}
	
	}

	public void SetupGuess(int points){

		pointsIcon.sprite = atlas.GetSprite ("star");
		pointsIcon.color = yellow;
		pointsText.text = points.ToString();

		if (points > 0) {
			awardIcon.sprite = atlas.GetSprite ("rightGuess");
		} else {
			awardIcon.sprite = atlas.GetSprite ("wrongGuess");
		}

	}

	public void SetupDupeGuess(int points, int dupeNum){

		pointsIcon.sprite = atlas.GetSprite ("star");
		pointsIcon.color = yellow;
		pointsText.text = points.ToString();

		if (points == 2) {
			
			if (dupeNum == 1) {
				awardIcon.sprite = atlas.GetSprite ("wrongDupeGuessRed");
			} else if (dupeNum == 2) {
				awardIcon.sprite = atlas.GetSprite ("wrongDupeGuessBlue");
			} else if (dupeNum == 3) {
				awardIcon.sprite = atlas.GetSprite ("wrongDupeGuessGreen");
			} else if (dupeNum == 4) {
				awardIcon.sprite = atlas.GetSprite ("wrongDupeGuessOrange");
			}

		} else {
			
			if (dupeNum == 1) {
				awardIcon.sprite = atlas.GetSprite ("rightDupeGuessRed");
			} else if (dupeNum == 2) {
				awardIcon.sprite = atlas.GetSprite ("rightDupeGuessBlue");
			} else if (dupeNum == 3) {
				awardIcon.sprite = atlas.GetSprite ("rightDupeGuessGreen");
			} else if (dupeNum == 4) {
				awardIcon.sprite = atlas.GetSprite ("rightDupeGuessOrange");
			}

		}

	}

}
