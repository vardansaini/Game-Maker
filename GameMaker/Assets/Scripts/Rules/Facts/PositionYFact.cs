using Assets.Scripts.Core;
using UnityEngine;

//Position X Fact
public class PositionYFact : Fact
{
    public int positionVal;

    public PositionYFact(string fact)
    {
        string[] separatingStrings = { "[", ",", "]" };
        string[] vals = fact.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
        this.componentID = (int.Parse(vals[vals.Length - 2]));
        this.positionVal = (int)float.Parse(vals[vals.Length - 1]);
    }

    //Check to see if this GridObject matches this fact
    public override bool Matches(GridObject g)
    {
        return Mathf.Abs(g.Y-positionVal)<Constants.threshold;
    }

    public override string ToString()
    {
        return "PositionYFact: " + positionVal;
    }
}