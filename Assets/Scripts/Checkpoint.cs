using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    public bool saveHere;
    public GameObject saveIcon;
    private Checkpoint[] otherCheckpoints;

    void Start()
    {
        saveHere = true;
        otherCheckpoints = FindObjectsOfType<Checkpoint>();
    }

    public void SaveGame(Player player)
    {
        if (saveHere)
        {
            StartCoroutine(ShowSaveIcon());
            SaveSystem.SavePlayerData(player, gameObject.transform.position.y, SceneManager.GetActiveScene().buildIndex);

            // no need to save at this checkpoint again if there was no other in the meantime
            foreach (Checkpoint checkpoint in otherCheckpoints)
            {
                checkpoint.saveHere = true;
            }
            saveHere = false;
        }
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
