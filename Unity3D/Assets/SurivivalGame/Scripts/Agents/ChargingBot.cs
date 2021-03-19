using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingBot : Robot, IFitnessFunction
{
    /**
	 * Our only goal will ever be to get batteries
	 * The PickUpBatteryAction will be able to fulfill this goal.
	 */
    [SerializeField] bool isComplete;
    private float lifetime = 0;

    

    private void Update()
    {
        lifetime += Time.deltaTime;
    }

    public override HashSet<KeyValuePair<string, object>> CreateGoalState()
    {
        int _numberOfGoals = 2;
        goalPriorityChoice %= _numberOfGoals;
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        //Prioritise Goals.
        switch (goalPriorityChoice)
        {
            case 0:
                goal = ChargeBaseGoal();
                break;
            case 1:
                goal = FindInteractables();
                break;
        }

        return goal;
    }

    public float GetFitness()
    {        
        return lifetime;
    }

    public bool IsEvalutionComplete()
    {
        if (inventory != null)
        {
            return !inventory.HasChargeLeft();
        }
        else
        {
            Debug.LogError(this + "has no inventory assigned.");
            return false;
        }
        
    }

    HashSet<KeyValuePair<string, object>> ChargeBaseGoal()
    {
        HashSet<KeyValuePair<string, object>> _goal = new HashSet<KeyValuePair<string, object>>();
        _goal.Add(new KeyValuePair<string, object>("hasBattery", true));
        return _goal;
    }

    HashSet<KeyValuePair<string, object>> FindInteractables()
    {
        HashSet<KeyValuePair<string, object>> _goal = new HashSet<KeyValuePair<string, object>>();
        _goal.Add(new KeyValuePair<string, object>("seek", true));
        return _goal;
    }

}
