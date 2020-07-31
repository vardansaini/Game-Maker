using System.Collections.Generic;
using Assets.Scripts.Core;
using UnityEngine;

//Empty facts for additions and removal
//{"type": "2", "fact": "EmptyFact: ", "id": 16}
//{"type": "1", "fact": "EmptyFact: VelocityXFact: [51, 0] VelocityYFact: [51, 0] AnimationFact: [51, 'pool 4', 1.0, 1.0] PositionXFact: [51, 25.0] PositionYFact: [51, 1.0] ", "id": 16}
public class EmptyFact : Fact
{
    public Fact[] facts;

    public EmptyFact(string fact)
    {
        if (!string.IsNullOrEmpty(fact))
        {
            List<Fact> _facts = new List<Fact>();
            string[] separatingStrings = { ":", "|"};
            string[] vals = fact.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            
            for (int i = 0; i < vals.Length; i++)
            {
                if (vals[i].Contains("VelocityX"))
                {
                    _facts.Add(new VelocityXFact(vals[i]+":"+vals[i + 1]));
                }
                else if (vals[i].Contains("VelocityY"))
                {
                    _facts.Add(new VelocityYFact(vals[i] + ":" + vals[i + 1]));
                }
                else if (vals[i].Contains("Animation"))
                {
                    _facts.Add(new AnimationFact(vals[i] + ":" + vals[i + 1]));
                }
                else if (vals[i].Contains("PositionX"))
                {
                    _facts.Add(new PositionXFact(vals[i] + ":" + vals[i + 1]));
                }
                else if (vals[i].Contains("PositionY"))
                {
                    _facts.Add(new PositionYFact(vals[i] + ":" + vals[i + 1]));
                }
                else if (vals[i].Contains("RelationshipFactX"))
                {
                    _facts.Add(new RelationshipXFact(vals[i] + ":" + vals[i + 1]));
                }
                else if (vals[i].Contains("RelationshipFactY"))
                {
                    _facts.Add(new RelationshipYFact(vals[i] + ":" + vals[i + 1]));
                }
                else if (vals[i].Contains("Variable"))
                {
                    _facts.Add(new VariableFact(vals[i] + ":" + vals[i + 1]));
                }
            }

            if (_facts.Count > 0)
            {
                facts = _facts.ToArray();
                this.componentID = (facts[0].componentID);
            }
            else
            {
                facts = new Fact[] { };
            }
        }
        else
        {
            facts = new Fact[] { };
        }
    }

    public override string ToString()
    {
        return "EmptyFact "+facts.Length;
    }

}

