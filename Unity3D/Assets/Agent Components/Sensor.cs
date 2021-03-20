using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For Detecting information by simulating the vision of the agent.
/// </summary>
public class Sensor : MonoBehaviour
{
    public float viewDistance;
    public float viewAngle;

    [SerializeField] bool senseActive;
    [SerializeField] [Tooltip("The number of rays to cast out for detecting obstacles.")] int numViewRangeRays = 5;
    [SerializeField] [Tooltip("The magnitude of how the desired direction of movement steered in the opposite direction to detected obstacles")]
    [Range(0,1)]float avoidenceSensitivity;
    /// <summary>
    /// The desired direction of movement based on detected obstacles.
    /// </summary>
    Vector3 desiredDirection;

    public List<Transform> visibleInteractables = new List<Transform>();
    public List<Transform> visibleMemories = new List<Transform>();
    const float sensorUpdateRate = 0.25f;

    [SerializeField] [Tooltip("Mask for objects that block the users vision.")] LayerMask obstacleMask;
    [SerializeField] [Tooltip("Mask for objects the user wants to interact with.")] LayerMask interactableMask;
    [SerializeField] [Tooltip("Mask for objects representing memories.")] LayerMask memoryMask;

    void Start()
    {
        StartCoroutine(FindTargetsWithDelay(sensorUpdateRate));
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
    void DetectObstacles()
    {
        for (int i = 0; i < numViewRangeRays; i++)
        {
            RaycastHit _hit;
            float _angle = -(viewAngle / 2) + ((float)(i / numViewRangeRays) * viewAngle);
            Vector3 _direction = Quaternion.Euler(0, _angle, 0) * transform.forward;

            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, _direction, out _hit, viewDistance, obstacleMask))
            {
                
            }
        }
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
}
