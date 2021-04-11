using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeToHealthAction : GoapAction
{
    private bool healed = false;
    private float startTime = 0;
    public float healDuration = .8f; // seconds
    [SerializeField] float healthRestorePercentage;
    public ChargeToHealthAction()
    {
        AddPrecondition("healed", false);
        AddPrecondition("hasMuchBattery", true);
        AddEffect("healed", true);
    }

    public override void ResetAction()
    {
        base.ResetAction();
        healed = false;
        startTime = 0;
    }

    public override bool IsDone()
    {
        return healed;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    protected override bool FindTargetObject(GameObject _agent)
    {
        return true;
    }

    public override bool ExecuteAction(GameObject _agent)
    {
        //check if the object is near the remembered location
       
        if (startTime == 0)
            startTime = Time.time;

        if (Time.time - startTime > healDuration)
        {
            healed = true;

            // finished charging
            Inventory _inventory = (Inventory)_agent.GetComponent(typeof(Inventory));
            Robot _robot = (Robot)_agent.GetComponent(typeof(Robot));
            if (_inventory != null && _robot!=null)
            {
                _inventory.DecreaseCharge(_robot.GetHealthIncreaseChargeCost());
                _robot.AlterHealth(healthRestorePercentage);
            }
        }
        return true;     
    }
}
