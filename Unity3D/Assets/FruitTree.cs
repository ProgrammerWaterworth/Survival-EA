using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns fruit over a period of time.
/// </summary>
public class FruitTree : MonoBehaviour
{
    [SerializeField]float timeToFillTree;
    [SerializeField] int totalFruit;
    int numFruit;
    [SerializeField] Fruit fruit;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float spawnRadius;
    float timer;
    List<Fruit> fruits;

    private void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = new Color(1, 0, 0, .3f);
            Gizmos.DrawSphere(spawnPoint.position, spawnRadius);
        }
    }

    private void Start()
    {
        fruits = new List<Fruit>();
    }

    private void Update()
    {
        GrowFruit();
    }

    void GrowFruit()
    {
        if (fruits.Count < totalFruit)
        {
            if (Time.time - timer > (timeToFillTree / totalFruit))
            {
                timer = Time.time;
                Vector3 spawnOffset = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
                Fruit _fruit = Instantiate(fruit, spawnPoint.transform.position + (spawnOffset * spawnRadius), Quaternion.identity,transform);
                _fruit.name = "Fruit";
                fruits.Add(_fruit);
            }
        }
    }
   public void RemoveFruit(Fruit _fruit)
    {
        fruits.Remove(_fruit);
    }
}
