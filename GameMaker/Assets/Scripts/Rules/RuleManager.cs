using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Core;

public class RuleManager : MonoBehaviour
{
    private List<Rule> rules;
    

    public void Start()
    {
        rules = LoadJSON.LoadJson(Constants.directory + "data.json");
    }

    public List<GridObject> RunRules(List<GridObject> gridObjects)
    {
        rules = LoadJSON.LoadJson(Constants.directory+"data.json");

        for (int i = 0; i<rules.Count; i++)
        {
            
            Rule rule = rules[i];
            //Debug.Log("I am inside Rule Manager = " + rules[i].ToString());
            if (typeof(EmptyFact).IsInstanceOfType(rule))
            {
                //Debug.Log("Trying rule: " + rule);
            }
            gridObjects=rule.RunRuleOnObjects(gridObjects);
        }

        return gridObjects;
    }
}
