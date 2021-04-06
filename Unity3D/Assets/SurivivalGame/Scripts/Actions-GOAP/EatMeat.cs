using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatMeat : GoapAction
{
    private bool eaten = false;
    private float startTime = 0;
    public float consumeDuration = 1f; // seconds

    public EatMeat()
    {
        AddEffect("isHungry", false);
    }

    private void Start()
    {
        targetObjectName = "Meat";
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

                if (target.name == "Meat")
                    _robot.ReduceHunger(0.8f);
                else
                    Debug.LogError(this + " target is not Meat.");

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
