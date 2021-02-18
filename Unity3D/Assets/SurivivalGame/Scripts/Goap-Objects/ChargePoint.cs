using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargePoint : MonoBehaviour
{
    Base parentBase; //The base this charge point charges

    public void SetBaseToCharge(Base _base)
    {
        if (_base != null)
            parentBase = _base;
        else Debug.LogError(this + " is attempting to set a null value as the parent base!");
    }

    public void ChargeBase(float _charge)
    {
        if (parentBase != null)
        {
            parentBase.ChargeBase(_charge);
        }
        else Debug.LogError(this + " isn't attached to a based.");
    }

}
