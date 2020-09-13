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
        [SerializeField] private float zoomLerpSpeed = 10;
        private int turnIdentifier = 0;
        // Start is called before the first frame update
        void Start()
        {

            cam = Camera.main;
            targetZoom = cam.orthographicSize;
            turnIdentifier = 0;

        }

        // Update is called once per frame
        void Update()
        {
            
                float scrollData;
                scrollData = Input.GetAxis("Mouse ScrollWheel");
            Debug.Log(scrollData);
            if (scrollData == 0)
            {
                Debug.Log("I am in 1st section");
                //targetZoom = Mathf.Clamp(targetZoom, 5f, 18f);
                //cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime);
                cam.orthographicSize = Mathf.Clamp(targetZoom, 5f, 18f);
            }
            else
            {
                Debug.Log("I am in 2nd section");
                targetZoom -= scrollData * zoomFactor;
                Debug.Log(targetZoom);
                cam.orthographicSize = Mathf.Clamp(targetZoom, 5f, 18f);
                //cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime);
                

            }
        }
    }
}