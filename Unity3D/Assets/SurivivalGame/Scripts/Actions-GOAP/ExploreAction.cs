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
        //Needs to create a destiantion point.
        GameObject _destinationPoint = new GameObject("ExplorePoint");
        //Get a point that's on the navmesh.

        //_destinationPoint.

        target = _destinationPoint;
        memoryTarget = target;

        //Access agent memory and see if there is any knowledge of a battery.
        if (GetComponent<AgentMemory>() != null)
        {
            GameObject _targetGameObject;
            GameObject _rememberedTarget; //the position that we think the object is in based on agent memory.

            if (GetComponent<AgentMemory>().CheckMemoryForObject("ExplorePoint", _agent.transform.position, out _targetGameObject, out _rememberedTarget))
            {
                target = _targetGameObject;
                memoryTarget = _rememberedTarget;
                return true;
            }
        }

        return false;
    }

    public override bool ExecuteAction(GameObject agent)
    {
        //look around for a bit

            
        return true;
    }
}
