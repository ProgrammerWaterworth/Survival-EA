using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GeneData
{
    [SerializeField] [Tooltip("Determines the value fo the gene by lerping between it minimum and maximum possible value by the weight value.")]float weight;
    [SerializeField] [Tooltip("Minimum possible value of the gene.")]  float minValue;
    [SerializeField] [Tooltip("Maximum possible value of the gene.")]  float maxValue;
}