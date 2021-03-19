using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneticAlgorithm : MonoBehaviour
{
    public enum GeneticAlgorithmState
    {
        WaitingForGenes,
        WaitingForFitness,
        FitnessFunctionNotPresent,
        ResettingScene
    }

    [SerializeField]GeneticAlgorithmState state;

    public static GeneticAlgorithm Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    [SerializeField] GameObject prefab;
    [SerializeField] GameObject individual;
    IFitnessFunction individualFitnessFunction;
    [SerializeField] ChromosomeData data;
    [SerializeField] Transform spawnPoint;
    /// <summary>
    /// The weights of the genes that dictate the agents behaviour.
    /// </summary>
    [SerializeField] float[] genes;
    AsyncOperation operation;
    /// <summary>
    /// Port to Python Implementation of Genetic Algorithm. Feed it a fitness and it will return chromosome weights.
    /// </summary>
    InformationPort port;
    // Start is called before the first frame update
    void Start()
    {
        SetIndividual();
        if (GetComponent<InformationPort>() != null)
        {
            port = GetComponent<InformationPort>();
        }
        else Debug.LogError(this + " requires an information port component");
        if(CheckIndividualFitnessFunction())
            StartCoroutine(DelayedFitnessReport());
        else
            Debug.LogError(this + " requires the indivual: "+ individual + " to implement IFitnessFunction Interface in one of it's scripts.");
    }

    public void SetGenes(float[] _genes)
    {
        genes = _genes;
    }

    public void SetIndividualToModify(GameObject _prefab)
    {
        prefab = _prefab;
    }

    public void SetChromosomeData(ChromosomeData _data)
    {
        data = _data;
    }

    void SetIndividual()
    {
        if (prefab != null)
        {
            if(spawnPoint!=null)
                individual = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            else
                individual = Instantiate(prefab, transform.position, Quaternion.identity);
            data.SetInstance(individual);
        }
        else Debug.LogError(this+" prefab is null");
    }

    /// <summary>
    /// Calculate fitness from agents ability to survive.
    /// </summary>
    float CalculateFitness()
    {
        float _fitness = 0;

        if (individual != null) //Debug #2 - Testing on agent
        {      
            _fitness = individualFitnessFunction.GetFitness();
            if (_fitness == 0)
                _fitness = 0.01f;
        }
        else Debug.LogError(this + " does not have an individual assigned.");
        return _fitness;
    }

    /// <summary>
    /// Checks if the individual that is being attempted to test implements fitness function.
    /// </summary>
    /// <returns></returns>
    bool CheckIndividualFitnessFunction()
    {
        individualFitnessFunction = individual.GetComponent<IFitnessFunction>();

        if (individualFitnessFunction != null)
        {
            return true;
        }
        return false;
    }

    IEnumerator DelayedFitnessReport()
    {
        if (port != null)
        {
            Debug.Log("genes:"+genes.Length);
            port.SetDataOut(genes);
        }


        while (true)
        {          
            if (state == GeneticAlgorithmState.WaitingForGenes)
            {
                RecieveGenes();              
            }
            else if (state == GeneticAlgorithmState.WaitingForFitness)
            {
                WaitForFitness();
            }
            else if (state == GeneticAlgorithmState.ResettingScene)
            {
                RestartSimulation();
            }
            
            yield return new WaitForSeconds(.1f);
        }
    }

    /// <summary>
    /// Sends fitness 
    /// </summary>
    void RecieveGenes()
    {
        Debug.Log(port);
        Debug.Log("Receiving");

        //Get new genes + apply to individual.            
        genes = port.GetArray();

        if (data != null && port.GetArray().Length == data.NumberOfGenes())
        {
            data.UpdateGenes(genes);
            SwitchState(GeneticAlgorithmState.ResettingScene);

            operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
        else Debug.LogError(this + " data is == "+ data + " and incoming num genes == "+ port.GetArray().Length +". Actual genes == "+ data.NumberOfGenes() + ". Cannot update genes.");
        /*
        if (!port.GetArray().Equals(genes))
        {
                                  
        }
        else
            Debug.Log("genes are still the same...");*/
    }

    void WaitForFitness()
    {
        if (individual != null && CheckIndividualFitnessFunction() &&  individualFitnessFunction.IsEvalutionComplete())
        {
            Debug.Log("Sending");
            SwitchState(GeneticAlgorithmState.WaitingForGenes);
            //Get fitness from running surivival.
            float[] _fitness = new float[1];
            _fitness[0] = CalculateFitness();
            port.SetDataOut(_fitness);
        }      
    }

    void RestartSimulation()
    {
        if (operation!=null && operation.isDone)
        {
            SetIndividual();
            SwitchState(GeneticAlgorithmState.WaitingForFitness);
        }
    }

    /// <summary>
    /// Switch State of the Genetic algorothm.
    /// </summary>
    /// <param name="_newState"></param>
    void SwitchState(GeneticAlgorithmState _newState)
    {
        state = _newState;
    }
}
