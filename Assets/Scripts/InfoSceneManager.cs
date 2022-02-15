using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoSceneManager : MonoBehaviour
{
    public float waitAfterSceneStart;
    public GameObject panel;

    void Awake()
    {
        panel.SetActive(false);
        StartCoroutine(WaitAndOpenPanel(waitAfterSceneStart));
    }

    private IEnumerator WaitAndOpenPanel(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        panel.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        panel.GetComponent<Animator>().SetTrigger("open");
    }

    public void ClosePanel()
    {
        panel.GetComponent<Animator>().SetTrigger("close");
    }
}
