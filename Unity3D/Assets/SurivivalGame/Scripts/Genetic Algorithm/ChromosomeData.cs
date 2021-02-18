using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
[CreateAssetMenu(fileName = "chromosome", menuName = "Genetic Algorithm/Chromosome")]
public class ChromosomeData : ScriptableObject
{
    [SerializeField] List<GeneData> geneData;

}
