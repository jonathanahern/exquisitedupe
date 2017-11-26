using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader : MonoBehaviour {

	public string[] items;

	// Use this for initialization
	IEnumerator Start () {
	
		WWW itemsData = new WWW ("http://dupesite.000webhostapp.com/ItemsData.php");
		yield return itemsData;
		//Debug.Log (itemsData.text);
		//string itemsDataString = itemsData.text;
		//items = itemsDataString.Split (';');
		//Debug.Log (GetDataValue(items[0], "Name:"));
	}
	
	string GetDataValue(string data, string index){
	
		string value = data.Substring (data.IndexOf (index) + index.Length);
		value = value.Remove (value.IndexOf ("|"));
		return value;

	}
}
