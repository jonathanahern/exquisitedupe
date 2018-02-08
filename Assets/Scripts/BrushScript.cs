using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BrushScript : MonoBehaviour {

	public bool XNeg;
	public bool XPos;
	public bool YNeg;
	public bool YPos;

	GameObject parentBrush;
	Vector3 parentBrushPos;
	Vector3 brushPosMid;

	public GameObject posHandle;
	public GameObject negHandle;

	bool hit;

	public bool xBrush;
	public bool yBrush;

	// Use this for initialization
	void Start () {

		if (XNeg == true) {
			parentBrush = transform.parent.gameObject;
			parentBrushPos = parentBrush.transform.position;
			brushPosMid = new Vector3 (parentBrushPos.x, parentBrushPos.y - .01f, parentBrushPos.z);

		} else if (XPos == true) {
			parentBrush = transform.parent.gameObject;
			parentBrushPos = parentBrush.transform.position;
			brushPosMid = new Vector3 (parentBrushPos.x, parentBrushPos.y + .01f, parentBrushPos.z);

		} else if (YNeg == true) {
			parentBrush = transform.parent.gameObject;
			parentBrushPos = parentBrush.transform.position;
			brushPosMid = new Vector3 (parentBrushPos.x - .01f, parentBrushPos.y, parentBrushPos.z);

		} else if (YPos == true) {
			parentBrush = transform.parent.gameObject;
			parentBrushPos = parentBrush.transform.position;
			brushPosMid = new Vector3 (parentBrushPos.x + .01f, parentBrushPos.y, parentBrushPos.z);

		}

	}


	void OnTriggerEnter(Collider other) {

		if (hit == true || other.tag != "Line Collider") {
			return;
		} 
			
		hit = true;

		parentBrush = transform.parent.gameObject;
		parentBrushPos = parentBrush.transform.position;

		if (XNeg == true) {
			brushPosMid = new Vector3 (parentBrushPos.x, parentBrushPos.y - .01f, parentBrushPos.z);

		} else if (XPos == true) {
			brushPosMid = new Vector3 (parentBrushPos.x, parentBrushPos.y + .01f, parentBrushPos.z);

		} else if (YNeg == true) {
			brushPosMid = new Vector3 (parentBrushPos.x - .01f, parentBrushPos.y, parentBrushPos.z);

		} else if (YPos == true) {
			brushPosMid = new Vector3 (parentBrushPos.x + .01f, parentBrushPos.y, parentBrushPos.z);

		}

		Invoke ("MakeUnHit", 1.5f);

		if (XNeg == true) {
			parentBrush.transform.DOLocalMoveY (1.0f, 1.0f).SetEase(Ease.InQuart);
			parentBrush.transform.DOShakeRotation (.4f, 30, 10);
		
		} else if (XPos == true) {
			parentBrush.transform.DOLocalMoveY (-1.0f, 1.0f).SetEase(Ease.InQuart);
			parentBrush.transform.DOShakeRotation (.4f, 30, 10);

		} else if (YNeg == true) {
			parentBrush.transform.DOLocalMoveX (1.0f, 1.0f).SetEase(Ease.InQuart);
			parentBrush.transform.DOShakeRotation (.4f, 30, 10);

		} else if (YPos == true) {
			parentBrush.transform.DOLocalMoveX (-1.0f, 1.0f).SetEase(Ease.InQuart);
			parentBrush.transform.DOShakeRotation (.4f, 30, 10);

		}

		//Debug.Log (brushPosMid.x + "," + brushPosMid.y);
		other.transform.parent.gameObject.GetComponent<LineScript> ().HitBrush (parentBrushPos, brushPosMid, parentBrush);

	}

	void MakeUnHit (){
	
		hit = false;
	
	}

	public void TurnOnHandle(int colorNum){
	
		if (colorNum == 1) {

			if (xBrush == true) {
				posHandle.SetActive (true);
			}

			if (yBrush == true) {
				negHandle.SetActive (true);
			}

		} else if (colorNum == 2) {

			if (xBrush == true) {
				posHandle.SetActive (true);
			}

			if (yBrush == true) {
				posHandle.SetActive (true);
			}

		} else if (colorNum == 3) {

			if (xBrush == true) {
				negHandle.SetActive (true);
			}

			if (yBrush == true) {
				posHandle.SetActive (true);
			}

		} else if (colorNum == 4) {

			if (xBrush == true) {
				negHandle.SetActive (true);
			}

			if (yBrush == true) {
				negHandle.SetActive (true);
			}

		}
			
	}

}
