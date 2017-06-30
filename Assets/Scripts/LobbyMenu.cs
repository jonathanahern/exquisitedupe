using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LobbyMenu : MonoBehaviour {

	public static LobbyMenu instance;

	public GameObject mainButtons;
//	public GameObject turnBasedButtons;
	public RectTransform loadScreen;

	public RectTransform loginName;
	public RectTransform centerMainButts;
	public RectTransform newCats;
	private float startPos;
	public RectTransform centerTurnButts;

	Vector3 zeroCounter;
	Vector3 oneEighty;

	// Use this for initialization
	void Awake () {

		instance = this;

	}

	void Start(){

		oneEighty = new Vector3 (0, 0, 180.0f);
		zeroCounter = new Vector3 (0, 0, 360.0f);
		startPos = newCats.position.x;

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.C)) {
		
			//MoveCurtain ();
		
		}
		
	}

	public void TurnBasedClicked(){
		
		centerMainButts.DOLocalRotate (oneEighty, 1.0f).SetEase(Ease.OutQuad);
		centerTurnButts.DOLocalRotate (zeroCounter, 1.0f).SetEase(Ease.OutQuad);

		//DOTween.To(()=> center.GetComponent<RectTransform>().rotation, x=> center.GetComponent<RectTransform>().rotation = x, 180, 1.0f);

		//mainButtons.SetActive (false);
		//turnBasedButtons.SetActive (true);
	
	}

	public void NewTurnBased() {
	
		newCats.DOLocalMoveX (0, 2.0f).SetEase(Ease.OutBounce);
		centerTurnButts.DOLocalMoveX (startPos * -1.0f, 2.0f).SetEase(Ease.OutBounce);

	}

	public void NewCatsOffScreen(){
	
		newCats.DOLocalMoveX (startPos, 2.0f);
		centerTurnButts.DOLocalMoveX (0, 2.0f).SetEase(Ease.OutBounce);
	
	}

	public void LoadingScreenFromNewCats (){
	
		loginName.DOLocalMoveX (startPos, 2.0f);
		newCats.DOLocalMoveX (startPos, 2.0f);
		loadScreen.DOLocalMoveY (0, 2.0f).SetEase(Ease.OutBounce);
	
	}

	void MoveCurtain () {
	
		//curtain.DOAnchorPos (Vector2.zero, 2.0f).SetEase (Ease.InOutCirc).on;
	
	}

}
