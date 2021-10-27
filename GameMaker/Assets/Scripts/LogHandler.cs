using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class LogHandler : MonoBehaviour {
	private static StreamWriter sw;
	public static LogHandler Instance;
	public string[] GameName;
	string path;

	void Awake()
	{
		//Debug.Log("Loghandler is in Awake.");
			Instance = this;
	}
	public void StartLog()
    {
		if (File.Exists(Application.dataPath + "/StreamingAssets/Frames/LoadedGame.txt"))
		{
			GameName = File.ReadAllLines(Application.dataPath + "/StreamingAssets/Frames/LoadedGame.txt");
			//Debug.Log(GameName[0]);

			if (Directory.Exists(Application.dataPath + "/StreamingAssets/Frames/" + GameName[0]))
			{
				if (!Directory.Exists(Application.dataPath + "/StreamingAssets/Frames/" + GameName[0] + "/Study_Data"))
				{
					Directory.CreateDirectory(Application.dataPath + "/StreamingAssets/Frames/" + GameName[0] + "/Study_Data");
				}

			}
		}
		path = Application.dataPath + "/StreamingAssets/Frames/" + GameName[0] + "/Study_Data/" + "Log.txt";
		//Debug.Log(path);
		if (!File.Exists(path))
		{
			//Debug.Log("Trying to create Log File.");
			//Debug.Log(path);
			sw = new StreamWriter(path, true);
			LogHandler.Instance.WriteLine("Study Start in LOGHANDLER:  time = " + Time.time);
			LogHandler.Instance.WriteLine("Study Start:  time = " + Time.time);
		}
		else
		{
			//Debug.Log("Trying to append in existing Log File.");
			//Debug.Log(path);
			sw = File.AppendText(path);
			LogHandler.Instance.WriteLine("");
			LogHandler.Instance.WriteLine("Re-enterd the Project" + Time.time);
			LogHandler.Instance.WriteLine("Study Start:  time = " + Time.time);

		}
		//sw = new StreamWriter(path, true);
		
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
