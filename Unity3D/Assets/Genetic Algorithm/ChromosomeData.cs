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
            foreach (FieldInfo fi in c.GetType().GetFields())
            {
                System.Object obj = (System.Object)c;
                if(fi.GetValue(obj).GetType().Equals(typeof(System.Single)))
                    Debug.Log("field name " + fi.Name + " val " + fi.GetValue(obj));
            }

        }      
    }

    public void GetCurrentGameObjectGenes()
    {
        if (gameObject == null)
            return;

        genes.Clear();
        Component[] cs = (Component[])gameObject.GetComponents(typeof(Component));
        foreach (Component c in cs)
        {
            foreach (FieldInfo fi in c.GetType().GetFields())
            {
                System.Object obj = (System.Object)c;
                if (fi.GetValue(obj).GetType().Equals(typeof(System.Single)))
                {
                    GeneData gd = new GeneData(c.GetType().Name + " - "+fi.Name, (float)fi.GetValue(obj));
                    genes.Add(gd);
                    Debug.Log("field name " + fi.Name + " val " + fi.GetValue(obj));
                }
            }

        }
    }

    /// <summary>
    /// Updates the genes of the gameobject. Will no function properly if the ordering of components is altered or genes have been removed/added.
    /// </summary>
    public void ApplyGenesToGameObject()
    {
        if (gameObject == null)
            return;

        int index = 0;

        Component[] cs = (Component[])gameObject.GetComponents(typeof(Component));
        foreach (Component c in cs)
        {
            foreach (FieldInfo fi in c.GetType().GetFields())
            {
                System.Object obj = (System.Object)c;
                if (fi.GetValue(obj).GetType().Equals(typeof(System.Single)))
                {
                    //if the retrieved value == the index of current one update its value.
                    if (genes[index].Equals(fi))
                    {
                        fi.SetValue(obj,genes[index].GetValue());
                    }


                    GeneData gd = new GeneData(c.GetType().Name + " - " + fi.Name, (float)fi.GetValue(obj));
                    genes.Add(gd);
                    Debug.Log("field name " + fi.Name + " val " + fi.GetValue(obj));
                }
            }

        }
    }
}
