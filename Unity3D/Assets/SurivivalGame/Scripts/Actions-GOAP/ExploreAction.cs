using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExploreAction : GoapAction
{

    public enum ExploreType
    {
        Wandering,
        NewArea
    }

    bool explored;
    float searchTurn; //dir + mag of turn when searching
    private float startTime = 0;
    Vector3 steeringDirection;

    [Header("Movement")]
    public int actionType;
    [SerializeField] ExploreType exploreType;
    [SerializeField] float exploreSteerWeight;
    [SerializeField] float exploreSteerSubWeight;
    [SerializeField] float avoidanceSteerWeight;
    [SerializeField] float movementDistanceIncrements;

    [Header("Search")]
    float searchTime;
    [SerializeField][Tooltip("Used to evaluate search time producing different search times.")] AnimationCurve searchTimeCurve;
    [SerializeField] float maxSearchTime;
    [SerializeField][Range(1,10f)] float searchRotationMagnitude;
    [SerializeField][Range(0,1f)] float percentageSearchChance;

    public ExploreAction()
    {
        AddEffect("explored", true);
    }

    private void Start()
    {
        targetObjectName = "ExplorePoint";
    }

    private void OnValidate()
    {
        //When action type changes update explore type.       
        int num = System.Enum.GetNames(typeof(ExploreType)).Length;
        exploreType = (ExploreType)(actionType % num);
    }

    public override void ResetAction()
    {
        inRange = false;
        explored = false;
        startTime = 0;
    }

    public override bool IsDone()
    {
        return explored;
    }

    public override bool RequiresInRange()
    {
        return true; // yes we need to be near the destination point
    }


    protected override bool FindTargetObject(GameObject _agent)
    {
        //Needs to create a destiantion point.
        if (target == null)
        {
            GameObject _destinationPoint = new GameObject(targetObjectName);
            //Get a point that's on the navmesh.

            if (GetComponent<AgentMemory>() != null)
                _destinationPoint.transform.parent = GetComponent<AgentMemory>().GetMemoryHolderTransform();


            _destinationPoint.transform.position = GetPointToMoveTo();
            NavMeshHit hit;

            if (NavMesh.SamplePosition(_destinationPoint.transform.position, out hit, movementDistanceIncrements*1.5f, NavMesh.AllAreas))
                _destinationPoint.transform.position = hit.position;
            else
                Debug.LogWarning(this + " could not find a point on navmesh.");

            //Set search time.
            if (Random.value < percentageSearchChance)
            {
                searchTime = maxSearchTime * searchTimeCurve.Evaluate(Random.Range(0.0f, 1.0f));
                searchTurn = (Random.value - .5f) * 2; //gets float between -1f <-> 1f
            }
            else
                searchTime = 0;
            

            target = _destinationPoint;
            memoryTarget = target;
        }
        return true;
    }

    public override bool ExecuteAction(GameObject agent)
    {
        if (memoryTarget != null && target != null)
        {
            if (startTime == 0)
                startTime = Time.time;

            //look around for a bit
            if (Time.time - startTime < searchTime)
            {
                transform.rotation = Quaternion.RotateTowards(Quaternion.LookRotation(transform.forward, transform.up), Quaternion.LookRotation(transform.right, transform.up), searchTurn*searchRotationMagnitude);
               
            }
            else
            {
                Destroy(target);
                explored = true;
                
            }
            return true;
        }
        return false;          
    }
    /// <summary>
    /// Get a world position to move to.
    /// </summary>
    Vector3 GetPointToMoveTo()
    {
        //ensure point is on navmesh
        Vector3 _point = transform.position;
        Vector3 _direction = transform.forward;
        //apply random steering to direction
        steeringDirection += (exploreSteerSubWeight * new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)));

        if (exploreType == ExploreType.NewArea)
        {
            if (GetComponent<AgentMemory>() != null)
            {
                steeringDirection += (exploreSteerSubWeight * GetComponent<AgentMemory>().GetUncommonDirection());
            }
        }
       
        _direction += (exploreSteerWeight * steeringDirection);
        _direction = _direction.normalized;
        //apply obstacle detection change to direction.
        if (GetComponent<AgentMemory>()!=null)
        {
            steeringDirection = (GetComponent<AgentMemory>().GetReactiveDirection());
            _direction += steeringDirection * avoidanceSteerWeight; //ensures steering follows on from avoid objects and doesnt try turn back on itself when avoidance is 0
            _direction = _direction.normalized;
            _direction = _direction * movementDistanceIncrements;
        }
        Debug.DrawLine(_point, _point + _direction, Color.green);
        _point += _direction;


        return _point;
    }
}
