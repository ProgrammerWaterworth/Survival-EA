using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEnemy : RangedGoapAction
{
    private bool shot = false;
    private float startTime = 0;
    public float shotDuration = .5f; // seconds
    [SerializeField] float damage;
    [SerializeField] string enemyName;
    public ShootEnemy()
    {
        AddEffect("killEnemy", true);
    }

    private void Start()
    {
        targetObjectName = enemyName;
    }

    public override void ResetAction()
    {
        base.ResetAction();
        shot = false;
        startTime = 0;
    }

    public override bool IsDone()
    {
        return shot;
    }

    public override bool RequiresInRange()
    {
        return true;
    }


    public override bool ExecuteAction(GameObject _agent)
    {
        //check if the object is near the remembered location
        if (base.ExecuteAction(_agent))
        {
            if (startTime == 0)
                startTime = Time.time;

           

            transform.rotation.SetLookRotation(target.transform.position- transform.position, transform.up);

            if (Time.time - startTime > shotDuration)
            {                        
                shot = true;
                if (target.GetComponent<Robot>() != null)
                {
                    // finished charging
                    Inventory _inventory = (Inventory)_agent.GetComponent(typeof(Inventory));

                    if (_inventory != null)
                    {
                        if (target.name == targetObjectName)
                        {
                            _inventory.ConsumeAmmo();
                            target.GetComponent<Robot>().Damage(damage, gameObject);
                        }
                        else
                            Debug.LogError(this + " target is not " + targetObjectName);


                    }

                    if (target.GetComponent<Robot>().IsDead())
                    {
                        if (GetComponent<AgentMemory>() != null)
                        {
                            GetComponent<AgentMemory>().RemoveObjectFromMemory(memoryTarget);
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }
}
