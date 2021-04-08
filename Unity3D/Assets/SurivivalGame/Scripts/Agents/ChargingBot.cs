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

    float averageHunger = 1, hungerAccumulation = 1, statUpdates = 1;
    float averageHealth = 1, healthAccumulation = 1;
    float averageCharge = 1, chargeAccumulation = 1;
    protected override void Update()
    {
        base.Update();
        UpdateAverageStats();
        if(!dead)
            lifetime += Time.deltaTime;
    }

    protected override void Start()
    {
        base.Start();

        totalNumGoals = 5;
    }

    public override HashSet<KeyValuePair<string, object>> CreateGoalState()
    {

        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        if (!dead)
        {
            switch (goalIndex)
            {
                case 0:
                    goal = GetCharge();
                    break;
                case 1:
                    goal = MaintainHunger();
                    break;
                case 2:
                    goal = Explore();
                    break;
                case 3:
                    goal = FindFood();
                    break;
                case 4:
                    goal = FindCharge();
                    break;

            }

        }
        return goal;
        
    }

    void UpdateAverageStats()
    {
        statUpdates++;

        if (inventory != null)
        {
            chargeAccumulation += inventory.GetCharge();
            averageCharge = chargeAccumulation / statUpdates;
        }
        hungerAccumulation += hunger;
        averageHunger = hungerAccumulation / statUpdates;
        healthAccumulation += health;
        averageHealth = healthAccumulation / statUpdates;
    }

    public override float GetGoalMultiplier()
    {
        float _multiplier = 1;
        //Return a value between 0 and 1 to determine how it modifies cost of a plan. Lowest is chosen as plan.
        switch (goalIndex)
        {
            case 0:
                if(inventory!=null)
                    _multiplier = (inventory.GetCharge() / inventory.GetMaxCharge());
                break;
            case 1:
                _multiplier = (hunger / maxHunger);
                break;
            case 3:
                _multiplier = (hunger / maxHunger); //find food is dependant on hunger.
                break;
            case 4:
                if (inventory != null)
                    _multiplier = (inventory.GetCharge() / inventory.GetMaxCharge());
                break;
        }
        return _multiplier;
    }

    public float GetFitness()
    {
        //fitness must be a combination of lifetime average hunger and average charge. lifetime needs to have greatest contribution.
        float _fitness = averageHealth+ averageCharge + averageHunger + (lifetime*10);
        return _fitness;
    }

    public bool IsEvalutionComplete()
    {
        if (inventory != null)
        {
            if (lifetime >= 80)
                return true;
            else if (dead)
                return true;
        }
        else
        {
            Debug.LogError(this + "has no inventory assigned.");
            return false;
        }
        return false;
    }

    HashSet<KeyValuePair<string, object>> GetCharge()
    {
        HashSet<KeyValuePair<string, object>> _goal = new HashSet<KeyValuePair<string, object>>();
        _goal.Add(new KeyValuePair<string, object>("hasBattery", true));
        return _goal;
    }

    HashSet<KeyValuePair<string, object>> Explore()
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

    HashSet<KeyValuePair<string, object>> FindFood()
    {
        HashSet<KeyValuePair<string, object>> _goal = new HashSet<KeyValuePair<string, object>>();
        _goal.Add(new KeyValuePair<string, object>("searchFood", true));
        return _goal;
    }

    HashSet<KeyValuePair<string, object>> FindCharge()
    {
        HashSet<KeyValuePair<string, object>> _goal = new HashSet<KeyValuePair<string, object>>();
        _goal.Add(new KeyValuePair<string, object>("searchBattery", true));
        return _goal;
    }

    HashSet<KeyValuePair<string, object>> OvercomeThreat()
    {
        HashSet<KeyValuePair<string, object>> _goal = new HashSet<KeyValuePair<string, object>>();
        _goal.Add(new KeyValuePair<string, object>("threatened", false));
        return _goal;
    }

}
