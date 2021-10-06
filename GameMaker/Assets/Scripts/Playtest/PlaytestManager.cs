using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaytestManager : MonoBehaviour
{
    [SerializeField]
    private GridObject gridObjectPrefab;

    [SerializeField]
    private RuleManager ruleManager;

    private List<GridObject> gridObjects;
    private float timer = 0;
    private float timerMax = 0f;

    public float FPS = 12;

    public static PlaytestManager Instance;

    public List<GridObject> GridObjects { get { return gridObjects; } }

    private static bool spacePrev, upPrev, downPrev, leftPrev, rightPrev;

    public static bool SpacePrev { get { return spacePrev; } }
    public static bool UpPrev { get { return upPrev; } }
    public static bool DownPrev { get { return downPrev; } }
    public static bool LeftPrev { get { return leftPrev; } }
    public static bool RightPrev { get { return rightPrev; } }

    private static bool space, up, down, left, right;

    public static bool Space { get { return space; } }
    public static bool Up { get { return up; } }
    public static bool Down { get { return down; } }
    public static bool Left { get { return left; } }
    public static bool Right { get { return right; } }


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        timerMax = 1 / FPS;
        VariableFact.testing = true;
        gridObjects = new List<GridObject>();
        //Debug.Log(Constants.directory + 0 + ".csv");
        string filePath = Constants.directory + 0 + ".csv";
        if (File.Exists(filePath))
        {
            // - Parse file
            string[] lines = File.ReadAllLines(filePath);
            //Debug.Log(lines[0]); //actions
            string[] gridSize = lines[2].Split(',');
            //Debug.Log(lines[2]); //grid size
            for (int i = 3; i < lines.Length; i++)
            {
                //Debug.Log("I am inside For");
                string[] line = lines[i].Split(',');
                //Debug.Log(gridObjectPrefab);
                GridObject clone = Instantiate(gridObjectPrefab);
                //Debug.Log(SpriteManager.Instance.GetSprite(line[0]));
                clone.SetSprite(SpriteManager.Instance.GetSprite(line[0]));
                //Debug.Log(int.Parse(line[1]) + " " + int.Parse(line[2]));
                clone.SetPosition(int.Parse(line[1]), int.Parse(line[2]));
                clone.transform.parent = transform;
                gridObjects.Add(clone);
            }

        }
    }


    public GridObject AddObject(string name, Vector2 position)
    {

        GridObject clone = Instantiate(gridObjectPrefab);
        clone.SetSprite(SpriteManager.Instance.GetSprite(name));
        clone.SetPosition((int)position.x, (int)position.y);
        clone.transform.position = new Vector3(position.x, position.y);

        bool canAdd = true;

        foreach(GridObject g in gridObjects)
        {
            //Double check that there's no overlap
            if (clone.X <=g.X && clone.X+clone.W>g.X && clone.Y<=g.Y && clone.Y+clone.H>g.Y)
            {
                canAdd = false;
                break;
            }
            else if (g.X <= clone.X && g.X + g.W > clone.X && g.Y <= clone.Y && g.Y + g.H > clone.Y)
            {
                canAdd = false;
                break;
            }
        }


        if (canAdd)
        {
            clone.transform.parent = transform;
            gridObjects.Add(clone);
        }
        else
        {
            Destroy(clone.gameObject);
        }

        return clone;
    }

    public void RemoveObject(int id)
    {
        if (id < gridObjects.Count && id >= 0)
        {
            GridObject g = gridObjects[id];
            if (g != null)
            {
                gridObjects.Remove(g);
                Destroy(g.gameObject);
            }
        }
    }

    void Update()
    {

        if (Input.GetKey(KeyCode.Escape))
        {
            LogHandler.Instance.WriteLine("Escape was pressed to go to main scene :  time = " + Time.time);
            //LogHandler.Instance.CloseWriter();
            SceneManager.LoadScene("Main");
        }


        if (timer < timerMax)
        {
            timer += Time.deltaTime;
        }
        else
        {

            space = Input.GetKey(KeyCode.Space);
            up = Input.GetKey(KeyCode.UpArrow);
            down = Input.GetKey(KeyCode.DownArrow);
            left = Input.GetKey(KeyCode.LeftArrow);
            right = Input.GetKey(KeyCode.RightArrow);
            //Debug.Log("Up: " + up);
            if (space == true)
            {
                LogHandler.Instance.WriteLine("Space was pressed to test :  time = " + Time.time);
            }
            if (up == true)
            {
                LogHandler.Instance.WriteLine("Up was pressed to test :  time = " + Time.time);
            }
            if (down == true)
            {
                LogHandler.Instance.WriteLine("Down was pressed to test :  time = " + Time.time);
            }
            if (left == true)
            {
                LogHandler.Instance.WriteLine("Left was pressed to test :  time = " + Time.time);
            }
            if (right == true)
            {
                LogHandler.Instance.WriteLine("Right was pressed to test :  time = " + Time.time);
            }

            timer = 0;
            gridObjects = ruleManager.RunRules(gridObjects);

            //Update positions based on velocity
            foreach (GridObject g in gridObjects)
            {
                int x = g.X;
                int y = g.Y;
                if (Mathf.Abs(g.VX) > 0)
                {
                    
                    x += g.VX;
                }
                else
                {
                    if (Mathf.Abs(g.VY) > 0)
                    {
                        y += g.VY;
                        Debug.Log("Updated to " + y + " due to " + g.VY);
                    }
                }

                g.SetPosition(x, y);
            }

            spacePrev = space;
            upPrev = up;
            downPrev = down;
            leftPrev = left;
            rightPrev = right;

        }

        



    }
}
