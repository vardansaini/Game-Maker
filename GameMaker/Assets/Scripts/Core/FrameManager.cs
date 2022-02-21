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

        [SerializeField]
        public InputField eraseField;
        int max;

        void Start()
        {
            Instance = this;
        }

        public static int ResetFrame()
        {
            return frame = 0;
        }

        public static int GetPrevFrame()
        {
            return frame - 1;
        }

        public static int GetPrevPrevFrame()
        {
            return frame - 2;
        }

        public static int GetNextFrame()
        {
            return frame + 1;
        }

        public static int GetCurrentFrame()
        {
            return frame;
        }

        public int GetmaxFrame()
        {
            return max;
        }

        public static void SetCurrentFrame(int current)
        {
            frame = current;
        }

        public void NumberManager(int frameNumber)
        {
            lastStep = Time.time;
            fileMenu.Check();
            eraseField.text = "";
            frame = frameNumber;
            text.text = "" + frame;
            fileMenu.ForRealLoad();
        }

        public void First()
        {
            NumberManager(fileMenu.GetFirstFrame());
        }

        public void Last()
        {
            NumberManager(fileMenu.GetLastFrame());
        }

        public void Next()
        {
            lastStep = Time.time;

            fileMenu.Check();
            frame++;

            if (frame > max)
            {
                max = frame;
            }

            eraseField.text = "";
            text.text = "" + frame;

            fileMenu.ForRealLoad();

            spacePrev = space;
            upPrev = up;
            downPrev = down;
            leftPrev = left;
            rightPrev = right;

            LogHandler.Instance.WriteLine("Next Frame button was pressed " + frame + " :  time = " + Time.time);
        }
        public void Back()
        {
            fileMenu.Check();
            frame--;

            if (frame < 0)
            {
                frame = 0;
            }

            eraseField.text = "";
            text.text = "" + frame;

            fileMenu.ForRealLoad();

            LogHandler.Instance.WriteLine("Previous Frame button was pressed:  time = " + Time.time);
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
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (Time.time - lastStep >= timeBetweenSteps)
                {
                    lastStep = Time.time;
                    Back();
                }

            }

        }

        public void FrameTextManager(String value)
        {
            int temp;
            bool success = int.TryParse(value, out temp);

            if (success)
            {
                if (temp >= 0)
                {
                    frame = temp;

                    text.text = "" + frame;
                    LogHandler.Instance.WriteLine("Frame Text was changed to" + frame + " :  time = " + Time.time);
                    fileMenu.ForRealLoad();

                }
            }

        }

        public void OnSpace()
        {
            space = !space;
            LogHandler.Instance.WriteLine("Space was pressed to " + space + " :  time = " + Time.time);
            UpdateButtonState(space, spaceButton);
        }

        public void OnUp()
        {

            up = !up;
            LogHandler.Instance.WriteLine("Up was pressed to " + Up + " :  time = " + Time.time);
            UpdateButtonState(up, upButton);
        }
        public void OnDown()
        {
            down = !down;
            LogHandler.Instance.WriteLine("Down was pressed to " + down + " :  time = " + Time.time);
            UpdateButtonState(down, downButton);
        }

        public void OnLeft()
        {
            left = !left;
            LogHandler.Instance.WriteLine("Left was pressed to " + left + " :  time = " + Time.time);
            UpdateButtonState(left, leftButton);
        }

        public void OnRight()
        {
            right = !right;
            LogHandler.Instance.WriteLine("Right was pressed to " + right + " :  time = " + Time.time);
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
                colors.normalColor = Color.green;
                colors.highlightedColor = Color.green;
            }
            button.colors = colors;

        }
    }
}