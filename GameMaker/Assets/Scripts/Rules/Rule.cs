using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Core;
using UnityEngine;

public class Rule
{
    public List<Fact> conditions;
    public Fact preEffect, postEffect;
    public Dictionary<int, List<Fact>> conditionsByID;

    public Rule(List<Fact> _conditions, Fact _preEffect, Fact _postEffect)
    {
        conditions = _conditions;
        preEffect = _preEffect;
        postEffect = _postEffect;
        conditionsByID = new Dictionary<int, List<Fact>>();

        foreach(Fact f in conditions)
        {
            if(!conditionsByID.ContainsKey(f.componentID)){
                conditionsByID[f.componentID] = new List<Fact>();
            }
            conditionsByID[f.componentID].Add(f);
        }


    }

    public List<GridObject> RunRuleOnObjects(List<GridObject> gridObjects)
    {
        Debug.Log("Hit here for rule "+preEffect+"->"+postEffect);
        List<int> effectIDs = new List<int>();
        foreach (KeyValuePair < int, List < Fact >> kvp in conditionsByID)
        {
            List<int> componentIDs = new List<int>();//All the potential matches for this condition
            foreach (Fact ruleFact in kvp.Value)
            {
                if (componentIDs.Count == 0)
                {
                    //Find all matching componentIDs in preview gridObjects
                    for(int g = 0; g<gridObjects.Count; g++)
                    {
                        if (ruleFact.Matches(gridObjects[g]))
                        {
                            componentIDs.Add(g);
                        }
                    }

                    Debug.Log("ComponentIDS: " + componentIDs.Count);
                }
                else
                {
                    Debug.Log("Rule Fact: " + ruleFact);
                    List<int> newComponentIDs = new List<int>();
                    foreach(int id in componentIDs)
                    {
                        //Find remaining matches
                        if (ruleFact.Matches(gridObjects[id]))
                        {
                            newComponentIDs.Add(id);
                        }
                        else
                        {
                            Debug.Log("Broke at: " + ruleFact);
                        }
                        
                    }

                    if (newComponentIDs.Count == 0)
                    {
                        //This rule can't fire, return
                        Debug.Log("Rule can't fire due to " + kvp.Key);
                        return gridObjects;
                    }
                    else
                    {
                        componentIDs = newComponentIDs;
                    }
                }

            }

            //TODO; fill out empty fact stuff

            if (kvp.Key == preEffect.componentID)
            {
                foreach(int cid in componentIDs)
                {
                    if (!effectIDs.Contains(cid))
                    {
                        effectIDs.Add(cid);
                    }
                }
            }
        }

        //Run rule on all effectIDs
        foreach(int effectID in effectIDs)
        {
            if (typeof(VelocityXFact).IsInstanceOfType(postEffect))
            {
                gridObjects[effectID].VX = ((VelocityXFact)postEffect).velocityVal;
            }
            else if (typeof(VelocityYFact).IsInstanceOfType(postEffect))
            {
                gridObjects[effectID].VY = ((VelocityYFact)postEffect).velocityVal;
            }
        }
        
        return gridObjects;
    }

}