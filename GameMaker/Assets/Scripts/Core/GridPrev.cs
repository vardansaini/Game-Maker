﻿using Assets.Scripts.Util;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts.Core
{
    public class GridPrev : Singleton<GridPrev>
    {
        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }

        public event Action<int, int> GridSizeChanged;

        [SerializeField]
        private GridObjectPrev gridObjectPrefab;
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

        private GridObjectPrev[,] gridFunctional;
        private GridObjectPrev[,] gridDecorative;

        private List<GridObjectPrev> gridObjects;
        private List<GridObjectPrev> previewObjects;

        public static GridPrev Instance;

        public bool gridSizeSet = false;

        void Awake()
        {
            VariableFact.testing = false;
            Instance = this;
            gridObjects = new List<GridObjectPrev>();
            previewObjects = new List<GridObjectPrev>();
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
            List<GridObjectPrev> oldGridObjects = new List<GridObjectPrev>();
            if (keepObjects)
            {
                foreach (GridObjectPrev gridObject in gridObjects)
                    oldGridObjects.Add(gridObject);
            }

            // Reinitialize grid
            ClearGrid();
            GridWidth = x;
            GridHeight = y;
            gridFunctional = new GridObjectPrev[x, y];
            gridDecorative = new GridObjectPrev[x, y];



            // Add old grid objects back
            if (keepObjects)
            {
                foreach (GridObjectPrev gridObject in oldGridObjects)
                    GridPrev.Instance.AddGridObject(gridObject.Data, gridObject.X, gridObject.Y, false);
            }

            if (GridSizeChanged != null)
                GridSizeChanged(x, y);
        }

        public void ClearGrid()
        {
            /*if (LogHandler.Instance != null)
            {
                LogHandler.Instance.WriteLine("Grid Cleared:  time = " + Time.time);
            }*/

            if (gridObjects == null)
                return;
            foreach (GridObjectPrev gridObject in gridObjects)
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

        public GridObjectPrev AddGridObject(SpriteData sprite, int x, int y, bool writeLog)
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
            GridObjectPrev clone = Instantiate(gridObjectPrefab, sprite.Functional ? gridObjectParentFunctional : gridObjectParentDecorative);

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

        public GridObjectPrev AddGridObject(GridObjectPrev g)
        {
            GridObjectPrev clone = AddGridObject(g.Data, g.X, g.Y, false);
            clone.VX = g.VX;
            clone.VY = g.VY;
            return clone;

        }

        public void RemoveGridObject(GridObjectPrev g)
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
            GridObjectPrev gridObject = (functional ? gridFunctional[x, y] : gridDecorative[x, y]);
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
            foreach (GridObjectPrev gridObject in gridObjects)
                builder.AppendLine(gridObject.Data.Name + "," + gridObject.X + "," + gridObject.Y + "," + gridObject.W + "," + gridObject.H);

            return builder.ToString();
        }

        public void SetAllTransparent(float transparency)
        {
            foreach (GridObjectPrev gridObject in gridObjects)
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


                foreach (GridObjectPrev gridObject in gridObjects)
                {
                    previewObjects.Add(gridObject);
                }
            }

            gridObjects.Clear();
            gridFunctional = new GridObjectPrev[GridWidth, GridHeight];
            gridDecorative = new GridObjectPrev[GridWidth, GridHeight];

        }

        public void UpdatePreviewGridObjectsFromLearnedRules()
        {            
            //Update positions based on velocity
            foreach (GridObjectPrev g in previewObjects)
            {
                int x = g.X;
                int y = g.Y;
                if (Mathf.Abs(g.VX) > 0)
                {
                    x += g.VX;
                }
                else
                {
                    y += g.VY;
                }

                g.SetPosition(x, y);
            }
        }

        public void ClearPreview()
        {


            foreach (GridObjectPrev gridObject in previewObjects)
                Destroy(gridObject.gameObject);

            previewObjects = new List<GridObjectPrev>();


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

        public GridObjectPrev[] GetObjects()
        {
            return gridObjects.ToArray();
        }
    }


}
