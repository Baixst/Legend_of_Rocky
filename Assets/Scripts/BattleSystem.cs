using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

// nur ein test

public class BattleSystem : MonoBehaviour
{
    public enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

    public StatusEffectHandler statusEffectHandler;
    public GameObject targetSelector;

    public GameObject infoBox;
    private TextMeshProUGUI infoText;

    public List<BattleUnit> battleUnits;
    public List<BattleHUD> huds;

    public GameObject combatButtonsParent;
    public GameObject moveButtonsParent;
    [HideInInspector]
    public List<Button> combatButtons = new List<Button>();
    [HideInInspector]
    public List<BattleButton> moveButtons = new List<BattleButton>();

    public TurnOrderUI turnOrderUI;
    [HideInInspector]
    public List<BattleUnit> turnOrder = new List<BattleUnit>();
    private int turnOrderIndex = 0;

    private bool cancelable = false;

    public BattleUtils utils; // TO-DO: change to private
    [HideInInspector] public BattleState state;
    private GameObject eventSystem;

    void Awake()
    {
        eventSystem = GameObject.Find("EventSystem");
    }

    void Start()
    {
        infoText = infoBox.GetComponentInChildren<TextMeshProUGUI>();
        infoBox.SetActive(false);
        targetSelector.SetActive(false);
        combatButtonsParent.SetActive(false);
        moveButtonsParent.SetActive(false);

        // get all children from the combatButtons parent object
        Button[] allCombatButtons = combatButtonsParent.GetComponentsInChildren<Button>();
        foreach (Button child in allCombatButtons)
        {
            combatButtons.Add(child);
        }

        // get all children from the moveButtons parent object
        BattleButton[] allBattleButtons = moveButtonsParent.GetComponentsInChildren<BattleButton>();
        foreach (BattleButton child in allBattleButtons)
        {
            moveButtons.Add(child);
        }

        state = BattleState.START;
        utils.SetupBattle();
        turnOrder = utils.SetupTurnOrder();

        utils.SetupTurnOrderUI(turnOrder);

        StartCoroutine(StartBattle());
    }

    private IEnumerator StartBattle()
    {
        // show initial info text
        yield return new WaitForSeconds(1);
        infoText.SetText("Fight the enemy");
        infoBox.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        infoBox.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        // start battle with the fastet unit
        if (turnOrder[turnOrderIndex].playerCharacter)
        {
            state = BattleState.PLAYER_TURN;
            PlayerTurn();
        }
        else
        {
            state = BattleState.ENEMY_TURN;
            EnemyTurn();
        }
    }

    private void PlayerTurn()
    {
        if (turnOrder[turnOrderIndex].currentHP == 0)
        {
            NextTurn();
        }
        else
        {   
            turnOrderUI.HighlightUnit(turnOrderIndex);
            turnOrder[turnOrderIndex].RegenerateAP();
            turnOrder[turnOrderIndex].isDefending = false;
            utils.UpdateHUDs();

            utils.MoveUnitForward(turnOrder[turnOrderIndex]);
            utils.UpdateButtons();
            utils.EnableCombatButtons();
            combatButtonsParent.SetActive(true);
            eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(combatButtons[0].gameObject);
        }
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYER_TURN)   return;
        utils.DisableCombatButtons();
        moveButtonsParent.SetActive(true);
        eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(moveButtons[0].gameObject);
        cancelable = true;
    }

    public void OnDefendButtonButtonWrapper()
    {
        StartCoroutine(OnDefendButton());
    }

    public IEnumerator OnDefendButton()
    {
        if (state != BattleState.PLAYER_TURN)   yield break;
        
        // set defending variable of unit to true
        turnOrder[turnOrderIndex].isDefending = true;

        combatButtonsParent.SetActive(false);
        infoText.SetText("defending");
        infoBox.SetActive(true);
        yield return new WaitForSeconds(1f); // this wait would be replaced by an attack animation

        infoBox.SetActive(false);
        yield return new WaitForSeconds(1);
        utils.MoveUnitBack(turnOrder[turnOrderIndex]);
        turnOrderUI.UnhighlightUnit(turnOrderIndex);
        NextTurn();
    }

    public void OnMoveButtonWrapper(int moveIndex) // gets called when player clicks on a attack button
    {
        StartCoroutine(OnMoveButton(moveIndex));
    }

    public IEnumerator OnMoveButton(int moveIndex)
    {
        if (state != BattleState.PLAYER_TURN)
        {
            yield break;
        }

        // disable buttons
        combatButtonsParent.SetActive(false);
        moveButtonsParent.SetActive(false);
        cancelable = false;

        // actually use move
        yield return StartCoroutine(turnOrder[turnOrderIndex].useMove(targetSelector, moveIndex));
        // check if move got canceled by player
        if (turnOrder[turnOrderIndex].moveCanceled)
        {
            turnOrder[turnOrderIndex].moveCanceled = false;
            combatButtonsParent.SetActive(true);
            moveButtonsParent.SetActive(true);
            cancelable = true;
            yield break;
        }

        // show move name in infobox
        infoText.SetText(turnOrder[turnOrderIndex].moves[moveIndex].moveName);
        infoBox.SetActive(true);
        yield return new WaitForSeconds(2); // this wait would be replaced by an attack animation

        infoBox.SetActive(false);
        utils.UpdateAfterMove();
        yield return new WaitForSeconds(1);
        utils.MoveUnitBack(turnOrder[turnOrderIndex]);
        turnOrderUI.UnhighlightUnit(turnOrderIndex);
        NextTurn();
    }

    private void NextTurn()
    {
        // only play next turn when battle is not won or lost
        if (state == BattleState.WON || state == BattleState.LOST) return;
        if (utils.PlayerWon())
        {
            state = BattleState.WON;
            EndBattle();
            return;
        }
        else if (utils.EnemyWon())
        {
            state = BattleState.LOST;
            EndBattle();
            return;
        }

        // if last unit in turn order acted: start at the beginning of the list
        if (turnOrderIndex + 1 == turnOrder.Count)
        {
            // sort turn order again, a units init could have changed
            turnOrder = utils.OrderByInitiative(turnOrder);
            turnOrderIndex = 0;
        }
        else
        {
            turnOrderIndex++;
        }

        if (turnOrder[turnOrderIndex].playerCharacter)
        {
            state = BattleState.PLAYER_TURN;
            PlayerTurn();
        }
        else
        {
            state = BattleState.ENEMY_TURN;
            EnemyTurn();
        }
    }

    private void EnemyTurn()
    {
        // check if unit is still alive
        if (turnOrder[turnOrderIndex].currentHP == 0)
        {
            NextTurn();
        }
        else
        {
            turnOrder[turnOrderIndex].RegenerateAP();
            StartCoroutine(EnemyAttack());
        }
    }

    private IEnumerator EnemyAttack()
    {
        yield return new WaitForSeconds(1f);
        turnOrderUI.HighlightUnit(turnOrderIndex);
        infoText.SetText(turnOrder[turnOrderIndex].unitName + " is attacking");
        infoBox.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        if (utils.GetPlayerUnits()[0].currentHP > 0)
        {
            utils.GetPlayerUnits()[0].TakeDamage(turnOrder[turnOrderIndex].phyAtk);
        }
        else if (utils.GetPlayerUnits()[1].currentHP > 0)
        {
            utils.GetPlayerUnits()[1].TakeDamage(turnOrder[turnOrderIndex].phyAtk);
        }
        else
        {
            utils.GetPlayerUnits()[2].TakeDamage(turnOrder[turnOrderIndex].phyAtk);
        }

        infoBox.SetActive(false);
        utils.UpdateAfterMove();
        yield return new WaitForSeconds(1.5f);
        turnOrderUI.UnhighlightUnit(turnOrderIndex);

        NextTurn();
    }

    private void EndBattle()
    {
        if (state == BattleState.WON)
        {
            infoText.SetText("You won :)");
        }
        else if (state == BattleState.LOST)
        {
            infoText.SetText("Rocky was defeated :(");
        }
        infoBox.SetActive(true);
    }

    public BattleUnit GetActiveUnit()
    {
        return turnOrder[turnOrderIndex];
    }

    public List<BattleUnit> GetPlayerUnits()
    {
        return utils.GetPlayerUnits();
    }

    public List<BattleUnit> GetEnemyUnits()
    {
        return utils.GetEnemyUnits();
    }

    public void GoBack(InputAction.CallbackContext context)
    {
        if (context.performed && cancelable)
        {
            moveButtonsParent.SetActive(false);
            utils.EnableCombatButtons();
            eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(combatButtons[0].gameObject);
            cancelable = false;
        }
    }
}