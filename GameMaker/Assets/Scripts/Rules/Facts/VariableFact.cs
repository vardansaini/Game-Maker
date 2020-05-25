using Assets.Scripts.Core;
using Assets.Scripts.UI;
using UnityEngine;

//Variable fact is special
public class VariableFact : Fact
{
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
        else if (variableName.Equals("space"))
        {
            if (variableValue[0] == 1 == FrameManager.Space)
            {
                return true;
            }
        }
        else if (variableName.Equals("up"))
        {
            if (variableValue[0] == 1 == FrameManager.Up)
            {
                return true;
            }
        }
        else if (variableName.Equals("down"))
        {
            if (variableValue[0] == 1 == FrameManager.Down)
            {
                return true;
            }
        }
        else if (variableName.Equals("left"))
        {
            if (variableValue[0] == 1 == FrameManager.Left)
            {
                return true;
            }
        }
        else if (variableName.Equals("right"))
        {
            if (variableValue[0] == 1 == FrameManager.Right)
            {
                return true;
            }
        }
        return false;
    }

    public override string ToString()
    {
        return "VariableFact " + variableName + " " + variableValue[0];
    }
}