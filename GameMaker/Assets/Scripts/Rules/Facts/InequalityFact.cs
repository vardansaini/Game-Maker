using Assets.Scripts.Core;
using UnityEngine;

public class InequalityFact : Fact
{
    public Fact referenceFact;
    public float value;
    public string relationship; 

    public InequalityFact(string fact)
    {
        string[] separatingStrings = { ":" };
        string[] vals = fact.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

        string[] separatingStrings2 = { "]" };
        string[] vals2 = vals[2].Split(separatingStrings2, System.StringSplitOptions.RemoveEmptyEntries);

        if (vals[1].Contains("Relationship"))
        {
            if (vals[1].Contains("X"))
            {
                referenceFact = new RelationshipXFact(vals[1] + vals2[0] + "]");
            }
            else {
                referenceFact = new RelationshipYFact(vals[1] + vals2[0] + "]");
            }

        }
        else if (vals[1].Contains("Velocity"))
        {
            if (vals[1].Contains("X"))
            {
                referenceFact = new VelocityXFact(vals[1] + vals2[0] + "]");
            }
            else
            {
                referenceFact = new VelocityYFact(vals[1] + vals2[0] + "]");
            }

        }
        else if (vals[1].Contains("Position"))
        {

            if (vals[1].Contains("X"))
            {
                referenceFact = new PositionXFact(vals[1] + vals2[0]+"]");
            }
            else
            {
                referenceFact = new PositionYFact(vals[1] + vals2[0] + "]");
            }
        }

        string[] separatingStrings3 = { ",", "'"};
        string[] vals3 = vals2[1].Split(separatingStrings3, System.StringSplitOptions.RemoveEmptyEntries);

        value = float.Parse(vals3[0]);
        relationship = vals3[2];
        this.componentID = (referenceFact.componentID);
    }

    public override bool Matches(GridObject g){
        //todo; finish this when you've figured out how to do types
        if (typeof(PositionXFact).IsInstanceOfType(referenceFact))
        {
            if (relationship.Equals(">=") )
            {
                return g.X >= value;
            }
            else
            {
                return g.X <= value;
            }
        }
        else if (typeof(PositionYFact).IsInstanceOfType(referenceFact))
        {
            if (relationship.Equals(">="))
            {
                return g.Y >= value;
            }
            else
            {
                return g.Y <= value;
            }
        }

        return false;
    }

    public override string ToString()
    {
        return "Inequality Fact" + referenceFact.ToString() + " " + relationship + " " + value;
    }
}
