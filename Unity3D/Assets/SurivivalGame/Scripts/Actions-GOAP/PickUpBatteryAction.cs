using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpBatteryAction : GoapAction
{
    private bool pickedUp = false;

    private float startTime = 0;
    public float pickUpDuration = .5f; // seconds

    public PickUpBatteryAction()
    {
        AddEffect("hasBattery", true);
    }


    public override void ResetVariables()
    {
        pickedUp = false;
        startTime = 0;
    }

    public override bool IsDone()
    {
        return pickedUp;
    }

    public override bool RequiresInRange()
    {
        return true; // yes we need to be near the battery
    }


    protected override bool FindTargetObject(GameObject _agent)
    {
        //Access agent memory and see if there is any knowledge of a battery.
        if (GetComponent<AgentMemory>() != null)
        {
            GameObject _targetGameObject;
            GameObject _rememberedTarget; //the position that we think the object is in based on agent memory.

            if (GetComponent<AgentMemory>().CheckMemoryForObject("Battery", _agent.transform.position, out _targetGameObject, out _rememberedTarget))
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

            if (Time.time - startTime > pickUpDuration)
            {
                // finished charging
                Inventory inventory = (Inventory)_agent.GetComponent(typeof(Inventory));
                inventory.IncreaseCharge(20);
                pickedUp = true;

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
