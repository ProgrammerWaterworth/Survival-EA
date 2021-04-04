using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentUI : MonoBehaviour
{
    [SerializeField] Slider healthBar, hungerBar, chargeBar;


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
        if (chargeBar == null)
            Debug.LogError(this + " does not have a hunger Bar  assigned!");
    }

    /// <summary>
    /// Update health bar with _percentage filled.
    /// </summary>
    /// <param name="_percentage"></param>
    public void UpdateHealth(float _percentage)
    {
        if (healthBar != null)
        {
            if(healthBar.fillRect.GetComponent<Image>()!=null)
                healthBar.fillRect.GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, _percentage);
            healthBar.value = _percentage;
        }

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
