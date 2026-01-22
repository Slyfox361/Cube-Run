using UnityEngine;
using System.Collections;

public class camerPivotScript : MonoBehaviour
{
    [SerializeField] private Transform jstalpPos;
    [SerializeField] private float rotateSpeed = 50f;
    [SerializeField] private bool gameInProgress = false;
    [SerializeField] private Transform playerPos;
    [SerializeField] private float camSmoothSpeed;
    [SerializeField] private Vector3 camvelcoity;

    void LateUpdate()
    {
        if (!jstalpPos) return;

        if (gameInProgress)
        {
            if (playerPos)
            {
                transform.position = Vector3.SmoothDamp(transform.position, playerPos.position, ref camvelcoity, camSmoothSpeed);
            }
        }
        else
        {
            transform.position = jstalpPos.position;
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
        
    }

    void OnEnable() //event disptachers
    {
        gameManager.gameStart += setGIPtrue;
        playerScript.died += setGIPfalse;
        playerManager.playerSpawned += findPlayer;
    }

    void OnDisable()
    {
        gameManager.gameStart -= setGIPtrue;
        playerScript.died -= setGIPfalse;
        playerManager.playerSpawned -= findPlayer;
    }

    void setGIPtrue()
    {
        gameInProgress = true;
        transform.rotation = Quaternion.identity;
    }

    void setGIPfalse(float x)
    {
        StartCoroutine(delayGIP());
    }

    IEnumerator delayGIP()
    {
        yield return new WaitForSeconds(0.3f);
        gameInProgress = false;
    }

    void findPlayer()
    {
        if (!playerPos)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("player");
            if (players.Length > 0)
            {
                playerPos = players[0].GetComponent<Transform>();
            }
        } 
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
