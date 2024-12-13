using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public GameObject rockPrefab; // Prefab batu
    public int numberOfRocks = 10; // Jumlah batu yang akan di-generate
    public Vector3 spawnArea = new Vector3(10, 0, 10); // Area spawn

    void Start()
    {
        GenerateRocks();
    }

    void GenerateRocks()
    {
        for (int i = 0; i < numberOfRocks; i++)
        {
            // Random posisi dalam area spawn
            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                0, // Di level tanah
                Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
            );

            // Random skala batu
            float randomScale = Random.Range(0.5f, 2.0f);

            // Spawn batu
            GameObject rock = Instantiate(rockPrefab, randomPosition, Quaternion.identity);
            rock.transform.localScale = Vector3.one * randomScale;

            // Random rotasi batu
            rock.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        }
    }
}
