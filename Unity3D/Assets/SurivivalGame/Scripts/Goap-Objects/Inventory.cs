using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //Player stats


    float charge;
    [SerializeField] float maxCharge;
    public float chargeUsageRate = 5f;

    // Start is called before the first frame update
    void Start()
    {
        charge = maxCharge;
    }

    public float GetMaxCharge()
    {
        return maxCharge;
    }

    // Update is called once per frame
    void Update()
    {
        //Reduce charge over time 
        //Could possibly make actions reduce charge faster/slower
        DecreaseCharge(Time.deltaTime*chargeUsageRate);
    }

    public void IncreaseCharge(float increaseAmmount)
    {
        charge = Mathf.Clamp(charge + increaseAmmount, 0, maxCharge);
    }

    public void DecreaseCharge(float decreaseAmmount)
    {
        charge = Mathf.Clamp(charge - decreaseAmmount, 0, maxCharge);
    }

    public float GetCharge()
    {
        return charge;
    }

    public bool HasChargeLeft()
    {
        if (charge > 0)
            return true;
        return false;
    }

    /*
    public Weapon GetWeapon()
    {

    }
    */
}
