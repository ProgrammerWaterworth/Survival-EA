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

    public delegate void FSMState(FSM fsm, GameObject gameObject);


    /// <summary>
    /// Trigger state at the top of the Finite State Machine stack.
    /// </summary>
    /// <param name="gameObject"></param>
    public void Update(GameObject gameObject)
    {
        if (stateStack.Peek() != null)
            stateStack.Peek().Invoke(this, gameObject);
    }

    /// <summary>
    /// Push a state to the Finite State Machine Stack.
    /// </summary>
    /// <param name="state">State pushed to top of the stack.</param>
    public void PushState(FSMState state)
    {
        stateStack.Push(state);
    }

    /// <summary>
    /// Pop the state at the top of the Finite State Machine stack.
    /// </summary>
    public void PopState()
    {
        stateStack.Pop();
    }
}