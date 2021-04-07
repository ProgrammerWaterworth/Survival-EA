using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentUI : BaseAgentUI
{
    [SerializeField] Slider hungerBar, chargeBar;


    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// Checks and sets up any necassary components for this script to function.
    /// </summary>
    protected override void SetupUI()
    {
        base.SetupUI();

        if (hungerBar == null)
            Debug.LogError(this + " does not have a hunger Bar  assigned!");
        if (chargeBar == null)
            Debug.LogError(this + " does not have a hunger Bar  assigned!");
    }

    /// <summary>
    /// Update Hunger bar with _percentage filled.
    /// </summary>
    /// <param name="_percentage"></param>
    public void UpdateHunger(float _percentage)
    {
        if (hungerBar != null)
            hungerBar.value = _percentage;
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
