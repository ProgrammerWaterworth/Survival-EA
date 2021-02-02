using UnityEngine;
using System.Collections;


/// <summary>
/// Am Interface for a Finite State Machine state.
/// </summary>
public interface FSMState
{
    void Update(FSM fsm, GameObject gameObject);
}

