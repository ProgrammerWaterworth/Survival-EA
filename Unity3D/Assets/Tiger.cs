using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiger : BaseAgent
{
    [Header("Survival Stats")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float maxHunger;
    [SerializeField] protected float hungerIncreaseRate;
    AgentUI agentUI;

    protected float health;
    bool dead;

    [Header("Movement")]
    public float maxMoveSpeed = 1;
    public float range = 2;
    float moveSpeed;
    Animator animator;
    Rigidbody rb;

    [SerializeField] float rotationStepAngle;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] bool moving;

    //Navigation
    UnityEngine.AI.NavMeshAgent navAgent;

    protected virtual void Start()
    {
        SetUpTiger();
        totalNumGoals = 1;
    }

    protected virtual void Update()
    {
        AnimateMovement();
        UpdateUI();
    }
    private void FixedUpdate()
    {
    }

    /// <summary>
    /// Set up the components required for the robot to function.
    /// </summary>
    void SetUpTiger()
    {       

        if (GetComponent<Rigidbody>() != null)
            rb = GetComponent<Rigidbody>();
        else
        {
            rb = gameObject.AddComponent<Rigidbody>() as Rigidbody;
            Debug.LogWarning(this + " has no Rigidbody Component, adding one.");
        }

        if (GetComponent<UnityEngine.AI.NavMeshAgent>() != null)
        {
            navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            navAgent.speed = maxMoveSpeed;
            navAgent.angularSpeed = rotationStepAngle;
        }
        else
        {
            Debug.LogWarning(this + " has no NavMeshAgent Component!");
        }

        //Set up stats
        health = maxHealth;

        if (GetComponent<AgentUI>() != null)
            agentUI = GetComponent<AgentUI>();
        else
            Debug.LogWarning(this + " has no AgentUI Component!");
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

    public override HashSet<KeyValuePair<string, object>> GetWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        return worldData;
    }

    public override HashSet<KeyValuePair<string, object>> CreateGoalState()
    {

        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        switch (goalIndex)
        {
            case 0:
                goal = Explore();
                break;
        }
        return goal;
    }

    public override float GetGoalMultiplier()
    {
        float _multiplier = 1;
        //Return a value between 0 and 1 to determine how it modifies cost of a plan. Lowest is chosen as plan.
        switch (goalIndex)
        {
            case 0:
                _multiplier = (health / maxHealth);
                break;
        }
        return _multiplier;
    }


    HashSet<KeyValuePair<string, object>> Explore()
    {
        HashSet<KeyValuePair<string, object>> _goal = new HashSet<KeyValuePair<string, object>>();
        _goal.Add(new KeyValuePair<string, object>("explored", true));
        return _goal;
    }

    public override void PlanFound(HashSet<KeyValuePair<string, object>> _goal, Queue<GoapAction> _actions, float _planCost)
    {
        base.PlanFound(_goal, _actions, _planCost);
    }

    public override bool MoveAgent(GoapAction _nextAction)
    {

        if (navAgent != null)
        {
            navAgent.SetDestination(_nextAction.memoryTarget.transform.position);
        }

        targetPosition = _nextAction.memoryTarget.transform.position;
        // Set direction the agent is facing.        
        /*      
        moving = true;
        transform.forward = Vector3.RotateTowards(transform.forward, targetPosition - transform.position, rotationStepAngle, 0);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        */
        //Debug.Log(this + " distance from target: "+ Vector3.Distance(transform.position, targetLocation.position));
        if (Vector3.Distance(transform.position, targetPosition) < range)
        {
            Debug.Log(this + " is in range of target location.");
            // we are at the target location, we are done
            _nextAction.SetInRange(true);
            return true;
        }
        else
            return false;
    }


    /// <summary>
    /// Animates the movement of Robot.
    /// </summary>
    /// <param name="_speed">Speed of the animation.</param>
    void AnimateMovement()
    {
        if (animator != null)
        {
            if (navAgent != null)
            {
                animator.SetFloat("Movespeed", navAgent.velocity.magnitude);
            }
            else Debug.LogWarning(this + " hasn't set NavMeshAgent Component!");
        }
        else Debug.LogWarning(this + " hasn't set Animator Component!");
    }

    public void AlterHealth(float _ammount)
    {
        health += _ammount * maxHealth;
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    void UpdateUI()
    {
        if (agentUI != null)
        {
            agentUI.UpdateHealth(health / maxHealth);
        }
    }
}
