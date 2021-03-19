using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// A GoapAgent implements a GOAP Plan through the use of three states: Idle, MoveTo and PerformAction.
/// </summary>
public sealed class GoapAgent : MonoBehaviour
{
    #region Finite State Machine
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
    #endregion

    #region Actions
    /// <summary>
    /// All actions available to the Goap Agent
    /// </summary>
    private HashSet<GoapAction> availableActions;
    /// <summary>
    /// Actions the agent must execute to reach their goal.
    /// </summary>
    private Queue<GoapAction> currentPlanActions;
    #endregion

    /// <summary>
    /// The agent which implements the IGoap interface.
    /// </summary>
    IGoap dataProvider; // this is the implementing class that provides our world data and listens to feedback on planning

    GoapPlanner goapPlanner;


    void Start()
    {
        SetUpGoapAgent();
    }


    void Update()
    {
        stateMachine.Update(gameObject);
    }

    /// <summary>
    /// Set up the components required for the Goap agent to function.
    /// </summary>
    void SetUpGoapAgent()
    {
        stateMachine = new FSM();
        availableActions = new HashSet<GoapAction>();
        currentPlanActions = new Queue<GoapAction>();
        goapPlanner = new GoapPlanner();
        FindAndSetDataProvider();
        CreateIdleState();
        CreateMoveToState();
        CreateExecuteActionState();
        stateMachine.PushState(idleState);
        LoadActions();
    }

    /// <summary>
    /// Checks if there are any current actions available.
    /// </summary>
    /// <returns>True if GOAP agent has an action plan.</returns>
     bool HasPlan()
    {
        return currentPlanActions.Count > 0;
    }

    /// <summary>
    /// Creates the agents Idle state. 
    /// A state in which the agent plans how to reach its goal.
    /// </summary>
    void CreateIdleState()
    {
        //Invoked when updating idle state.
        idleState = (_fsm, _agentObj) => {
            // GOAP planning

            // Get the world state and the goal we want to plan for
            HashSet<KeyValuePair<string, object>> _worldState = dataProvider.GetWorldState();
            HashSet<KeyValuePair<string, object>> _goal = dataProvider.CreateGoalState();

            Debug.Log(this + " is Idle");

            // Plan
            Queue<GoapAction> _plan = goapPlanner.Plan(gameObject, availableActions, _worldState, _goal);
            if (_plan != null)
            {
                // Obtained plan successfully.
                currentPlanActions = _plan;
                dataProvider.PlanFound(_goal, _plan);

                _fsm.PopState(); // move to executeAction state
                _fsm.PushState(executeActionState);

            }
            else
            {
                // Failed to make a plan.
                Debug.Log("<color=orange>Failed to plan for goal with conditions: </color>" + PrintStateConditions(_goal));
                dataProvider.PlanFailed(_goal);
            }
        };
    }

    /// <summary>
    /// Creates the agents MoveToState. 
    /// A state in which the agent navigates to a location in which it can execute an action.
    /// </summary>
    void CreateMoveToState()
    {
        //Invoked when in updating moveTo state.
        moveToState = (fsm, gameObj) => {
            // move the game object
            //Debug.Log(this + " is Moving");
            GoapAction action = currentPlanActions.Peek();

            // get the agent to move itself
            if (dataProvider.MoveAgent(action))
            {
                fsm.PopState();
            }
        };
    }

    /// <summary>
    /// Creates the agents Execute Action state. 
    /// A state in which the agent carries out their action.
    /// </summary>
    void CreateExecuteActionState()
    {
        //Invoked when in updating executeAction state.
        executeActionState = (fsm, gameObj) => {
            // execute the action

            //Debug.Log(this + " is Executing Action.");

            if (!HasPlan())
            {
                // no actions to execute
                Debug.Log("<color=red>Done actions</color>");
                fsm.PopState();
                fsm.PushState(idleState);
                dataProvider.ActionsFinished();
                return;
            }

            GoapAction action = currentPlanActions.Peek();
            if (action.IsDone())
            {
                // the action is done. Remove it so we can execute the next one
                currentPlanActions.Dequeue();
            }

            if (HasPlan())
            {
                // execute the next action
                action = currentPlanActions.Peek();
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
    bool FindAndSetDataProvider()
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
    void LoadActions()
    {
        GoapAction[] actions = gameObject.GetComponents<GoapAction>();
        foreach (GoapAction action in actions)
        {
            availableActions.Add(action);
        }
        PrintAllAgentsActions(actions);
    }

    /// <summary>
    /// Prints conditions of _state.
    /// </summary>
    /// <param name="_state"></param>
    /// <returns>List of state conditions in string form.</returns>
    public static string PrintStateConditions(HashSet<KeyValuePair<string, object>> _state)
    {
        String _allStates = "";
        foreach (KeyValuePair<string, object> _condition in _state)
        {
            _allStates += _condition.Key + ":" + _condition.Value.ToString();
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
