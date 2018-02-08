using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour {

	float speed = 1;
	float amplitude = 1.3f;
	int octaves = 4;

	Vector3 destination;
	int currentTime = 0;

	private Vector3 velocity = Vector3.zero;
	private Vector3 startPos;

	public bool move;

	float nextChange;


	void FixedUpdate ()
	{

		if (move == false) {
			return;
		}
		// if number of frames played since last change of direction > octaves create a new destination
		if (currentTime > octaves)
		{
			currentTime = 0;
			destination = generateRandomVector(amplitude);
		}


		// smoothly moves the object to the random destination
		transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, speed);

		currentTime++;
	}

	// generates a random vector based on a single amplitude for x y and z
	Vector3 generateRandomVector(float amp)
	{
		Vector2 result = new Vector2();


		for (int i = 0; i < 2; i++)
		{
			float x = Random.Range(-amp, amp);
			result[i] = x;
		}

		Vector3 newResult = new Vector3 (result.x + startPos.x, result.y + startPos.y, transform.position.z);

		return newResult;
	}

	public void StartWiggle (){

		startPos = transform.position;
		move = true;
		Invoke ("ChangeSpeed", Random.Range(1.0f,5.0f));
	
	}
		
	public void StopWiggle (){
		move = false;
		transform.DOMove (startPos, .5f);
		CancelInvoke ("ChangeSpeed");
	}
		
	void ChangeSpeed (){
		float newSpeed;

		if (speed < .6f) {
			newSpeed = Random.Range (1.0f, 1.5f);
		} else if (speed > 1.2f) {
			newSpeed = Random.Range (.3f, .75f);
		} else {
			newSpeed = Random.Range (.4f, 1.4f);
		}

		speed = newSpeed;
		Invoke ("ChangeSpeed", Random.Range(1.0f,4.0f));
	
	}


}
