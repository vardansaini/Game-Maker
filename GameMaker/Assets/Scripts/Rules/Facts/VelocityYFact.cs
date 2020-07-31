using Assets.Scripts.Core;

//Velocity Y Fact
public class VelocityYFact : Fact
{
    public int velocityVal;

    public VelocityYFact(string fact)
    {
        string[] separatingStrings = { "[", ",", "]" };
        string[] vals = fact.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
        this.componentID = (int.Parse(vals[vals.Length - 2]));
        this.velocityVal = (int)float.Parse(vals[vals.Length - 1]);
    }

    //Check to see if this GridObject matches this fact
    public override bool Matches(GridObject g)
    {
        return g.VY == velocityVal;
    }

    public override string ToString()
    {
        return "VelocityYFact " + velocityVal;
    }
}
