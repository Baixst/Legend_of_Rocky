using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject partyMenu;
    public PlayerInput playerMovementInputs;
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
        playerMovementInputs.enabled = false;
        partyMenu.SetActive(true);
        partyMenuOpen = true;
        Time.timeScale = 0;
    }

    public void ClosePartyMenu()
    {
        partyMenu.SetActive(false);
        playerMovementInputs.enabled = true;
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
        playerMovementInputs.enabled = false;
        pauseMenu.SetActive(true);
        pauseMenuOpen = true;
        Time.timeScale = 0;
    }

    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
        playerMovementInputs.enabled = true;
        pauseMenuOpen = false;
        Time.timeScale = 1;
    }
}
