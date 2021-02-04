using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages what an agent has observed in it's lifetime.
/// </summary>

[RequireComponent(typeof(Sensor))]
public class AgentMemory : MonoBehaviour
{
    Sensor sensor;

    [SerializeField] [Tooltip("Show the debug information for processing memories.")] bool showDebugLog;

    /// <summary>
    /// A memory represents the agents known information about a specific object type.
    /// </summary>
    class Memory
    {
        public Vector3 worldPosition;
        public GameObject gameObject;
        public float timeStamp;

        public Memory(GameObject _gameObject)
        {
            this.worldPosition = _gameObject.transform.position;
            this.gameObject = _gameObject;
            this.timeStamp = Time.time;
        }  
    }
    /// <summary>
    /// Dictionary for storing and retrieving memories.
    /// </summary>
    Dictionary<string, Dictionary<GameObject,Memory>> agentMemories = new Dictionary<string, Dictionary<GameObject, Memory>>();

    // Key objects need to be remembered by timestamp, object type and location.
    // Start is called before the first frame update
    void Start()
    {
        SetUpMemoryComponents();
    }

    // Update is called once per frame
    void Update()
    {
        GetWorldInformation();
    }

    /// <summary>
    /// Set up the components required for the memory to function.
    /// </summary>
    void SetUpMemoryComponents()
    {
        if (GetComponent<Sensor>() != null)
        {
            sensor = GetComponent<Sensor>();
        }
        else Debug.LogError(this+" does not have a sensor component to detect new information.");
    }

    /// <summary>
    /// Checks if there is a memory of an object with the name _objectName.
    /// </summary>
    /// <param name="_objectName">object to search for.</param>
    /// <returns>Closest object by memory recollection, otherwise null.</returns>
    public GameObject CheckMemoryForObject(string _objectName, Vector3 _agentPosition)
    {
        if(agentMemories.ContainsKey(_objectName))
        {
            if(agentMemories[_objectName].Count > 0)
            {
                GameObject closestObject = null;
                float closestDistance = Mathf.Infinity;

                foreach(KeyValuePair<GameObject,Memory> _pair in agentMemories[_objectName])
                {
                    if(Vector3.Distance( _pair.Value.worldPosition, _agentPosition) < closestDistance)
                    {
                        closestDistance = Vector3.Distance(_pair.Value.worldPosition, _agentPosition);
                        closestObject = _pair.Value.gameObject;
                    }
                }

                return closestObject;
            }
        }
        return null;
    }

    /// <summary>
    /// Use a sensor to retrieve information from the world and store it as a memory.
    /// </summary>
    void GetWorldInformation()
    {
        if (sensor != null)
        {
            if (sensor.visibleInteractables == null)
                return;

            foreach(Transform _informationTransform in sensor.visibleInteractables)
            {
                UpdateMemory(_informationTransform.gameObject);
            }
            if(showDebugLog)
                Debug.Log(this + " types of memory: " + agentMemories.Count);
        }
    }

    /// <summary>
    /// Adds a memory to current knowledge base.
    /// </summary>
    void AddMemory(GameObject _keyObject)
    {
        Memory _memory = new Memory(_keyObject);
        
        if (!agentMemories.ContainsKey(_keyObject.name))
        {
            agentMemories.Add(_keyObject.name, new Dictionary<GameObject, Memory>());
        }   

        agentMemories[_keyObject.name].Add(_keyObject, _memory);       
    }

    /// <summary>
    /// Updates current memories or adds new ones based on the _keyObject found.
    /// </summary>
    /// <param name="_keyObject"></param>
    void UpdateMemory(GameObject _keyObject)
    {
        if (agentMemories.ContainsKey(_keyObject.name) && agentMemories[_keyObject.name].ContainsKey(_keyObject))
        {
            if(showDebugLog)
                Debug.Log(this + " is updating existing memory: " + agentMemories[_keyObject.name][_keyObject]);

            Memory _memory = agentMemories[_keyObject.name][_keyObject];
            _memory.worldPosition = _keyObject.transform.position;
            _memory.timeStamp = Time.time;           
        }
        else  // Not a memory, make one.
            AddMemory(_keyObject);
        
    }

    /// <summary>
    /// Removes a memory from knowledge base.
    /// </summary>
    void RemoveMemory()
    {
     
    }
}
