using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{

    float totalCharge;
    [SerializeField] float maxCharge = 250f, chargeUsageRate = 0.1f;
    [SerializeField] ChargePoint[] chargePoints;
    //Visuals
    
    void Start()
    {
        SetUpChargePoints();
    }

    void Update()
    {
        //If it doesn't have enough charge - fail!
        if(!UseCharge(chargeUsageRate))
        {
            Debug.Log(this + " has ran out of charge!");
        }
        
    }

    void SetUpChargePoints()
    {
        chargePoints = GetComponentsInChildren<ChargePoint>();

        foreach(ChargePoint _chargePoint in chargePoints)
        {
            _chargePoint.SetBaseToCharge(this);
        }
    }

    public void ChargeBase(float _charge)
    {
        totalCharge += _charge;
        totalCharge = Mathf.Clamp(totalCharge, 0, maxCharge);
    }

    /// <summary>
    /// Uses charge stored in the base.
    /// </summary>
    /// <param name="_charge"></param>
    /// <returns>True if it has sufficient charge to use. False if there isn't enough charge to perform action.</returns>
    bool UseCharge(float _charge)
    {
        //ensure charge is a positive value.
        if (_charge < 0)
        {
            Debug.LogError(this + " is attempting to take away a negative value of charge.");
            return false;
        }

        if (totalCharge >= _charge)
        {
            totalCharge -= _charge;
            totalCharge = Mathf.Clamp(totalCharge, 0, maxCharge);
            return true;
        }
        return false;
    }
}
