using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraScript : MonoBehaviour {

	bool zoomIn;
	float camZoom;

	public GameObject center;
	private Vector3 centerPos;

	public Camera seekerCam;
	private bool moveCamera = false;

	float direction = 1.0f;
	float goalX;
	float moveRate;

	public LocalRoomManager localRoomMan;
	public TutorialPhaseOneScript tutorialOne;

	// Use this for initialization
	void Start () {

		moveRate = .019f;

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Z)) {

			ZoomIn (4);

		}

		if (moveCamera == true) {

			centerPos = center.transform.position;
			Vector3 screenPoint = seekerCam.WorldToViewportPoint (centerPos);

			float diff = screenPoint.x - goalX;

//			Debug.Log (screenPoint.x);

			if (diff > .0025f) {
				moveCamera = true;
				direction = 1.0f;
			} else if (diff < -.0025f) {
				moveCamera = true;
				direction = -1.0f;
			} else {
				moveCamera = false;
			}

				Vector3 nextPos = new Vector3 (seekerCam.transform.position.x + moveRate * direction,
					seekerCam.transform.position.y,
					seekerCam.transform.position.z);

				seekerCam.transform.position = nextPos;

//			camZoom = Camera.main.orthographicSize;
//			Camera.main.orthographicSize = Mathf.Lerp (camZoom, 2.5f, Time.deltaTime * 1.1f);
//
//			if (camZoom < 2.52) {
//			
//				Camera.main.orthographicSize = 2.5f;
//				zoomIn = false;
//
//			}

		}

	}

	public void ZoomIn (int playerNum){
	
		seekerCam.orthographicSize = 2.5f;

		if (playerNum == 1) {
			goalX = 1;
			seekerCam.transform.position = new Vector3 (-1.35f, 2, -10);
			moveCamera = true;

		} else if (playerNum == 2) {
			goalX = 0;
			seekerCam.transform.position = new Vector3 (1.35f, 2, -10);
			moveCamera = true;

		} else if(playerNum == 3) {
			goalX = 0;
			seekerCam.transform.position = new Vector3 (1.35f, -2, -10);
			moveCamera = true;

		} else if (playerNum == 4) {
			goalX = 1;
			seekerCam.transform.position = new Vector3 (-1.35f, -2, -10);
			moveCamera = true;

		}
	
	}

	public void MoveToSection () {

		Camera.main.transform.DOMove (seekerCam.transform.position, 2.0f).OnComplete(StartTimer);
		DOTween.To(()=> Camera.main.orthographicSize, x=> Camera.main.orthographicSize = x, seekerCam.orthographicSize, 2.0f);

	}

	void StartTimer(){
	
		localRoomMan.StartGame ();

	}

	public void MoveToSectionTutorial () {

		moveCamera = false;

		Vector3 camPos = seekerCam.transform.position;

		Camera.main.transform.DOMove (camPos, 2.0f).OnComplete(StartTutorial);
		DOTween.To(()=> Camera.main.orthographicSize, x=> Camera.main.orthographicSize = x, seekerCam.orthographicSize, 2.0f);

	}

	void StartTutorial(){
		tutorialOne.StartGame ();
	
	}

}
