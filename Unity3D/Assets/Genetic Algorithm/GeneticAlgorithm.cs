using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    [SerializeField] TestAgent agent;
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

    /// <summary>
    /// Calculate fitness from agents ability to survive.
    /// </summary>
    float CalculateFitness()
    {
        float _fitness = 0;

        if (agent != null) //Debug#2 - Testing on agent
        {
            agent.SetRangeGene(genes[0]);
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
        if (!port.GetArray().Equals(genes))
        {
            genes = port.GetArray();
            float _fitness = CalculateFitness();
            Debug.Log("fitness: " + _fitness);
            port.SetDataOut(_fitness);
        }
        else
            Debug.Log("genes are still the same...");
    }
}
