using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject obstaclePrefab;
    public GameObject laneObject;
    public string ballTag = "Interactable";

    [Header("Spawn Settings")]
    public int maxObstacles = 9;
    public float obstacleSpacing = 1.5f;

    private List<Vector3> usedPositions = new List<Vector3>();
    private List<GameObject> spawnedObstacles = new List<GameObject>();
    private bool hasSpawned = false;
    private Bounds laneBounds;

    void Start()
    {
        if (laneObject == null || obstaclePrefab == null)
        {
            Debug.LogError("Missing lane object or obstacle prefab.");
            return;
        }

        Collider laneCollider = laneObject.GetComponent<Collider>();
        if (laneCollider == null)
        {
            Debug.LogError("Lane object requires a collider.");
            return;
        }

        laneBounds = laneCollider.bounds;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasSpawned && other.CompareTag(ballTag))
        {
            SpawnObstacles();
            hasSpawned = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(ballTag))
        {
            DestroyAllObstacles();
            hasSpawned = false;
            usedPositions.Clear();
        }
    }

    void SpawnObstacles()
    {
        int spawned = 0;
        int attempts = 0;

        while (spawned < maxObstacles && attempts < 100)
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(laneBounds.min.x + 0.5f, laneBounds.max.x - 0.5f),
                laneBounds.min.y + 0.1f,
                Random.Range(laneBounds.min.z + 0.5f, laneBounds.max.z - 0.5f)
            );

            if (IsFarFromOthers(spawnPos))
            {
                GameObject obstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
                spawnedObstacles.Add(obstacle);
                usedPositions.Add(spawnPos);
                spawned++;
            }

            attempts++;
        }
    }

    void DestroyAllObstacles()
    {
        foreach (GameObject obstacle in spawnedObstacles)
        {
            if (obstacle != null)
            {
                Destroy(obstacle);
            }
        }
        spawnedObstacles.Clear();
    }

    bool IsFarFromOthers(Vector3 pos)
    {
        foreach (Vector3 used in usedPositions)
        {
            if (Vector3.Distance(pos, used) < obstacleSpacing)
                return false;
        }
        return true;
    }
}
