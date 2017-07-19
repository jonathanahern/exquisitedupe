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

		} else if (mousePos.y > minY && mousePos.x < minX) {

			mousePos.y = minY;

		} else if (mousePos.y > minY && mousePos.x > maxX) {

			mousePos.y = minY;

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


	void OnMouseUpAsButton(){
	
//		if (stuck == true) {
//			return;
//		}
//
//		awardScript.CheckLocation ();

		localTurn.FlipSignToConfirm ();
	
	}

//	public void StopMoving(){
//	
//		stuck = true;
//
//	}

//	void Stuck(){
//	
//		stuck = true;
//
//	}

	public void CheckColor(){

		pos = transform.position;

		if (pos.x < -.003f && pos.y > .003f) {
			outerLayer.color = red;
		} else if (pos.x > .003f && pos.y > .003f) {
			outerLayer.color = blue;
		} else if (pos.x > .003f && pos.y < -.003f) {
			outerLayer.color = green;
		} else if (pos.x < -.003f && pos.y < -.003f) {
			outerLayer.color = orange;
		} else {
			outerLayer.color = offWhite;
		}

	}
		
//	public void SetSprite (int spriteNum){
	
//		if (spriteNum == 1) {
//		
//			awardPic.sprite = dupeSprite;
//		
//		} else if (spriteNum == 2) {
//
//			awardPic.sprite = deadGiveawaySprite;
//
//		} else if (spriteNum == 3) {
//
//			awardPic.sprite = simplyDivineSprite;
//
//		} else if (spriteNum == 4) {
//
//			awardPic.sprite = monkeyArtistSprite;
//
//		}
	
//	}

//	public void SetupVote(int fromColor){
//	
//		//stuck = true;
//		CheckColor ();
//		//awardPic.sprite = dupeSprite;
//
//		if (fromColor == 1) {
//			GetComponent<SpriteRenderer> ().sprite = redDupe;
//		} else if (fromColor == 2) {
//			GetComponent<SpriteRenderer> ().sprite = blueDupe;
//		} else if (fromColor == 3) {
//			GetComponent<SpriteRenderer> ().sprite = greenDupe;
//		} else if (fromColor == 4) {
//			GetComponent<SpriteRenderer> ().sprite = orangeDupe;
//		}
//
//	}

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

	public void SetupSecondVote(int fromColor, int awardNum){

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

	public void SetupThirdVote(int fromColor, string dupeCaught){

		Debug.Log ("DUPED?: " + dupeCaught);

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
