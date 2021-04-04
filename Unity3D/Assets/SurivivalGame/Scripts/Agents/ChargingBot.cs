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

    private void Start()
    {
        totalNumGoals = 3;
    }


    protected override void Update()
    {
        base.Update();

        lifetime += Time.deltaTime;
    }
    public override HashSet<KeyValuePair<string, object>> CreateGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        //Prioritise Goals.
        switch (goalIndex)
        {
            case 0:
                goal = MaintainHunger();
                break;
            case 1:
                goal = GetCharge();
                break;
            case 2:
                goal = FindInteractables();
                break;

        }
        goalIndex++;
        return goal;
    }

    public float GetFitness()
    {
        float _fitness = (hunger / maxHunger) + (health / maxHealth);
        return _fitness;
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

    HashSet<KeyValuePair<string, object>> GetCharge()
    {
        HashSet<KeyValuePair<string, object>> _goal = new HashSet<KeyValuePair<string, object>>();
        _goal.Add(new KeyValuePair<string, object>("hasBattery", true));
        return _goal;
    }

    HashSet<KeyValuePair<string, object>> FindInteractables()
    {
        HashSet<KeyValuePair<string, object>> _goal = new HashSet<KeyValuePair<string, object>>();
        _goal.Add(new KeyValuePair<string, object>("explored", true));
        return _goal;
    }

    HashSet<KeyValuePair<string, object>> MaintainHunger()
    {
        HashSet<KeyValuePair<string, object>> _goal = new HashSet<KeyValuePair<string, object>>();
        _goal.Add(new KeyValuePair<string, object>("isHungry", false));
        return _goal;
    }

    HashSet<KeyValuePair<string, object>> OvercomeThreat()
    {
        HashSet<KeyValuePair<string, object>> _goal = new HashSet<KeyValuePair<string, object>>();
        _goal.Add(new KeyValuePair<string, object>("threatened", false));
        return _goal;
    }

}
