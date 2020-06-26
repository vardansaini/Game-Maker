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



    // Start is called before the first frame update
    void Awake()
    {
        timerMax = 1 / FPS;
        VariableFact.testing = true;
        gridObjects = new List<GridObject>();
        string filePath = Constants.directory + Constants.GetGameName() + " " + 0 + ".csv";
        if (File.Exists(filePath))
        {
            // - Parse file
            string[] lines = File.ReadAllLines(filePath);
            //Debug.Log(lines[0]); actions
            string[] gridSize = lines[1].Split(',');
            //Debug.Log(lines[1]); grid size
            for (int i = 2; i < lines.Length; i++)
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
        }
    }
}
