using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A base class for implementing a GOAP agent from.
/// </summary>
public abstract class BaseAgent : MonoBehaviour, IGoap
{
    /// <summary>
    /// The prioirty of the current goal to pursue.
    /// </summary>
    protected int goalPriorityChoice;

    public virtual void ActionsFinished()
    {
        Debug.Log("<color=cyan>Actions completed!</color>");
    }

    /// <summary>
    /// Handles the decision making and assignment of the goal here.
    /// </summary>
    /// <returns></returns>
    public virtual HashSet<KeyValuePair<string, object>> CreateGoalState()
    {
        //This should be overwritten in a child class.
        throw new System.NotImplementedException();
    }

    public virtual HashSet<KeyValuePair<string, object>> GetWorldState()
    {
        //Child class should implement state of agent in world.
        throw new System.NotImplementedException();
    }

    public virtual bool MoveAgent(GoapAction _nextAction)
    {
        //Implement the agents movement in a child class.
        throw new System.NotImplementedException();
    }

    public virtual void PlanAborted(GoapAction _aborter)
    {
        Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.ActionName(_aborter));
    }

    public virtual void PlanFailed(HashSet<KeyValuePair<string, object>> _failedGoal)
    {
        //Child class should implement manage switching of goal or waiting for world state to change.
        goalPriorityChoice++;
        //By Default it should idle.
    }

    public virtual void PlanFound(HashSet<KeyValuePair<string, object>> _goalState, Queue<GoapAction> _actions)
    {
        //Reset position working through list of goals.
        goalPriorityChoice = 0;

        Debug.Log("<color=green>Plan found</color> - " + GoapAgent.PrintActionPlan(_actions));
    }
}
