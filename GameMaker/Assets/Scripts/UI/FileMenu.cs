using Assets.Scripts.Core;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Assets.Scripts.UI
{
    public class FileMenu : MonoBehaviour
    {
        public int LastFrame = 0;
        public static string gameName = "";
        public string GameName
        {
            get { return gameName; }
            set { gameName = FormatGameName(value); }
        }
        [SerializeField]
        private DialogueMenu dialogueMenu;

        [SerializeField]
        private InputField loadLevelInput;

        [SerializeField]
        private InputField gameNamefield;

        public Text gameFiles;
        public Text error;

        public void Load()
        {
            Constants.directory = Application.dataPath + "/StreamingAssets/Frames/";
            FrameManager.ResetFrame();
            LogHandler.Instance.WriteLine("Home was clicked:  time = " + Time.time);
            LogHandler.Instance.WriteLine("");
            LogHandler.Instance.CloseWriter();
            SceneManager.LoadScene("Menu");

        }

        public void StartClick()
        {
            string fileName = "LoadedGame.txt";

            if (gameNamefield.text != "")
            {
                Constants.directory = Application.dataPath + "/StreamingAssets/Frames/";
                if (Directory.Exists(Constants.directory + gameNamefield.text))
                {
                    File.WriteAllText(Constants.directory + fileName, gameNamefield.text);
                    Constants.directory = Application.dataPath + "/StreamingAssets/Frames/" + gameNamefield.text + "/";
                }
                else
                {
                    Directory.CreateDirectory(Constants.directory + gameNamefield.text);
                    File.WriteAllText(Constants.directory + fileName, gameNamefield.text);
                    Constants.directory = Application.dataPath + "/StreamingAssets/Frames/" + gameNamefield.text + "/";
                }

                if (LogHandler.Instance != null)
                {
                    LogHandler.Instance.StartLog();
                    LogHandler.Instance.WriteLine("Start was clicked:  time = " + Time.time);
                }
                SceneManager.LoadScene("Main");
            }
            else
            {
                error.text = "INVALID INPUT!";
            }

        }

        private void Start()
        {
            GamesList();
        }

        public int GetFirstFrame()
        {
            int val;
            var dir = new DirectoryInfo(Constants.directory);
            FileInfo[] info = dir.GetFiles("*.*");

            foreach (FileInfo f in info)
            {
                string a = f.ToString();
                string b = string.Empty;

                if (a.EndsWith(".csv"))
                {
                    b = Path.GetFileName(a).Replace(".csv", "");
                    val = int.Parse(b);
                    if (val < LastFrame)
                    {
                        if (File.Exists(GetFile(val)))
                        {
                            LastFrame = val;
                        }
                    }

                }
            }
            LogHandler.Instance.WriteLine("First Frame button was pressed:  time = " + Time.time);
            return LastFrame;
        }

        public int GetLastFrame()
        {
            int val;
            var dir = new DirectoryInfo(Constants.directory);
            FileInfo[] info = dir.GetFiles("*.*");

            foreach (FileInfo f in info)
            {
                string a = f.ToString();
                string b = string.Empty;

                if (a.EndsWith(".csv"))
                {

                    b = Path.GetFileName(a).Replace(".csv", "");

                    val = int.Parse(b);
                    if (val > LastFrame)
                    {
                        if (File.Exists(GetFile(val)))
                        {
                            LastFrame = val;
                        }
                    }

                }
            }
            LogHandler.Instance.WriteLine("Last Frame button was pressed:  time = " + Time.time);
            return LastFrame;
        }

        public void UpdateVelocities(List<GridObject> PreviewObjects, List<bool> RuleCheck)
        {

            int prevCount = 0;
            int CurrCount = 0;

            if (File.Exists(GetFile(FrameManager.GetPrevFrame())) && !File.Exists(GetFile(FrameManager.GetNextFrame())) && (FrameManager.GetPrevPrevFrame() >= 0))
            {
                //Load prior objects
                string[] linesPrev = File.ReadAllLines(GetFile(FrameManager.GetPrevPrevFrame()));
                for (int i = 3; i < linesPrev.Length; i++)
                {
                    prevCount += 1;
                }
                foreach (GridObject go in PreviewObjects)
                {
                    CurrCount += 1;
                }

                if (prevCount < CurrCount)
                {
                    for (int i = 3; i < linesPrev.Length; i++)
                    {
                        string[] lines = linesPrev[i].Split(',');
                        int bestMatch1 = -1;
                        int bestMatch2 = -1;
                        int bestDist = 1000;

                        for (int j = 0; j < RuleCheck.Count; j++)
                        {
                            if (RuleCheck[j] == false)
                            {
                                if (PreviewObjects[j].Name == lines[0])
                                {
                                    int dist = Mathf.Abs(PreviewObjects[j].X - int.Parse(lines[1])) + Mathf.Abs(PreviewObjects[j].Y - int.Parse(lines[2]));
                                    if (dist < bestDist)
                                    {
                                        bestDist = dist;
                                        bestMatch1 = j;
                                        bestMatch2 = i;
                                    }
                                }

                                if (bestMatch1 >= 0 && bestMatch2 > 0)
                                {
                                    string[] line = linesPrev[bestMatch2].Split(',');
                                    if (int.Parse(line[1]) != PreviewObjects[bestMatch1].X || int.Parse(line[2]) != PreviewObjects[bestMatch1].Y)
                                    {
                                        PreviewObjects[bestMatch1].VX = PreviewObjects[bestMatch1].X - int.Parse(line[1]);
                                        //Debug.Log("Updated VX in velocity function = " + PreviewObjects[bestMatch1].VX);

                                        PreviewObjects[bestMatch1].VY = PreviewObjects[bestMatch1].Y - int.Parse(line[2]);
                                        //Debug.Log("Updated VY in velocity function = " + PreviewObjects[bestMatch1].VY);

                                        int x = PreviewObjects[bestMatch1].X;
                                        int y = PreviewObjects[bestMatch1].Y;
                                        x += PreviewObjects[bestMatch1].VX;
                                        y += PreviewObjects[bestMatch1].VY;
                                        PreviewObjects[bestMatch1].SetPosition(x, y);
                                    }
                                }
                            }
                        }
                    }
                }
                if (prevCount >= CurrCount)
                {
                    int bestMatch1 = -1;
                    int bestMatch2 = -1;
                    int bestDist = 1000;
                    for (int j = 0; j < RuleCheck.Count; j++)
                    {
                        if (RuleCheck[j] == false)
                        {
                            for (int i = 3; i < linesPrev.Length; i++)
                            {
                                string[] line = linesPrev[i].Split(',');
                                if (line[0] == PreviewObjects[j].Name)
                                {
                                    int dist = Mathf.Abs(PreviewObjects[j].X - int.Parse(line[1])) + Mathf.Abs(PreviewObjects[j].Y - int.Parse(line[2]));
                                    if (dist < bestDist)
                                    {
                                        bestDist = dist;
                                        bestMatch1 = j;
                                        bestMatch2 = i;
                                    }
                                }
                            }
                        }


                        if (bestMatch1 >= 0 && bestMatch2 > 0)
                        {
                            string[] line = linesPrev[bestMatch2].Split(',');
                            if (int.Parse(line[1]) != PreviewObjects[bestMatch1].X || int.Parse(line[2]) != PreviewObjects[bestMatch1].Y)
                            {
                                PreviewObjects[bestMatch1].VX = PreviewObjects[bestMatch1].X - int.Parse(line[1]);
                                //Debug.Log("Updated VX in velocity function = " + PreviewObjects[bestMatch1].VX);

                                PreviewObjects[bestMatch1].VY = PreviewObjects[bestMatch1].Y - int.Parse(line[2]);
                                //Debug.Log("Updated VY in velocity function = " + PreviewObjects[bestMatch1].VY);

                                int x = PreviewObjects[bestMatch1].X;
                                int y = PreviewObjects[bestMatch1].Y;
                                x += PreviewObjects[bestMatch1].VX;
                                y += PreviewObjects[bestMatch1].VY;
                                PreviewObjects[bestMatch1].SetPosition(x, y);
                            }
                        }
                    }

                }
            }
        }

        public void GamesList()
        {
            string[] info = Directory.GetDirectories(Constants.directory);

            foreach (String f in info)
            {
                string a = Path.GetFileName(f);
                gameFiles.text = gameFiles.text + "\n" + a;
            }
        }

        void Awake()
        {
            File.WriteAllText(Application.dataPath + "/StreamingAssets/Frames/" + "LoadedGame.txt", "");
            GameName = "";
        }
        void Update()
        {
            gameFiles.text = "";
            GamesList();
            if (Input.GetKey(KeyCode.Return))
            {
                StartClick();
            }
        }

        public void OnSave()
        {
            // From Morai Maker maybe needed later
            /*if (GameName == null)
            {
                //dialogueMenu.OpenDialogue(Dialogue.SaveFailed);
            }*/

            string fileName = FrameManager.GetCurrentFrame() + ".csv";
            File.WriteAllText(Constants.directory + fileName, FrameManager.GetKeys());
            File.AppendAllText(Constants.directory + fileName, FrameManager.GetPrevKeys());
            File.AppendAllText(Constants.directory + fileName, GridManager.Instance.FormatToCSV());


        }

        // TODO: Check if we really need this function.
        /*public bool ExternalSave()
        {
            OnSave();
            return GameName != null;
        }*/

        public string GetFile(int inputFile)
        {
            int fileToGet = inputFile;
            string filePath = Constants.directory + fileToGet + ".csv";
            return filePath;
        }

        public void ForRealLoad()
        {
            LogHandler.Instance.WriteLine("Load Grid Start:  time = " + Time.time);
           
            if (File.Exists(GetFile(FrameManager.GetNextFrame())))
            {

                GridNext.Instance.ClearPreview();
                // - Parse file
                string[] lines = File.ReadAllLines(GetFile(FrameManager.GetNextFrame()));
                string[] gridSize = lines[2].Split(',');

                GridNext.Instance.SetGridSize(int.Parse(gridSize[0]), int.Parse(gridSize[1]), false);

                for (int i = 3; i < lines.Length; i++)
                {
                    string[] line = lines[i].Split(',');

                    GridNext.Instance.AddGridObject(SpriteManager.Instance.GetSprite(line[0]), int.Parse(line[1]), int.Parse(line[2]), false);
                }
            }
            else
            {
                GridNext.Instance.ClearGrid();
                //GridNext.Instance.DestroyThisGrid();
            }

            if (File.Exists(GetFile(FrameManager.GetPrevFrame())))
            {
                GridPrev.Instance.ClearPreview();
                // - Parse file
                string[] lines = File.ReadAllLines(GetFile(FrameManager.GetPrevFrame()));
                string[] gridSize = lines[2].Split(',');

                GridPrev.Instance.SetGridSize(int.Parse(gridSize[0]), int.Parse(gridSize[1]), false);

                for (int i = 3; i < lines.Length; i++)
                {
                    string[] line = lines[i].Split(',');

                    GridPrev.Instance.AddGridObject(SpriteManager.Instance.GetSprite(line[0]), int.Parse(line[1]), int.Parse(line[2]), false);
                }

            }
            else
            {
                GridPrev.Instance.ClearGrid();

            }

            if (File.Exists(GetFile(FrameManager.GetCurrentFrame())))
            {
                List<GridObject> currGridObjects = new List<GridObject>();
                GridManager.Instance.ClearPreview();

                // - Parse file
                string[] lines = File.ReadAllLines(GetFile(FrameManager.GetCurrentFrame()));

                FrameManager.Instance.SetKeys(lines[0]);
                FrameManager.Instance.SetPrevKeys(lines[1]);
                
                string[] gridSize = lines[2].Split(',');
                
                GridManager.Instance.SetGridSize(int.Parse(gridSize[0]), int.Parse(gridSize[1]), false);

                for (int i = 3; i < lines.Length; i++)
                {
                    string[] line = lines[i].Split(',');

                    GridObject go = GridManager.Instance.AddGridObject(SpriteManager.Instance.GetSprite(line[0]), int.Parse(line[1]), int.Parse(line[2]), false);

                    //Load velocity
                    if (go != null && line.Length > 5)
                    {
                        go.VX = int.Parse(line[5]);
                        go.VY = int.Parse(line[6]);

                    }
                    else if (go != null)
                    {
                        currGridObjects.Add(go);
                    }
                }

            }
            else
            {

                GridManager.Instance.SetPriorGridObjectsToPreviewOnly(0.5f);
                GridManager.Instance.UpdatePreviewGridObjectsFromLearnedRules();
                UpdateVelocities(GridManager.Instance.PreviewObjects, Rule.RuleActiveCheck);
                FrameManager.Instance.ResetKeys();
                // - Load an empty level instead
                //GridManager.Instance.ClearGrid();
            }
            LogHandler.Instance.WriteLine("Load Grid End:  time = " + Time.time);
        }

        public void OnClear()
        {
            if (File.Exists(GetFile(FrameManager.GetCurrentFrame())))
            {
                File.Delete(GetFile(FrameManager.GetCurrentFrame()));
            }

            GridManager.Instance.ClearGrid();
            LogHandler.Instance.WriteLine("Screen was cleared:  time = " + Time.time);
        }

        public void OnTest()
        {
            FrameManager.SetCurrentFrame(0);
            LogHandler.Instance.WriteLine("Play was pressed:  time = " + Time.time);
            SceneManager.LoadScene("Playtest");
        }

        public void OnExit()
        {

            //Tell writer to close 
            LogHandler.Instance.WriteLine("Study End:  time = " + Time.time);
            LogHandler.Instance.CloseWriter();
            Application.Quit();
        }

        public static string FormatGameName(string gameName)
        {
            //if (gameName == null)
            //return null;

            string formattedGameName = gameName.ToLower().Replace(' ', '_').Trim('_');
            return formattedGameName == string.Empty ? null : formattedGameName;
        }

        public void Check()
        {
            if (GridManager.Instance.Checklist())
            {
                OnSave();
            }
        }
    }
}