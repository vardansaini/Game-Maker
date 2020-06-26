using Assets.Scripts.Core;
using UnityEngine;

[System.Serializable]
public class Fact
{
    public int type;
    public string fact;
    public int componentID;

    public virtual bool Matches(GridObject g)
    {
        return false;
    }
}
