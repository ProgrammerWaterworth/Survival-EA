using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpBatteryAction : GoapAction
{
    private bool pickedUp = false;
    private Battery targetBattery; // where the battery is located

    private float startTime = 0;
    public float pickUpDuration = .5f; // seconds

    public PickUpBatteryAction()
    {
        AddEffect("hasBattery", true);
    }


    public override void ResetVariables()
    {
        pickedUp = false;
        targetBattery = null;
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

    public override bool CheckProceduralPrecondition(GameObject agent)
    {

        // find the nearest battery to pick up
        Battery[] batteries = (Battery[])UnityEngine.GameObject.FindObjectsOfType(typeof(Battery));
        Battery closest = null;
        float closestDist = 0;

        foreach (Battery battery in batteries)
        {
            if (closest == null)
            {
                // first one, so choose it for now
                closest = battery;
                closestDist = (battery.gameObject.transform.position - agent.transform.position).magnitude;
            }
            else
            {
                // is this one closer than the last?
                float dist = (battery.gameObject.transform.position - agent.transform.position).magnitude;
                if (dist < closestDist)
                {
                    // we found a closer one, use it
                    closest = battery;
                    closestDist = dist;
                }
            }
        }
        if (closest == null)
            return false;

        targetBattery = closest;
        target = targetBattery.gameObject;

        return closest != null;
    }

    public override bool ExecuteAction(GameObject agent)
    {
        if (startTime == 0)
            startTime = Time.time;

        if (Time.time - startTime > pickUpDuration)
        {
            // finished charging
            Inventory inventory = (Inventory)agent.GetComponent(typeof(Inventory));
            inventory.IncreaseCharge(20);
            pickedUp = true;
            Destroy(targetBattery.gameObject); // For now destroy as if it has been used
        }
        return true;
    }
}
