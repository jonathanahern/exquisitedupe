﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LineScript : MonoBehaviour {

	public LineRenderer lineRend;
	public LineSpawnerScipt lineSpawn;

	List<Vector2> points;

	Vector2 midPos;
	Vector2 thirdPos;

	bool deadLine;

	public void UpdateLine (Vector2 mousePos){
	
		if (points == null) {
		
			points = new List<Vector2> ();
			SetPoint (mousePos);

			Vector2 dot = new Vector2(mousePos.x, mousePos.y - .001f);

			SetPoint (dot);
			return;
		
		}
	
		if (Vector2.Distance (points.Last (), mousePos) > .01f) {

			if (deadLine == true) {
				return;
			}

			SetPoint (mousePos);
		}
	
	}

	public void SetPoint (Vector2 point){

		if (points == null) {
			points = new List<Vector2> ();
		}

		int pointTotal = points.Count;

		if (pointTotal > 1){
			thirdPos = points [pointTotal - 2];
			midPos = points [pointTotal - 1];
		}

		if (pointTotal == 2) {

			float midx = (point.x + thirdPos.x) / 2;
			float midy = (point.y + thirdPos.y) / 2;

			Vector2 newMidPos = new Vector2 (midx, midy);

			lineRend.SetPosition (1, newMidPos);
			points.RemoveAt (1);
			points.Insert(1, newMidPos);

		}

		if (pointTotal > 2) {
			float angle = AnglePoints (point, midPos, thirdPos);

			if (angle < 125) {

				deadLine = true;
				lineSpawn.MakeNewLine (point, midPos);
				return;
		
			}
		}

		points.Add (point);

		lineRend.numPositions = points.Count;
		lineRend.SetPosition (points.Count - 1, point);

	}

	public static float AnglePoints(Vector2 mousePos, Vector2 midPos, Vector2 thirdPos){

		Vector2 first = (mousePos - midPos);
		Vector2 second = (thirdPos - midPos);

		return Vector2.Angle(first, second);

	}

}
