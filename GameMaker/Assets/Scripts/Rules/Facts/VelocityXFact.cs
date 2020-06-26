using Assets.Scripts.Core;
using UnityEngine;

//Velocity X Fact
public class VelocityXFact : Fact
{
    public int velocityVal;

    public VelocityXFact(string fact)
    {
        string[] separatingStrings = { "[", ",", "]" };
        string[] vals = fact.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
        if (vals.Length == 3)
        {
            this.componentID = (int.Parse(vals[1]));
            this.velocityVal = (int)float.Parse(vals[2]);
        }
        else
        {//TODO; fix this laziness
            this.componentID = (int.Parse(vals[2]));
            this.velocityVal = (int)float.Parse(vals[3]);
        }
    }

    //Check to see if this GridObject matches this fact
    public override bool Matches(GridObject g)
    {
        return g.VX == velocityVal;
    }

    public override string ToString()
    {
        return "VelocityXFact " + velocityVal;
    }
}