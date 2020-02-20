using Assets.Scripts.UI;
using Assets.Scripts.Util;
using System;
using System.Collections;
using System.Diagnostics;
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
    [RequireComponent(typeof(Button))]
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
        public Button mybutton1;
        public Button mybutton2;
        public Button mybutton3;
        public Button mybutton4;
        public Button mybutton5;
        public Sprite able;
        public Sprite disable;
        
        //bool[] keyDefault = {false, false,false,false,false};
        // Dictionary<string, bool> buttons = new Dictionary<string, bool>();
        float lastStep, timeBetweenSteps = 0.5f;

        void Start()
        {
            mybutton1 = GetComponent<Button>();
            mybutton2 = GetComponent<Button>();
            mybutton3 = GetComponent<Button>();
            mybutton4 = GetComponent<Button>();
            mybutton5 = GetComponent<Button>();
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
                this.Back();
                //}

            }
        }
        /*public void changeButton()
        {
            counter++;
            if(counter % 2 == 0)
            {
                mybutton1.image.overrideSprite = able;
                mybutton2.image.overrideSprite = able;
                mybutton3.image.overrideSprite = able;
                mybutton4.image.overrideSprite = able;
                mybutton5.image.overrideSprite = able;

            }
            else
            {
                mybutton1.image.overrideSprite = disable;
            }
        }*/
        public void OnSpace()
        {
            space = !space;

            UpdateButtonState(space, mybutton1);
        }
        /*public static bool GetSpaces()
        {
            return space;
        }*/
        public void OnUp()
        {
            up = !up;

            UpdateButtonState(up, mybutton2);
        }
        /*public static int GetUp()
        {
            return up;
        }*/
        public void OnDown()
        {
            down = !down;

            UpdateButtonState(down, mybutton3);
        }
        /*public static int GetDown()
        {
            return down;
        }*/
        public void OnLeft()
        {
            left = !left;

            UpdateButtonState(left, mybutton4);
        }
        /*public static int GetLeft()
        {
            return left;
        }*/
        public void OnRight()
        {
            right = !right;

            UpdateButtonState(right, mybutton5);
        }
        /*public int GetRight()
        {
            return buttons[right];
        }*/
        public static string GetKeys()
        {
            return space + "," + up + "," + down + "," + left + "," + right + "\n";
        }
        public static void SetKeys(string line)
        {
            string[] keys = line.Split(',');
            space = bool.Parse(keys[0]);
            up = bool.Parse(keys[1]);
            down = bool.Parse(keys[2]);
            left = bool.Parse(keys[3]);
            right = bool.Parse(keys[4]);
            UpdateButtonState(space, mybutton1);
            UpdateButtonState(up, mybutton2);
            UpdateButtonState(down, mybutton3);
            UpdateButtonState(left, mybutton4);
            UpdateButtonState(right, mybutton5);
        }
        public static void ResetKeys()
        {
            space = false;
            up = false;
            down = false;
            left = false;
            right = false;
            UpdateButtonState(space, mybutton1);
            UpdateButtonState(up, mybutton2);
            UpdateButtonState(down, mybutton3);
            UpdateButtonState(left, mybutton4);
            UpdateButtonState(right, mybutton5);
        }
        private void UpdateButtonState(bool value, Button button)
        {
            if (value)
            {
                button.image.overrideSprite = able;

            }
            else
            {
                button.image.overrideSprite = disable;
            }
        }
    }
}