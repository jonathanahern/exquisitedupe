using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AwardIconScript : MonoBehaviour {

	public Image awardIcon;
	public Image pointsIcon;
	public Text pointsText;

	public Color red;
	public Color blue;
	public Color green;
	public Color orange;
	public Color yellow;

	public Sprite star;
	public Sprite splatter;

	public Sprite escapeDupeRed;
	public Sprite escapeDupeBlue;
	public Sprite escapeDupeGreen;
	public Sprite escapeDupeOrange;

	public Sprite capturedDupeRed;
	public Sprite capturedDupeBlue;
	public Sprite capturedDupeGreen;
	public Sprite capturedDupeOrange;

	public Sprite rightDupeRed;
	public Sprite rightDupeBlue;
	public Sprite rightDupeGreen;
	public Sprite rightDupeOrange;

	public Sprite wrongDupeRed;
	public Sprite wrongDupeBlue;
	public Sprite wrongDupeGreen;
	public Sprite wrongDupeOrange;

	public Sprite monkeySprite;
	public Sprite monaSprite;
	public Sprite vagueSprite;
	public Sprite obviousSprite;
	public Sprite rightSprite;
	public Sprite wrongSprite;
	public Sprite rightDupeSprite;
	public Sprite wrongDupeSprite;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetupEscaped (int fromColor){
	
		if (fromColor == 1) {
			awardIcon.sprite = escapeDupeRed;
		} else if (fromColor == 2) {
			awardIcon.sprite = escapeDupeBlue;
		} else if (fromColor == 3) {
			awardIcon.sprite = escapeDupeGreen;
		} else if (fromColor == 4) {
			awardIcon.sprite = escapeDupeOrange;
		}

		pointsIcon.sprite = star;
		pointsText.text = "3";
	
	}

	public void SetupCaptured (int dupeNum){

		if (dupeNum == 1) {
			awardIcon.sprite = capturedDupeRed;
		} else if (dupeNum == 2) {
			awardIcon.sprite = capturedDupeBlue;
		} else if (dupeNum == 3) {
			awardIcon.sprite = capturedDupeGreen;
		} else if (dupeNum == 4) {
			awardIcon.sprite = capturedDupeOrange;
		}

		pointsIcon.sprite = star;
		pointsText.text = "2";

	}

	public void SetupMonkey (int fromColor){

		awardIcon.sprite = monkeySprite;
		pointsIcon.sprite = splatter;

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

		awardIcon.sprite = vagueSprite;
		if (points == 0) {
			pointsIcon.sprite = splatter;
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
			
			pointsIcon.sprite = star;
			pointsText.text = "2";

		}

	}

	public void SetupMona (int points){

		pointsIcon.sprite = star;
		pointsText.text = points.ToString();
		awardIcon.sprite = monaSprite;



	}

	public void SetupSplatter(int fromColor, int awardNum){
	
		pointsIcon.sprite = splatter;
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
			awardIcon.sprite = monkeySprite;
		} else if (awardNum == 2) {
			awardIcon.sprite = vagueSprite;
		} else if (awardNum == 3) {
			awardIcon.sprite = obviousSprite;
		}
	
	}

	public void SetupSplatterAvoider(int awardNum){
	
		pointsIcon.sprite = star;
		pointsIcon.color = yellow;
		pointsText.text = "2";

		if (awardNum == 1) {
			awardIcon.sprite = monkeySprite;
		} else if (awardNum == 2) {
			awardIcon.sprite = vagueSprite;
		} else if (awardNum == 3) {
			awardIcon.sprite = obviousSprite;
		}
	
	}

	public void SetupGuess(int points){

		pointsIcon.sprite = star;
		pointsIcon.color = yellow;
		pointsText.text = points.ToString();

		if (points > 0) {
			awardIcon.sprite = rightSprite;
		} else {
			awardIcon.sprite = wrongSprite;
		}

	}

	public void SetupDupeGuess(int points, int dupeNum){

		pointsIcon.sprite = star;
		pointsIcon.color = yellow;
		pointsText.text = points.ToString();

		if (points == 2) {
			
			if (dupeNum == 1) {
				awardIcon.sprite = wrongDupeRed;
			} else if (dupeNum == 2) {
				awardIcon.sprite = wrongDupeBlue;
			} else if (dupeNum == 3) {
				awardIcon.sprite = wrongDupeGreen;
			} else if (dupeNum == 4) {
				awardIcon.sprite = wrongDupeOrange;
			}

		} else {
			
			if (dupeNum == 1) {
				awardIcon.sprite = rightDupeRed;
			} else if (dupeNum == 2) {
				awardIcon.sprite = rightDupeBlue;
			} else if (dupeNum == 3) {
				awardIcon.sprite = rightDupeGreen;
			} else if (dupeNum == 4) {
				awardIcon.sprite = rightDupeOrange;
			}

		}

	}

}
