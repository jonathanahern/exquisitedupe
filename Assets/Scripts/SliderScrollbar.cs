using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SliderScrollbar : MonoBehaviour {

	public ScrollRect MyScrollRect;
	public bool VerticalMovement;
	
	public void PassThrough(float scrollValue)
	{
		if(VerticalMovement)
		{
			MyScrollRect.verticalNormalizedPosition = scrollValue;
		}
		else
		{
			MyScrollRect.horizontalNormalizedPosition = scrollValue;
		}
	}
}
