using Assets.Scripts.Core;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace Assets.Scripts.UI
{

    public class cameraZoomControler : MonoBehaviour
    {
        //private CameraOverview cameraOverview;
        private Camera cam;
        private float targetZoom;
        private float zoomFactor = 3f;
        public Text framenumber;
        public Text framenumberprev;
        public Text framenumbernext;
        // Start is called before the first frame update
        void Start()
        {

            cam = Camera.main;
            targetZoom = cam.orthographicSize;

        }

        // Update is called once per frame
        void Update()
        {

            float scrollData;
            scrollData = Input.GetAxis("Mouse ScrollWheel");
            targetZoom -= scrollData * zoomFactor;
            if (targetZoom < 5f)
            {
                targetZoom = 5f;
            }
            if (targetZoom > 18f)
            {
                targetZoom = 18f;
            }
            if (targetZoom > 12f)
            {
                framenumber.text = FrameManager.GetCurrentFrame().ToString();
                string prevNumber = FrameManager.GetPrevFrame().ToString();
                if (File.Exists(Constants.directory + FileMenu.gameName + prevNumber + ".csv"))
                {
                    framenumberprev.text = prevNumber;
                }
                else
                {
                    framenumberprev.text = "";
                }
                string nextNumber = FrameManager.GetNextFrame().ToString();
                if (File.Exists(Constants.directory + FileMenu.gameName + nextNumber + ".csv"))
                {
                    framenumbernext.text = nextNumber;
                }
                else 
                {
                    framenumbernext.text = "";
                }
            }
            else
            {
                framenumber.text = "";
                framenumberprev.text = "";
                framenumbernext.text = "";
            }
            //Debug.Log(targetZoom);
            cam.orthographicSize = Mathf.Clamp(targetZoom, 5f, 18f);
        }
    }
}