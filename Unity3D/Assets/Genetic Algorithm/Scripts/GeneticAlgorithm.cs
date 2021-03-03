using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    public static GeneticAlgorithm Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    [SerializeField] TestAgent agent;
    [SerializeField] GameObject individual;
    [SerializeField] ChromosomeData data;
    /// <summary>
    /// The weights of the genes that dictate the agents behaviour.
    /// </summary>
    [SerializeField] float[] genes;

    /// <summary>
    /// Port to Python Implementation of Genetic Algorithm. Feed it a fitness and it will return chromosome weights.
    /// </summary>
    InformationPort port;
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<InformationPort>() != null)
        {
            port = GetComponent<InformationPort>();
        }
        else Debug.LogError(this + " requires an information port component");

        StartCoroutine(DelayedFitnessReport());
    }

    public void SetGenes(float[] _genes)
    {
        genes = _genes;
    }

    public void SetIndividualToModify(GameObject prefab)
    {
        individual = prefab;
    }

    public void SetChromosomeData(ChromosomeData _data)
    {
        data = _data;
    }

    /// <summary>
    /// Calculate fitness from agents ability to survive.
    /// </summary>
    float CalculateFitness()
    {
        float _fitness = 0;

        if (agent != null) //Debug #2 - Testing on agent
        {
            //agent.SetRangeGene(genes[0]);
            if (data != null)
            {
                data.UpdateGenes(genes);
                //data.UpdateEditorWithInstanceGeneValues();
            }
            int _count = agent.CheckForBatteries();
            _fitness = _count +1;
        }
        else //Debug #1 - Testing if fitness affects ga
        {
            for (int i = 0; i < genes.Length; i++)
            {
                _fitness = genes[i];
            }
            _fitness /= genes.Length;
        }
        return _fitness;
    }

    IEnumerator DelayedFitnessReport()
    {
        if (port != null)
        {
            port.SetDataOut(genes);
        }


        while (true)
        {
            yield return new WaitForSeconds(.1f);
            SendFitness();
        }
    }

    /// <summary>
    /// Sends fitness 
    /// </summary>
    void SendFitness()
    {
        Debug.Log(port);
        if (!port.GetArray().Equals(genes))
        {
            genes = port.GetArray();

            float[] _fitness = new float[1];
            _fitness[0] = CalculateFitness();
            port.SetDataOut(_fitness);
        }
        else
            Debug.Log("genes are still the same...");
    }
}
