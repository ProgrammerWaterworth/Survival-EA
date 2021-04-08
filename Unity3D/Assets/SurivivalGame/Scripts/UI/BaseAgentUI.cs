using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseAgentUI : MonoBehaviour
{
    [SerializeField] protected Slider healthBar;


    protected virtual void Start()
    {
        SetupUI();
    }

    /// <summary>
    /// Checks and sets up any necassary components for this script to function.
    /// </summary>
    protected virtual void SetupUI()
    {
        if (healthBar == null)
            Debug.LogError(this + " does not have a health Bar assigned!");
    }

    /// <summary>
    /// Update health bar with _percentage filled.
    /// </summary>
    /// <param name="_percentage"></param>
    public void UpdateHealth(float _percentage)
    {
        if (healthBar != null)
        {
            healthBar.value = _percentage;
        }

    }
}
