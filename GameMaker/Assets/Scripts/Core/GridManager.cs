﻿using Assets.Scripts.Util;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Specialized;

namespace Assets.Scripts.Core
{
    public class GridManager : Singleton<GridManager>
    {
        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }

        public event Action<int, int> GridSizeChanged;

        [SerializeField]
        private FileMenu fileMenu;
        [SerializeField]
        private GridObject gridObjectPrefab;
        [SerializeField]
        private Transform gridObjectParentFunctional;
        [SerializeField]
        private Transform gridObjectParentDecorative;
        [SerializeField]
        private Image gridBackground;

        [SerializeField]
        private RuleManager ruleManager;

        [SerializeField]
        private Vector2 initialGridSize;

        private GridObject[,] gridFunctional;
        private GridObject[,] gridDecorative;

        public List<GridObject> gridObjects;
        private List<GridObject> previewObjects;

        public List<GridObject> PreviewObjects { get { return previewObjects; } }

        public static GridManager Instance;

        public bool gridSizeSet = false;
        public bool RulesActivated = false;

        public static bool RulesActive(bool value)
        {
            return value;
        }

        void Awake()
        {
            VariableFact.testing = false;
            Instance = this;
            gridObjects = new List<GridObject>();
            previewObjects = new List<GridObject>();
            SetGridSize(Mathf.RoundToInt(initialGridSize.x), Mathf.RoundToInt(initialGridSize.y), false);

        }

        public void ResetGridSize()
        {
            SetGridSize(Mathf.RoundToInt(initialGridSize.x), Mathf.RoundToInt(initialGridSize.y), false);
        }

        public void SetGridSize(int x, int y, bool keepObjects)
        {
            gridSizeSet = true;
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
                    GridManager.Instance.AddGridObject(gridObject.Data, gridObject.X, gridObject.Y, false);
            }

            if (GridSizeChanged != null)
                GridSizeChanged(x, y);
        }

        public void ClearGrid()
        {
            if (LogHandler.Instance != null)
            {
                LogHandler.Instance.WriteLine("Grid Cleared:  time = " + Time.time);
            }
            //LogHandler.Instance.WriteLine("Grid Cleared:  time = " + Time.time);
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
                    if ((sprite.Functional ? gridFunctional[i, j] : gridDecorative[i, j]) != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool GridBounds(SpriteData sprite, int x, int y)
        {
            if (x < 0 || x > GridWidth)
                return false;
            else if (y < 0 || y > GridHeight)
                return false;

            return true;
        }

        //Attempts to create a new preview object of the specified type at the specified position if possible
        public GridObject CreateNewPreviewObject(SpriteData sprite, int x, int y)
        {
            if (!CanAddGridObject(sprite, x, y))
            {
                return null;
            }

            if (ContainsGridObject(sprite.Functional, x, y))
            {
                return null;
            }
            // Instantiate object
            GridObject clone = Instantiate(gridObjectPrefab, sprite.Functional ? gridObjectParentFunctional : gridObjectParentDecorative);
            clone.SetSprite(sprite);
            clone.SetPosition(x, y);

            clone.SetAlpha(0.5f);

            return clone;
        }

        public GridObject AddGridObject(SpriteData sprite, int x, int y, bool writeLog)
        {
            if (!CanAddGridObject(sprite, x, y))
            {
                Debug.Log("Cannot add grid object");
                return null;
            }

            if (ContainsGridObject(sprite.Functional, x, y))
            {
                Debug.Log("Grid contains functional");
                return null;
            }


            if (writeLog)
            {
                LogHandler.Instance.WriteLine("Added " + sprite.Name + " at " + x + ", " + y + ":  time = " + Time.time);
            }

            // Instantiate object

            //Check and see if preview object of same type in same location
            GridObject previewObjectToAdd = null;

            foreach (GridObject go in previewObjects)
            {
                if (go.Name == sprite.Name && x == go.X && go.Y == y)
                {
                    previewObjectToAdd = go;
                }
            }

            // Instantiate object
            GridObject clone = Instantiate(gridObjectPrefab, sprite.Functional ? gridObjectParentFunctional : gridObjectParentDecorative);


            clone.SetSprite(sprite);
            clone.SetPosition(x, y);

            if (previewObjectToAdd != null)
            {
                clone.VX = previewObjectToAdd.VX;
                clone.VY = previewObjectToAdd.VY;
            }

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

        public GridObject AddGridObject(GridObject g)
        {
            GridObject clone = AddGridObject(g.Data, g.X, g.Y, false);
            if (clone != null)
            {
                clone.VX = g.VX;
                clone.VY = g.VY;
            }
            return clone;

        }

        public void RemoveGridObject(GridObject g)
        {
            if (gridFunctional[g.X, g.Y] != null && gridFunctional[g.X, g.Y].Equals(g))
            {
                RemoveGridObject(true, g.X, g.Y);
            }
            else
            {
                RemoveGridObject(false, g.X, g.Y);
            }
        }

        public void RemoveGridObject(bool functional, int x, int y)
        {
            if (!ContainsGridObject(functional, x, y))
                return;

            // Remove the grid object
            GridObject gridObject = (functional ? gridFunctional[x, y] : gridDecorative[x, y]);
            LogHandler.Instance.WriteLine("Remove " + gridObject.Data.Name + " at " + x + ", " + y + ":  time = " + Time.time);
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

        public bool ContainsSolid(int x, int y)
        {
            if (ContainsGridObject(true, x, y))
            {
                if (gridFunctional[x, y].Data.Name != "Coin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public string FormatToCSV()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(GridWidth + "," + GridHeight);
            foreach (GridObject gridObject in gridObjects)
                builder.AppendLine(gridObject.Data.Name + "," + gridObject.X + "," + gridObject.Y + "," + gridObject.W + "," + gridObject.H + "," + gridObject.VX + "," + gridObject.VY);

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
            if (gridObjects.Count > 0)
            {
                ClearPreview();


                foreach (GridObject gridObject in gridObjects)
                {
                    previewObjects.Add(gridObject);
                }
                Rule.InitialiseRuleActivationCheck(gridObjects.Count);
            }

            gridObjects.Clear();
            gridFunctional = new GridObject[GridWidth, GridHeight];
            gridDecorative = new GridObject[GridWidth, GridHeight];

        }

        public void UpdatePreviewGridObjectsFromLearnedRules()
        {
            previewObjects = ruleManager.RunRules(previewObjects);

            //Update positions based on velocity
            for (int i = 0; i < previewObjects.Count; i++)
            {
                int x = previewObjects[i].X;
                int y = previewObjects[i].Y;

                List<bool> RuleActivation = Rule.RuleActiveCheck;

                foreach (bool b in RuleActivation)
                {

                    if (b)
                    {
                        x += previewObjects[i].VX;
                        y += previewObjects[i].VY;
                        previewObjects[i].SetPosition(x, y);
                    }
                }
            }

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
                AddGridObject(gridObject);
                Destroy(gridObject.gameObject);
            }

            previewObjects.Clear();

        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Load();
            }

        }
        public void Load()
        {
            AddPreviousFrameToCurrentFrame();
        }
        public bool Checklist()
        {
            return gridObjects.Count > 0;
        }

        public int[] GetColor()
        {
            int[] color = new int[3];
            color[0] = (int)(255f * gridBackground.color.r);
            color[1] = (int)(255f * gridBackground.color.g);
            color[2] = (int)(255f * gridBackground.color.b);

            return color;
        }

        public void SetColor(int[] color)
        {
            gridBackground.color = new Color(color[0] / 255f, color[1] / 255f, color[2] / 255f);
        }

        public GridObject[] GetObjects()
        {
            return gridObjects.ToArray();
        }
    }


}
