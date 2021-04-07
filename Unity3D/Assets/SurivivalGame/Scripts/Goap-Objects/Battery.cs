using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    [SerializeField] float chargeDispense;
    [SerializeField] float charge;
    [SerializeField] float maxCharge;

    public float GetCharge()
    {
        return charge;
    }

    public void IncreaseCharge(float _increase)
    {
        charge += _increase;
        charge = Mathf.Clamp(charge, 0, maxCharge);
    }

    public float GetChargePercentage()
    {
        return charge / maxCharge;
    }

    public void SetChargePercentage(float _perc)
    {
       charge = _perc*  maxCharge;
    }
}
