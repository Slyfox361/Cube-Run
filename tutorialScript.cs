using UnityEngine;

public class tutorialScript : MonoBehaviour
{
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject[] tutorialScreens;
    [SerializeField] private int currentIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switchBetweenTutorialScreem()
    {
        if (!tutorialUI.activeInHierarchy)
        {
            mainMenuUI.SetActive(false);
            tutorialUI.SetActive(true);
        }
        else
        {
            mainMenuUI.SetActive(true);
            tutorialUI.SetActive(false);
        }
    }

    public void cycleTutorialScreens()
    {
        if (currentIndex < tutorialScreens.Length-1)
        {
            tutorialScreens[currentIndex].SetActive(false);
            currentIndex++;
            tutorialScreens[currentIndex].SetActive(true);
        }
        else
        {
            currentIndex = 0;
            switchBetweenTutorialScreem();
        }
        
    }
}
