using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public Button pauseFirstSelected;
    public GameObject partyMenu;
    public Button partyFirstSelected;
    public PlayerInput playerMovementInputs;
    public PlayerMovement playerMovement;
    public PlayerInput dialogueInputs;
    private bool pauseMenuOpen = false;
    private bool partyMenuOpen = false;

    public void Awake()
    {
        pauseMenu.SetActive(false);
        partyMenu.SetActive(false);
    }

    public void TogglePartyMenu()
    {
        if (pauseMenuOpen)  return;
        if (partyMenuOpen)  ClosePartyMenu();
        else                OpenPartyMenu();
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
        if (partyFirstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(partyFirstSelected.gameObject);
        }
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

    public void TogglePauseMenu()
    {
        if (partyMenuOpen)  return;
        if (pauseMenuOpen)  ClosePauseMenu();
        else                OpenPauseMenu();
    }

    public void OpenPauseMenu()
    {
        if (playerMovementInputs != null)   playerMovementInputs.enabled = false;
        if (playerMovement != null)         playerMovement.allowMovement = false;
        if (dialogueInputs != null)         dialogueInputs.enabled = false;
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
        Time.timeScale = 1;
    }
}
