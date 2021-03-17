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
    [SerializeField] [Tooltip("Determines the value of the gene by lerping between it minimum and maximum possible value by the weight value.")] bool activeGene;

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
        this.activeGene = false;
    }

    public float GetValue()
    {
        return value;
    }

    public string GetName()
    {
        return geneName;
    }

    public void SetValue(float _val)
    {
        value = _val;
        minValue = Math.Min(_val, minValue);
        maxValue = Math.Max(_val, maxValue);
        weight = Mathf.InverseLerp(minValue, maxValue, _val);
    }

    public void SetWeight(float _weight)
    {
        weight = _weight;
        value = Mathf.Lerp(minValue,maxValue,weight);   
    }

    public float GetWeight()
    {
       return weight;
    }

    public bool IsActive()
    {
        return activeGene;
    }
}