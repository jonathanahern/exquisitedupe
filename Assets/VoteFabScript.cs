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
	public Sprite dupeSprite;
	public Sprite deadGiveawaySprite;
	public Sprite simplyDivineSprite;
	public Sprite monkeyArtistSprite;

	public Sprite redDupe;
	public Sprite blueDupe;
	public Sprite greenDupe;
	public Sprite orangeDupe;


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

			}

		transform.position = mousePos;

	}

//	void OnMouseDown(){
//		
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
//
//
//	}
//
//
//	void OnMouseUpAsButton(){
//	
//		if (stuck == true) {
//			return;
//		}
//
//		awardScript.CheckLocation ();
//	
//	}

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

		if (pos.x < -.3f && pos.y > .3f) {
			outerLayer.color = red;
		} else if (pos.x > .3f && pos.y > .3f) {
			outerLayer.color = blue;
		} else if (pos.x > .3f && pos.y < -.3f) {
			outerLayer.color = green;
		} else if (pos.x < -.3f && pos.y < -.3f) {
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

}
