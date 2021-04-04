using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plans how an agent can reach the goal state with the available actions from its current state.
/// </summary>
public class GoapPlanner
{
    /// <summary>
    /// Try create plan: sequence of actions that can the reach goal.
    /// </summary>
    /// <param name="_agent">The GOAP agent executing the plan.</param>
    /// <param name="_availableActions">The current available actions.</param>
    /// <param name="_worldState">The current state the GOAP agent is in.</param>
    /// <param name="_goal">The desired state of the GOAP agent</param>
    /// <returns>Successful: List of actions in order. Failure: null. </returns>
    public Queue<GoapAction> Plan(GameObject _agent,
                                  HashSet<GoapAction> _availableActions,
                                  HashSet<KeyValuePair<string, object>> _worldState,
                                  HashSet<KeyValuePair<string, object>> _goal,float _goalCostMultiplier, out float _planCost)
    {
        _planCost = Mathf.Infinity;

        //Ensure there are available actions.
        if (_availableActions == null)
        {
            Debug.Log("<color=red>" + this +" has no available actions to form a plan with. </color>");
            return null;
        }
        
        // Reset all available actions.
        foreach (GoapAction _action in _availableActions)
        {
            _action.ResetAction();
        }

        Debug.Log("<color=yellow>All Actions: </color>" + GetActionsAsStringList(_availableActions));

        // check what actions can run based on preconditions.
        HashSet<GoapAction> _usableActions = new HashSet<GoapAction>();
        foreach (GoapAction _action in _availableActions)
        {
            if (_action.CheckProceduralPrecondition(_agent))
                _usableActions.Add(_action);
        }

        Debug.Log("<color=yellow>Available Actions: </color>" + GetActionsAsStringList(_usableActions));

        // Used to store the leaf nodes which reached the goal.
        List<ActionNode> _leafNodes = new List<ActionNode>();

        // Starting node for planning tree.
        ActionNode _start = new ActionNode(null, 0, _worldState, null);

        // Start building a tree of possible plan paths.
        bool _success = CreatePlanTree(_start, _leafNodes, _usableActions, _goal);

        if (!_success)
        {
            Debug.Log("<color=red>No Plan Found!</color>");
            return null;
        }

        // Get the lowest cost plan out the solutions found.
        ActionNode _cheapestGoalNode = null;

        foreach (ActionNode _leaf in _leafNodes)
        {
            if (_cheapestGoalNode == null)
                _cheapestGoalNode = _leaf;
            else
            {
                if (_leaf.cumulitiveCost < _cheapestGoalNode.cumulitiveCost)
                    _cheapestGoalNode = _leaf;
            }
        }

        // Backtrack through parent nodes required to get to _cheapestGoalNode.
        List<GoapAction> _goalActionSequence = new List<GoapAction>();
        ActionNode _currentNode = _cheapestGoalNode;
        //Set plan cost for evaluation
        _planCost = _cheapestGoalNode.cumulitiveCost * _goalCostMultiplier;

        while (_currentNode != null)
        {
            if (_currentNode.action != null)
            {
                // insert the action at the front
                _goalActionSequence.Insert(0, _currentNode.action); 
            }
            _currentNode = _currentNode.parent;
        }

        // Create action queue from sequence of actions.
        Queue<GoapAction> _actionQueue = new Queue<GoapAction>();
        foreach (GoapAction _action in _goalActionSequence)
        {
            _actionQueue.Enqueue(_action);
        }

        // Return action plan.
        return _actionQueue;
    }

    /// <summary>
    /// Recursive Function: Creates part of a tree plan with usable actions.
    /// Terminates when all possible action paths have been created.
    /// </summary>
    /// <param name="_parentNode">Root node of tree.</param>
    /// <param name="_leafNodes"></param>
    /// <param name="_usableActions"></param>
    /// <param name="_goal"></param>
    /// <returns>True if at least one solution was found.</returns>
    private bool CreatePlanTree(ActionNode _parentNode, List<ActionNode> _leafNodes, HashSet<GoapAction> _usableActions, HashSet<KeyValuePair<string, object>> _goal)
    {
        bool _pathFound = false;

        //See if any of the available actions can be used here.
        foreach (GoapAction _action in _usableActions)
        {
            // Check if parent states conditions meet required preconditions to execute action.
            if (CheckConditions(_action.Preconditions, _parentNode.state))
            {
                // Apply the action's effects to the parent state
                HashSet<KeyValuePair<string, object>> currentState = SetNewState(_parentNode.state, _action.Effects);

                //calculate movement cost too.                

                ActionNode _currentNode = new ActionNode(_parentNode, _parentNode.cumulitiveCost + _action.GetCost(), currentState, _action);

                //Check if current state meets the conditions of the goal state.
                if (CheckConditions(_goal, currentState))
                {
                    _leafNodes.Add(_currentNode);
                    _pathFound = true;
                }
                else // not at a solution yet, so test all the remaining actions and branch out the tree
                {                    
                    HashSet<GoapAction> _usableActionSubset = GetUsableActionSubset(_usableActions, _action);
                    if (CreatePlanTree(_currentNode, _leafNodes, _usableActionSubset, _goal))
                        _pathFound = true;
                }
            }
        }

        return _pathFound;
    }

    /// <summary>
    /// Create a subset of the actions which excludes the action used in current state.
    /// </summary>
    /// <param name="_usableActions"></param>
    /// <param name="_usedAction"></param>
    /// <returns>Updated set of usable actions.</returns>
    private HashSet<GoapAction> GetUsableActionSubset(HashSet<GoapAction> _usableActions, GoapAction _usedAction)
    {
        HashSet<GoapAction> _actionSubset = new HashSet<GoapAction>();
        foreach (GoapAction _action in _usableActions)
        {
            if (!_action.Equals(_usedAction))
                _actionSubset.Add(_action);
        }
        return _actionSubset;
    }

  


    /// <summary>
    ///  Check that a states _currentConditions satisfy actions _preconditions
    /// </summary>
    /// <param name="_preconditions">conditions required to execute action.</param>
    /// <param name="_currentConditions">conditions of state.</param>
    /// <returns>True: If all preconditions are met. False: If a precondition isn't met.</returns>
    private bool CheckConditions(HashSet<KeyValuePair<string, object>> _preconditions, HashSet<KeyValuePair<string, object>> _currentConditions)
    {
        bool allMatch = true;
        foreach (KeyValuePair<string, object> _precondition in _preconditions)
        {
            bool match = false;
            foreach (KeyValuePair<string, object> _condition in _currentConditions)
            {
                if (_condition.Equals(_precondition))
                {
                    match = true;
                    break;
                }
            }
            if (!match)
                allMatch = false;
        }
        return allMatch;
    }

    /// <summary>
    /// Apply changes to state.
    /// </summary>
    /// <param name="_currentState">The state being changed.</param>
    /// <param name="_stateChanges">The changes being applied to _currentState</param>
    /// <returns>New state with the _stateChanges applied to _currentState.</returns>
    private HashSet<KeyValuePair<string, object>> SetNewState(HashSet<KeyValuePair<string, object>> _currentState, HashSet<KeyValuePair<string, object>> _stateChanges)
    {
        HashSet<KeyValuePair<string, object>> _newState = new HashSet<KeyValuePair<string, object>>();

        // copy the KVPs over as new objects
        foreach (KeyValuePair<string, object> s in _currentState)
        {
            _newState.Add(new KeyValuePair<string, object>(s.Key, s.Value));
        }

        foreach (KeyValuePair<string, object> change in _stateChanges)
        {
            // if the key exists in the current state, update the Value
            bool exists = false;

            foreach (KeyValuePair<string, object> s in _newState)
            {
                if (s.Equals(change))
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                _newState.RemoveWhere((KeyValuePair<string, object> kvp) => { return kvp.Key.Equals(change.Key); });
                KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key, change.Value);
                _newState.Add(updated);
            }
            // if it does not exist in the current state, add it
            else
            {
                _newState.Add(new KeyValuePair<string, object>(change.Key, change.Value));
            }
        }
        return _newState;
    }

    /// <summary>
    /// A component for constructing a tree of paths to the goal.
    /// Represents actions used in a plan and keeps runnning cost of actions to that point.
    /// </summary>
    private class ActionNode
    {
        /// <summary>
        /// The parent node to this node in a planning tree.
        /// </summary>
        public ActionNode parent;
        /// <summary>
        /// The cost at the current action node based on the sum of costs to get to this point.
        /// </summary>
        public float cumulitiveCost;
        /// <summary>
        /// The state at this point in the planning tree.
        /// </summary>
        public HashSet<KeyValuePair<string, object>> state;
        /// <summary>
        /// The action to be executed at this point in the plan
        /// </summary>
        public GoapAction action;

        public ActionNode(ActionNode parent, float runningCost, HashSet<KeyValuePair<string, object>> state, GoapAction action)
        {
            this.parent = parent;
            this.cumulitiveCost = runningCost;
            this.state = state;
            this.action = action;
        }
    }

    /// <summary>
    /// Prints the actions available for planning.
    /// </summary>
    string GetActionsAsStringList(HashSet<GoapAction> _usableActions)
    {
        string _message = "";

        foreach(GoapAction _action in _usableActions)
        {
            _message += _action.GetType().ToString() + ", ";
        }
        if (_message.Length > 2)
            _message = _message.Substring(0, _message.Length - 2); //remove final ", "
        else
            _message = "None.";

        return _message;
   
    }

}


