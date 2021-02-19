﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAgent : MonoBehaviour
{
    [SerializeField] GeneFloat range;

    private void Update()
    {
        
    }

    public int CheckForBatteries()
    {
        int count = 0;
        Collider[] cols = Physics.OverlapSphere(transform.position, range.value);

        foreach(Collider col in cols)
        {
            if (col.GetComponent<Battery>() != null)
            {
                count++;
            }
        }
        return count;
    }

    public float GetRangeGene()
    {
        return range.weight;
    }

    public void SetRangeGene(float _weight)
    {
       range.value = Mathf.Max(_weight,0);
       //range.value = Mathf.Lerp(range.min, range.max, range.weight);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range.value);
    }
}
