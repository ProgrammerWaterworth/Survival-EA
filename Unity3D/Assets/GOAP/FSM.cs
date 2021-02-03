using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Finite State Machine - Stack.
/// Push and pop states to the FSM.
/// States should push other states onto the stack 
/// and pop themselves off.
/// </summary>
public class FSM 
{
    private Stack<FSMState> stateStack = new Stack<FSMState>();

    public delegate void FSMState(FSM _fsm, GameObject _agent);


    /// <summary>
    /// Run the state at the top of the Finite State Machine stack.
    /// </summary>
    /// <param name="_agentGameObject">The agent running the finite state machine.</param>
    public void Update(GameObject _agentGameObject)
    {
        if (stateStack.Peek() != null)
        {
            stateStack.Peek().Invoke(this, _agentGameObject);
        }
    }

    /// <summary>
    /// Push a state to the Finite State Machine Stack.
    /// </summary>
    /// <param name="_state">State pushed to top of the stack.</param>
    public void PushState(FSMState _state)
    {
        stateStack.Push(_state);
    }

    /// <summary>
    /// Pop the state at the top of the Finite State Machine stack.
    /// </summary>
    public void PopState()
    {
        stateStack.Pop();
    }
}