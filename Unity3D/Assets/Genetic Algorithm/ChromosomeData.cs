using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
[CreateAssetMenu(fileName = "chromosome", menuName = "Genetic Algorithm/Chromosome")]
public class ChromosomeData : ScriptableObject
{
    [SerializeField] GameObject gameObject;
    [SerializeField] List<GeneData> genes;

    public void GetGameObjectProperties()
    {
        if (gameObject == null)
            return;

        Component[] cs = (Component[])gameObject.GetComponents(typeof(Component));
        foreach (Component c in cs)
        {
            Debug.Log("name " + c.name + " type " + c.GetType() + " basetype " + c.GetType().BaseType);
            foreach (FieldInfo fi in c.GetType().GetFields())
            {
                System.Object obj = (System.Object)c;
                if(fi.GetValue(obj).GetType().Equals(typeof(System.Single)))
                    Debug.Log("field name " + fi.Name + " val " + fi.GetValue(obj));
            }

        }      
    }
}
