using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class VoteFabScript : MonoBehaviour {

	Vector3 pos;

	private float minX = -2;
	private float maxX = 2;
	private float minY = -3.25f;
	private float maxY = 3.25f;

	Vector3 mousePos;
	public bool stuck = false;
	bool inFrame = false;

	public Color red; 
	public Color blue; 
	public Color green; 
	public Color orange; 
	public Color offWhite;

	public SpriteRenderer outerLayer;
	public SpriteRenderer innerSprite;

	public Sprite ribbonSprite;
	public Sprite splatterSprite;
	public Sprite stampSprite;
	public Sprite stampCircleSprite;
	Sprite rightSprite;

	public Sprite xOut;
	public Sprite outline;

	bool postDupe = false;
	int dupeNum;
	int myColor;

	public Sprite redDupe;
	public Sprite blueDupe;
	public Sprite greenDupe;
	public Sprite orangeDupe;

	public Sprite redMonkey;
	public Sprite blueMonkey;
	public Sprite greenMonkey;
	public Sprite orangeMonkey;

	public Sprite redVagueArtist;
	public Sprite blueVagueArtist;
	public Sprite greenVagueArtist;
	public Sprite orangeVagueArtist;

	public Sprite redCaptObvious;
	public Sprite blueCaptObvious;
	public Sprite greenCaptObvious;
	public Sprite orangeCaptObvious;

	public Sprite redMona;
	public Sprite blueMona;
	public Sprite greenMona;
	public Sprite orangeMona;

	public LocalTurnVoting localTurn;

	int badNum;
	public bool tutorialMode = false;

	void Start() {

		if (tutorialMode == true) {
			outerLayer.sprite = stampSprite;
			rightSprite = stampSprite;
		}

	}

	public void WiggleStart (){

		InvokeRepeating ("WiggleVote", 1.0f, 2.5f);

	}

	void WiggleVote(){

		gameObject.transform.DOShakeScale (1.5f, 1f, 10, 90);
		
	}

	void OnMouseDrag() {

		if (stuck == true) {
			return;
		}

		mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePos.z = -1.0f;

		if (inFrame == true) {
			
			if (mousePos.x < minX) {
				mousePos.x = minX;
			} else if (mousePos.x > maxX) {
				mousePos.x = maxX;
			} 

			if (mousePos.y < minY) {
				mousePos.y = minY;
			} else if (mousePos.y > maxY) {
				mousePos.y = maxY;
			}

			CheckColor ();

		} else if (mousePos.x > minX && mousePos.x < maxX && mousePos.y > minY && mousePos.y < maxY) {
			
			inFrame = true;
			// -3, -4.5
		} else if (mousePos.y > -4.5f && mousePos.x < -2.5f) {

			mousePos.y = -4.5f;


		} else if (mousePos.y > -4.5f && mousePos.x > 2.5f) {

			mousePos.y = -4.5f;

		}

		if (inFrame == false) {
		
			if (mousePos.x > 2.5f) {
				mousePos.x = 2.5f;
			} else if (mousePos.x < -2.5f) {
				mousePos.x = -2.5f;
			} 

			if (mousePos.y < -6f) {
				mousePos.y = -6f;
			}
		
		}

		transform.position = mousePos;

	}

	void OnMouseDown(){
		
		if (stuck == true) {
			return;
		}


		if (transform.parent != null) {
			Debug.Log ("SHAKE STOP");
			DOTween.Kill ("voteshake");
			CancelInvoke ();
			DOTween.PauseAll();
			transform.parent = null;
		}
			
		localTurn.FlipSignToWords ();

	}


	void OnMouseUp(){
		//0 onDupe, 1 offDupeandself, -1 onSelf

		if (inFrame == false) {
			return;
		}

		int onDupe;

		if (pos.x <= 0 && pos.y >= 0) {

			if (postDupe == true && dupeNum == 1) {
				onDupe = 0;
			} else if (postDupe == true && myColor == 1) {
				onDupe = -1;
			}else {
				onDupe = 1;
			}

		} else if (pos.x >= 0 && pos.y >= 0) {

			if (postDupe == true && dupeNum == 2) {
				onDupe = 0;
			} else if (postDupe == true && myColor == 2) {
				onDupe = -1;
			} else {
				onDupe = 1;
			}

		} else if (pos.x >= 0 && pos.y <= 0) {

			if (postDupe == true && dupeNum == 3) {
				onDupe = 0;
			} else if (postDupe == true && myColor == 3) {
				onDupe = -1;
			} else {
				onDupe = 1;
			}

		} else if (pos.x <= 0 && pos.y <= 0) {

			if (postDupe == true && dupeNum == 4) {
				onDupe = 0;
			} else if (postDupe == true && myColor == 4) {
				onDupe = -1;
			} else {
				onDupe = 1;
			}

		} else {
			onDupe = 1;
		}


		localTurn.FlipSignToConfirm (onDupe);
	
	}
		
	public void CheckColor(){

		pos = transform.position;

			if (pos.x <= 0 && pos.y >= 0) {

			if (postDupe == true && badNum == 1) {
				outerLayer.sprite = xOut;
			} else {
				outerLayer.sprite = rightSprite;
				outerLayer.color = red;
			}

			} else if (pos.x >= 0 && pos.y >= 0) {
			
			if (postDupe == true && badNum == 2) {
				outerLayer.sprite = xOut;
			} else {
				outerLayer.sprite = rightSprite;
				outerLayer.color = blue;
			}

			} else if (pos.x >= 0 && pos.y <= 0) {
			
			if (postDupe == true && badNum == 3) {
				outerLayer.sprite = xOut;
			} else {
				outerLayer.sprite = rightSprite;
				outerLayer.color = green;
			}

			} else if (pos.x <= 0 && pos.y <= 0) {
			
			if (postDupe == true && badNum == 4) {
				outerLayer.sprite = xOut;
			} else {
				outerLayer.sprite = rightSprite;
				outerLayer.color = orange;
			}

			} else {
			
				outerLayer.color = offWhite;
			}
			
	}

	public void SetupDupeVote(int fromColor){

		outerLayer.sprite = stampSprite;
		rightSprite = stampSprite;

		if (fromColor == 1) {
			innerSprite.sprite = redDupe;
		} else if (fromColor == 2) {
			innerSprite.sprite = blueDupe;
		} else if (fromColor == 3) {
			innerSprite.sprite = greenDupe;
		} else if (fromColor == 4) {
			innerSprite.sprite = orangeDupe;
		}

	}

	public void SetupSecondVote(int fromColor, int awardNum, int dupe){

		postDupe = true;
		dupeNum = dupe;
		myColor = fromColor;

		if (awardNum == 1) {
			badNum = dupe;
			outerLayer.sprite = splatterSprite;
			rightSprite = splatterSprite;
			if (fromColor == 1) {
				innerSprite.sprite = redMonkey;
			} else if (fromColor == 2) {
				innerSprite.sprite = blueMonkey;
			} else if (fromColor == 3) {
				innerSprite.sprite = greenMonkey;
			} else if (fromColor == 4) {
				innerSprite.sprite = orangeMonkey;
			}
		} else if (awardNum > 1) {
			badNum = myColor;
			outerLayer.sprite = ribbonSprite;
			rightSprite = ribbonSprite;
			if (fromColor == 1) {
				innerSprite.sprite = redMona;
			} else if (fromColor == 2) {
				innerSprite.sprite = blueMona;
			} else if (fromColor == 3) {
				innerSprite.sprite = greenMona;
			} else if (fromColor == 4) {
				innerSprite.sprite = orangeMona;
			}
		} 
	}

	public void SetupThirdVote(int fromColor, string dupeCaught, int dupe){

		postDupe = true;
		dupeNum = dupe;
		badNum = dupe;
		outerLayer.sprite = splatterSprite;
		rightSprite = splatterSprite;
		if (dupeCaught == "x") {
			if (fromColor == 1) {
				innerSprite.sprite = redVagueArtist;
			} else if (fromColor == 2) {
				innerSprite.sprite = blueVagueArtist;
			} else if (fromColor == 3) {
				innerSprite.sprite = greenVagueArtist;
			} else if (fromColor == 4) {
				innerSprite.sprite = orangeVagueArtist;
			}
		}  else {
			if (fromColor == 1) {
				innerSprite.sprite = redCaptObvious;
			} else if (fromColor == 2) {
				innerSprite.sprite = blueCaptObvious;
			} else if (fromColor == 3) {
				innerSprite.sprite = greenCaptObvious;
			} else if (fromColor == 4) {
				innerSprite.sprite = orangeCaptObvious;
			}
		}
	}

	public void SetupDupeReveal(int fromColor){

		outerLayer.sprite = stampCircleSprite;
		rightSprite = stampCircleSprite;

		if (fromColor == 1) {
			innerSprite.sprite = redDupe;
			outerLayer.sprite = stampCircleSprite;
		} else if (fromColor == 2) {
			innerSprite.sprite = blueDupe;
			outerLayer.sprite = stampCircleSprite;
		} else if (fromColor == 3) {
			innerSprite.sprite = greenDupe;
			outerLayer.sprite = stampCircleSprite;
		} else if (fromColor == 4) {
			innerSprite.sprite = orangeDupe;
			outerLayer.sprite = stampCircleSprite;
		}

	}

}
