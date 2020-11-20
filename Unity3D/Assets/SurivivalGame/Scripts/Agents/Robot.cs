using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A Robot parent class that should have children classes that implement CreateGoalState() 
 */

public abstract class Robot : MonoBehaviour, IGoap
{
    public Inventory inventory;
    public float moveSpeed = 1;
    public float range = 5;

    void Start()
    {
        if (GetComponent<Inventory>() != null)
            inventory = GetComponent<Inventory>();
        else
            inventory = gameObject.AddComponent<Inventory>() as Inventory;
        /*
        if (inventory.GetWeapon() == null)
        {
            GameObject prefab = Resources.Load<GameObject>(inventory.toolType);
            GameObject tool = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
            inventory.SetWeapon(tool);
        }
        */
    }

    /*Spotlight representation of visable range*/


    void Update()
    {

    }

    /**
	 * Key-Value data that will feed the GOAP actions and system while planning.
	 */
    public HashSet<KeyValuePair<string, object>> GetWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        //worldData.Add(new KeyValuePair<string, object>("hasCharge", (inventory.GetCharge()>10)));
        Debug.Log("<color=orange>Charge:</color>" + inventory.GetCharge());
        //worldData.Add(new KeyValuePair<string, object>("hasWeapon", (inventory.GetWeapon() != null)));

        return worldData;
    }

    /**
	 * Implement in subclasses
	 */
    public abstract HashSet<KeyValuePair<string, object>> CreateGoalState();


    public void PlanFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {
        // Not handling this here since we are making sure our goals will always succeed.
        // But normally you want to make sure the world state has changed before running
        // the same goal again, or else it will just fail.
    }

    public void PlanFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions)
    {
        // Yay we found a plan for our goal
        Debug.Log("<color=green>Plan found</color> " + GoapAgent.PrintAllActions(actions));
    }

    public void ActionsFinished()
    {
        // Everything is done, we completed our actions for this gool. Hooray!
        Debug.Log("<color=blue>Actions completed</color>");
    }

    public void PlanAborted(GoapAction aborter)
    {
        // An action bailed out of the plan. State has been reset to plan again.
        // Take note of what happened and make sure if you run the same goal again
        // that it can succeed.
        Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.PrettyPrint(aborter));
    }

    //called per step
    public bool MoveAgent(GoapAction nextAction)
    {
        // move towards the NextAction's target
        float step = moveSpeed * Time.deltaTime;
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, nextAction.target.transform.position, step);

        if (gameObject.transform.position.Equals(nextAction.target.transform.position))
        {
            // we are at the target location, we are done
            nextAction.SetInRange(true);
            return true;
        }
        else
            return false;
    }
}
