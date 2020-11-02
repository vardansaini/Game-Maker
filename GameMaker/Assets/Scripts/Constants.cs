using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Constants : MonoBehaviour {

	public static string idOne = "test";
	public static int round;

	public static float threshold = 0.1f;

	public static string directory=Application.dataPath+ "/StreamingAssets/Frames/";



	public static string GetGameName(){
		return idOne;
	}

}
