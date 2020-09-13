using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Core;
using UnityEngine;
namespace Assets.Scripts.UI
{

    public class cameraZoomControler : MonoBehaviour
    {
        //private CameraOverview cameraOverview;
        private Camera cam;
        private float targetZoom;
        private float zoomFactor = 3f;
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
            //Debug.Log(targetZoom);
            cam.orthographicSize = Mathf.Clamp(targetZoom, 5f, 18f);
        }
    }
}