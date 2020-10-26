using Assets.Scripts.Core;

//Animation Fact
public class AnimationFact : Fact
{
    public string name;
    public int width;
    public int height;

    public AnimationFact(string fact)
    {
        string[] separatingStrings = { "[", "'", ",", "]"};
        string[] vals = fact.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
        this.componentID = int.Parse(vals[1]);
        this.name = vals[3];
        this.width = (int)float.Parse(vals[4]);
        this.height = (int)float.Parse(vals[5]);
    }

    public override string ToString()
    {
        return "Animation Fact: "+this.name+": "+this.width+", "+this.height;
    }

    //Check to see if this GridObject matches this fact
    public override bool Matches(GridObject g)
    {
        return this.name == g.Name && this.width == g.W && this.height == g.H;
    }
}