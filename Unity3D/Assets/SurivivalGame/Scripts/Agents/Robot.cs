﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// A Goap agent implemenation.
/// </summary>
public abstract class Robot : BaseAgent
{
    [Header("Survival Stats")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float maxHunger;
    [SerializeField] protected float hungerIncreaseRate;
    AgentUI agentUI;

    protected float health, hunger;
    bool dead;

    public Inventory inventory;
    
    [Header("Movement")]
    public float maxMoveSpeed = 1;
    public float range = 2;
    float moveSpeed;
    Animator animator;
    Rigidbody rb;

    [SerializeField] float rotationStepAngle;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] bool moving;

    //Testing Spawn//
    public float posX;
    public float posY;
    public float RotY;

    //Navigation
    NavMeshAgent navAgent;

    void Start()
    {
        SetUpRobot();
        //Set spawn point
        transform.position = new Vector3(posX, transform.position.y, posY);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, RotY, transform.eulerAngles.z);

        totalNumGoals = 3;
    }

    protected virtual void Update()
    {
        AnimateMovement();
        Metabolise();
        UpdateUI();
    }
    private void FixedUpdate()
    {
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

        if (GetComponent<NavMeshAgent>() != null)
        {
            navAgent = GetComponent<NavMeshAgent>();
            navAgent.speed = maxMoveSpeed;
            navAgent.angularSpeed = rotationStepAngle;
        }
        else
        {
            Debug.LogWarning(this + " has no NavMeshAgent Component!");
        }

        //Set up stats
        health = maxHealth;
        hunger = maxHunger;

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

        //worldData.Add(new KeyValuePair<string, object>("hasCharge", (inventory.GetCharge()>10)));
        Debug.Log("<color=orange>"+this+" charge:</color>" + inventory.GetCharge());
        //worldData.Add(new KeyValuePair<string, object>("hasWeapon", (inventory.GetWeapon() != null)));

        return worldData;
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
        if (Vector3.Distance(transform.position,targetPosition) < range)
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
    /// FixedUpdate Function: Applies movement to agent's Rigidbody.
    /// </summary>
    void MoveAgentToTarget()
    {
        if (targetPosition == Vector3.zero)
        {
            return;
        }   
            
        if (rb != null)
        {
            // Move towards the NextAction's target
            moveSpeed = maxMoveSpeed * Time.fixedDeltaTime;
            Vector3 _direction = targetPosition - transform.position;
            _direction = new Vector3(_direction.x, 0, _direction.z).normalized;
            rb.AddForce(_direction * moveSpeed, ForceMode.Force);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxMoveSpeed);
        }
        else Debug.LogWarning(this + " hasn't set Rigidbody Component!");

        moving = false;
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

    /// <summary>
    /// Increase Hunger and Thirst over time.
    /// </summary>
    void Metabolise()
    {
        if (!dead)
        {
            hunger -= hungerIncreaseRate * Time.deltaTime;
        }

        if (hunger <= 0)
            Die();
    }

    /// <summary>
    /// Cause the player to die.
    /// </summary>
    void Die()
    {
        dead = true;
    }

    public void ReduceHunger(float _ammount)
    {
        hunger += _ammount*maxHunger;
        hunger = Mathf.Clamp(hunger, 0, maxHunger);
    }

    public void AlterHealth(float _ammount)
    {
        health += _ammount*maxHealth;
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    void UpdateUI()
    {
        if (agentUI != null)
        {
            agentUI.UpdateHunger(hunger / maxHunger);
            agentUI.UpdateHealth(health / maxHealth);
            if (inventory != null)
                agentUI.UpdateCharge(inventory.GetCharge() / inventory.GetMaxCharge());
        }
    }
}
