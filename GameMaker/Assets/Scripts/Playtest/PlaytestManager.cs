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


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        timerMax = 1 / FPS;
        VariableFact.testing = true;
        gridObjects = new List<GridObject>();
        string filePath = Constants.directory + Constants.GetGameName() + " " + 0 + ".csv";
        if (File.Exists(filePath))
        {
            // - Parse file
            string[] lines = File.ReadAllLines(filePath);
            //Debug.Log(lines[0]); actions
            string[] gridSize = lines[2].Split(',');
            //Debug.Log(lines[1]); grid size
            for (int i = 3; i < lines.Length; i++)
            {
                string[] line = lines[i].Split(',');
                GridObject clone = Instantiate(gridObjectPrefab);
                clone.SetSprite(SpriteManager.Instance.GetSprite(line[0]));
                clone.SetPosition(int.Parse(line[1]), int.Parse(line[2]));
                clone.transform.parent = transform;
                gridObjects.Add(clone);
            }

        }
    }

    public void AddObject(string name, Vector2 position)
    {
        GridObject clone = Instantiate(gridObjectPrefab);
        clone.SetSprite(SpriteManager.Instance.GetSprite(name));
        clone.SetPosition((int)position.x, (int)position.y);
        clone.transform.position = new Vector3(position.x, position.y);
        clone.transform.parent = transform;
        gridObjects.Add(clone);
    }

    public void RemoveObject(int id)
    {
        if (id < gridObjects.Count && id >= 0)
        {
            GridObject g = gridObjects[id];
            if (g != null)
            {
                gridObjects.Remove(g);
            }
        }
    }

    void Update()
    {
        

        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("Main");
        }


        if (timer < timerMax)
        {
            timer += Time.deltaTime;
        }
        else
        {
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
                    y += g.VY;
                }

                g.SetPosition(x, y);
            }

            spacePrev = Input.GetKey(KeyCode.Space);
            upPrev = Input.GetKey(KeyCode.UpArrow);
            downPrev = Input.GetKey(KeyCode.DownArrow);
            leftPrev = Input.GetKey(KeyCode.LeftArrow);
            rightPrev = Input.GetKey(KeyCode.RightArrow);

        }

        


    }
}
