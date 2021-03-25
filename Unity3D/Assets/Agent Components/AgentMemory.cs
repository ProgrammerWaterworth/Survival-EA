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
    [SerializeField] float memories, objectMemories;
    [SerializeField] [Tooltip("Show the debug information for processing memories.")] bool showDebugLog;

    const string memoryString = "-Memory"; //used to identify an object as a memory object.
    /// <summary>
    /// A memory represents the agents known information about a specific object type.
    /// </summary>
    class Memory
    {
        public GameObject gameObject;
        public float timeStamp;

        public Memory(GameObject _gameObject)
        {
            GameObject _memoryObject = new GameObject();
            _memoryObject.name = _gameObject.name + memoryString;
            _memoryObject.transform.position = _gameObject.transform.position;

            this.gameObject = _gameObject;
            this.timeStamp = Time.time;
        }
        

    }
    /// <summary>
    /// Dictionary for storing and retrieving objects and their memory
    /// </summary>
    Dictionary<string, Dictionary<GameObject,GameObject>> objectMemory = new Dictionary<string, Dictionary<GameObject, GameObject>>();
    /// <summary>
    /// Dictionary for storing and retrieving memory and their object
    /// </summary>
    Dictionary<string, Dictionary<GameObject, GameObject>> memoryOfObjects = new Dictionary<string, Dictionary<GameObject, GameObject>>();

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
        if(memoryOfObjects.Count > 0)
            memories = memoryOfObjects["Battery"].Count;
        if (objectMemory.Count > 0)
            objectMemories = objectMemory["Battery"].Count;
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
    public bool CheckMemoryForObject(string _objectName, Vector3 _agentPosition, out GameObject _targetGameObject, out GameObject _rememberedTarget)
    {
        bool _foundObject = false;
        _rememberedTarget = null;
        _targetGameObject = null;

        if (memoryOfObjects.ContainsKey(_objectName))
        {
            if (memoryOfObjects[_objectName].Count > 0) //Has at least 1 memory of object stored.
            {
                float closestDistance = Mathf.Infinity;
                _foundObject = true;

                foreach (KeyValuePair<GameObject, GameObject> _pair in memoryOfObjects[_objectName])
                {
                    if (Vector3.Distance(_pair.Key.transform.position, _agentPosition) < closestDistance)
                    {
                        closestDistance = Vector3.Distance(_pair.Key.transform.position, _agentPosition);
                        _targetGameObject = _pair.Value;
                        _rememberedTarget = _pair.Key;
                    }
                }
            }
        }
        return _foundObject;
    }

    //Memory check for when an object isn't present which it is expecting. Using name of expected object.

    public void RemoveObjectFromMemory(GameObject _memoryObject)
    {
        string objectName = _memoryObject.name.Substring(0, _memoryObject.name.Length - memoryString.Length);
        if (memoryOfObjects.ContainsKey(objectName))
        {
            if (memoryOfObjects[objectName].Count > 0) //Has at least 1 memory of object stored.
            {
                if (memoryOfObjects[objectName].ContainsKey(_memoryObject))
                {
                    GameObject obj = memoryOfObjects[objectName][_memoryObject];
                    Debug.Log(this + " is removing " + memoryOfObjects[objectName][_memoryObject]);

                    //remove _memoryObject's value in pair
                    objectMemory[objectName].Remove(obj);
                    memoryOfObjects[objectName].Remove(_memoryObject);
                    Destroy(_memoryObject); //destory real world memory representation
                }
                else Debug.LogError(this + " cannot find object in memories: " + objectName);
            }
            else Debug.LogError(this + " currently has no memories of type: " + objectName);
        }
        else Debug.LogError(this + " hasn't recorded memories of this type: " + objectName);
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

            //Update actual objects in memory - when the agent encounters an object and wants to add or update the current memory of that object
            foreach(Transform _informationTransform in sensor.visibleInteractables)
            {
                if(_informationTransform!=null)
                    UpdateObjectInMemory(_informationTransform.gameObject);
            }
            //Update memories revisitted - when agent visits a point in which it has a memory it updates with new relevant info
            foreach (Transform _informationTransform in sensor.visibleMemories)
            {
                if (_informationTransform != null)
                    UpdateMemoryOfObject(_informationTransform.gameObject);
            }
        }
    }

    /// <summary>
    /// Adds a memory to current knowledge base.
    /// </summary>
    void AddMemory(GameObject _keyObject)
    {
        //Memory _memory = new Memory(_keyObject);
        
        if (!memoryOfObjects.ContainsKey(_keyObject.name))
        {
            //add to both dictionaries.
            memoryOfObjects.Add(_keyObject.name, new Dictionary<GameObject, GameObject>());
            objectMemory.Add(_keyObject.name, new Dictionary<GameObject, GameObject>());
        }
        GameObject memoryObj = CreateMemoryObject(_keyObject);
        memoryOfObjects[_keyObject.name].Add(memoryObj, _keyObject);
        objectMemory[_keyObject.name].Add(_keyObject, memoryObj);

        if (showDebugLog)
            Debug.Log(this + " is adding memory: " + objectMemory[_keyObject.name][_keyObject].gameObject);
    }

    /// <summary>
    /// Updates current memories of object or adds new ones based on the _keyObject found.
    /// </summary>
    /// <param name="_actualObject">The object from the scene which a memory needs to be formed from.</param>
    void UpdateObjectInMemory(GameObject _actualObject)
    {
        //Updates memory if a known object if spotted in another location.
        if (objectMemory.ContainsKey(_actualObject.name) && objectMemory[_actualObject.name].ContainsKey(_actualObject))
        {
            GameObject memoryObj = objectMemory[_actualObject.name][_actualObject];
            memoryObj.transform.position = _actualObject.transform.position;
           // memoryOfObjects[memoryName][memoryObj].transform.position = _actualObject.transform.position; //should update as its a reference to the actual object.
        }
        else  // Not a memory, make one.
            AddMemory(_actualObject);      
    }

    GameObject CreateMemoryObject(GameObject _actualObject)
    {
        GameObject _memoryObject = new GameObject(_actualObject.name + memoryString);
        _memoryObject.transform.position = _actualObject.transform.position;
        return _memoryObject;
    }

    /// <summary>
    /// Updates current memories of object if observing memory is no longer near object.
    /// </summary>
    /// <param name="_memoryObject">An existing memory to update.</param>
    void UpdateMemoryOfObject(GameObject _memoryObject)
    {
        string objectName = _memoryObject.name.Substring(0, _memoryObject.name.Length - memoryString.Length);

        //Updates memory of gameObject by 
        if (memoryOfObjects.ContainsKey(objectName) && memoryOfObjects[objectName].ContainsKey(_memoryObject))
        {
            //remove ?
            if (Vector3.Distance(  memoryOfObjects[objectName][_memoryObject].transform.position, _memoryObject.transform.position) > 1)
            {
                RemoveObjectFromMemory(_memoryObject);
            }          
        }
        //otherwise remove object?
    }
}
