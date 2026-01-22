using UnityEngine;
using System.Collections.Generic;

public class collectableSpawn : MonoBehaviour
{
    [SerializeField] private GameObject collectablePrefab;
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private GameObject enemy1Prefab;
    public Dictionary<string, int> probabilities = new Dictionary<string, int>
    {
        {"star", 99},
        {"default", 80}
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int val = rnjesus.rand.Next(0,100);
        
        if (val >= probabilities["star"])
        {
            Instantiate(starPrefab, transform.position, collectablePrefab.transform.rotation);
        }
        else if (val % 30 == 0)
        {
            Instantiate(enemy1Prefab, transform.position, enemy1Prefab.transform.rotation);
        }
        else if (val >= probabilities["default"])
        {
            Instantiate(collectablePrefab, transform.position, collectablePrefab.transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
