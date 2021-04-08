using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //Player stats
    Sensor sensor;

    float charge;
    [SerializeField] float maxCharge;
    [SerializeField] Battery batteryPrefab;
    [SerializeField] float ammoShotChargeCost;
    public float chargeUsageRate = 5f;

    // Start is called before the first frame update
    void Start()
    {
        charge = maxCharge;

        if (GetComponent<Sensor>() != null)
        {
            sensor = GetComponent<Sensor>();
        }
        else Debug.LogError(this + " does not have a sensor.");
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
        //Charge determines view distance.
        UpdateViewDistance();
    }

    void UpdateViewDistance()
    {
        if (sensor != null)
        {
            sensor.SetViewDistancePercentage(charge / maxCharge);
        }
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

    /// <summary>
    /// Consumes the ammount of charge it costs for a shot of a weapon.
    /// </summary>
    public void ConsumeAmmo()
    {
        DecreaseCharge(ammoShotChargeCost);
    }

    public void DropChargeAsBattery()
    {
        if (batteryPrefab == null)
            return;
        Battery currentBattery = Instantiate(batteryPrefab, transform.position, transform.rotation, null);
        currentBattery.gameObject.name = batteryPrefab.name;
        currentBattery.SetChargePercentage(charge/maxCharge);
        charge = 0;
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
