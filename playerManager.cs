using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class playerData
{
    public int highScore;
    public int previousScore;
    public int longestDistance;
    public int previousDistance;
    public int playerLevel;
    public int playerXP;
    public int xpForNextLevel;
    public int playerMoney;
    public float xpModifyer;
    public int[] prefabPoints;
    public int[] prefabRates;
    public playerData(playerManager p)
    {
        highScore = p.highScore;
        previousScore = p.previousScore;
        playerLevel = p.playerLevel;
        playerXP = p.playerXP;
        xpForNextLevel = p.xpForNextLevel;
        playerMoney = p.playerMoney;
        xpModifyer = p.xpModifyer;
        prefabPoints = p.prefabPoints;
        prefabRates = p.prefabRates;
        longestDistance = p.longestDistance;
        previousDistance = p.previousDistance;
    }
}

public class playerManager : MonoBehaviour
{
    //things to be saved (MUST BE PUBLIC)
    public int highScore;
    public int previousScore;
    public int longestDistance;
    public int previousDistance;
    public int playerLevel;
    public int playerXP;
    public int xpForNextLevel;
    public int playerMoney;
    public float xpModifyer = 1;
    public int[] prefabPoints = new int[2] {50, 500};
    public int[] prefabRates = new int[2] {80, 9};
    //other
    [SerializeField] private GameObject playerPrefab;
    public playerScript pScript;
    [SerializeField] private collectableScript[] colScripts;
    [SerializeField] private storeHandler[] storeStuff;
    [SerializeField] private collectableSpawn colSpawner; 
    public static event Action playerSpawned;
    //UI
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text lastScoreText;
    [SerializeField] private TMP_Text HighScoreText;
    [SerializeField] private TMP_Text lastDistanceText;
    [SerializeField] private TMP_Text longestDistanceText;
    [SerializeField] private Image xpBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerData data = saveAndLoad.LoadPlayerData(); //loads the data from the file
        if (data != null)
        {
            highScore = data.highScore;
            previousScore = data.previousScore;
            playerLevel = data.playerLevel;
            playerXP = data.playerXP;
            xpForNextLevel = data.xpForNextLevel;
            playerMoney = data.playerMoney;
            xpModifyer = data.xpModifyer;
            prefabPoints = data.prefabPoints;
            prefabRates = data.prefabRates;
            longestDistance = data.longestDistance;
            previousDistance = data.previousDistance;
        }

        updatePrefabs();
        updateTexts();
        
    }

    void OnEnable() //event disptachers
    {
        gameManager.gameStart += summonNewPlayer;
        storeHandler.upgrade += upgrades;
    }

    void OnDisable()
    {
        gameManager.gameStart -= summonNewPlayer;
        storeHandler.upgrade += upgrades;
    }

    // Update is called once per frame
    void Update()
    {
        if (levelText.gameObject.activeInHierarchy)
        {
            updateTexts();
        }
        else if (moneyText.gameObject.activeInHierarchy)
        {
            moneyText.SetText(playerMoney.ToString());
        }
        
    }

    public void updatePlayerScores(int newScore, int distance)
    {
        if (previousScore != newScore)
        {
            previousScore = newScore;
        }

        if (newScore > highScore)
        {
            highScore = newScore;
        }

        if (previousDistance != distance)
        {
            previousDistance = distance;
        }

        if (distance > longestDistance)
        {
            longestDistance = distance;
        }

        int y = newScore / 1000;
        while (y <= 0)
        {
            y = 1;
        }
        float xpEarned = (Mathf.Sqrt(newScore) * y) * xpModifyer;
        xpEarned = Mathf.Round(xpEarned);
        Debug.Log("Score: " + newScore.ToString() + ", XP Earned: " + xpEarned.ToString());
        playerXP += (int)xpEarned;

        float floatyMoney = xpEarned/3;
        playerMoney += (int)Mathf.Round(floatyMoney);

        levelUp();
        updateTexts();

        SAVE();
    }

    public void SAVE()
    {
        saveAndLoad.SavePlayerData(this);
    }

    public int changeMoney(int change = 0)
    {
        playerMoney += change;
        return playerMoney;
    }

    void upgrades(string item)
    {
        switch(item)
        {
            case "XP Multiplier":
                xpModifyer += 0.25f;
                break;
            case "Yellow Rate":
                prefabRates[0] -= 1;
                break;
            case "Purple Rate":
                prefabRates[1] -= 1;
                break;
            case "Yellow Points":
                prefabPoints[0] += 5;
                break;
            case "Purple Points":
                prefabPoints[1] += 10;
                break;
        }

        updatePrefabs();
    }

    void updatePrefabs()
    {
        for (int i=0;i<colScripts.Length;i++)
        {
            colScripts[i].points = prefabPoints[i];
        }

        colSpawner.probabilities["default"] = prefabRates[0];
        colSpawner.probabilities["star"] = prefabRates[1];

        for (int i=0;i<storeStuff.Length;i++)
        {
            switch(storeStuff[i].item)
            {
                case "XP Multiplier":
                    storeStuff[i].purchases[0] = (int)((xpModifyer-1)/0.25f);
                    break;
                case "Yellow Rate":
                    storeStuff[i].purchases[0] = 80-prefabRates[0];
                    break;
                case "Purple Rate":
                    storeStuff[i].purchases[0] = 99-prefabRates[1];
                    break;
                case "Yellow Points":
                    storeStuff[i].purchases[0] = (prefabPoints[0]-50)/5;
                    break;
                case "Purple Points":
                    storeStuff[i].purchases[0] = (prefabPoints[1]-500)/10;
                    break;
            }
        }
    }

    void summonNewPlayer()
    {
        GameObject p = Instantiate(playerPrefab);
        pScript = p.GetComponent<playerScript>();
        playerSpawned?.Invoke();
    }

    void updateTexts()
    {
        levelText.SetText(playerLevel.ToString());
        xpText.SetText(playerXP.ToString());
        moneyText.SetText(playerMoney.ToString());
        lastScoreText.SetText(previousScore.ToString());
        HighScoreText.SetText(highScore.ToString());
        longestDistanceText.SetText(longestDistance.ToString());
        lastDistanceText.SetText(previousDistance.ToString());

        if (playerXP != 0 && xpForNextLevel != 0)
        {
            float editedplayerxp = (float)playerXP;
            float editednextrxp = (float)xpForNextLevel;
            float fill = editedplayerxp/editednextrxp;
            //Debug.Log(fill);
            xpBar.fillAmount = fill;
        }
        else
        {
            xpBar.fillAmount = 0;
        }
        
    }

    void levelUp()
    {
        while (playerXP >= xpForNextLevel)
        {
            playerLevel++;
            xpForNextLevel += playerLevel*1250;
            playerMoney += playerLevel*250;
        }
        
    }

    public void getplayertojump()
    {
        pScript.mobileJump();
    }

    public void getplayertoatk()
    {
        pScript.mobileAttack();
    }
}
