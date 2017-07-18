using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UpdateSlider : MonoBehaviour {
	
	public Slider MySlider;
	public bool VerticalMove;
	
	private float updateValue;
	
	public void SliderValue(Vector2 updateVector2)
	{
		if(VerticalMove)
		{
			updateValue = updateVector2.y;			// use the y value for vertical movement
		}
		else
		{
			updateValue = updateVector2.x;			// use x for horizontal movement
		}
		
		if(updateValue <= 1 && updateValue >= 0)
		{
			MySlider.value= updateValue;
		}
		else if(updateValue > 1)
		{
			MySlider.value = 1;
		}
		else
		{
			MySlider.value = 0;
		}
	}
}
