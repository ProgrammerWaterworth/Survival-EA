using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAgent : MonoBehaviour, IFitnessFunction
{
    public float range;
    public float posX;
    public float posY;
    [SerializeField] bool isComplete;


    private void Start()
    {
        transform.position = new Vector3(posX, transform.position.y, posY);
    }

    public int CheckForBatteries()
    {
        int count = 0;
        Collider[] cols = Physics.OverlapSphere(transform.position, range);

        foreach(Collider col in cols)
        {
            if (col.GetComponent<Battery>() != null)
            {
                count++;
            }
        }
        Debug.Log("Battery Score: " + count);
        return count;
    }

    private void OnValidate()
    {
        transform.position = new Vector3(posX, transform.position.y, posY);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public float GetFitness()
    {
        return CheckForBatteries()-(range*0.1f);
    }

    public bool IsEvalutionComplete()
    {
        return true;
    }
}
