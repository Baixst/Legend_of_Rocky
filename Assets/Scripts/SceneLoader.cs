using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    public AudioSource music;
    public AudioSource sound;
    public bool saveOnLoad;
    public Player player;
    public GameObject saveIcon;

    void Awake()
    {
        if(music != null)
        {
            StartCoroutine(FadeInAudio(music, transitionTime));
        }
        if(sound != null)
        {
            StartCoroutine(EnableSoundAfter(sound, 0.5f));
        }
    }

    void Start()
    {
        if(saveOnLoad)
        {
            SaveSystem.SavePlayerData(player, gameObject.transform.position.y, SceneManager.GetActiveScene().buildIndex);
            StartCoroutine(ShowSaveIcon());
        }
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneByIndex(sceneIndex));
    }

    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadSceneByString(sceneName));
    }

    public void LoadNextScene()
    {
        StartCoroutine(LoadSceneByIndex(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadPreviousScene()
    {
        StartCoroutine(LoadSceneByIndex(SceneManager.GetActiveScene().buildIndex - 1));
    }

    IEnumerator LoadSceneByIndex(int sceneIndex)
    {
        if (music != null)
        {
            StartCoroutine(FadeOutAudio(music, transitionTime));
        }
        transition.SetTrigger("StartTransition");
        yield return new WaitForSeconds(transitionTime + 0.1f);
        SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator LoadSceneByString(string sceneName)
    {
        if (music != null)
        {
            StartCoroutine(FadeOutAudio(music, transitionTime));
        }
        transition.SetTrigger("StartTransition");
        yield return new WaitForSeconds(transitionTime + 0.1f);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeOutAudio(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
 
        audioSource.Stop ();
        audioSource.volume = startVolume;
    }

    private IEnumerator FadeInAudio(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;
        audioSource.volume = 0;
        while (audioSource.volume < startVolume)
        {
            audioSource.volume += startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        audioSource.volume = startVolume;
    }

    private IEnumerator EnableSoundAfter(AudioSource audioSource, float waitTime)
    {
        float startVolume = audioSource.volume;
        audioSource.volume = 0;
        yield return new WaitForSeconds(waitTime);
        audioSource.volume = startVolume;
    }

    private IEnumerator ShowSaveIcon()
    {
        if (saveIcon != null)
        {
            saveIcon.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            saveIcon.SetActive(false);
        }
    }
}
