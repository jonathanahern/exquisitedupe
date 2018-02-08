using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EDAnimations : MonoBehaviour {

	//Vector3 doubleSize;
	public static EDAnimations Instance { get; private set;}

	//GameObject tempObject;
	public AnimationCurve shakeVanish;

	// Use this for initialization
	void Awake () {

		Instance = this;
		//doubleSize = new Vector3 (.05f, .05f, .05f);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void BurstDisappear (GameObject theObject){
	
		theObject.transform.DOScale (Vector3.zero, .5f).SetEase(shakeVanish);
		//theObject.transform.DOPunchScale (doubleSize, .5f).OnComplete (Disappear);;
		//tempObject = theObject;
	}

//	public void Disappear (){
//	
//		tempObject.transform.DOScale (Vector3.zero, 0.5f);
//	
//	}
}


