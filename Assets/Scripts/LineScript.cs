using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LineScript : MonoBehaviour {

	public LineRenderer lineRend;
	public LineSpawnerScipt lineSpawn;

	List<Vector2> points;

	public List<GameObject> hitBrushes; 

	Vector2 midPos;
	Vector2 thirdPos;

	bool deadLine;

	public GameObject myCollider;

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

		myCollider.transform.position = point;

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
				Destroy (myCollider);
				lineSpawn.MakeNewLine (point, midPos);
				return;
		
			}
		}

		points.Add (point);

		lineRend.positionCount = points.Count;
		lineRend.SetPosition (points.Count - 1, point);

	}

	public static float AnglePoints(Vector2 mousePos, Vector2 midPos, Vector2 thirdPos){

		Vector2 first = (mousePos - midPos);
		Vector2 second = (thirdPos - midPos);

		return Vector2.Angle(first, second);

	}

	public void DestroyCollider () {

		if (myCollider != null) {
			Destroy (myCollider);
		}

	}

	public void HitBrush (Vector3 brushPos, Vector3 brushPosMid, GameObject brushHit) {
	
		deadLine = true;
		DestroyCollider ();
		lineRend.positionCount = points.Count;
		Vector3 lastPoint = lineRend.GetPosition(lineRend.positionCount-1);
		lineSpawn.MakeNewDot (brushPos, brushPosMid,lastPoint,brushHit);
		lineSpawn.MakeNewLine (lastPoint, midPos);
	
	}

	public void ParentBrush (GameObject brushHit){

		if (hitBrushes == null) {
			hitBrushes = new List<GameObject> ();
		}

		hitBrushes.Add (brushHit);

	}

	public void SmoothTheLine(){
	
		Vector2[] pointsArray2 = points.ToArray ();
		Vector3[] pointsArray = new Vector3[pointsArray2.Length];

		for (int i = 0; i < pointsArray2.Length; i++) {

			Vector3 tempVect = new Vector3 (
				pointsArray2[i].x,
				pointsArray2[i].y,
				0);

			pointsArray [i] = tempVect;
		}

		Vector3[] newArray = SmoothLine (pointsArray, .08f);
//		Debug.Log ("Old Length: " + pointsArray2.Length);
//		Debug.Log ("New Length: " + newArray.Length);
		lineRend.positionCount = newArray.Length;

		for (int i = 0; i < newArray.Length; i++) {

			lineRend.SetPosition (i, newArray [i]);

		}

//		lineRend.SetVertexCount(points.Length);
//		var counter : int = 0;
//		for(var i : Vector3 in points){
//			lineRenderer.SetPosition(counter, i);
//			++counter;
//		}


	}

//	public static Vector3[] MakeSmoothCurve(Vector3[] arrayToCurve, float smoothness){
//		List<Vector3> points;
//		List<Vector3> curvedPoints;
//		int pointsLength = 0;
//		int curvedLength = 0;
//
//		if(smoothness < 1.0f) smoothness = 1.0f;
//
//		pointsLength = arrayToCurve.Length;
//
//		curvedLength = (pointsLength*Mathf.RoundToInt(smoothness))-1;
//		curvedPoints = new List<Vector3>(curvedLength);
//
//		float t = 0.0f;
//		for(int pointInTimeOnCurve = 0;pointInTimeOnCurve < curvedLength+1;pointInTimeOnCurve++){
//			t = Mathf.InverseLerp(0,curvedLength,pointInTimeOnCurve);
//
//			points = new List<Vector3>(arrayToCurve);
//
//			for(int j = pointsLength-1; j > 0; j--){
//				for (int i = 0; i < j; i++){
//					points[i] = (1-t)*points[i] + t*points[i+1];
//				}
//			}
//
//			curvedPoints.Add(points[0]);
//		}
//
//		return(curvedPoints.ToArray());
//	}

	public static Vector3[] SmoothLine( Vector3[] inputPoints, float segmentSize )
	{
		//create curves
		AnimationCurve curveX = new AnimationCurve();
		AnimationCurve curveY = new AnimationCurve();
		AnimationCurve curveZ = new AnimationCurve();

		//create keyframe sets
		Keyframe[] keysX = new Keyframe[inputPoints.Length];
		Keyframe[] keysY = new Keyframe[inputPoints.Length];
		Keyframe[] keysZ = new Keyframe[inputPoints.Length];

		//set keyframes
		for( int i = 0; i < inputPoints.Length; i++ )
		{
			keysX[i] = new Keyframe( i, inputPoints[i].x );
			keysY[i] = new Keyframe( i, inputPoints[i].y );
			keysZ[i] = new Keyframe( i, inputPoints[i].z );
		}

		//apply keyframes to curves
		curveX.keys = keysX;
		curveY.keys = keysY;
		curveZ.keys = keysZ;

		//smooth curve tangents
		for( int i = 0; i < inputPoints.Length; i++ )
		{
			curveX.SmoothTangents( i, 0 );
			curveY.SmoothTangents( i, 0 );
			curveZ.SmoothTangents( i, 0 );
		}

		//list to write smoothed values to
		List<Vector3> lineSegments = new List<Vector3>();

		//find segments in each section
		for( int i = 0; i < inputPoints.Length; i++ )
		{
			//add first point
			lineSegments.Add( inputPoints[i] );

			//make sure within range of array
			if( i+1 < inputPoints.Length )
			{
				//find distance to next point
				float distanceToNext = Vector3.Distance(inputPoints[i], inputPoints[i+1]);

				//number of segments
				int segments = (int)(distanceToNext / segmentSize);

				//add segments
				for( int s = 1; s < segments; s++ )
				{
					//interpolated time on curve
					float time = ((float)s/(float)segments) + (float)i;

					//sample curves to find smoothed position
					Vector3 newSegment = new Vector3( curveX.Evaluate(time), curveY.Evaluate(time), curveZ.Evaluate(time) );

					//add to list
					lineSegments.Add( newSegment );
				}
			}
		}

		return lineSegments.ToArray();
	}

}
