using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteFabScript : MonoBehaviour {

	Vector3 pos;

	private float minX = -2;
	private float maxX = 2;
	private float minY = -3.25f;
	private float maxY = 3.25f;

	Vector3 mousePos;
	public bool stuck = true;
	bool inFrame = false;

	public Color red; 
	public Color blue; 
	public Color green; 
	public Color orange; 
	public Color offWhite;

	public SpriteRenderer outerLayer;
	public SpriteRenderer innerSprite;

	public Sprite xOut;
	public Sprite outline;

	bool postDupe = false;
	int dupeNum;

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

	public LocalTurnVoting localTurn;


	void Start() {

		
	}

	void OnMouseDrag() {

//		if (stuck == true) {
//			return;
//		}

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
		
//		if (stuck == true) {
//			return;
//		}
//
//
//		if (awardScript.voteSignOut == true) {
//		
//			awardScript.MoveItOut ();
//		
//		}

		if (transform.parent != null) {
			transform.parent = null;
		}

		localTurn.FlipSignToWords ();


	}


	void OnMouseUp(){
		//0 onDupe, 1 offDupe

		if (inFrame == false) {
			return;
		}

		int onDupe;

		if (pos.x <= 0 && pos.y >= 0) {

			if (postDupe == true && dupeNum == 1) {
				onDupe = 0;
			} else {
				onDupe = 1;
			}

		} else if (pos.x >= 0 && pos.y >= 0) {

			if (postDupe == true && dupeNum == 2) {
				onDupe = 0;
			} else {
				onDupe = 1;
			}

		} else if (pos.x >= 0 && pos.y <= 0) {

			if (postDupe == true && dupeNum == 3) {
				onDupe = 0;
			} else {
				onDupe = 1;
			}

		} else if (pos.x <= 0 && pos.y <= 0) {

			if (postDupe == true && dupeNum == 4) {
				onDupe = 0;
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

			if (postDupe == true && dupeNum == 1) {
				outerLayer.sprite = xOut;
			} else {
				outerLayer.sprite = outline;
				outerLayer.color = red;
			}

			} else if (pos.x >= 0 && pos.y >= 0) {
			
			if (postDupe == true && dupeNum == 2) {
				outerLayer.sprite = xOut;
			} else {
				outerLayer.sprite = outline;
				outerLayer.color = blue;
			}

			} else if (pos.x >= 0 && pos.y <= 0) {
			
			if (postDupe == true && dupeNum == 3) {
				outerLayer.sprite = xOut;
			} else {
				outerLayer.sprite = outline;
				outerLayer.color = green;
			}

			} else if (pos.x <= 0 && pos.y <= 0) {
			
			if (postDupe == true && dupeNum == 4) {
				outerLayer.sprite = xOut;
			} else {
				outerLayer.sprite = outline;
				outerLayer.color = orange;
			}

			} else {
			
				outerLayer.color = offWhite;
			}


	}

	public void SetupDupeVote(int fromColor){

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

		if (awardNum > 0) {
			if (fromColor == 1) {
				innerSprite.sprite = redMonkey;
			} else if (fromColor == 2) {
				innerSprite.sprite = blueMonkey;
			} else if (fromColor == 3) {
				innerSprite.sprite = greenMonkey;
			} else if (fromColor == 4) {
				innerSprite.sprite = orangeMonkey;
			}
		}
	}

	public void SetupThirdVote(int fromColor, string dupeCaught, int dupe){

		postDupe = true;
		dupeNum = dupe;

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

}
