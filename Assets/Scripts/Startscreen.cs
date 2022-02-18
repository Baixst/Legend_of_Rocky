using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Startscreen : MonoBehaviour
{
    public GameObject mainFirstSelected;
    public GameObject mainFirstSelectedAlt;
    public GameObject optionsPanel;
    public GameObject controlsPanel;
    public GameObject quitPanel;
    public GameObject optionsFirstSelected;
    public GameObject rocky;
    public GameObject lucy;
    public Button controlsFirstSelected;
    public Button quitFirstSelected;
    public Slider masterVol, musicVol, soundVol;

    private SaveSettings settings;

    void Awake()
    {
        optionsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        quitPanel.SetActive(false);
    }

    void Start()
    {
        LoadAudioSettings();
        if (!mainFirstSelected.GetComponent<Button>().interactable)
        {
            mainFirstSelected = mainFirstSelectedAlt;
        }
        BackToMain();
    }

    private void LoadAudioSettings()
    {
        settings = SaveSystem.LoadPlayerSettings();
        if (settings != null)
        {
            masterVol.value = settings.masterVol;
            masterVol.gameObject.GetComponent<AudioVolumeHandler>().SetVolumeLevel(masterVol.value);
            musicVol.value = settings.musicVol;
            musicVol.gameObject.GetComponent<AudioVolumeHandler>().SetVolumeLevel(musicVol.value);
            soundVol.value = settings.soundVol;
            soundVol.gameObject.GetComponent<AudioVolumeHandler>().SetVolumeLevel(soundVol.value);
        }
    }

    public void OpenOptionsPanel()
    {
        optionsPanel.SetActive(true);
        if (optionsFirstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(optionsFirstSelected);
        }
        rocky.SetActive(false);
        lucy.SetActive(false);
    }

    public void CloseOptionsPanel()
    {
        optionsPanel.SetActive(false);
        SaveAudioSettings();
        BackToMain();
    }

    public void OpenControlsPanel()
    {
        controlsPanel.SetActive(true);
        if (controlsFirstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(controlsFirstSelected.gameObject);
        }
        rocky.SetActive(false);
        lucy.SetActive(false);
    }

    public void CloseControlsPanel()
    {
        controlsPanel.SetActive(false);
        BackToMain();
    }

    public void OpenQuitPanel()
    {
        quitPanel.SetActive(true);
        if (quitFirstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(quitFirstSelected.gameObject);
        }
        rocky.SetActive(false);
        lucy.SetActive(false);
    }

    public void CloseQuitPanel()
    {
        quitPanel.SetActive(false);
        BackToMain();
    }

    private void BackToMain()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainFirstSelected);
        rocky.SetActive(true);
        lucy.SetActive(true);
    }

    public void SaveAudioSettings()
    {
        SaveSystem.SavePlayerSettings(masterVol.value, musicVol.value, soundVol.value);
    }
}
