using Assets.Scripts.Core;
using UnityEngine;

//Position X Fact
public class PositionXFact : Fact
{
    public int positionVal;

    public PositionXFact(string fact)
    {
        string[] separatingStrings = { "[", ",", "]" };
        string[] vals = fact.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
        this.componentID = (int)float.Parse(vals[vals.Length - 2]);
        this.positionVal = (int)float.Parse(vals[vals.Length - 1]);
    }

    //Check to see if this GridObject matches this fact
    public override bool Matches(GridObject g)
    {
        //TODO; fix this so that it doesn't depend on size of gridobject

        return g.X-1 == positionVal || g.X==positionVal;
    }

    public override string ToString()
    {
        return "PositionXFact: " + positionVal;
    }
}