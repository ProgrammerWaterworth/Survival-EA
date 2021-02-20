using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Component of a GOAP Plan executed by a GOAP agent to achieve their goal.
/// </summary>
public abstract class GoapAction : MonoBehaviour
{
    /// <summary>
    /// The preconiditions required to use an action in a plan.
    /// </summary>
    private HashSet<KeyValuePair<string, object>> preconditions;

    /// <summary>
    /// Effects of taking an action, possibly altering if other actions preconditions are met.
    /// </summary>
    private HashSet<KeyValuePair<string, object>> effects;

    /// <summary>
    /// Indicator of whether GOAP agent is within range to perform this action.
    /// </summary>
    private bool inRange = false;

    /// <summary>
    /// Cost of an action. Affects the actions chosen when planning to reach goal.
    /// </summary>
    public float cost = 1f;

    /// <summary>
    /// Target that the action must be performed on, if not null.
    /// </summary>
    public GameObject target;

    /// <summary>
    /// Known location of target - could be different to true target position
    /// </summary>
    public Vector3 targetPosition;

    public GoapAction()
    {
        preconditions = new HashSet<KeyValuePair<string, object>>();
        effects = new HashSet<KeyValuePair<string, object>>();
    }

    /// <summary>
    /// Reset action for reuse.
    /// </summary>
    public void ResetAction()
    {
        inRange = false;
        target = null;
        ResetVariables();
    }

    /// <summary>
    /// Reset action variables.
    /// </summary>
    public abstract void ResetVariables();


    /// <summary>
    /// Check if the action has been completed.
    /// </summary>
    /// <returns>True if complete.</returns>
    public abstract bool IsDone();

    /// <summary>
    /// Procedurally check if actions precondition is met.
    /// </summary>
    /// <param name="_agent">The GOAP agent performing the action.</param>
    /// <returns>True if preconditions are met.</returns>
    public virtual bool CheckProceduralPrecondition(GameObject _agent)
    {
        return FindTargetObject(_agent);
    }

    /// <summary>
    /// Execute an action.
    /// </summary>
    /// <param name="_agent">The GOAP agent performing the action.</param>
    /// <returns>True: action performed successfully. False: Can't perform action, clears action queue.</returns>
    public abstract bool ExecuteAction(GameObject _agent);

    /// <summary>
    /// Check if GOAP agent must be in range of target to perform action.
    /// </summary>
    /// <returns>True: if required to be in range.</returns>
    public abstract bool RequiresInRange();

    /// <summary>
    /// Check GOAP agent is within target range to perform action.
    /// </summary>
    /// <returns></returns>
    public bool IsInRange()
    {
        return inRange;
    }

    /// <summary>
    /// Check if Target object is present for executing action.
    /// </summary>
    /// <returns></returns>
    protected abstract bool FindTargetObject(GameObject _agent);
 

    /// <summary>
    /// Get the cost of executing this action.
    /// </summary>
    /// <returns>Cost of action.</returns>
    public virtual float GetCost()
    {
        return cost;
    }

    /// <summary>
    /// Set whether GOAP agent is in range of performing action or not.
    /// </summary>
    /// <param name="_inRange">Is within range of performing action.</param>
    public void SetInRange(bool _inRange)
    {
        this.inRange = _inRange;
    }

    /// <summary>
    /// Add a precondition for the action to meet in order to be performed.
    /// </summary>
    /// <param name="_key">Name of precondition.</param>
    /// <param name="_value">Value of the precondition.</param>
    public void AddPrecondition(string _key, object _value)
    {
        preconditions.Add(new KeyValuePair<string, object>(_key, _value));
    }

    /// <summary>
    /// Removes a precondition from an action.
    /// </summary>
    /// <param name="_key">Name of the precondition to be removed.</param>
    public void RemovePrecondition(string _key)
    {
        KeyValuePair<string, object> _toRemove = default(KeyValuePair<string, object>);
        foreach (KeyValuePair<string, object> _precondition in preconditions)
        {
            if (_precondition.Key.Equals(_key))
                _toRemove = _precondition;
        }
        if (!default(KeyValuePair<string, object>).Equals(_toRemove))
            preconditions.Remove(_toRemove);
    }

    /// <summary>
    /// Add effect for when an action is performed.
    /// </summary>
    /// <param name="_key">Name of the effect to be added.</param>
    /// <param name="_value">Value of the effect.</param>
    public void AddEffect(string _key, object _value)
    {
        effects.Add(new KeyValuePair<string, object>(_key, _value));
    }

    /// <summary>
    /// Remove an effect of when an action is performed.
    /// </summary>
    /// <param name="key">Name of effect to remove.</param>
    public void RemoveEffect(string key)
    {
        KeyValuePair<string, object> _toRemove = default(KeyValuePair<string, object>);
        foreach (KeyValuePair<string, object> _effect in effects)
        {
            if (_effect.Key.Equals(key))
                _toRemove = _effect;
        }
        if (!default(KeyValuePair<string, object>).Equals(_toRemove))
            effects.Remove(_toRemove);
    }

    /// <summary>
    /// Return the preconditions required to use an action in a plan.
    /// </summary>
    public HashSet<KeyValuePair<string, object>> Preconditions
    {
        get
        {
            return preconditions;
        }
    }

    /// <summary>
    /// Return the effects of taking an action, these can possibly alter if other actions preconditions are met.
    /// </summary>
    public HashSet<KeyValuePair<string, object>> Effects
    {
        get
        {
            return effects;
        }
    }
}
