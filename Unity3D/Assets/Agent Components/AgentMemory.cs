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
    /// Check for obstacles at current time and report back information.
    /// </summary>
    /// <param name="_memoryPosition"></param>
    /// <returns></returns>
    public bool CheckForObstacles()
    {
        if (sensor != null)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// Checks if there is a memory of an object with the name _objectName. Can return True even if object is null as memory of it at that position depicts it being there.
    /// </summary>
    /// <param name="_objectName">object to search for.</param>
    /// <returns>True if it has a memory of an object.</returns>
    public bool CheckMemoryForObject(string _objectName, Vector3 _agentPosition, out GameObject _object, out Vector3 _memoryPosition)
    {
        bool _foundObject = false;
        _memoryPosition = Vector3.zero;
        _object = null;

        if (agentMemories.ContainsKey(_objectName))
        {
            if (agentMemories[_objectName].Count > 0) //Has at least 1 memory of object stored.
            {
                float closestDistance = Mathf.Infinity;
                _foundObject = true;

                foreach (KeyValuePair<GameObject, Memory> _pair in agentMemories[_objectName])
                {
                    if (Vector3.Distance(_pair.Value.worldPosition, _agentPosition) < closestDistance)
                    {
                        closestDistance = Vector3.Distance(_pair.Value.worldPosition, _agentPosition);
                        _object = _pair.Value.gameObject;
                        _memoryPosition = _pair.Value.worldPosition;
                    }
                }
            }
        }
        return _foundObject;
    }

    public void RemoveObjectFromMemory(GameObject _object)
    {
        if (agentMemories.ContainsKey(_object.name))
        {
            if (agentMemories[_object.name].Count > 0) //Has at least 1 memory of object stored.
            {
                if (agentMemories[_object.name].ContainsKey(_object))
                {
                    Debug.Log(this + " is removing " + agentMemories[_object.name][_object]);
                    agentMemories[_object.name].Remove(_object);
                }
                else Debug.LogError(this + " cannot find object in memories: " + _object.name);
            }
            else Debug.LogError(this + " currently has no memories of type: " + _object.name);

        }
        else Debug.LogError(this + " hasn't recorded memories of this type: " + _object.name);
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
                if(_informationTransform!=null)
                    UpdateMemory(_informationTransform.gameObject);
            }
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

        if (showDebugLog)
            Debug.Log(this + " is adding memory: " + agentMemories[_keyObject.name][_keyObject].gameObject);
    }

    /// <summary>
    /// Updates current memories or adds new ones based on the _keyObject found.
    /// </summary>
    /// <param name="_keyObject"></param>
    void UpdateMemory(GameObject _keyObject)
    {
        if (agentMemories.ContainsKey(_keyObject.name) && agentMemories[_keyObject.name].ContainsKey(_keyObject))
        {          
            Memory _memory = agentMemories[_keyObject.name][_keyObject];
            _memory.worldPosition = _keyObject.transform.position;
            _memory.timeStamp = Time.time;           
        }
        else  // Not a memory, make one.
            AddMemory(_keyObject);
        
    }
}
