﻿using Assets.Scripts.Core;
using UnityEngine;

//RelationshipFactX
public class RelationshipYFact : Fact
{
    public int componentID2;
    public string connectPoint1, connectPoint2;
    public float distance;

    public RelationshipYFact(string fact)
    {
        string[] separatingStrings = { "[", ",", "]", "'" };
        string[] vals = fact.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
        this.componentID = (int.Parse(vals[1]));
        this.componentID2 = int.Parse(vals[2]);
        this.connectPoint1 = vals[4];
        this.connectPoint2 = vals[6];
        this.distance = float.Parse(vals[7]);
    }

    public override bool Matches(GridObject g)
    {
        if (VariableFact.testing)
        {
            foreach (GridObject go in PlaytestManager.Instance.GridObjects)
            {
                if (Matches(g, go))
                {
                    return true;
                }
            }
        }
        else
        {
            foreach (GridObject go in GridManager.Instance.PreviewObjects)
            {
                if (Matches(g, go))
                {
                    return true;
                }
            }
        }
        return false;

    }


    //Check to see if this GridObject matches this fact (just in one direction)
    public bool Matches(GridObject g, GridObject g2)
    {
        Vector2 pt1 = g.GetNamedPoint(connectPoint1);
        Vector2 pt2 = g.GetNamedPoint(connectPoint2);

        float dist = Mathf.Abs(pt1[1] - pt2[1]);
        return System.Math.Abs(distance - dist) < 0.001f;

    }

    public override string ToString()
    {
        return "RelationshipYFact " + componentID + " to " + componentID2 + " at points " + connectPoint1 + " and " + connectPoint2 + " dist " + distance;
    }
}