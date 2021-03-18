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
    GameObject instance;


    /// <summary>
    /// Get's the genes of the currently set GameObject. Resets Genes to default values.
    /// </summary>
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
                if (fi.GetValue(c).GetType().Equals(typeof(System.Single)))
                {
                    GeneData gd = new GeneData(c.GetType().Name + " - "+fi.Name, (float)fi.GetValue(c));
                    genes.Add(gd);
                    Debug.Log("field name " + fi.Name + " val " + fi.GetValue(c));
                }
            }

        }
    }

    /// <summary>
    /// Update gameobject instance with genes retrieved from genetic algorithm.
    /// </summary>
    public void UpdateGenes(float[] _genes)
    {
        if (instance == null)
            return;

        int index = 0;

        for (int i = 0; i < genes.Count; i++)
        {
            
            if (genes[i].IsActive())
            {
                Debug.Log(genes[i].GetWeight());
                genes[i].SetWeight(_genes[index]);
                index++;
            }
        }
        if (index != _genes.Length)
            Debug.LogError(this + " has not updated all genes. Error has occured. Updated index = "+index+ ". Number of genes = "+_genes.Length);

        UpdateInstance();
    }

    public void GetGameObjectInstanceGenes()
    {
        if (instance == null)
            return;

        genes.Clear();
        Component[] cs = (Component[])instance.GetComponents(typeof(Component));
        foreach (Component c in cs)
        {
            foreach (FieldInfo fi in c.GetType().GetFields())
            {
                if (fi.GetValue(c).GetType().Equals(typeof(System.Single)))
                {
                    GeneData gd = new GeneData(c.GetType().Name + " - " + fi.Name, (float)fi.GetValue(c));
                    genes.Add(gd);
                    Debug.Log("field name " + fi.Name + " val " + fi.GetValue(c));
                }
            }

        }
    }

    /// <summary>
    /// Update Genetic Algorithm with set of genes to iterate on. (Initial Population)
    /// </summary>
    public void UpdateGeneticAlgorithm()
    {
        GeneticAlgorithm geneticAlgorithm = GameObject.FindObjectOfType<GeneticAlgorithm>();

        if (geneticAlgorithm != null)
        {
            //Get the genes of the current individual and apply them to the genetic algorithm.
            
            List<float> _genesList = new List<float>();
            for(int i = 0; i < genes.Count; i++)
            {
                if(genes[i].IsActive())
                    _genesList.Add(genes[i].GetWeight());               
            }
            float[] _genes = new float[_genesList.Count];
            _genes = _genesList.ToArray();
            Debug.Log("Setting Genetic Algorithm to " + gameObject.name);
            geneticAlgorithm.SetGenes(_genes);

            //find an instance of the gameobject in the scene and set it as the object to modify.
            geneticAlgorithm.SetIndividualToModify(gameObject);
            geneticAlgorithm.SetChromosomeData(this);
        }
        else
        {
            Debug.LogError("No existing Genetic Algorithm. Create one.");
        }
    }



    GameObject FindPrefabInstance(UnityEngine.Object myPrefab)
    {
        List<GameObject> result = new List<GameObject>();
        GameObject[] allObjects = (GameObject[])FindObjectsOfType(typeof(GameObject));
        foreach (GameObject GO in allObjects)
        {
            if (EditorUtility.GetPrefabType(GO) == PrefabType.PrefabInstance)
            {
                UnityEngine.Object GO_prefab = EditorUtility.GetPrefabParent(GO);
                if (myPrefab == GO_prefab)
                {
                    Debug.Log("Setting genetic algorithm instance of " + gameObject + " to: "+GO + ". If another instance is wanted manually set in Inspector on Genetic Algorithm Script.");
                    instance = GO;
                    Debug.Log("instance:" + instance);
                    return GO;
                }
            }
        }
        Debug.LogError("Could not find an instance of " + gameObject + " in scene to set for Genetic Algorithm. Add an instance to the scene and retry.");
        return null;
    }

    /// <summary>
    /// Updates the genes of the gameobject. Will not function properly if the ordering of components is altered or genes have been removed/added.
    /// </summary>
    public void ApplyGenesToGameObject()
    {
        UpdatePrefab();
        //UpdateInstance();
    }

    public void SetInstance(GameObject _instance)
    {
        instance = _instance;
    }
    /// <summary>
    /// Updates the already selected instance with the values from the Chromosome in Editor
    /// </summary>
    void UpdateInstance()
    {
        if (instance == null)
            return;

        int index = 0;
        Debug.Log("updating: " + instance);
        Component[] cs = (Component[])instance.GetComponents(typeof(Component));
        foreach (Component c in cs)
        {
            foreach (FieldInfo fi in c.GetType().GetFields())
            {
                Debug.Log("field variable: " + fi);
                if (!genes[index].IsActive())
                {
                    index++;
                }
                else if (fi.GetValue(c) != null && fi.GetValue(c).GetType().Equals(typeof(System.Single)))
                {
                    //if the retrieved value == the index of current one update its value.
                    if (genes[index].GetName().Equals(c.GetType().Name + " - " + fi.Name))
                    {
                        Debug.Log("Found gene, altering: " + genes[index].GetName() + " from " + fi.GetValue(c) + " to " + genes[index].GetValue());
                        fi.SetValue(c, genes[index].GetValue());

                        //Inform Unity that the instance has been modified
                        EditorUtility.SetDirty(c);
                        index++;
                    }
                }
                if (genes.Count <= index)
                    return;
            }
        }
    }


    /// <summary>
    /// Updates the already selected prefab with the values from the Chromosome in Editor
    /// </summary>
    void UpdatePrefab()
    {
        if (gameObject == null)
            return;

        int index = 0;

        Component[] cs = (Component[])gameObject.GetComponents(typeof(Component));
        foreach (Component c in cs)
        {
            foreach (FieldInfo fi in c.GetType().GetFields())
            {
                if (!genes[index].IsActive())
                {
                    index++;
                }
                else if (fi.GetValue(c).GetType().Equals(typeof(System.Single)))
                {
                    //if the retrieved value == the index of current one update its value.
                    if (genes[index].GetName().Equals(c.GetType().Name + " - " + fi.Name))
                    {
                        Debug.Log("Found gene, altering: " + genes[index].GetName() + " from " + fi.GetValue(c) + " to " + genes[index].GetValue());
                        fi.SetValue(c, genes[index].GetValue());

                        //Inform Unity that the instance has been modified
                        EditorUtility.SetDirty(c);
                        index++;
                    }
                }
                if (genes.Count <= index)
                    return;
            }
        }
    }

    /// <summary>
    /// Updates the genes of the gameobject. Will not function properly if the ordering of components is altered or genes have been removed/added.
    /// </summary>
    public void UpdateEditorWithPrefabGeneValues()
    {
        if (gameObject == null)
            return;

        int index = 0;

        Component[] cs = (Component[])gameObject.GetComponents(typeof(Component));
        foreach (Component c in cs)
        {
            foreach (FieldInfo fi in c.GetType().GetFields())
            {
                if (fi.GetValue(c).GetType().Equals(typeof(System.Single)))
                {
                    //if the retrieved value == the index of current one update its value.
                    if (genes[index].GetName().Equals(c.GetType().Name + " - " + fi.Name))
                    {
                        Debug.Log("Found gene, setting value in Editor: " + genes[index].GetName() + " from " + genes[index].GetValue()  + " to " + fi.GetValue(c));
                        genes[index].SetValue((float)fi.GetValue(c));
                        //inform Unity that the instance has been modified
                        EditorUtility.SetDirty(c);
                        index++;
                    }
                }
            }

        }
    }

    /// <summary>
    /// Updates the genes of the gameobject instance. Will not function properly if the ordering of components is altered or genes have been removed/added.
    /// </summary>
    public void UpdateEditorWithInstanceGeneValues()
    {
        if (instance == null)
            return;

        int index = 0;

        Component[] cs = (Component[])instance.GetComponents(typeof(Component));
        foreach (Component c in cs)
        {
            foreach (FieldInfo fi in c.GetType().GetFields())
            {
                if (fi.GetValue(c).GetType().Equals(typeof(System.Single)))
                {
                    //if the retrieved value == the index of current one update its value.
                    if (genes[index].GetName().Equals(c.GetType().Name + " - " + fi.Name))
                    {
                        Debug.Log("Found gene, setting value in Editor: " + genes[index].GetName() + " from " + genes[index].GetValue() + " to " + fi.GetValue(c));
                        genes[index].SetValue((float)fi.GetValue(c));
                        //inform Unity that the instance has been modified
                        EditorUtility.SetDirty(c);
                        index++;
                    }
                }
            }

        }
    }
}
