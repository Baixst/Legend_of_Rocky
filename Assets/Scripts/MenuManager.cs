using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject optionsPanel;
    public GameObject controlsPanel;
    public GameObject quitGameDialogue;
    public Button pauseFirstSelected;
    public GameObject optionsFirstSelected;
    public Button controlsFirstSelected;
    public Button quitFirstSelected;
    public GameObject partyMenu;
    public PlayerInput playerMovementInputs;
    public PlayerMovement playerMovement;
    public PlayerInput dialogueInputs;
    public Slider masterVol, musicVol, soundVol;

    private bool pauseMenuOpen = false;
    private bool partyMenuOpen = false;
    private bool optionsPanelOpen = false;
    private RockyGameManager gameManager;
    private SaveSettings settings;

    void Awake()
    {
        pauseMenu.SetActive(false);
        partyMenu.SetActive(false);
        optionsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        quitGameDialogue.SetActive(false);
        gameManager = FindObjectOfType<RockyGameManager>();
    }

    void Start()
    {
        LoadAudioSettings();
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

    public void TogglePartyMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (pauseMenuOpen)  return;
            if (partyMenuOpen)  ClosePartyMenu();
            else                OpenPartyMenu();
        }
    }

    public void OpenPartyMenu()
    {
        if (playerMovementInputs != null)   playerMovementInputs.enabled = false;
        if (playerMovement != null)         playerMovement.allowMovement = false;
        if (dialogueInputs != null)         dialogueInputs.enabled = false;
        partyMenu.SetActive(true);
        partyMenu.GetComponent<PartyMenu>().charInfoPanel.SetActive(false);
        partyMenu.GetComponent<PartyMenu>().SetCharButtons();
        partyMenu.GetComponent<PartyMenu>().partyMenuInput.enabled = true;
        partyMenuOpen = true;
        Time.timeScale = 0;
    }

    public void ClosePartyMenu()
    {
        partyMenu.SetActive(false);
        if (playerMovementInputs != null)   playerMovementInputs.enabled = true;
        if (playerMovement != null)         playerMovement.allowMovement = true;
        if (dialogueInputs != null)         dialogueInputs.enabled = true;
        partyMenu.GetComponent<PartyMenu>().partyMenuInput.enabled = false;
        partyMenuOpen = false;
        Time.timeScale = 1;
    }

    public void TogglePauseMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (partyMenuOpen)  return;
            if (pauseMenuOpen)  ClosePauseMenu();
            else                OpenPauseMenu();
        }
    }

    public void OpenPauseMenu()
    {
        if (playerMovementInputs != null)   playerMovementInputs.enabled = false;
        if (playerMovement != null)         playerMovement.allowMovement = false;
        if (dialogueInputs != null)         dialogueInputs.enabled = false;
        optionsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        quitGameDialogue.SetActive(false);
        pauseMenu.SetActive(true);
        pauseMenuOpen = true;
        if (pauseFirstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(pauseFirstSelected.gameObject);
        }
        Time.timeScale = 0;
    }

    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
        if (playerMovementInputs != null)   playerMovementInputs.enabled = true;
        if (playerMovement != null)         playerMovement.allowMovement = true;
        if (dialogueInputs != null)         dialogueInputs.enabled = true;
        pauseMenuOpen = false;

        if (optionsPanelOpen)
        {
            SaveAudioSettings();
        }
        optionsPanelOpen = false;

        Time.timeScale = 1;
    }

    public void OpenOptionsPanel()
    {
        optionsPanel.SetActive(true);
        optionsPanelOpen = true;
        if (optionsFirstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(optionsFirstSelected);
        }
    }

    public void CloseOptionsPanel()
    {
        optionsPanel.SetActive(false);
        BackToPausePanel();
        SaveAudioSettings();
        optionsPanelOpen = false;
    }

    public void OpenControlsPanel()
    {
        controlsPanel.SetActive(true);
        if (controlsFirstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(controlsFirstSelected.gameObject);
        }
    }

    public void CloseControlsPanel()
    {
        controlsPanel.SetActive(false);
        BackToPausePanel();
    }

    public void OpenQuitGameDialogue()
    {
        quitGameDialogue.SetActive(true);
        if (quitFirstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(quitFirstSelected.gameObject);
        }
    }

    public void CloseQuitGameDialogue()
    {
        quitGameDialogue.SetActive(false);
        BackToPausePanel();
    }

    private void BackToPausePanel()
    {
        if (pauseFirstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(pauseFirstSelected.gameObject);
        }
    }

    public void SaveAudioSettings()
    {
        SaveSystem.SavePlayerSettings(masterVol.value, musicVol.value, soundVol.value);
    }

    public void QuitGame()
    {
        if(gameManager != null)
        {
            gameManager.QuitGame();
        }
    }
}
