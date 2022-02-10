using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class TutorialMenu : MonoBehaviour
{
    public List<GameObject> pages;
    public List<GameObject> continueButtons;
    public GameObject title;
    public GameObject background;
    public BattleSystem battleSystem;
    public PlayerInput menuInput;
    public AudioSource buttonHoverSound;
    private int pageIndex;

    void Awake()
    {
        foreach (GameObject page in pages)
        {
            page.SetActive(false);
        }
        title.SetActive(false);
        background.SetActive(false);
        menuInput.enabled = false;
        pageIndex = 0;
    }

    void Start()
    {
        StartCoroutine(WaitAndOpen(2.5f));
    }

    private IEnumerator WaitAndOpen(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        title.SetActive(true);
        background.SetActive(true);
        pages[pageIndex].SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(continueButtons[pageIndex]);
        if (buttonHoverSound != null)   buttonHoverSound.Stop();
    }

    public void ShowNextPage()
    {
        if (pageIndex + 1 < pages.Count)
        {
            pages[pageIndex].SetActive(false);
            pageIndex++;
            pages[pageIndex].SetActive(true);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(continueButtons[pageIndex]);
            if (buttonHoverSound != null)   buttonHoverSound.Stop();
        }
        else
        {
            StartCoroutine(CloseTutorial());
        }
    }

    public void ShowPreviousPage()
    {
        pages[pageIndex].SetActive(false);
        pageIndex--;
        pages[pageIndex].SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(continueButtons[pageIndex]);
        if (buttonHoverSound != null)   buttonHoverSound.Stop();
    }

    private IEnumerator CloseTutorial()
    {
        pages[pageIndex].SetActive(false);
        title.SetActive(false);
        background.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);

        yield return new WaitForSeconds(0.5f);
        menuInput.enabled = true;

        battleSystem.StartBattleAfterTutorial();
    }
}
