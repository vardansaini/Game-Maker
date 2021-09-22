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
        if (variableName.Equals("rightPrev"))
        {
            //Debug.Log(FrameManager.RightPrev);
        }
        if (variableName.Equals("bgColor"))
        {
            return variableName.Equals(GridManager.Instance.GetColor());
        }
        else if ((!testing && variableName.Equals("space") && variableValue[0] == 1 == FrameManager.Space) || (testing && variableName.Equals("space") && variableValue[0] == 1 == PlaytestManager.Space))
        {
            return true;
        }
        else if ( (!testing && variableName.Equals("up") && variableValue[0] == 1 == FrameManager.Up) || (testing && variableName.Equals("up") && variableValue[0] == 1 == PlaytestManager.Up))
        {
            return true;
        }
        else if ((!testing && variableName.Equals("down") && variableValue[0] == 1 == FrameManager.Down) || (testing && variableName.Equals("down") && variableValue[0] == 1 == PlaytestManager.Down))
        {
            return true;
        }
        else if ((!testing && variableName.Equals("left") && variableValue[0] == 1 == FrameManager.Left) || (testing && variableName.Equals("left") && variableValue[0] == 1 == PlaytestManager.Left))
        {
            return true;
        }
        else if ((!testing && variableName.Equals("right") && variableValue[0] == 1 == FrameManager.Right) || (testing && variableName.Equals("right") && variableValue[0] == 1 == PlaytestManager.Right))
        {
            return true;
        }
        else if ((!testing && variableName.Equals("spacePrev") && variableValue[0] == 1 == FrameManager.SpacePrev) || (testing && variableName.Equals("spacePrev") && variableValue[0] == 1 == PlaytestManager.SpacePrev))
        {
            return true;
        }
        else if ((!testing && variableName.Equals("upPrev") && variableValue[0] == 1 == FrameManager.UpPrev) || (testing && variableName.Equals("upPrev") && variableValue[0] == 1 == PlaytestManager.UpPrev))
        {
            return true;
        }
        else if ((!testing && variableName.Equals("downPrev") && variableValue[0] == 1 == FrameManager.DownPrev) || (testing && variableName.Equals("downPrev") && variableValue[0] == 1 == PlaytestManager.DownPrev))
        {
            return true;
        }
        else if ((!testing && variableName.Equals("leftPrev") && variableValue[0] == 1 == FrameManager.LeftPrev) || (testing && variableName.Equals("leftPrev") && variableValue[0] == 1 == PlaytestManager.LeftPrev))
        {
            return true;
        }
        else if ((!testing && variableName.Equals("rightPrev") && variableValue[0] == 1 == FrameManager.RightPrev) || (testing && variableName.Equals("rightPrev") && variableValue[0] == 1 == PlaytestManager.RightPrev))
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