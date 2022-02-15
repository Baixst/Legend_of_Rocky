using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoScene : MonoBehaviour
{
    private SceneLoader sceneLoader;
    public bool showInfoPanel;
    public GameObject panel;
    public float nextSceneAfter;
    public float openPanelAfter;
    public float closePanelAfter;

    void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
        
        if (showInfoPanel)
        {
            StartCoroutine(WaitAndOpenPanel(openPanelAfter));
        }

        StartCoroutine(WaitAndLoadNextScene(nextSceneAfter));
    }

    private IEnumerator WaitAndLoadNextScene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        sceneLoader.LoadNextScene();
    }
    
    private IEnumerator WaitAndOpenPanel(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        panel.GetComponent<Animator>().SetTrigger("open");
        yield return new WaitForSeconds(closePanelAfter);
        ClosePanel();
    }

    private void ClosePanel()
    {
        panel.GetComponent<Animator>().SetTrigger("close");
    }
}
