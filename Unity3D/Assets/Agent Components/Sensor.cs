using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For Detecting information by simulating the vision of the agent.
/// </summary>
public class Sensor : MonoBehaviour
{
    public float viewDistance, maxViewDistance, minViewDistance, avoidanceDistance;
    public float viewAngle;

    [SerializeField] bool senseActive;
    [SerializeField] [Tooltip("The number of rays to cast out for detecting obstacles.")] int numViewRangeRays = 5;

    [Header("Visual Representation")]
    [SerializeField] Light spotLight;

    /// <summary>
    /// The desired direction of movement based on detected obstacles.
    /// </summary>
    Vector3 desiredDirection;

    public List<Transform> visibleInteractables = new List<Transform>();
    public List<Transform> visibleMemories = new List<Transform>();
    const float sensorUpdateRate = 0.25f;
    Vector3 avoidanceMovementDirection, seekingMovementDirection, seekSteerRandomiser;
    [SerializeField] [Tooltip("The magnitude of how much the randomiser steer vector is affected each update.")] float seekSteerMagnitude;
    [SerializeField] [Tooltip("The magnitude of how much each of the seek behaviours are affected by the randomiser steering direction.")] float seekRandomiserMagnitude;
    [SerializeField] AnimationCurve avoidanceOverDistance;

    [SerializeField] [Tooltip("Mask for objects that block the users vision.")] LayerMask obstacleMask;
    [SerializeField] [Tooltip("Mask for objects the user wants to interact with.")] LayerMask interactableMask;
    [SerializeField] [Tooltip("Mask for objects representing memories.")] LayerMask memoryMask;

    void Start()
    {
        StartCoroutine(FindTargetsWithDelay(sensorUpdateRate));
    }

    private void Update()
    {
        UpdateObstacleAvoidanceDirection();
        UpdateObstacleSeekDirection();
    }

    public void SetViewDistancePercentage(float _percentage)
    {
        viewDistance = Mathf.Lerp(minViewDistance, maxViewDistance, _percentage);
        UpdateLightRange();
    }

    /// <summary>
    /// Converts an angle into a direction relative to this gameobjects transform.
    /// </summary>
    /// <param name="_angle">Angle is in degrees.</param>
    /// <returns></returns>
    public Vector3 DirFromAngle(float _angle, bool _globalAngle)
    {
        if (!_globalAngle)
            _angle += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(_angle*Mathf.Deg2Rad),0,Mathf.Cos(_angle * Mathf.Deg2Rad));
    }

    /// <summary>
    /// Updates visible target list with visible objects on the interactable mask layer.
    /// </summary>
    void FindVisibleInteractables()
    {
        visibleInteractables.Clear();

        Collider[] _targetsInRange = Physics.OverlapSphere(transform.position, viewDistance, interactableMask);

        for(int i = 0; i < _targetsInRange.Length; i++)
        {
            Transform _target = _targetsInRange[i].transform;
            Vector3 _targetDirection = (_target.position - transform.position).normalized;

            
            if (Vector3.Angle(transform.forward, _targetDirection) < viewAngle / 2)
            {
                float _targetDistance = Vector3.Distance(_target.position, transform.position);

                if(!Physics.Raycast(transform.position, _targetDirection, _targetDistance, obstacleMask))
                {
                    visibleInteractables.Add(_target);
                }
            }
        }
    }

    /// <summary>
    /// Updates visible target list with visible objects on the interactable mask layer.
    /// </summary>
    void FindMemoriesInView()
    {
        visibleMemories.Clear();

        Collider[] _targetsInRange = Physics.OverlapSphere(transform.position, viewDistance, memoryMask);

        for (int i = 0; i < _targetsInRange.Length; i++)
        {
            Transform _target = _targetsInRange[i].transform;
            Vector3 _targetDirection = (_target.position - transform.position).normalized;


            if (Vector3.Angle(transform.forward, _targetDirection) < viewAngle / 2)
            {
                float _targetDistance = Vector3.Distance(_target.position, transform.position);

                if (!Physics.Raycast(transform.position, _targetDirection, _targetDistance, obstacleMask))
                {
                    visibleMemories.Add(_target);
                }
            }
        }
    }

    /// <summary>
    /// Raycast in view range to see what obstacles the agent is facing and update desired direction of movement.
    /// </summary>
    void UpdateObstacleAvoidanceDirection()
    {
        avoidanceMovementDirection = Vector3.zero;
        //Accumulate the detection ray values 
        for (int i = 0; i < numViewRangeRays; i++)
        {
            RaycastHit _hit;
            float _angle = -(viewAngle / 2) + (((float)i / (float)numViewRangeRays) * viewAngle);
            Vector3 _direction = Quaternion.Euler(0, _angle, 0) * transform.forward;
            Debug.DrawLine(transform.position, transform.position + _direction * 5, Color.cyan);
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, _direction, out _hit, viewDistance, obstacleMask))
            {
                Vector3 _dir = -_direction.normalized * (avoidanceOverDistance.Evaluate(1 - (_hit.distance / Mathf.Min(avoidanceDistance,viewDistance))));
                Debug.DrawLine(transform.position, transform.position + _dir * 7, Color.yellow);
                avoidanceMovementDirection += _dir;
            }
        }
        Debug.DrawLine(transform.position, transform.position + avoidanceMovementDirection*6, Color.red);
    }

    /// <summary>
    /// Raycast in view range to see what obstacles the agent is facing and update desired direction of movement.
    /// </summary>
    void UpdateObstacleSeekDirection()
    {
        seekingMovementDirection = Vector3.zero;
        float _maxRayDis = 0;

        seekSteerRandomiser += new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)) * seekSteerMagnitude;
        seekSteerRandomiser = seekSteerRandomiser.normalized;

        //Accumulate the detection ray values 
        for (int i = 0; i < numViewRangeRays; i++)
        {
            RaycastHit _hit;
            float _angle = -(viewAngle / 2) + (((float)i / (float)numViewRangeRays) * viewAngle);
            Vector3 _direction = Quaternion.Euler(0, _angle, 0) * transform.forward;
            Debug.DrawLine(transform.position, transform.position + _direction , Color.cyan);
            Debug.DrawLine(transform.position + _direction, transform.position + _direction +(seekSteerRandomiser *seekRandomiserMagnitude), Color.white);
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, _direction, out _hit, Mathf.Infinity, obstacleMask))
            {
                Vector3 _dir = _direction.normalized;

                if ((_dir +(seekSteerRandomiser * seekRandomiserMagnitude)).magnitude > _maxRayDis)
                {
                    _maxRayDis = (_dir + (seekSteerRandomiser * seekRandomiserMagnitude)).magnitude;
                    seekingMovementDirection = _dir;
                }
            }
            
        }
        Debug.DrawLine(transform.position, transform.position + (seekingMovementDirection *4), Color.blue);
    }

    /// <summary>
    /// Returns a direction vector which indicates the best direction to move based on detected obstacles.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetObstacleAvoidanceDirection()
    {
        return avoidanceMovementDirection;
    }

    public Vector3 GetSeekDirection()
    {
        return seekingMovementDirection;
    }


    IEnumerator FindTargetsWithDelay(float _delay)
    {
        //indefinite loop
        while (true)
        {
            yield return new WaitForSeconds(_delay);
            FindVisibleInteractables();
            FindMemoriesInView();
        }
    }

    void UpdateLightRange()
    {
        if (spotLight != null)
        {
           float angle =  Mathf.Rad2Deg* Mathf.Atan(viewDistance / spotLight.transform.position.y);
            spotLight.spotAngle = angle * 2;
        }
    }
}
