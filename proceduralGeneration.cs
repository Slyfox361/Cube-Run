using UnityEngine;
using System;

public static class rnjesus
{
    public static readonly System.Random rand = new System.Random();
}

public class proceduralGeneration : MonoBehaviour
{
    [SerializeField] private GameObject[] segmentPrefabs;
    [SerializeField] private GameObject[] ceilingPrefabs;
    [SerializeField] private int blockadeCounter = 6;
    [SerializeField] private int wallCounter = 1;
    [SerializeField] private Vector3 currentPos;
    //[SerializeField] private Vector3 startPos;
    [SerializeField] private Transform playerPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPos = transform.position;

        for (int i = 1; i<= 3; i++)
        {
            placeSegment(0, i);
        }

        blockadeCounter = 6;
        wallCounter = 1;
    }

    void OnEnable() //event disptachers
    {
        playerScript.died += end;
        gameManager.gameStart += Start;
        playerManager.playerSpawned += findPlayer;
    }

    void OnDisable()
    {
        playerScript.died -= end;
        gameManager.gameStart += Start;
        playerManager.playerSpawned -= findPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerPos != null)
        {
            if (playerPos.position.x > (currentPos.x - 25))
            {
                nextPos();
            }
        }
    }

    void findPlayer()
    {
        if (playerPos == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("player");
            if (players.Length > 0)
            {
                playerPos = players[0].GetComponent<Transform>();
            }
        } 
    }

    void nextPos(int[] index = null)
    {
        if (index == null)
        {
            index = new int[] {0,0,0};
            for (int j = 0; j < 3; j++)
            {
                int endVal = segmentPrefabs.Length;

                if (blockadeCounter > 0)
                {
                    endVal -= 4;
                    blockadeCounter--;
                }
                else
                {
                    blockadeCounter = 6;
                }

                if (wallCounter > 0 && endVal != segmentPrefabs.Length)
                {
                    wallCounter--;
                    endVal = 0;
                }
                else
                {
                    wallCounter = 1;
                }
                
                int val = rnjesus.rand.Next(0, endVal);
                
                index[j] = val;
            }
        }

        currentPos += new Vector3(25, 0, 0);

        destroySegments();

        for (int i = 1; i<= 3; i++)
        {
            placeSegment(index[i-1], i);
        }
    }

    void end(float x)
    {
        playerPos = null;
        destroySegments(true);
    }

    void destroySegments(bool end = false)
    {
        GameObject[] allSegs = GameObject.FindGameObjectsWithTag("level");
        GameObject[] allEnemys = GameObject.FindGameObjectsWithTag("enemy");

        foreach (GameObject seg in allSegs)
        {
            if (!end)
            {
                if (seg.transform.position.x < (currentPos.x - 75))
                {
                    Destroy(seg);
                }
            }
            else
            {
                Destroy(seg);
            }
        }

        foreach (GameObject enemy in allEnemys)
        {
            if (!end)
            {
                if (enemy.transform.position.x < (currentPos.x - 75))
                {
                    Destroy(enemy);
                }
            }
            else
            {
                Destroy(enemy);
            }
        }
    }

    void placeSegment(int index, int laneNo)
    {
        if (index < 0 || index >= segmentPrefabs.Length) return;

        float z = 3 - (3*(laneNo-1));

        int ceilignVal = rnjesus.rand.Next(0,100);

        Vector3 placePos = currentPos + new Vector3(12.5f, 0, z);
        GameObject newSeg = Instantiate(segmentPrefabs[index], placePos, Quaternion.identity);
        if (ceilignVal <= 30)
        {
            int ceilingIndex = rnjesus.rand.Next(0,ceilingPrefabs.Length);
            GameObject newCeiling = Instantiate(ceilingPrefabs[ceilingIndex], placePos, Quaternion.identity);
        }
    }
}
