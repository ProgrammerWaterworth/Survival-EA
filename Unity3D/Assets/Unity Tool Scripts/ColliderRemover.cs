using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this to objects where you want to remove all colliders.
/// Copy object in playmode to obtain collider-less object permanently.
/// </summary>
public class ColliderRemover : MonoBehaviour
{
    void Start()
    {
        RemoveColliders();
    }

    /// <summary>
    /// Used for those tedious jobs of removing all those damn colliders.
    /// </summary>
    void RemoveColliders()
    {
        Collider[] _colliders = GetComponentsInChildren<Collider>();

        foreach (Collider _collider in _colliders)
        {
            Destroy(_collider);
        }
    }

}
