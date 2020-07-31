using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

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



    public Vector2 GetTopLeftCorner(GridObject g, RelationshipXFact relationshipXFact, RelationshipYFact relationshipYFact, AnimationFact animationFact)
    {
        //X Position
        Vector2 connectPoint1X = g.GetNamedPoint(relationshipXFact.connectPoint1);
        float xPos = connectPoint1X.x - relationshipXFact.distance;

        if (relationshipXFact.connectPoint2 == "North")
        {
            xPos -= animationFact.width / 2f;
        }
        else if (relationshipXFact.connectPoint2 == "South")
        {
            xPos -= animationFact.width / 2f;
        }
        else if (relationshipXFact.connectPoint2 == "East")
        {
            xPos -= animationFact.width;
        }
        else if (relationshipXFact.connectPoint2 == "West")
        {
            xPos -= 0;
        }
        else
        {
            xPos -= animationFact.width / 2f;
        }

        //Y Position
        Vector2 connectPoint1Y = g.GetNamedPoint(relationshipYFact.connectPoint1);
        float yPos = connectPoint1Y.y - relationshipYFact.distance;

        if (relationshipYFact.connectPoint2 == "North")
        {
            yPos -= 0;
        }
        else if (relationshipYFact.connectPoint2 == "South")
        {
            yPos -= animationFact.height;
        }
        else if (relationshipYFact.connectPoint2 == "East")
        {
            yPos -= animationFact.height/2f;
        }
        else if (relationshipYFact.connectPoint2 == "West")
        {
            yPos -= animationFact.height / 2f;
        }
        else
        {
            yPos -= animationFact.height / 2f;
        }

        return new Vector2(xPos,yPos);
    }

    public List<GridObject> RunRuleOnObjects(List<GridObject> gridObjects)
    {
        if (typeof(EmptyFact).IsInstanceOfType(preEffect))
        {
        //    Debug.Log("Checking Rule: " + preEffect.ToString() + "->" + postEffect.ToString());
        }
        List<int> effectIDs = new List<int>();
        foreach (KeyValuePair < int, List < Fact >> kvp in conditionsByID)
        {
            List<int> componentIDs = new List<int>();//All the potential matches for this condition
            foreach (Fact ruleFact in kvp.Value)
            {
                if (typeof(EmptyFact).IsInstanceOfType(preEffect))
                {
                //    Debug.Log("     Checking Condition: " + ruleFact.ToString());
                }


                if (typeof(VariableFact).IsInstanceOfType(ruleFact))
                {
                    if (!ruleFact.Matches(null)){
                        //This rule can't fire, return
                        if (typeof(EmptyFact).IsInstanceOfType(preEffect))
                        {
                        //   Debug.Log("Rule can't fire due to " + ruleFact);
                        }
                        return gridObjects;
                    }
                }
                else
                {
                    if (componentIDs.Count == 0)
                    {
                        //Find all matching componentIDs in preview gridObjects
                        for (int g = 0; g < gridObjects.Count; g++)
                        {
                            if (ruleFact.Matches(gridObjects[g]))
                            {
                                componentIDs.Add(g);
                            }
                        }
                    }
                    else
                    {
                        List<int> newComponentIDs = new List<int>();
                        foreach (int id in componentIDs)
                        {
                            //Find remaining matches
                            if (ruleFact.Matches(gridObjects[id]))
                            {
                                newComponentIDs.Add(id);
                            }


                        }

                        if (newComponentIDs.Count == 0)
                        {
                            //This rule can't fire, return
                            if (typeof(EmptyFact).IsInstanceOfType(preEffect))
                            {
                            //    Debug.Log("Rule can't fire due to " + ruleFact);
                            }
                            return gridObjects;
                        }
                        else
                        {
                            componentIDs = newComponentIDs;
                        }
                    }
                }

            }

            if (typeof(EmptyFact).IsInstanceOfType(preEffect))
            {
                //Disappearing side
                if (((EmptyFact)preEffect).facts.Length > 0)
                {
                    if (kvp.Key == preEffect.componentID)
                    {
                        foreach (int cid in componentIDs)
                        {
                            //Ensure this one totally matches
                            if (!effectIDs.Contains(cid))
                            {
                                bool totalMatch = true;

                                foreach (Fact vanishFact in ((EmptyFact)preEffect).facts) {
                                    if (!vanishFact.Matches(gridObjects[cid]))
                                    {
                                        totalMatch = false;
                                        break;
                                    }

                                }

                                if (totalMatch) {
                                    effectIDs.Add(cid);
                                }
                            }
                        }
                    }
                    
                }
                //Appearing Side
                if (((EmptyFact)preEffect).facts.Length == 0 && ((EmptyFact)postEffect).facts.Length > 0)
                {
                    if (kvp.Key == postEffect.componentID) {
                        foreach (int cid in componentIDs) {
                            if (!effectIDs.Contains(cid))
                            {
                                effectIDs.Add(cid);
                            }
                        }
                    }
                }
            }

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

        if (typeof(EmptyFact).IsInstanceOfType(preEffect))
        {

            if (effectIDs.Count > 0)
            {
            //    Debug.Log("     Running Rule: " + preEffect.ToString() + "->" + postEffect.ToString());
            }
        }

        //Run rule on all effectIDs
        Scene scene = SceneManager.GetActiveScene();
        foreach (int effectID in effectIDs)
        {
            if (typeof(EmptyFact).IsInstanceOfType(preEffect))
            {
                if (((EmptyFact)preEffect).facts.Length > 0 && ((EmptyFact)postEffect).facts.Length == 0)
                {
                    //Disappear
                    if (scene.name == "Main")
                    {
                        GridObject toRemove = gridObjects[effectID];
                        if (toRemove != null)
                        {
                            GridManager.Instance.RemoveGridObject(toRemove);
                        }
                    }
                    else
                    {
                        PlaytestManager.Instance.RemoveObject(effectID);
                    }
                }
                else if (((EmptyFact)preEffect).facts.Length == 0 && ((EmptyFact)postEffect).facts.Length > 0)
                {
                    //Grab the thing that needs to appear
                    AnimationFact thingToAppear = null;

                    foreach(Fact f in ((EmptyFact)postEffect).facts)
                    {
                        if (typeof(AnimationFact).IsInstanceOfType(f))
                        {
                            thingToAppear = (AnimationFact)f;
                        }
                    }

                    Vector2 positionToAppearTo = GetTopLeftCorner(gridObjects[effectID], (RelationshipXFact)((EmptyFact)postEffect).facts[0], (RelationshipYFact)((EmptyFact)postEffect).facts[1], thingToAppear);

                    if (scene.name == "Main")
                    {

                        int posX = (int)positionToAppearTo.x;
                        int posY = (int)positionToAppearTo.y;
                        SpriteData newobject = SpriteManager.Instance.GetSprite(thingToAppear.name);
                        GridManager.Instance.AddGridObject(newobject, posX, posY, false);
                    }
                    else{
                        PlaytestManager.Instance.AddObject(thingToAppear.name, positionToAppearTo);
                    }
                }
                else
                {
                    //Transform 
                    //Grab the thing that needs to appear
                    AnimationFact thingToAppear = null;

                    foreach (Fact f in ((EmptyFact)postEffect).facts)
                    {
                        if (typeof(AnimationFact).IsInstanceOfType(f))
                        {
                            thingToAppear = (AnimationFact)f;
                        }
                    }

                    Vector2 positionToAppearTo = GetTopLeftCorner(gridObjects[effectID], (RelationshipXFact)((EmptyFact)postEffect).facts[0], (RelationshipYFact)((EmptyFact)postEffect).facts[1], thingToAppear);

                    if (scene.name == "Main")
                    {
                        GridObject toRemove = gridObjects[effectID];
                        if (toRemove != null)
                        {
                            GridManager.Instance.RemoveGridObject(toRemove);
                        }
                    }
                    else
                    {
                        PlaytestManager.Instance.RemoveObject(effectID);
                    }
                    if (scene.name == "Main")
                    {

                        int posX = (int)positionToAppearTo.x;
                        int posY = (int)positionToAppearTo.y;
                        SpriteData newobject = SpriteManager.Instance.GetSprite(thingToAppear.name);
                        GridManager.Instance.AddGridObject(newobject, posX, posY, false);
                    }
                    else
                    {
                        PlaytestManager.Instance.AddObject(thingToAppear.name, positionToAppearTo);
                    }

                }
            }

            else if (typeof(VelocityXFact).IsInstanceOfType(postEffect))
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
 