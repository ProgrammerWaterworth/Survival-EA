using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Sensor))]
public class SensorEditor : Editor
{
   void OnSceneGUI()
    {
        Sensor _view = (Sensor) target;
        Handles.color = Color.grey;
        Handles.DrawWireArc(_view.transform.position, Vector3.up, Vector3.forward, 360,_view.viewDistance);
        Vector3 viewAngleA = _view.DirFromAngle(-_view.viewAngle / 2, false);
        Vector3 viewAngleB = _view.DirFromAngle(_view.viewAngle / 2, false);
        Handles.color = Color.white;
        Handles.DrawWireArc(_view.transform.position, Vector3.up, viewAngleA, _view.viewAngle, _view.viewDistance);
        Handles.DrawLine(_view.transform.position, _view.transform.position + viewAngleA * _view.viewDistance);
        Handles.DrawLine(_view.transform.position, _view.transform.position + viewAngleB * _view.viewDistance);

        Handles.color = Color.green;

        foreach (Transform _target in _view.visibleInteractables)
        {
            Handles.DrawLine(_view.transform.position, _target.transform.position);
        }
    }
}
