using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    private void OnDestroy()
    {
        if (GetComponentInParent<FruitTree>() != null)
        {
            GetComponentInParent<FruitTree>().RemoveFruit(this);
        }
    }
}
