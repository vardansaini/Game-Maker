using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public struct SimpleJsonFact
{
    public int type;
    public string fact;
    public int id;
}

[System.Serializable]
public class RootObject
{
    public SimpleJsonFact[] facts;
}

public class LoadJSON
{
    public static List<Rule> LoadJson(string filename)
    {
        if (!File.Exists(filename))
        {
            return new List<Rule>();
        }

        List<Fact> facts = new List<Fact>();
        Dictionary<int, List<Fact>> factsById = new Dictionary<int, List<Fact>>();
        RootObject myObject = null;
        using (StreamReader r = new StreamReader(filename))
        {
            string json = r.ReadToEnd();
            myObject = JsonUtility.FromJson<RootObject>("{\"facts\":" + json + "}");
        }

        int largestRuleID = 0;
        bool anyFactFound = false;
        foreach (SimpleJsonFact fact in myObject.facts)
        {
            anyFactFound = true;
            Fact f = null;
            if (fact.fact.Contains("Inequality"))
            {
                f = new InequalityFact(fact.fact);
            }
            else if (fact.fact.Contains("Empty"))
            {
                f = new EmptyFact(fact.fact);
            }
            else if (fact.fact.Contains("VelocityXFact"))
            {
                f = new VelocityXFact(fact.fact);
            }
            else if (fact.fact.Contains("VelocityYFact"))
            {
                f = new VelocityYFact(fact.fact);
            }
            else if (fact.fact.Contains("AnimationFact"))
            {
                f = new AnimationFact(fact.fact);
            }
            else if (fact.fact.Contains("PositionXFact"))
            {
                f = new PositionXFact(fact.fact);
            }
            else if (fact.fact.Contains("PositionYFact"))
            {
                f = new PositionYFact(fact.fact);
            }
            else if (fact.fact.Contains("PositionYFact"))
            {
                f = new PositionYFact(fact.fact);
            }
            else if (fact.fact.Contains("VariableFact"))
            {
                f = new VariableFact(fact.fact);
            }
            else if (fact.fact.Contains("RelationshipFactX"))
            {
                f = new RelationshipXFact(fact.fact);
            }
            else if (fact.fact.Contains("RelationshipFactY"))
            {
                f = new RelationshipYFact(fact.fact);
            }

            if (f != null)
            {
                f.type = fact.type;
                if (!factsById.ContainsKey(fact.id))
                {
                    factsById[fact.id] = new List<Fact>();
                }
                factsById[fact.id].Add(f);

                if (fact.id > largestRuleID)
                {
                    largestRuleID = fact.id;
                }
            }
        }
       
        //Convert into rules (has to be done in order)
        List<Rule> rules = new List<Rule>();
        if (anyFactFound)
        {
            //largestRuleID
            for (int i = 0; i < largestRuleID + 1; i++)
            {
                List<Fact> conditions = new List<Fact>();
                Fact preEffect = null;
                Fact postEffect = null;

                foreach (Fact f in factsById[i])
                {
                    if (f.type == 1)
                    {
                        preEffect = f;
                    }
                    else if (f.type == 2)
                    {
                        postEffect = f;
                    }
                    else
                    {
                        conditions.Add(f);
                    }
                }
                if (!conditions.Contains(preEffect))
                {
                    conditions.Add(preEffect);
                }

                rules.Add(new Rule(conditions, preEffect, postEffect));
            }
        }


        return rules;
        
    }

}
