using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Core;

public class RuleManager : MonoBehaviour
{
    private List<Rule> rules;
    

    public void Start()
    {
        //If we have rules
        rules = LoadJSON.LoadJson(Constants.directory + "data.json");
    }

    public List<GridObject> RunRules(List<GridObject> gridObjects)
    {

        //Before running, recheck that we have up to date rules:
        rules = LoadJSON.LoadJson(Constants.directory+"data.json");

        for (int i = 0; i<rules.Count; i++)
        {
            Rule rule = rules[i];
            rule.RunRuleOnObjects(gridObjects);
        }

        return gridObjects;
    }
}
