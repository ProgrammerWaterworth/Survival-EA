using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    [SerializeField] float charge;


    public float GetCharge()
    {
        return charge;
    }
}
