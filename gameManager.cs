using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    [SerializeField] private int score;
    [SerializeField] private int distance;
    [SerializeField] private int currentDisplayScore;
    [SerializeField] private int currentDisplayDist;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text distanceText;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject notStoreUi;
    [SerializeField] private GameObject storeUi;
    [SerializeField] private playerManager pManager;
    [SerializeField] private Transform pPos;
    public static event Action gameStart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //startGame();
        //StartCoroutine(delayStartDEBUG());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (scoreText.gameObject.activeInHierarchy)
        {
            if (currentDisplayScore <= score)
            {
                scoreText.SetText(currentDisplayScore.ToString());
                currentDisplayScore++;
            }
        }

        if (distanceText.gameObject.activeInHierarchy && pPos)
        {
            distance = (int)Mathf.Round(pPos.position.x);
            if (currentDisplayDist <= distance)
            {
                distanceText.SetText(currentDisplayDist.ToString());
                currentDisplayDist++;
            }
        }
        
    }

    void OnEnable() //event disptachers
    {
        collectableScript.scoreUpdate += updateScore;
        enemyScript.scoreUpdate += updateScore;
        playerScript.died += endGame;
    }

    void OnDisable()
    {
        collectableScript.scoreUpdate -= updateScore;
        enemyScript.scoreUpdate -= updateScore;
        playerScript.died -= endGame;
    }

    void updateScore(int amount)
    {
        score += amount;
        if (!(int.TryParse(scoreText.text, out currentDisplayScore)))
        {
            Debug.Log("Error: text not int");
        }
    }

    public void startGame()
    {
        gameStart?.Invoke();
        mainMenuUI.SetActive(false);
        gameUI.SetActive(true);
        GameObject[] temp = GameObject.FindGameObjectsWithTag("player");
        pPos = temp[0].transform;
    }

    void endGame(float d)
    {
        distance = (int)Mathf.Round(d);
        score += distance;
        Debug.Log("disatnce -> " + Mathf.Round(distance).ToString());
        if (distance >= 10)
        {
            pManager.updatePlayerScores(score, distance);
        }
        score = 0;
        currentDisplayScore = 0;
        currentDisplayDist = 0;
        mainMenuUI.SetActive(true);
        gameUI.SetActive(false);
    }

    IEnumerator delayStartDEBUG()
    {
        yield return new WaitForSeconds(5f);
        startGame();
    }

    public void goToStore()
    {
        if (notStoreUi.activeInHierarchy)
        {
            notStoreUi.SetActive(false);
            storeUi.SetActive(true);
        }
        else
        {
            notStoreUi.SetActive(true);
            storeUi.SetActive(false);
        }

        pManager.SAVE();
    }

    public void QUIT()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    public void RESET()
    {
        saveAndLoad.DeleteInventoryData();
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Debug.Log("Reset");
    }
}
