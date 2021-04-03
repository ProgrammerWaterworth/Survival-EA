using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentUI : MonoBehaviour
{
    [SerializeField] Slider healthBar, hungerBar, thirstBar;


    private void Start()
    {
        SetupUI();
    }

    /// <summary>
    /// Checks and sets up any necassary components for this script to function.
    /// </summary>
    void SetupUI()
    {
        if (healthBar == null)
            Debug.LogError(this + " does not have a health Bar assigned!");
        if (hungerBar == null)
            Debug.LogError(this + " does not have a hunger Bar  assigned!");
        if (thirstBar == null)
            Debug.LogError(this + " does not have a health Bar assigned!");
    }

    /// <summary>
    /// Update health bar with _percentage filled.
    /// </summary>
    /// <param name="_percentage"></param>
    public void UpdateHealth(float _percentage)
    {
        if(healthBar!=null)
            healthBar.value = _percentage;
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
    /// Update Thirst bar with _percentage filled.
    /// </summary>
    /// <param name="_percentage"></param>
    public void UpdateThirst(float _percentage)
    {
        if (thirstBar != null)
            thirstBar.value = _percentage;
    }
}
