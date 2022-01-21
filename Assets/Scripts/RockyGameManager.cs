using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class RockyGameManager : MonoBehaviour
{
    public int startGameAtScene;
    public Vector3 playerStartPosition;
    public bool playerStartPositionSet = false;
    public Button resumeButton;
    private SaveData saveData;

    [HideInInspector] public SceneLoader sceneLoader;

    void Awake()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();

        saveData = SaveSystem.LoadPlayerData();
        if (saveData == null)
        {
            if (resumeButton != null)
            {
                resumeButton.interactable = false;
            }
        }

        if (saveData != null)
        {
            LoadPlayerSaveData();

            // if a new scene is loaded, don't set player to last saved position
            if (SceneManager.GetActiveScene().buildIndex != saveData.sceneIndex)
            {
                playerStartPositionSet = false;
            }
        }
    }

    private void LoadPlayerSaveData()
    {
        playerStartPosition.x = saveData.playerPosition[0];
        playerStartPosition.y = saveData.playerPosition[1];
        playerStartPosition.z = saveData.playerPosition[2];
        playerStartPositionSet = true;
    }

    public void StartNewGame()
    {
        sceneLoader.LoadNextScene();
    }

    public void ContinueFromLastSave()
    {
        if (saveData == null)   return;

        sceneLoader.LoadScene(saveData.sceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
