using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LoadingAnimation : MonoBehaviour {

	public Transform[] letter;
	public Text[] letterText;
	public AnimationCurve upAndDown;
	int letNum = 0;
	Vector3 bigSize;
	bool goingUp = true;
	public Color[] letterColors;
	int colorNum;
	//Vector3 full360;


	// Use this for initialization
	void Start () {
		bigSize = new Vector3 (2.0f, 1.4f, 1);
		//full360 = new Vector3 (0, 0, 360);
		colorNum = Random.Range (0, 5);
		AnimateLetter();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void AnimateLetter(){

		letterText [letNum].color = letterColors [colorNum];
		letter [letNum].DOScale (bigSize, .13f).OnComplete(AddNum);
		letter [letNum].DOScale (Vector3.one, .13f).SetDelay (.11f);
	
	}

	void AddNum(){

		colorNum++;

		if (colorNum == 5) {
			colorNum = 0;
		}

		if (goingUp == true) {
			
			letNum = letNum + 1;
			if (letNum == 7) {
				//gameObject.transform.DORotate (full360, .7f, RotateMode.FastBeyond360);
				letNum = 5;
				goingUp = false;
			}

		} else {
			
			letNum = letNum - 1;
			if (letNum == -1) {
				letNum = 1;
				goingUp = true;
			}
		
		}

		AnimateLetter ();

	}

}
