﻿using Assets.Scripts.Util;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Core
{
    public class GridManager : Singleton<GridManager>
    {
        public int GridWidth
        { get; private set; }
        public int GridHeight { get; private set; }

        public event Action<int, int> GridSizeChanged;

        [SerializeField]
        private GridObject gridObjectPrefab;
        [SerializeField]
        private Transform gridObjectParentFunctional;
        [SerializeField]
        private Transform gridObjectParentDecorative;

        [SerializeField]
        private Vector2 initialGridSize;

        private GridObject[,] gridFunctional;
        private GridObject[,] gridDecorative;

        private List<GridObject> gridObjects;
        private List<GridObject> previewObjects;
        //private List<GridObject> addobjects;


        private void Start()
        {
            gridObjects = new List<GridObject>();
            //addobjects = new List<GridObject>();
            previewObjects = new List<GridObject>();
            SetGridSize(Mathf.RoundToInt(initialGridSize.x), Mathf.RoundToInt(initialGridSize.y), false);
        }

        public void SetGridSize(int x, int y, bool keepObjects)
        {
            // Store old grid objects
            List<GridObject> oldGridObjects = new List<GridObject>();
            if (keepObjects)
            {
                foreach (GridObject gridObject in gridObjects)
                    oldGridObjects.Add(gridObject);
            }

            // Reinitialize grid
            ClearGrid();
            GridWidth = x;
            GridHeight = y;
            gridFunctional = new GridObject[x, y];
            gridDecorative = new GridObject[x, y];
           
            

            // Add old grid objects back
            if (keepObjects)
            {
                foreach (GridObject gridObject in oldGridObjects)
                    AddGridObject(gridObject.Data, gridObject.X, gridObject.Y, false);
            }

            if (GridSizeChanged != null)
                GridSizeChanged(x, y);
        }

        public void ClearGrid()
        {
            LogHandler.Instance.WriteLine("Grid Cleared:  time = " + Time.time);
            if (gridObjects == null)
                return;
            foreach (GridObject gridObject in gridObjects)
                Destroy(gridObject.gameObject);
            gridObjects.Clear();
        }

        public bool CanAddGridObject(SpriteData sprite, int x, int y)
        {
            if (x < 0 || x + sprite.Width > GridWidth)
                return false;
            else if (y < 0 || y + sprite.Height > GridHeight)
                return false;

            for (int i = x; i < x + sprite.Width; i++)
            {
                for (int j = y; j < y + sprite.Height; j++)
                {
                    if ((sprite.Functional ? gridFunctional[i, j] : gridDecorative[i, j]) != null) {
                        return false;
                    }
                }
            }

            return true;
        }

        public GridObject AddGridObject(SpriteData sprite, int x, int y, bool writeLog)
        {
            if (!CanAddGridObject(sprite, x, y)) {
                return null;
            }

            if (ContainsGridObject(sprite.Functional, x, y))
                return null;

            if (writeLog) {
                LogHandler.Instance.WriteLine("Added " + sprite.Name + " at " + x + ", " + y + ":  time = " + Time.time);
            }

            // Instantiate object
            GridObject clone = Instantiate(gridObjectPrefab, sprite.Functional ? gridObjectParentFunctional : gridObjectParentDecorative);

            clone.SetSprite(sprite);
            clone.SetPosition(x, y);
            gridObjects.Add(clone);

            // Add references to object in grid
            for (int i = x; i < x + sprite.Width; i++)
            {
                for (int j = y; j < y + sprite.Height; j++)
                {
                    if (sprite.Functional)
                        gridFunctional[i, j] = clone;
                    else
                        gridDecorative[i, j] = clone;
                }
            }

            return clone;
        }

        public void RemoveGridObject(bool functional, int x, int y)
        {
            if (!ContainsGridObject(functional, x, y))
                return;

            // Remove the grid object
            GridObject gridObject = (functional ? gridFunctional[x, y] : gridDecorative[x, y]);
            LogHandler.Instance.WriteLine("Removeh " + gridObject.Data.Name + " at " + x + ", " + y + ":  time = " + Time.time);
            gridObjects.Remove(gridObject);
            // - Automatically removes all references in grid
            Destroy(gridObject.gameObject);
        }

        public bool ContainsGridObject(bool functional, int x, int y)
        {
            if (x < 0 || x >= GridWidth)
                return false;
            else if (y < 0 || y >= GridHeight)
                return false;

            return (functional ? gridFunctional[x, y] : gridDecorative[x, y]) != null;
        }

        public bool ContainsSolid(int x, int y) {
            if (ContainsGridObject(true, x, y)) {
                if (gridFunctional[x, y].Data.Name != "Coin") {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        public string FormatToCSV()
        {

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(GridWidth + "," + GridHeight);
            foreach (GridObject gridObject in gridObjects)
                builder.AppendLine(gridObject.Data.Name + "," + gridObject.X + "," + gridObject.Y + "," + gridObject.W + "," + gridObject.H);
            
            
            //for (int i = 0; i < FrameManager.GetSpaces(); i++)
            //for (int j = 0; j < FrameManager.GetUp(); j++)
            //builder.AppendLine("SPACE");
            //builder.AppendLine("UP");

            //for (int i = 0; i < FrameManager.GetSpaces(); i++)
            //builder.AppendLine("SPACE");
            //for (int i = 0; i < FrameManager.GetUp(); i++)
            //builder.AppendLine("UP");
            //for (int i = 0; i < FrameManager.GetDown(); i++)
            //builder.AppendLine("DOWN");
            //for (int i = 0; i < FrameManager.GetLeft(); i++)
            // builder.AppendLine("LEFT");
            //for (int i = 0; i < FrameManager.GetRight(); i++)
            // builder.AppendLine("RIGHT");
            return builder.ToString();
        }

        public void SetAllTransparent(float transparency)
        {
            foreach (GridObject gridObject in gridObjects)
            {
                gridObject.SetAlpha(transparency);
            }
        }

        public void SetPriorGridObjectsToPreviewOnly(float transparency)
        {
            SetAllTransparent(transparency);
            ClearPreview();
           
            foreach (GridObject gridObject in gridObjects)
            {
                previewObjects.Add(gridObject);
            }

            gridObjects.Clear();
            gridFunctional = new GridObject[GridWidth, GridHeight];
            gridDecorative = new GridObject[GridWidth, GridHeight];

        }

        public void ClearPreview()
        {
           
            
                foreach (GridObject gridObject in previewObjects)
                    Destroy(gridObject.gameObject);

                previewObjects = new List<GridObject>();
            

        }
        public void AddPreviousFrameToCurrentFrame()
        {
           
              foreach (GridObject gridObject in previewObjects)
              {
                gridObject.SetAlpha(1);
                AddGridObject(gridObject.Data, gridObject.X, gridObject.Y, false);
                Destroy(gridObject.gameObject);
                //gridObjects.Add(gridObject);
              }

            previewObjects.Clear();
   
        }

        /**
        public void AddKeptObjectsToGrid()
        { 
            foreach (GridObject gridObject in addobjects)
            {
                gridObjects.Add(gridObject);
            }
        }
    */

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("hello");
                AddPreviousFrameToCurrentFrame();
            }
           
        }
        public bool Checklist()
        {
            return gridObjects.Count > 0;
        }


    }
}
