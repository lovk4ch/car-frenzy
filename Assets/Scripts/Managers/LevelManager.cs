using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelManager : Manager<LevelManager>
{
    [SerializeField]
    private PlayerController playerPrefab = null;

    [SerializeField]
    private CarController[] carPrefabs = null;

    [SerializeField]
    private GameObject teleport = null;

    private List<CarController> cars;

    public int Cars => cars.Count;
    public int Score { get; private set; }
    public PlayerController Player { get; private set; }

    private void Awake()
    {
        Player = Instantiate(playerPrefab, new Vector3(23, 0, 61), Quaternion.identity);
        cars = new List<CarController>();

        for (int i = 0; i < 6 - cars.Count; i++)
            StartCoroutine(Spawn());
    }

    public IEnumerator Spawn()
    {
        Vector3 position = RandomPoint(Player.transform.position, 100);
        yield return new WaitForSeconds(1);

        if (carPrefabs.Length > 0)
        {
            int i = Random.Range(0, carPrefabs.Length);

            CarController car = Instantiate(carPrefabs[i], position, Quaternion.identity);
            Instantiate(teleport, position, Quaternion.identity);
            car.Target = Player;
            cars.Add(car);
        }
    }

    public void Remove(CarController car)
    {
        Score++;
        cars.Remove(car);
        StartCoroutine(Spawn());
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