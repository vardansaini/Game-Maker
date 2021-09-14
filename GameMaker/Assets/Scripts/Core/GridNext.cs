using Assets.Scripts.Util;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts.Core
{
    public class GridNext : Singleton<GridNext>
    {
        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }

        public event Action<int, int> GridSizeChanged;

        [SerializeField]
        private GridObjectNext gridObjectPrefab;
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

        private GridObjectNext[,] gridFunctional;
        private GridObjectNext[,] gridDecorative;

        private List<GridObjectNext> gridObjects;
        private List<GridObjectNext> previewObjects;

        public static GridNext Instance;

        public bool gridSizeSet = false;

        void Awake()
        {
            VariableFact.testing = false;
            Instance = this;
            gridObjects = new List<GridObjectNext>();
            //addobjects = new List<GridObject>();
            previewObjects = new List<GridObjectNext>();
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
            List<GridObjectNext> oldGridObjects = new List<GridObjectNext>();
            if (keepObjects)
            {
                foreach (GridObjectNext gridObject in gridObjects)
                    oldGridObjects.Add(gridObject);
            }

            // Reinitialize grid
            ClearGrid();
            GridWidth = x;
            GridHeight = y;
            gridFunctional = new GridObjectNext[x, y];
            gridDecorative = new GridObjectNext[x, y];



            // Add old grid objects back
            if (keepObjects)
            {
                foreach (GridObjectNext gridObject in oldGridObjects)
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
            // CHeck for why handler is not working.

            LogHandler.Instance.WriteLine("Grid Cleared:  time = " + Time.time);
            if (gridObjects == null)
                return;
            foreach (GridObjectNext gridObject in gridObjects)
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

        public GridObjectNext AddGridObject(SpriteData sprite, int x, int y, bool writeLog)
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
                // LogHandler.Instance.WriteLine("Added " + sprite.Name + " at " + x + ", " + y + ":  time = " + Time.time);
            }
            LogHandler.Instance.WriteLine("grid previous added" + sprite.Name + " at " + x + ", " + y + ":  time = " + Time.time);

            // Instantiate object
            GridObjectNext clone = Instantiate(gridObjectPrefab, sprite.Functional ? gridObjectParentFunctional : gridObjectParentDecorative);

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

        public GridObjectNext AddGridObject(GridObjectNext g)
        {
            GridObjectNext clone = AddGridObject(g.Data, g.X, g.Y, false);
            clone.VX = g.VX;
            clone.VY = g.VY;
            return clone;

        }

        public void RemoveGridObject(GridObjectNext g)
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
            GridObjectNext gridObject = (functional ? gridFunctional[x, y] : gridDecorative[x, y]);
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
            foreach (GridObjectNext gridObject in gridObjects)
                builder.AppendLine(gridObject.Data.Name + "," + gridObject.X + "," + gridObject.Y + "," + gridObject.W + "," + gridObject.H);

            return builder.ToString();
        }

        public void SetAllTransparent(float transparency)
        {
            foreach (GridObjectNext gridObject in gridObjects)
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


                foreach (GridObjectNext gridObject in gridObjects)
                {
                    previewObjects.Add(gridObject);
                }
            }

            gridObjects.Clear();
            gridFunctional = new GridObjectNext[GridWidth, GridHeight];
            gridDecorative = new GridObjectNext[GridWidth, GridHeight];

        }

        public void UpdatePreviewGridObjectsFromLearnedRules()
        {
            //previewObjects = ruleManager.RunRules(previewObjects);


            //Update positions based on velocity
            foreach (GridObjectNext g in previewObjects)
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


            foreach (GridObjectNext gridObject in previewObjects)
                Destroy(gridObject.gameObject);

            previewObjects = new List<GridObjectNext>();


        }
        /*public void AddPreviousFrameToCurrentFrame()
        {

            foreach (GridObjectNext gridObject in previewObjects)
            {
                gridObject.SetAlpha(1);
                AddGridObject(GridObjectNext);
                Destroy(gridObject.gameObject);
                //gridObjects.Add(gridObject);
            }

            previewObjects.Clear();

        }*/

        /**
        public void AddKeptObjectsToGrid()
        { 
            foreach (GridObject gridObject in addobjects)
            {
                gridObjects.Add(gridObject);
            }
        }
    */

        /*void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))//Changed to P so it would connect to preview
            {
                Load();
            }

        }
        public void Load()
        {
            AddPreviousFrameToCurrentFrame();
        }*/
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

        public GridObjectNext[] GetObjects()
        {
            return gridObjects.ToArray();
        }
        public void DestroyThisGrid() {
            Destroy(gameObject);
        }
    }


}
