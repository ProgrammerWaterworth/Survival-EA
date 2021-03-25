using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExploreAction : GoapAction
{
    bool explored;
    Vector3 steeringDirection;
    [SerializeField] float exploreSteerWeight;
    [SerializeField] float exploreSteerSubWeight;
    [SerializeField] float avoidanceSteerWeight;
    [SerializeField] float movementDistanceIncrements;

    public ExploreAction()
    {
        AddEffect("explored", true);
    }


    public override void ResetVariables()
    {
        explored = false;
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
            GameObject _destinationPoint = new GameObject("ExplorePoint");
            //Get a point that's on the navmesh.

            _destinationPoint.transform.position = GetPointToMoveTo();
            NavMeshHit hit;

            if (NavMesh.SamplePosition(_destinationPoint.transform.position, out hit, movementDistanceIncrements*1.5f, NavMesh.AllAreas))
                _destinationPoint.transform.position = hit.position;
            else
                Debug.LogWarning(this + " could not find a point on navmesh.");
            target = _destinationPoint;
            memoryTarget = target;
        }
        return true;
    }

    public override bool ExecuteAction(GameObject agent)
    {
        //look around for a bit
       

        if (memoryTarget != null && target != null)
        {
            Destroy(target);
            explored = true;
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
        steeringDirection +=  (exploreSteerSubWeight* new Vector3(Random.Range(-1f, 1f),0, Random.Range(-1f, 1f)));
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
