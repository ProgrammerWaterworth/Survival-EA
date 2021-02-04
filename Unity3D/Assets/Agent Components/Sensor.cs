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

     public List<Transform> visibleInteractables = new List<Transform>();

    const float sensorUpdateRate = 0.25f;

    [SerializeField] [Tooltip("Mask for objects that block the users vision.")] LayerMask obstacleMask;
    [SerializeField] [Tooltip("Mask for objects the user wants to interact with.")] LayerMask interactableMask;

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

        for(int i =0; i < _targetsInRange.Length; i++)
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

    IEnumerator FindTargetsWithDelay(float _delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(_delay);
            FindVisibleInteractables();
        }
    }
}
