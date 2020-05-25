using Assets.Scripts.Core;

//Velocity X Fact
public class VelocityXFact : Fact
{
    public int velocityVal;

    public VelocityXFact(string fact)
    {
        string[] separatingStrings = { "[", ",", "]" };
        string[] vals = fact.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
        this.componentID = (int.Parse(vals[1]));
        this.velocityVal = (int)float.Parse(vals[2]);
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