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

    private void Start()
    {
        targetObjectName = "Battery";
    }

    public override void ResetAction()
    {
        base.ResetAction();
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

                if (target.GetComponent<Battery>() != null)
                    inventory.IncreaseCharge(target.GetComponent<Battery>().GetCharge());
                else
                    Debug.LogError(this + " target is not a battery.");

                pickedUp = true;

                if (GetComponent<AgentMemory>() != null)
                {
                    Debug.Log(this + " is removing battery from memory.");
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
