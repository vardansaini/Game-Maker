using Assets.Scripts.UI;
using Assets.Scripts.Util;
using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Assets.Scripts.Core;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class FrameManager : MonoBehaviour
    {
        [SerializeField]
        private FileMenu fileMenu;
        [SerializeField]
        private Text text;
        private static int frame = 0;
        private static bool space;
        private static bool up;
        private static bool down;
        private static bool left;
        private static bool right;
        private bool loaded = false;
        public Button spaceButton;
        public Button upButton;
        public Button downButton;
        public Button leftButton;
        public Button rightButton;
        public static FrameManager Instance;
        
        //bool[] keyDefault = {false, false,false,false,false};
        // Dictionary<string, bool> buttons = new Dictionary<string, bool>();
        float lastStep, timeBetweenSteps = 0.5f;

        void Start()
        {
            Instance = this;
        }
        public static int GetCurrentFrame()
        {
            return frame;
        }
        public static void SetCurrentFrame(int current)
        {
            frame = current;
        }

        public void Next()
        {
            lastStep = Time.time;
            fileMenu.check();
            frame++;
            text.text = "" + frame;
            fileMenu.ForRealLoad();
        }
        public void Back()
        {
            fileMenu.check();
            frame--;

            if (frame < 0)
            {
                frame = 0;
            }
            text.text = "" + frame;
           
            fileMenu.ForRealLoad();
        }
        public void Update()
        {
            if (!loaded)
            {
                loaded = true;
                fileMenu.ForRealLoad();
            }

            if (Input.GetKey(KeyCode.H))
            { //|| Input.GetMouseButton(0)){
              //if (Time.time - lastStep > timeBetweenSteps)
              //{
                lastStep = Time.time;
                Next();
                //}
            }
            else if (Input.GetKey(KeyCode.G))
            {
                //if (Time.time - lastStep > timeBetweenSteps)
                //{
                lastStep = Time.time;
                Back();
                //}

            }
        }
        
        public void OnSpace()
        {
            space = !space;

            UpdateButtonState(space, spaceButton);
        }
        /*public static bool GetSpaces()
        {
            return space;
        }*/
        public void OnUp()
        {
            Debug.Log("On Up");
            Debug.Log("Upbutton null? " + (upButton == null));
            up = !up;

            UpdateButtonState(up, upButton);
        }
        /*public static int GetUp()
        {
            return up;
        }*/
        public void OnDown()
        {
            down = !down;

            UpdateButtonState(down, downButton);
        }
        /*public static int GetDown()
        {
            return down;
        }*/
        public void OnLeft()
        {
            left = !left;

            UpdateButtonState(left, leftButton);
        }
        /*public static int GetLeft()
        {
            return left;
        }*/
        public void OnRight()
        {
            right = !right;

            UpdateButtonState(right, rightButton);
        }
        /*public int GetRight()
        {
            return buttons[right];
        }*/
        public static string GetKeys()
        {
            return space + "," + up + "," + down + "," + left + "," + right + "\n";
        }
        public void SetKeys(string line)
        {
            string[] keys = line.Split(',');
            space = bool.Parse(keys[0]);
            up = bool.Parse(keys[1]);
            down = bool.Parse(keys[2]);
            left = bool.Parse(keys[3]);
            right = bool.Parse(keys[4]);
            UpdateButtonState(space, spaceButton);
            UpdateButtonState(up, upButton);
            UpdateButtonState(down, downButton);
            UpdateButtonState(left, leftButton);
            UpdateButtonState(right, rightButton);
        }
        public void ResetKeys()
        {
            space = false;
            up = false;
            down = false;
            left = false;
            right = false;
            UpdateButtonState(space, spaceButton);
            UpdateButtonState(up, upButton);
            UpdateButtonState(down, downButton);
            UpdateButtonState(left, leftButton);
            UpdateButtonState(right, rightButton);
        }
        private void UpdateButtonState(bool off, Button button)
        {
            ColorBlock colors = button.colors;
            if (!off)
            {
                colors.normalColor = Color.white;
                colors.highlightedColor = Color.white;
            }
            else
            {
                colors.normalColor = Color.gray;
                colors.highlightedColor = Color.gray;
            }
            button.colors = colors;
            
        }
    }
}