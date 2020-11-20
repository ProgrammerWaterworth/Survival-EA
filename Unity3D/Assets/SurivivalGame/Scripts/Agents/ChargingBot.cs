using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingBot : Robot
{
    /**
	 * Our only goal will ever be to get batteries
	 * The PickUpBatteryAction will be able to fulfill this goal.
	 */
    public override HashSet<KeyValuePair<string, object>> CreateGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        goal.Add(new KeyValuePair<string, object>("hasBattery", true));
        return goal;
    }
}
