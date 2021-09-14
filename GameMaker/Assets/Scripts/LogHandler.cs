using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class LogHandler : MonoBehaviour {
	private static StreamWriter sw;
	public static LogHandler Instance;
	public string[] GameName;

    
	void Awake(){
		if (File.Exists(Application.dataPath + "/StreamingAssets/Frames/LoadedGame.txt")){
			GameName = File.ReadAllLines(Application.dataPath + "/StreamingAssets/Frames/LoadedGame.txt");
			Debug.Log(GameName[0]);

		}
		if (Directory.Exists(Application.dataPath + "/StreamingAssets/Frames/" + GameName[0]))
		{
			if (!Directory.Exists(Application.dataPath + "/StreamingAssets/Frames/" + GameName[0] + "/Study_Data"))
			{
				Directory.CreateDirectory(Application.dataPath + "/StreamingAssets/Frames/" + GameName[0] + "/Study_Data");
			}
		}
		sw = new StreamWriter (Application.dataPath + "/StreamingAssets/Frames/" + GameName[0] + "/Study_Data/" + "Log.txt");
		Instance = this;
		LogHandler.Instance.WriteLine("Study Start in LOGHANDLER:  time = " + Time.time);
		LogHandler.Instance.WriteLine ("Study Start:  time = " + Time.time);
	}
	

    public void WriteLine(string line){
		try{
			sw.WriteLine (line);
		}
		catch(ObjectDisposedException e){
			return;
		}
		
	}

	public void CloseWriter(){
		try{
			sw.Close ();
		}
		catch(ObjectDisposedException e){
			return;
		}
	}

}
