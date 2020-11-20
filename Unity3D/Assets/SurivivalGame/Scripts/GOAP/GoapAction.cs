using System.Collections.Generic;
using UnityEngine;

public abstract class GoapAction : MonoBehaviour
{

    //The preconiditions required to pursue the action
    private HashSet<KeyValuePair<string, object>> preconditions;
    //The effects of having taken that action
    private HashSet<KeyValuePair<string, object>> effects;

    private bool inRange = false;

    /* Cost of an action
	 * Changing it will affect what actions are chosen during planning.*/
    public float cost = 1f;

    /**
	 * Actions can be performed on targets */
    public GameObject target;

    public GoapAction()
    {
        preconditions = new HashSet<KeyValuePair<string, object>>();
        effects = new HashSet<KeyValuePair<string, object>>();
    }

    public void ResetAction()
    {
        inRange = false;
        target = null;
        ResetVariables();
    }

    /**
	 * Reset variables for planning to restart
	 */
    public abstract void ResetVariables();

    /**
	 * Check action completion
	 */
    public abstract bool IsDone();

    /**
	 * Procedurally check if this action can run.
	 */
    public abstract bool CheckProceduralPrecondition(GameObject agent);

    /**
	 * Run the action.
	 * Returns True if the action performed successfully or false
	 * if something happened and it can no longer perform. In this case
	 * the action queue should clear out and the goal cannot be reached.
	 */
    public abstract bool Perform(GameObject agent);

    /**
	 * Check if action needs to be in range of target.
	 */
    public abstract bool RequiresInRange();


    /**
	 * Check if action is in target range
	 * Set by MoveTo state.
	 */
    public bool IsInRange()
    {
        return inRange;
    }


    public void SetInRange(bool inRange)
    {
        this.inRange = inRange;
    }


    public void AddPrecondition(string key, object value)
    {
        preconditions.Add(new KeyValuePair<string, object>(key, value));
    }


    public void RemovePrecondition(string key)
    {
        KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
        foreach (KeyValuePair<string, object> kvp in preconditions)
        {
            if (kvp.Key.Equals(key))
                remove = kvp;
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
            preconditions.Remove(remove);
    }


    public void AddEffect(string key, object value)
    {
        effects.Add(new KeyValuePair<string, object>(key, value));
    }


    public void RemoveEffect(string key)
    {
        KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
        foreach (KeyValuePair<string, object> kvp in effects)
        {
            if (kvp.Key.Equals(key))
                remove = kvp;
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
            effects.Remove(remove);
    }


    public HashSet<KeyValuePair<string, object>> Preconditions
    {
        get
        {
            return preconditions;
        }
    }

    public HashSet<KeyValuePair<string, object>> Effects
    {
        get
        {
            return effects;
        }
    }
}
