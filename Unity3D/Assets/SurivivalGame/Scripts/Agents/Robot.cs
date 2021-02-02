using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A Robot parent class that should have children classes that implement CreateGoalState() 
 */

public abstract class Robot : MonoBehaviour, IGoap
{
    public Inventory inventory;
    public float maxMoveSpeed = 1;
    public float range = 2;
    float moveSpeed;
    Animator animator;
    Rigidbody rb;
    [SerializeField] float rotationStepAngle;

    Transform targetLocation;

    void Start()
    {
        SetUpRobot();
    }

    void Update()
    {
        AnimateMovement();
    }

    private void FixedUpdate()
    {
        MoveAgentToTarget();
    }

    /// <summary>
    /// Set up the components required for the robot to function.
    /// </summary>
    void SetUpRobot()
    {
        if (GetComponent<Inventory>() != null)
            inventory = GetComponent<Inventory>();
        else
        {
            inventory = gameObject.AddComponent<Inventory>() as Inventory;
            Debug.LogWarning(this + " has no Inventory Component, adding one.");
        }

        if (GetComponent<Rigidbody>() != null)
            rb = GetComponent<Rigidbody>();
        else
        {
            rb = gameObject.AddComponent<Rigidbody>() as Rigidbody;
            Debug.LogWarning(this + " has no Rigidbody Component, adding one.");
        }
        /*
        if (inventory.GetWeapon() == null)
        {
            GameObject prefab = Resources.Load<GameObject>(inventory.toolType);
            GameObject tool = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
            inventory.SetWeapon(tool);
        }
        */

        if (GetComponent<Animator>() != null)
        {
            animator = GetComponent<Animator>();
        }
        else Debug.LogWarning(this + " has no animator set!");
    }

    public HashSet<KeyValuePair<string, object>> GetWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        //worldData.Add(new KeyValuePair<string, object>("hasCharge", (inventory.GetCharge()>10)));
        Debug.Log("<color=orange>Charge:</color>" + inventory.GetCharge());
        //worldData.Add(new KeyValuePair<string, object>("hasWeapon", (inventory.GetWeapon() != null)));

        return worldData;
    }

    public abstract HashSet<KeyValuePair<string, object>> CreateGoalState();


    public void PlanFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {
        // Not handling this here since we are making sure our goals will always succeed.
        // But normally you want to make sure the world state has changed before running
        // the same goal again, or else it will just fail.
    }

    public void PlanFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions)
    {
        Debug.Log("<color=green>Plan found</color> - " + GoapAgent.PrintActionPlan(actions));
    }

    public void ActionsFinished()
    {
        Debug.Log("<color=cyan>Actions completed!</color>");
    }

    public void PlanAborted(GoapAction aborter)
    {
        // An action bailed out of the plan. State has been reset to plan again.
        // Take note of what happened and make sure if you run the same goal again
        // that it can succeed.
        Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.ActionName(aborter));
    }

    public bool MoveAgent(GoapAction nextAction)
    {
        Debug.Log(this + " is moving.");
        targetLocation = nextAction.target.transform;

        // Set direction the agent is facing.        
        transform.forward = Vector3.RotateTowards(transform.forward, targetLocation.position - transform.position, rotationStepAngle, 0);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        Debug.Log(this + " distance from target: "+ Vector3.Distance(transform.position, targetLocation.position));
        if (Vector3.Distance(transform.position,targetLocation.position) < range)
        {
            Debug.Log(this + " is in range of target location.");
            // we are at the target location, we are done
            nextAction.SetInRange(true);
            targetLocation = null;
            return true;
        }
        else
            return false;     
    }

    /// <summary>
    /// FixedUpdate Function: Applies movement to agent's Rigidbody.
    /// </summary>
    void MoveAgentToTarget()
    {
        if (targetLocation == null)      
            return;

        if (rb != null)
        {
            // Move towards the NextAction's target
            moveSpeed = maxMoveSpeed * Time.fixedDeltaTime;
            Vector3 _direction = targetLocation.transform.position - transform.position;
            _direction = new Vector3(_direction.x, 0, _direction.z).normalized;
            rb.velocity = _direction * moveSpeed;
        }
        else Debug.LogWarning(this + " hasn't set Rigidbody Component!");

    }

    /// <summary>
    /// Animates the movement of Robot.
    /// </summary>
    /// <param name="_speed">Speed of the animation.</param>
    void AnimateMovement()
    {      
        if (animator != null)
        {
            if (rb != null)
            {
                animator.SetFloat("Movespeed", rb.velocity.magnitude);
            }
            else Debug.LogWarning(this + " hasn't set Rigidbody Component!");
        }
        else Debug.LogWarning(this + " hasn't set Animator Component!");
    }
}
