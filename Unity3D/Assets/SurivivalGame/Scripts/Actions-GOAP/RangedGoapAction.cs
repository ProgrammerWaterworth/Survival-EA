using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedGoapAction : GoapAction
{
    [SerializeField] float range;

    public override bool IsDone()
    {
        throw new System.NotImplementedException();
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public float GetRange()
    {
        return range;
    }
}
