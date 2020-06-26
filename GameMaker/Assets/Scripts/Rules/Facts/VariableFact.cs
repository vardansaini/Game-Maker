using Assets.Scripts.Core;
using Assets.Scripts.UI;
using UnityEngine;

//Variable fact is special
public class VariableFact : Fact
{
    public static bool testing = false;

    public string variableName;
    public int[] variableValue;

    public VariableFact(string fact)
    {
        string[] separatingStrings = { "[", ",", "]", "(", ")", "'" };
        string[] vals = fact.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
        variableName = vals[1];
        if (vals.Length == 3)
        {
            variableValue = new int[1];
            if (vals[2].Contains("T"))
            {
                variableValue[0] = 1;
            }
            else
            {
                variableValue[0] = 0;
            }
        }
        else
        {
            variableValue = new int[vals.Length - 3];

            for (int i = 3; i < vals.Length; i++)
            {
                variableValue[i - 3] = int.Parse(vals[i]);
            }
        }
    }

    //GetType Check
    public override bool Matches(GridObject g)
    {
        return Matches();
    }

    public bool Matches()
    {
        if (variableName.Equals("bgColor"))
        {
            return variableName.Equals(GridManager.Instance.GetColor());
        }
        else if ((!testing && variableName.Equals("space") && variableValue[0] == 1 == FrameManager.Space) || (testing && variableName.Equals("space") && variableValue[0] == 1 == Input.GetKey(KeyCode.Space)))
        {
            return true;
        }
        else if ( (!testing && variableName.Equals("up") && variableValue[0] == 1 == FrameManager.Up) || (testing && variableName.Equals("up") && variableValue[0] == 1 == Input.GetKey(KeyCode.UpArrow) ))
        {
            return true;
        }
        else if ((!testing && variableName.Equals("down") && variableValue[0] == 1 == FrameManager.Down) || (testing && variableName.Equals("down") && variableValue[0] == 1 == Input.GetKey(KeyCode.DownArrow)))
        {
            return true;
        }
        else if ((!testing && variableName.Equals("left") && variableValue[0] == 1 == FrameManager.Left) || (testing && variableName.Equals("left") && variableValue[0] == 1 == Input.GetKey(KeyCode.LeftArrow)))
        {
            return true;
        }
        else if ((!testing && variableName.Equals("right") && variableValue[0] == 1 == FrameManager.Right) || (testing && variableName.Equals("right") && variableValue[0] == 1 == Input.GetKey(KeyCode.RightArrow)))
        {
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        return "VariableFact " + variableName + " " + variableValue[0];
    }
}