using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarManager : Manager<CarManager>
{
    [SerializeField]
    private PlayerController player = null;

    [SerializeField]
    private CarController carPrefab = null;

    private List<CarController> cars;

    private void Awake()
    {
        cars = new List<CarController>();
        for (int i = 0; i < 3; i++)
            Spawn();
    }

    public void Spawn()
    {
        Vector3 position = RandomPoint(player.transform.position, 100);

        CarController car = Instantiate(carPrefab, position, Quaternion.identity);
        car.Target = player;
        cars.Add(car);
    }

    private Vector3 RandomPoint(Vector3 center, float range)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            return hit.position;
        else
            return RandomPoint(center, range);
    }
}