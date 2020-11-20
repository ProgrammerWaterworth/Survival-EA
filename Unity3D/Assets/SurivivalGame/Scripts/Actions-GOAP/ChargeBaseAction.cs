using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeBaseAction : GoapAction
{
    private bool charged = false;
    private Base targetBase; // where the battery is located

    private float startTime = 0;
    public float chargeDuration = 2f; // seconds

    public ChargeBaseAction()
    {
        AddPrecondition("hasCharge", true);
        AddEffect("chargeBase", true);
    }


    public override void ResetVariables()
    {
        charged = false;
        targetBase = null;
        startTime = 0;
    }

    public override bool IsDone()
    {
        return charged;
    }

    public override bool RequiresInRange()
    {
        return true; // yes we need to be near the battery
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {

        // find the nearest battery to pick up
        Base[] batteries = (Base[])UnityEngine.GameObject.FindObjectsOfType(typeof(Base));
        Base closest = null;
        float closestDist = 0;

        foreach (Base battery in batteries)
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

        targetBase = closest;
        target = targetBase.gameObject;

        return closest != null;
    }

    public override bool Perform(GameObject agent)
    {
        if (startTime == 0)
            startTime = Time.time;

        if (Time.time - startTime > chargeDuration)
        {
            // finished charging
            Inventory inventory = (Inventory)agent.GetComponent(typeof(Inventory));
            inventory.DecreaseCharge(20);
            charged = true;
           
        }
        return true;
    }
}
