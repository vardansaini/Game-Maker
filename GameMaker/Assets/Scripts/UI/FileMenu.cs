﻿using Assets.Scripts.Core;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.UI
{
    public class FileMenu : MonoBehaviour
    {
        public int LastFrame = 0;
        public static string gameName = "test";
        public string GameName
        {
            get { return gameName; }
            set { gameName = FormatGameName(value); }
        }
        [SerializeField]
        private DialogueMenu dialogueMenu;

        [SerializeField]
        private InputField loadLevelInput;

        public int GetLastFrame()
        {
            bool Last = false;
            int i = FrameManager.GetmaxFrame();
            while (!Last)
            {
                if (File.Exists(GetFile(i)))
                {
                    LastFrame = i;
                    Last = true;
                }
                else
                {
                    i--;
                }
            }
                return LastFrame;
            }

        void Awake() {
            gameName = Constants.GetGameName();
        }


        public void OnRun()
        {

            ExternalSave();

            LogHandler.Instance.WriteLine("Starting Run:  time = " + Time.time);
            //TODO; handle running
            //SceneManager.LoadScene("LevelTest");

        }

        public void OnSave()
        {
            if (GameName == null)
            {
                dialogueMenu.OpenDialogue(Dialogue.SaveFailed);
            }
            else
            {
                string fileName = GameName + " " + FrameManager.GetCurrentFrame() + ".csv";
                File.WriteAllText(Constants.directory + fileName, FrameManager.GetKeys());
                File.AppendAllText(Constants.directory + fileName, GridManager.Instance.FormatToCSV());
                //string LastFrameFile = GetLastFrame().ToString()+ ".csv";
                //File.Create(Constants.directory + LastFrameFile);
                
                
                
            }
        }

        public bool ExternalSave() {
            OnSave();
            return GameName != null;
        }

        public void OnLoad()
        {
            // Validate input
            string newLevelName = FormatGameName(loadLevelInput.text);
            if (newLevelName == null)
                return;
            else
                GameName = newLevelName;

            ForRealLoad();

            dialogueMenu.CloseDialogue();
        }
        public string GetFile(int inputFile)
        {
            int fileToGet = inputFile;
            string filePath = Constants.directory + GameName + " " + fileToGet + ".csv";
            return filePath;
        }
        public void ForRealLoad() {
            LogHandler.Instance.WriteLine("Load Grid Start:  time = " + Time.time);
            // Check level exists
            //string filePath = Constants.directory + GameName + " " + FrameManager.GetCurrentFrame() + ".csv";
            if (File.Exists(GetFile(FrameManager.GetNextFrame())))
            {
                Debug.Log(FrameManager.GetNextFrame());
                //if (FrameManager.GetNextFrame()> LastFrame)
                    //LastFrame = FrameManager.GetNextFrame();
                GridNext.Instance.ClearPreview();
                // - Parse file
                string[] lines = File.ReadAllLines(GetFile(FrameManager.GetNextFrame()));
                string[] gridSize = lines[1].Split(',');
                GridNext.Instance.SetGridSize(int.Parse(gridSize[0]), int.Parse(gridSize[1]), false);
                for (int i = 2; i < lines.Length; i++)
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
                
                string[] gridSize = lines[1].Split(',');
                
                GridPrev.Instance.SetGridSize(int.Parse(gridSize[0]), int.Parse(gridSize[1]), false);
                //GridNext.Instance.SetGridSize(int.Parse(gridSize[0]), int.Parse(gridSize[1]), false);
                for (int i = 2; i < lines.Length; i++)
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
                GridManager.Instance.ClearPreview();
                // - Parse file
                string[] lines = File.ReadAllLines(GetFile(FrameManager.GetCurrentFrame()));
                FrameManager.Instance.SetKeys(lines[0]);
                //Debug.Log(lines[0]); actions
                string[] gridSize = lines[1].Split(',');
                //Debug.Log(lines[1]); grid size
                GridManager.Instance.SetGridSize(int.Parse(gridSize[0]), int.Parse(gridSize[1]), false);
                
                for (int i = 2; i < lines.Length; i++)
                {
                    string[] line = lines[i].Split(',');
                    GridManager.Instance.AddGridObject(SpriteManager.Instance.GetSprite(line[0]), int.Parse(line[1]), int.Parse(line[2]), false);
                   
                }

            }
            else
            {

                GridManager.Instance.SetPriorGridObjectsToPreviewOnly(0.5f);
                GridManager.Instance.UpdatePreviewGridObjectsFromLearnedRules();

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
        }

        public void OnTest()
        {
            FrameManager.SetCurrentFrame(0);
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
            if (gameName == null)
                return null;

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