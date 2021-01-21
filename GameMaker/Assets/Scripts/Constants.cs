using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Constants : MonoBehaviour
{

    public static string idOne = "";
    public static int round;

<<<<<<< HEAD
	public static float threshold = 0.1f;
=======
    public static readonly string SOLID = "Solid";
    public static readonly string BREAKABLE = "Breakable";
    public static readonly string POWERUP = "PowerupBlock";
    public static readonly string GROUND = "Ground";
    public static string directory = Application.dataPath + "/StreamingAssets/Frames/";
>>>>>>> GitFix

	public static string directory=Application.dataPath+ "/StreamingAssets/Frames/";


<<<<<<< HEAD
=======
    public static bool IsSolid(string tag)
    {
        List<string> solidSet = new List<string>(new string[] { SOLID, BREAKABLE, POWERUP, GROUND });
>>>>>>> GitFix

        return solidSet.Contains(tag);
    }

    public static string GetGameName()
    {
        return idOne;
    }

}
