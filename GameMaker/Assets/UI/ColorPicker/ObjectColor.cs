using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ObjectColor : MonoBehaviour {

	void OnSetColor(Color color)
	{
		GetComponent<Image>().color = color;
	}

	void OnGetColor(ColorPicker picker)
	{
		picker.NotifyColor(GetComponent<Image>().color);
	}
}
