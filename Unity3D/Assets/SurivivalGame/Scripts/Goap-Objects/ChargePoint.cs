using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargePoint : MonoBehaviour
{
    [SerializeField] float chargeReplenishRate;
    [SerializeField] float beginReplenishDelay;
    [SerializeField] Transform spawnPoint;
    float dispenseTime;

    [SerializeField] Battery batteryPrefab;
    Battery currentBattery;
    [Header("UI")]
    [SerializeField] ChargePointUI chargePointUI;

    private void Update()
    {
        if (Time.time - dispenseTime > beginReplenishDelay)
        {
            ReplenishCharge();
        }
    }

    void ReplenishCharge()
    {
        if(currentBattery!=null)
            currentBattery.gameObject.SetActive(true);

        if (currentBattery == null)
        {
            currentBattery = Instantiate(batteryPrefab, spawnPoint.position, spawnPoint.rotation, null);
            currentBattery.gameObject.name = batteryPrefab.name;
            currentBattery.SetChargePercentage(0);
            currentBattery.gameObject.SetActive(false);
            dispenseTime = Time.time;
        }
        else
        {
            currentBattery.IncreaseCharge(chargeReplenishRate * Time.deltaTime);
            chargePointUI.UpdateCharge(currentBattery.GetChargePercentage());
        }
    }
}
