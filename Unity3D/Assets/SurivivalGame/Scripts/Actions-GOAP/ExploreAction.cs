using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreAction : GoapAction
{
    bool explored;

    public ExploreAction()
    {
        AddEffect("explored", true);
    }


    public override void ResetVariables()
    {
    
    }

    public override bool IsDone()
    {
        return explored;
    }

    public override bool RequiresInRange()
    {
        return true; // yes we need to be near the destination point
    }

    protected override bool FindTargetObject(GameObject _agent)
    {
        //Access agent memory and get information on where to move.
        if (GetComponent<AgentMemory>() != null)
        {
            GameObject _gameObject;
            Vector3 _rememberedPosition; //the position that we think the object is in based on agent memory.

            if (GetComponent<AgentMemory>().CheckMemoryForObject("ChargePoint", _agent.transform.position, out _gameObject, out _rememberedPosition))
            {
                target = _gameObject;
                targetPosition = _rememberedPosition;
                return true;
            }
        }
        else Debug.LogError(this + " cannot find Agent Memory!");

        return false;
    }

    public override bool ExecuteAction(GameObject agent)
    {
        //look around

            
        return true;
    }
}
