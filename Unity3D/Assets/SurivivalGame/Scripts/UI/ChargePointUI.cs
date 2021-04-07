using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargePointUI : MonoBehaviour
{
    [SerializeField] Slider chargeBar;


    private void Start()
    {
        SetupUI();
    }

    /// <summary>
    /// Checks and sets up any necassary components for this script to function.
    /// </summary>
    void SetupUI()
    {
        if (chargeBar == null)
            Debug.LogError(this + " does not have a hunger Bar  assigned!");
    }

    /// <summary>
    /// Update charge bar with _percentage filled.
    /// </summary>
    /// <param name="_percentage"></param>
    public void UpdateCharge(float _percentage)
    {
        if (chargeBar != null)
        {
            chargeBar.value = _percentage;
        }
    }
}
