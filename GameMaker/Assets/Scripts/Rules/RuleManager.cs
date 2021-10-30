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
        //Debug.Log(Constants.directory + "data.json");
        rules = LoadJSON.LoadJson(Constants.directory + "data.json");
    }

    public List<GridObject> RunRules(List<GridObject> gridObjects)
    {

        //Before running, recheck that we have up to date rules:
        //Debug.Log(Constants.directory + "data.json");
        rules = LoadJSON.LoadJson(Constants.directory+"data.json");

        for (int i = 0; i<rules.Count; i++)
        {
            
            Rule rule = rules[i];
            //Debug.Log("I am inside Rule Manager = " + rules[i].ToString());
            if (typeof(EmptyFact).IsInstanceOfType(rule))
            {
                //Debug.Log("Trying rule: " + rule);
            }

            Debug.Log("I am here to run rules on objects");
            gridObjects=rule.RunRuleOnObjects(gridObjects);
        }

        return gridObjects;
    }
}
