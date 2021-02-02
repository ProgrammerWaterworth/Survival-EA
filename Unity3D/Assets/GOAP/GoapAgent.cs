using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// A GoapAgent implements a GOAP Plan through the use of three states: Idle, MoveTo and PerformAction.
/// </summary>
public sealed class GoapAgent : MonoBehaviour
{

    private FSM stateMachine;
    /// <summary>
    /// Finds something to do - Searching
    /// </summary>
    private FSM.FSMState idleState;
    /// <summary>
    /// Moves to a target - Moves Straight to target
    /// </summary>
    private FSM.FSMState moveToState;
    /// <summary>
    /// Performs an action
    /// </summary>
    private FSM.FSMState executeActionState;
    /// <summary>
    /// All actions available to the Goap Agent
    /// </summary>
    private HashSet<GoapAction> availableActions;
    /// <summary>
    /// All actions currently available to the Goap Agent
    /// </summary>
    private Queue<GoapAction> currentActions;

    private IGoap dataProvider; // this is the implementing class that provides our world data and listens to feedback on planning

    private GoapPlanner goapPlanner;


    void Start()
    {
        //Initialise GOAP agent
        stateMachine = new FSM();
        availableActions = new HashSet<GoapAction>();
        currentActions = new Queue<GoapAction>();
        goapPlanner = new GoapPlanner();
        FindAndSetDataProvider();
        CreateIdleState();
        CreateMoveToState();
        CreatePerformActionState();
        stateMachine.PushState(idleState);
        LoadActions();
    }


    void Update()
    {
        stateMachine.Update(gameObject);
    }


    /// <summary>
    /// Add action to the available actions.
    /// </summary>
    /// <param name="_action">Action to add.</param>
    public void AddAction(GoapAction _action)
    {
        availableActions.Add(_action);
    }

    /// <summary>
    /// Get Action from available actions.
    /// </summary>
    /// <param name="_action"></param>
    /// <returns></returns>
    public GoapAction GetAction(Type _action)
    {
        foreach (GoapAction availableAction in availableActions)
        {
            if (availableAction.GetType().Equals(_action))
                return availableAction;
        }
        return null;
    }

    /// <summary>
    /// Remove action from list of available actions.
    /// </summary>
    /// <param name="_action">Action to remove.</param>
    public void RemoveAction(GoapAction _action)
    {
        availableActions.Remove(_action);
    }

    /// <summary>
    /// Checks if there are any current actions available.
    /// </summary>
    /// <returns>True if GOAP agent has an action plan.</returns>
    private bool HasPlan()
    {
        return currentActions.Count > 0;
    }

    private void CreateIdleState()
    {
        idleState = (_fsm, _agentObj) => {
            // GOAP planning

            // Get the world state and the goal we want to plan for
            HashSet<KeyValuePair<string, object>> _worldState = dataProvider.GetWorldState();
            HashSet<KeyValuePair<string, object>> _goal = dataProvider.CreateGoalState();

            // Plan
            Queue<GoapAction> _plan = goapPlanner.Plan(gameObject, availableActions, _worldState, _goal);
            if (_plan != null)
            {
                // Obtained plan successfully.
                currentActions = _plan;
                dataProvider.PlanFound(_goal, _plan);

                _fsm.PopState(); // move to executeAction state
                _fsm.PushState(executeActionState);

            }
            else
            {
                // Failed to make a plan.
                Debug.Log("<color=orange>Plan for goal failed:</color>" + PrintAllStates(_goal));
                dataProvider.PlanFailed(_goal);
                _fsm.PopState(); // move back to IdleAction state
                _fsm.PushState(idleState);
            }

        };
    }

    private void CreateMoveToState()
    {
        moveToState = (fsm, gameObj) => {
            // move the game object

            GoapAction action = currentActions.Peek();
            if (action.RequiresInRange() && action.target == null)
            {
                Debug.Log("<color=red>Fatal error:</color> Action requires a target but has none. Planning failed. You did not assign the target in your Action.checkProceduralPrecondition()");
                fsm.PopState(); // move
                fsm.PopState(); // execute action
                fsm.PushState(idleState);
                return;
            }

            // get the agent to move itself
            if (dataProvider.MoveAgent(action))
            {
                fsm.PopState();
            }
        };
    }

    private void CreatePerformActionState()
    {

        executeActionState = (fsm, gameObj) => {
            // execute the action

            if (!HasPlan())
            {
                // no actions to execute
                Debug.Log("<color=red>Done actions</color>");
                fsm.PopState();
                fsm.PushState(idleState);
                dataProvider.ActionsFinished();
                return;
            }

            GoapAction action = currentActions.Peek();
            if (action.IsDone())
            {
                // the action is done. Remove it so we can execute the next one
                currentActions.Dequeue();
            }

            if (HasPlan())
            {
                // execute the next action
                action = currentActions.Peek();
                bool inRange = action.RequiresInRange() ? action.IsInRange() : true;

                if (inRange)
                {
                    // we are in range, so perform the action
                    bool success = action.ExecuteAction(gameObj);

                    if (!success)
                    {
                        // action failed, we need to plan again
                        fsm.PopState();
                        fsm.PushState(idleState);
                        dataProvider.PlanAborted(action);
                    }
                }
                else
                {
                    // Not in range, must move there first
                    fsm.PushState(moveToState);
                }

            }
            else
            {
                // no actions left, move to Plan state
                fsm.PopState();
                fsm.PushState(idleState);
                dataProvider.ActionsFinished();
            }

        };
    }

    /// <summary>
    /// Search for a component that implements IGoap interface and set it to dataProvider.
    /// </summary>
    private bool FindAndSetDataProvider()
    {
        foreach (Component comp in gameObject.GetComponents(typeof(Component)))
        {
            if (typeof(IGoap).IsAssignableFrom(comp.GetType()))
            {
                dataProvider = (IGoap)comp;
                return true;
            }
        }
        Debug.LogError(this+ " did not find Data Provider! (A component that implements "+ "<color=red>IGoap Interface.</color>");
        return false;
    }

    /// <summary>
    /// Add all GOAP Agent actions to the list of available actions.
    /// </summary>
    private void LoadActions()
    {
        GoapAction[] actions = gameObject.GetComponents<GoapAction>();
        foreach (GoapAction action in actions)
        {
            availableActions.Add(action);
        }
        PrintAllAgentsActions(actions);
    }

    /// <summary>
    /// Prints all states leading up to the goal.
    /// </summary>
    /// <param name="_states">FSM states.</param>
    /// <returns>List of states in string form.</returns>
    public static string PrintAllStates(HashSet<KeyValuePair<string, object>> _states)
    {
        String _allStates = "";
        foreach (KeyValuePair<string, object> _state in _states)
        {
            _allStates += _state.Key + ":" + _state.Value.ToString();
            _allStates += ", ";
        }
        return _allStates;
    }

    /// <summary>
    /// Prints all actions that are required to reach the goal.
    /// </summary>
    /// <param name="_actions">Queue of actions required to execute plan.</param>
    /// <returns>Action plan as a string.</returns>
    public static string PrintActionPlan(Queue<GoapAction> _actions)
    {
        String _actionPlan = "";
        foreach (GoapAction _action in _actions)
        {
            _actionPlan += _action.GetType().Name;
            _actionPlan += " --> ";
        }
        _actionPlan += "Goal State.";
        return _actionPlan;
    }

    /// <summary>
    /// Print all available actions to the GOAP agent.
    /// </summary>
    /// <param name="_actions">array of actions available to the agent.</param>
    /// <returns>string listing the available actions.</returns>
    public static void PrintAllAgentsActions(GoapAction[] _actions)
    {
        String _message = "<color=yellow> All Agents Actions: </color>";
        foreach (GoapAction _action in _actions)
        {
            _message += _action.GetType().Name;
            _message += ", ";
        }
        _message = _message.Substring(0, _message.Length - 2); //remove final ", "

        Debug.Log(_message);
    }

    /// <summary>
    /// Get name of action.
    /// </summary>
    /// <param name="_action">action name to return.</param>
    /// <returns>Action name.</returns>
    public static string ActionName(GoapAction _action)
    {
        String _actionName = "" + _action.GetType().Name;
        return _actionName;
    }
}
