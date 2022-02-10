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
    public GameObject menuCloseFirstSelected;
    public GameObject partyMenu;
    public PlayerInput playerMovementInputs;
    public PlayerMovement playerMovement;
    public DialogueManager dialogueManager;
    public PlayerInput dialogueInputs;
    public Slider masterVol, musicVol, soundVol;
    public bool menuInBattle;

    private bool pauseMenuOpen = false;
    private bool partyMenuOpen = false;
    private bool optionsPanelOpen = false;
    private RockyGameManager gameManager;
    private SaveSettings settings;

    public AudioSource buttonHoverSound;

    void Awake()
    {
        pauseMenu.SetActive(false);
        if (partyMenu != null)  partyMenu.SetActive(false);
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
        if (menuInBattle) menuCloseFirstSelected = EventSystem.current.currentSelectedGameObject;
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
        if (dialogueManager != null && !dialogueManager.dialogueActive)
        {
            if (playerMovementInputs != null)   playerMovementInputs.enabled = true;
            if (playerMovement != null)         playerMovement.allowMovement = true;
        }
        if (dialogueInputs != null && dialogueManager.dialogueActive) dialogueInputs.enabled = true;
        partyMenu.GetComponent<PartyMenu>().partyMenuInput.enabled = false;
        partyMenu.GetComponent<PartyMenu>().ChangeCharButtonsState(true);
        partyMenuOpen = false;

        if (menuCloseFirstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(menuCloseFirstSelected);
        }
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
        if (menuInBattle) menuCloseFirstSelected = EventSystem.current.currentSelectedGameObject;
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
        if (dialogueManager != null && !dialogueManager.dialogueActive)
        {
            if (playerMovementInputs != null)   playerMovementInputs.enabled = true;
            if (playerMovement != null)         playerMovement.allowMovement = true;
        }
        if (dialogueInputs != null && dialogueManager.dialogueActive) dialogueInputs.enabled = true;
        pauseMenuOpen = false;

        if (optionsPanelOpen)
        {
            SaveAudioSettings();
        }
        optionsPanelOpen = false;

        if (menuCloseFirstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(menuCloseFirstSelected);
        }

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
            if (buttonHoverSound != null)   buttonHoverSound.Stop();
        }
    }

    public void CloseOptionsPanel()
    {
        optionsPanel.SetActive(false);
        BackToPausePanel();
        SaveAudioSettings();
        optionsPanelOpen = false;
        if (buttonHoverSound != null)   buttonHoverSound.Stop();
    }

    public void OpenControlsPanel()
    {
        controlsPanel.SetActive(true);
        if (controlsFirstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(controlsFirstSelected.gameObject);
            if (buttonHoverSound != null)   buttonHoverSound.Stop();
        }
    }

    public void CloseControlsPanel()
    {
        controlsPanel.SetActive(false);
        BackToPausePanel();
        if (buttonHoverSound != null)   buttonHoverSound.Stop();
    }

    public void OpenQuitGameDialogue()
    {
        quitGameDialogue.SetActive(true);
        if (quitFirstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(quitFirstSelected.gameObject);
            if (buttonHoverSound != null)   buttonHoverSound.Stop();
        }
    }

    public void CloseQuitGameDialogue()
    {
        quitGameDialogue.SetActive(false);
        BackToPausePanel();
        if (buttonHoverSound != null)   buttonHoverSound.Stop();
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
