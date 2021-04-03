using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatFruit : GoapAction
{
    private bool eaten = false;
    private float startTime = 0;
    public float consumeDuration = 1f; // seconds

    public EatFruit()
    {
        AddEffect("isHungry", false);
    }

    public override void ResetAction()
    {
        base.ResetAction();
        eaten = false;
        startTime = 0;
    }

    public override bool IsDone()
    {
        return eaten;
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

            if (GetComponent<AgentMemory>().CheckMemoryForObject("Fruit", _agent.transform.position, out _targetGameObject, out _rememberedTarget))
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

            if (Time.time - startTime > consumeDuration)
            {
                // finished charging
                Robot _robot = (Robot)_agent.GetComponent(typeof(Robot));

                if (target.name == "Fruit")
                    _robot.ReduceHunger(0.2f);
                else
                    Debug.LogError(this + " target is not a Fruit.");

                eaten = true;

                if (GetComponent<AgentMemory>() != null)
                {
                    GetComponent<AgentMemory>().RemoveObjectFromMemory(memoryTarget);
                }
                Destroy(target); // For now destroy as if it has been used
                //target = null;
            }
            return true;
        }
        return false;
    }

}
