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
using Debug = UnityEngine.Debug;

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
        public List<bool> PreviewUpdated;

        public void Load()
        {
            Constants.directory = Application.dataPath + "/StreamingAssets/Frames/";
            FrameManager.ResetFrame();
            SceneManager.LoadScene("Menu");
            LogHandler.Instance.WriteLine("Home was clicked:  time = " + Time.time);
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
                SceneManager.LoadScene("Main");
            }
            else
            {
                error.text = "INVALID INPUT!";
            }
            LogHandler.Instance.WriteLine("Start was clicked:  time = " + Time.time);
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
                        if (File.Exists(GetFile(val)) == true)
                        {
                            LastFrame = val;
                        }
                    }

                }
            }
            LogHandler.Instance.WriteLine("First Frame button was pressed:  time = " + Time.time);
            return LastFrame;
        }

        public void UpdateVelocities(List<GridObject> currGridObjects)
        {
            // If object does not exist in previous frame then no prediction
            int prevCount = 0;
            int CurrCount = 0;

            if (File.Exists(GetFile(FrameManager.GetPrevFrame())) && !File.Exists(GetFile(FrameManager.GetNextFrame())))
            {
                //Load prior objects
                string[] linesPrev = File.ReadAllLines(GetFile(FrameManager.GetPrevFrame()));
                for (int i = 3; i < linesPrev.Length; i++)
                {
                    prevCount += 1;
                }
                foreach (GridObject go in currGridObjects)
                {
                    CurrCount += 1;
                }
                //Debug.Log("counter1 = " + prevCount);
                //Debug.Log("counter2 = " + CurrCount);

                if (prevCount < CurrCount)
                {
                    for (int i = 3; i < linesPrev.Length; i++)
                    {
                        string[] lines = linesPrev[i].Split(',');

                        //Debug.Log("Data check: " + lines[0]);
                        int bestMatch = -1;
                        int bestDist = 1000;
                        foreach (GridObject go in currGridObjects)
                        {
                            //Debug.Log("Prev Frame check line[0], it should be name: " + lines[0]);
                            if (go.Name == lines[0])
                            {
                                //Debug.Log("Prev Frame check line[1] and line[2], it should be velocity: " + lines[1] + "||||" + lines[2]);
                                int dist = Mathf.Abs(go.X - int.Parse(lines[1])) + Mathf.Abs(go.Y - int.Parse(lines[2]));
                                if (dist < bestDist)
                                {
                                    bestDist = dist;
                                    //Debug.Log("bestDist " + bestDist);
                                    bestMatch = i;
                                    //Debug.Log("bestMatch" + bestMatch);
                                }


                                if (bestMatch > 0)
                                {
                                    //Debug.Log("I am predicting new Frame.");
                                    string[] line = linesPrev[bestMatch].Split(',');

                                    if (int.Parse(line[1]) != go.X || int.Parse(line[2]) != go.Y)
                                    {
                                        go.VX = go.X - int.Parse(line[1]);
                                        go.VY = go.Y - int.Parse(line[2]);
                                    }
                                }
                                //Debug.Log("Print the number of main for loop: " + i + "Expected to stop at: " + linesPrev.Length);
                                if (i == linesPrev.Length - 1) break;
                            }
                        }
                    }
                }
                if (prevCount >= CurrCount) {

                    //foreach (GridObject obj in)
                    foreach (GridObject go in currGridObjects)
                    {
                        //Debug.Log("This is currGridObjects "+currGridObjects);
                        //Debug.Log("go " + go);

                        int bestMatch = -1;
                        int bestDist = 1000;

                        for (int i = 3; i < linesPrev.Length; i++)
                        {
                            string[] line = linesPrev[i].Split(',');
                            //Debug.Log("Prev Frame check line[0], it should be name: " + line[0]);
                            if (line[0] == go.Name)
                            {

                                //Debug.Log("Prev Frame check line[1] and line[2], it should be velocity: " + line[1] + line[2]);
                                int dist = Mathf.Abs(go.X - int.Parse(line[1])) + Mathf.Abs(go.Y - int.Parse(line[2]));
                                if (dist < bestDist)
                                {
                                    bestDist = dist;
                                    //Debug.Log("bestDist " + bestDist);
                                    bestMatch = i;
                                    //Debug.Log("bestMatch" + bestMatch);
                                }
                            }
                        }


                        if (bestMatch > 0)
                        {
                            //Debug.Log("I am predicting new Frame.");
                            string[] line = linesPrev[bestMatch].Split(',');

                            if (int.Parse(line[1]) != go.X || int.Parse(line[2]) != go.Y)
                            {
                                go.VX = go.X - int.Parse(line[1]);
                                go.VY = go.Y - int.Parse(line[2]);
                            }
                        }
                        //Debug.Log("Print the number of main for loop: " + "Expected to stop at: " + linesPrev.Length);

                    }

                }
            }
        }
    
        public int GetLastFrame()
        {
            int val;
            var dir = new DirectoryInfo(Constants.directory);
            //Debug.Log(Constants.directory);
            FileInfo[] info = dir.GetFiles("*.*");


            foreach (FileInfo f in info)
            {
                string a = f.ToString();
                string b = string.Empty;
 
                if (a.EndsWith(".csv"))
                {
                   
                    b = Path.GetFileName(a).Replace(".csv","");
                    //Debug.Log(b);

               
                        val = int.Parse(b);
                        //Debug.Log(val);
                        if (val > LastFrame)
                        {
                        if (File.Exists(GetFile(val)) == true)
                        {                          
                            LastFrame = val;
                        }                                                
                    }

                }
            }
            //Debug.Log(LastFrame);
            LogHandler.Instance.WriteLine("Last Frame button was pressed:  time = " + Time.time);
            return LastFrame;

        }
        public void GamesList()
        {
            //var dir = new DirectoryInfo(Constants.directory);
            string[] info = Directory.GetDirectories(Constants.directory);

            foreach (String f in info)
            {
                //Debug.Log(f);
                string a = Path.GetFileName(f);
                gameFiles.text = gameFiles.text + "\n" + a;
            }
        }

        void Awake()
        {
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


        /*public void OnRun()
        {

            ExternalSave();

            LogHandler.Instance.WriteLine("Starting Run:  time = " + Time.time);
            //TODO; handle running
            //SceneManager.LoadScene("LevelTest");

        }*/

        public void OnSave()
        {
            //if (GameName == null)
            //{
                //dialogueMenu.OpenDialogue(Dialogue.SaveFailed);
            //}
            
            //{
                string fileName = FrameManager.GetCurrentFrame() + ".csv";
                File.WriteAllText(Constants.directory + fileName, FrameManager.GetKeys());
                File.AppendAllText(Constants.directory + fileName, FrameManager.GetPrevKeys());
                File.AppendAllText(Constants.directory + fileName, GridManager.Instance.FormatToCSV());

            //}
        }

        // Only EndTurn calls this function
        // TODO: Check if we really need this function.
        public bool ExternalSave()
        {
            OnSave();
            return GameName != null;
        }

        /*public void OnLoad()
        {
            // Validate input
            string newLevelName = FormatGameName(loadLevelInput.text);
            if (newLevelName == null)
                return;
            else
                GameName = newLevelName;

            ForRealLoad();

            dialogueMenu.CloseDialogue();
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
            // Check level exists

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
                //GridNext.Instance.SetGridSize(int.Parse(gridSize[0]), int.Parse(gridSize[1]), false);
                for (int i = 3; i < lines.Length; i++)
                {
                    string[] line = lines[i].Split(',');
                    //GridManager.Instance.AddGridObject(SpriteManager.Instance.GetSprite(line[0]), int.Parse(line[1]), int.Parse(line[2]), false);
                    GridPrev.Instance.AddGridObject(SpriteManager.Instance.GetSprite(line[0]), int.Parse(line[1]), int.Parse(line[2]), false);
                    //GridNext.Instance.AddGridObject(SpriteManager.Instance.GetSprite(line[0]), int.Parse(line[1]), int.Parse(line[2]), false);
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
                //Debug.Log(lines[0]); actions
                string[] gridSize = lines[2].Split(',');
                //Debug.Log(lines[1]); grid size
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
                PreviewUpdated = GridManager.Instance.UpdatePreviewGridObjectsFromLearnedRules();
               /* foreach (bool b in PreviewUpdated)
                {
                    Debug.Log(b);
                }
                Debug.Log("##########END OF FILE MENU CHECK> NOW RETURNING TO FRAME MANAGER.");*/

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
            SceneManager.LoadScene("Playtest");
            LogHandler.Instance.WriteLine("Play was pressed:  time = " + Time.time);
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