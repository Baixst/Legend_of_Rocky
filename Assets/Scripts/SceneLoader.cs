using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneByIndex(sceneIndex));
    }

    public void LoadNextScene()
    {
        StartCoroutine(LoadSceneByIndex(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadSceneByIndex(int sceneIndex)
    {
        transition.SetTrigger("StartTransition");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneIndex);
    }
}
