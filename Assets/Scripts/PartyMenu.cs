using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class PartyMenu : MonoBehaviour
{
    public Party party;
    public GameObject charInfoPanel;
    public Image charIcon;
    public TextMeshProUGUI charName, charDescription;
    public TextMeshProUGUI hp, phyAtk, magAtk, phyDef, magDef, init, ap, ap_regen;
    public TextMeshProUGUI moveDescription, moveType, movePower, moveAP, moveTargets;

    public List<Button> charButtons;
    public List<Button> moveButtons;

    private BattleUnit currentBattleUnit;
    private bool canGoBack = false;
    private int lastCharButton = 0;
    public PlayerInput partyMenuInput;

    void Start()
    {
        charInfoPanel.SetActive(false);
    }

    public void SetCharButtons()
    {
        for (int i = 0; i < charButtons.Count; i++)
        {
            if (party.battleUnits[i] != null)
            {
                charButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = party.battleUnits[i].unitName;
            }
            else
            {
                charButtons[i].gameObject.SetActive(false);
            }
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(charButtons[0].gameObject);
    }

    public void ShowPartyMemberInfo(int partyIndex)
    {
        SelectUnit(party.battleUnits[partyIndex]);
        lastCharButton = partyIndex;
    }

    private void SelectUnit(BattleUnit unit)
    {
        currentBattleUnit = unit;
        canGoBack = true;

        charIcon.sprite = unit.menuIcon.GetComponent<Image>().sprite;
        charName.text = unit.unitName;
        charDescription.text = unit.description;
        hp.text = unit.maxHP.ToString();
        phyAtk.text = unit.phyAtk.ToString();
        magAtk.text = unit.magAtk.ToString();
        phyDef.text = unit.phyDef.ToString();
        magDef.text = unit.magDef.ToString();
        init.text = unit.init.ToString();
        ap.text = unit.maxAP.ToString();
        ap_regen.text = unit.APregenartion.ToString();

        SelectMove(unit.moves[0]);

        for (int i = 0; i < moveButtons.Count; i++)
        {
            if (unit.moves[i] != null)
            {
                moveButtons[i].gameObject.SetActive(true);
                moveButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = unit.moves[i].moveName;
            }
            else
            {
                moveButtons[i].gameObject.SetActive(false);
            }
        }

        charInfoPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(moveButtons[0].gameObject);

        ChangeCharButtonsState(false);
    }

    private void SelectMove(Move move)
    {
        moveAP.text = move.apCost.ToString();
        moveTargets.text = move.numberOfTargets.ToString();
        moveDescription.text = move.description;

        if (move.damageTyp == Move.DamageTyp.Physical)
        {
            moveType.text = "Physisch";
        }
        else
        {
            moveType.text = "Magisch";
        }

        if (move.damage > 0)
        {
            movePower.text = move.damage.ToString();
        }
        else
        {
            movePower.text = move.healing.ToString();
        }
    }

    public void SelectBattleUnitMove(int moveIndex)
    {
        SelectMove(currentBattleUnit.moves[moveIndex]);
    }

    public void ChangeCharButtonsState(bool state)
    {
        foreach (Button button in charButtons)
        {
            button.interactable = state;
        }
    }

    public void GoBack()
    {
        if (canGoBack)
        {
            charInfoPanel.SetActive(false);
            ChangeCharButtonsState(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(charButtons[lastCharButton].gameObject);
            canGoBack = false;
        }
    }
}