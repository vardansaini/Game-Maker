using Assets.Scripts.Core;
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
        public static string levelName = "";
        public static bool prevLoaded;
        public string LevelName
        {
            get { return levelName; }
            set { levelName = FormatLevelName(value); }
        }
        [SerializeField]
        private DialogueMenu dialogueMenu;
        [SerializeField]
        private OptionsMenu optionsMenu;

        [SerializeField]
        private InputField loadLevelInput;

        void Awake() {
            levelName = Constants.GetLevelName();
        }

        void Update() {
            if (prevLoaded) {
                ForRealLoad();
                prevLoaded = false;
            }
        }


        public void OnRun()
        {

            ExternalSave();
            Map.level_name = levelName;

            LogHandler.Instance.WriteLine("Starting Run:  time = " + Time.time);
            SceneManager.LoadScene("LevelTest");

        }

        public void OnSave()
        {
            if (LevelName == null)
            {
                dialogueMenu.OpenDialogue(Dialogue.SaveFailed);
            }
            else
            {
                string fileName = LevelName + "  " + FrameManager.GetCurrentFrame() + ".csv";
                File.WriteAllText(Constants.directory + "/StreamingAssets/Levels/" + fileName, FrameManager.GetKeys());
                File.AppendAllText(Constants.directory + "/StreamingAssets/Levels/" + fileName, GridManager.Instance.FormatToCSV());
               
                
                
            }
        }

        public bool ExternalSave() {
            OnSave();
            return LevelName != null;
        }

        public void OnLoad()
        {
            // Validate input
            string newLevelName = FormatLevelName(loadLevelInput.text);
            if (newLevelName == null)
                return;
            else

                LevelName = newLevelName;

            ForRealLoad();

            dialogueMenu.CloseDialogue();
        }

        public void ForRealLoad() {
            LogHandler.Instance.WriteLine("Load Grid Start:  time = " + Time.time);
            // Check level exists
            string filePath = Constants.directory + "/StreamingAssets/Levels/" + LevelName + " " + FrameManager.GetCurrentFrame() + ".csv";
            if (File.Exists(filePath))
            {
                //GridManager.AddPreviousFrameToCurrentFrame();

                GridManager.Instance.ClearPreview();
                // - Parse file
                string[] lines = File.ReadAllLines(filePath);
                FrameManager.SetKeys(lines[0]);
                Debug.Log(lines[0]);
                string[] gridSize = lines[1].Split(',');
                Debug.Log(lines[1]);
                GridManager.Instance.SetGridSize(int.Parse(gridSize[0]), int.Parse(gridSize[1]), false);
                for (int i = 2; i < lines.Length; i++)
                {
                    /* if (string.Equals(lines[i], "SPACE"))
                        continue;
                    if (string.Equals(lines[i], "UP"))
                        continue;
                    if (string.Equals(lines[i], "DOWN"))
                        continue;
                    if (string.Equals(lines[i], "LEFT"))
                        continue;
                    if (string.Equals(lines[i], "RIGHT"))
                        continue;*/
                    Debug.Log(lines[i]);
                    string[] line = lines[i].Split(',');
                    GridManager.Instance.AddGridObject(SpriteManager.Instance.GetSprite(line[0]), int.Parse(line[1]), int.Parse(line[2]), false);
                }

               

            }
            else
            {
                GridManager.Instance.SetPriorGridObjectsToPreviewOnly(0.5f);

                FrameManager.ResetKeys();
                // - Load an empty level instead
                //GridManager.Instance.ClearGrid();
            }
            LogHandler.Instance.WriteLine("Load Grid End:  time = " + Time.time);
        }
        

        public void OnClear()
        {
            GridManager.Instance.ClearGrid();
        }

        public void OnExit()
        {

            //Tell writer to close 
            LogHandler.Instance.WriteLine("Study End:  time = " + Time.time);
            LogHandler.Instance.CloseWriter();
            Application.Quit();
        }

        public static string FormatLevelName(string levelName)
        {
            if (levelName == null)
                return null;

            string formattedLevelName = levelName.ToLower().Replace(' ', '_').Trim('_');
            return formattedLevelName == string.Empty ? null : formattedLevelName;
        }
        public void check()
            {
            if (GridManager.Instance.Checklist())
            {
                OnSave();
            }
}
    }
} 