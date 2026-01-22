using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class storeHandler : MonoBehaviour
{
    public string item;
    [SerializeField] private int cost;
    [SerializeField] private int modifier;
    public int[] purchases;
    [SerializeField] private playerManager pManager;
    [SerializeField] private TMP_Text displayText;
    public static event Action<string> upgrade;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button btn = this.gameObject.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);

        cost += modifier * purchases[0];
        displayText.SetText(item + "\n" + cost.ToString());
        if (purchases[0] == purchases[1])
        {
            displayText.SetText(item + "\nSOLD OUT");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void TaskOnClick()
    {
        if (pManager.changeMoney() >= cost && purchases[0] < purchases[1])
        {
            pManager.changeMoney(-cost);
            purchases[0] = purchases[0] + 1;
            cost += modifier;
            
            upgrade?.Invoke(item);

            displayText.SetText(item + "\n" + cost.ToString());
            if (purchases[0] == purchases[1])
            {
                displayText.SetText(item + "\nSOLD OUT");
            }
        }
    }
}
