using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GeneData
{
    [SerializeField] string geneName;
    [SerializeField] [Tooltip("Determines the value of the gene by lerping between it minimum and maximum possible value by the weight value.")] float weight;
    [SerializeField] [Tooltip("Minimum possible value of the gene.")]  float minValue;
    [SerializeField] [Tooltip("Maximum possible value of the gene.")]  float maxValue;
    [SerializeField] [Tooltip("Actual value of gene.")] float value;

    public GeneData(string name, float weight, float minValue, float maxValue, float value)
    {
        this.geneName = name;
        this.weight = weight;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.value = maxValue;
    }

    public GeneData(string name, float value)
    {
        this.geneName = name;
        this.weight = .5f;
        this.minValue = value/2;
        this.maxValue = value*1.5f;
        this.value = value;
    }

    public float GetValue()
    {
        return value;
    }

    public string GetName()
    {
        return geneName;
    }

    public void SetFloat(float val)
    {
        value = val;
        minValue = Math.Min(val, minValue);
        maxValue = Math.Max(val, maxValue);
        weight = Mathf.InverseLerp(minValue, maxValue, val);
    }

}