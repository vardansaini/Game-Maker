using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IDTextInputCheck : MonoBehaviour
{
    //public Text t;
    public InputField gameName;
    //public string folderName;

    public void StartClick()
    {
        Debug.Log("I am here 3");
        string a = gameName.text;
        Debug.Log(a);
        Directory.CreateDirectory(Constants.directory + gameName.text);
        Constants.directory = Application.dataPath + "/StreamingAssets/Frames/" + gameName.text;
        //Directory.SetCurrentDirectory(Constants.directory);
        Debug.Log(Directory.GetCurrentDirectory());
        Debug.Log("I made new folder");
        //SceneManager.LoadScene ("Main");
    }

    public void Round2Click()
    {
        if (CheckOkay())
        {
            //Constants.idOne = t.text;
            Constants.round = 2;

            if (Application.isEditor)
            {
                Constants.directory = Application.dataPath + "/StreamingAssets/Frames/";
            }
            else
            {
                if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    Constants.directory = Application.dataPath;
                }
                else
                {
                    Constants.directory = Application.dataPath + "/Resources/Data";
                }
            }
            SceneManager.LoadScene("Main");
        }
    }
    public void Round1Click()
    {
        if (CheckOkay())
        {
            //Constants.idOne = t.text;
            Constants.round = 1;

            if (Application.isEditor)
            {
                Constants.directory = Application.dataPath;
            }
            else
            {
                if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    Constants.directory = Application.dataPath + "/StreamingAssets/Frames/";
                }
                else
                {
                    Constants.directory = Application.dataPath + "/Resources/Data" + "/StreamingAssets/Frames/";
                }
            }

            SceneManager.LoadScene("Main");
        }
    }

    private bool CheckOkay()
    {
        return true;
    }

}
