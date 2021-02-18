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
        AddPrecondition("hasBattery", true);
        AddEffect("chargeBase", true);
        AddEffect("hasBattery", false);
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

    protected override bool FindTargetObject(GameObject _agent)
    {
        //Access agent memory and see if there is any knowledge of a Base.
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
