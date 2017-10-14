using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TutorialExitScript : MonoBehaviour {

	RoomManager roomMan;

	public RectTransform messageBoard;
	float offscreen;


	// Use this for initialization
	void Start () {

		offscreen = messageBoard.anchoredPosition.y;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MoveDownBoard (){
	
		messageBoard.DOAnchorPosY (0, .75f).SetEase (Ease.OutQuad);

	}

	public void MoveBackUp(){
	 
		messageBoard.DOAnchorPosY (offscreen, .75f).SetEase (Ease.InQuad);

	}

	public void Exit(){

		roomMan = GameObject.FindGameObjectWithTag ("Room Manager").GetComponent<RoomManager>();

		roomMan.CurtainsIn ();
		roomMan.cameFromTutorial = true;
		roomMan.cameFromTurnBased = true;
		Invoke ("EndTutorial", 2.5f);


	}

	void EndTutorial (){
		
		SceneManager.LoadScene ("Lobby Menu");

	}
}
