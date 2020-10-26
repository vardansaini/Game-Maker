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
        
        private bool loaded = false;
        public Button spaceButton;
        public Button upButton;
        public Button downButton;
        public Button leftButton;
        public Button rightButton;
        public static FrameManager Instance;


        private static bool space;
        private static bool up;
        private static bool down;
        private static bool left;
        private static bool right;

        private static bool spacePrev;
        private static bool upPrev;
        private static bool downPrev;
        private static bool leftPrev;
        private static bool rightPrev;

        public static bool Space { get { return space; } }
        public static bool Up { get { return up; } }
        public static bool Down { get { return down; } }
        public static bool Left { get { return left; } }
        public static bool Right { get { return right; } }

        public static bool SpacePrev { get { return spacePrev; } }
        public static bool UpPrev { get { return upPrev; } }
        public static bool DownPrev { get { return downPrev; } }
        public static bool LeftPrev { get { return leftPrev; } }
        public static bool RightPrev { get { return rightPrev; } }

        float lastStep, timeBetweenSteps = 0.5f;
        public InputField eraseField;
        int max = 0;
        void Start()
        {
            Instance = this;
        }
        public static int GetPrevFrame()
        {
            return frame - 1;
        }
        public static int GetNextFrame()
        {
            return frame + 1;
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
            if (frame > max)
                max = frame;
            eraseField.text = "";
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
            eraseField.text = "";
            text.text = "" + frame;
            fileMenu.ForRealLoad();
        }
        public void Update()
        {
             if (!loaded)
             {
                 loaded = true;
                GridManager.Instance.ResetGridSize();
                 fileMenu.ForRealLoad();
             }

             if (Input.GetKey(KeyCode.RightArrow))
             { 
               if (Time.time - lastStep >= timeBetweenSteps)
               {
                 lastStep = Time.time;
                 Next();
                 }
             }
             else if (Input.GetKey(KeyCode.LeftArrow))
             {
                 if (Time.time - lastStep >= timeBetweenSteps)
                 {
                 lastStep = Time.time;
                 Back();
                 }

             }

        }

        public void Update(String value)
        {                      
            Debug.Log(value);
            int temp;
            bool success = int.TryParse(value,out temp); 
            if (success)
            {                
                if (temp >= 0)                
                {
                    frame = temp;
                    text.text = "" + frame;
                    fileMenu.ForRealLoad();
                }
            }
        }

        public void OnSpace()
        {
            space = !space;

            UpdateButtonState(space, spaceButton);
        }
        
        public void OnUp()
        {
            up = !up;

            UpdateButtonState(up, upButton);
        }
        
        public void OnDown()
        {
            down = !down;

            UpdateButtonState(down, downButton);
        }
       
        public void OnLeft()
        {
            left = !left;

            UpdateButtonState(left, leftButton);
        }
        
        public void OnRight()
        {
            right = !right;

            UpdateButtonState(right, rightButton);
        }
      
        
        public static string GetKeys()
        {
            return space + "," + up + "," + down + "," + left + "," + right + "\n";
        }

        public static string GetPrevKeys()
        {
            return spacePrev + "," + upPrev + "," + downPrev + "," + leftPrev + "," + rightPrev + "\n";
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

        public void SetPrevKeys(string line)
        {
            string[] keys = line.Split(',');
            spacePrev = bool.Parse(keys[0]);
            upPrev = bool.Parse(keys[1]);
            downPrev = bool.Parse(keys[2]);
            leftPrev = bool.Parse(keys[3]);
            rightPrev = bool.Parse(keys[4]);
        }

        public void ResetPrevKeys()
        {
            spacePrev = false;
            upPrev = false;
            downPrev = false;
            leftPrev = false;
            rightPrev = false;
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
                colors.normalColor = Color.green ;
                colors.highlightedColor = Color.green;
            }
            button.colors = colors;
            
        }
    }
}