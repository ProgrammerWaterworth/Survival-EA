using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigateTreeAction : GoapAction
{
    private bool investigated = false;
    private float startTime = 0;
    public float investigateDuration = .5f; // seconds

    public InvestigateTreeAction()
    {
        AddEffect("searchFood", true);
    }

    private void Start()
    {
        targetObjectName = "Fruit Tree";
    }

    public override void ResetAction()
    {
        base.ResetAction();
        investigated = false;
        startTime = 0;
    }

    public override bool IsDone()
    {
        return investigated;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    protected override bool FindTargetObject(GameObject _agent)
    {
        //Access agent memory and see if there is any knowledge of a battery.
        if (GetComponent<AgentMemory>() != null)
        {
            GameObject _targetGameObject;
            GameObject _rememberedTarget; //the position that we think the object is in based on agent memory.

            if (GetComponent<AgentMemory>().CheckWeightedMemoryForObject("Fruit Tree", _agent.transform.position, out _targetGameObject, out _rememberedTarget))
            {
                target = _targetGameObject;
                memoryTarget = _rememberedTarget;
                return true;
            }
        }

        return false;
    }
    public override bool ExecuteAction(GameObject _agent)
    {
        //check if the object is near the remembered location
        if (base.ExecuteAction(_agent))
        {
            if (startTime == 0)
                startTime = Time.time;

            if (Time.time - startTime > investigateDuration)
            {
                investigated = true;
            }
            return true;
        }
        return false;
    }
}
