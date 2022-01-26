using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoScene : MonoBehaviour
{
    private SceneLoader sceneLoader;

    void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
        StartCoroutine(WaitAndLoadNextScene());
    }

    private IEnumerator WaitAndLoadNextScene()
    {
        yield return new WaitForSeconds(4f);
        sceneLoader.LoadNextScene();
    }
}
