using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GeneData
{
    [SerializeField] string geneName;
    [SerializeField] [Tooltip("Determines the value fo the gene by lerping between it minimum and maximum possible value by the weight value.")] float weight;
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
        this.weight = 1f;
        this.minValue = 0;
        this.maxValue = value;
        this.value = value;
    }

    public float GetValue()
    {
        return value;
    }

}