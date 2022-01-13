using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class BattleSystem : MonoBehaviour
{
    public enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

    public StatusEffectHandler statusEffectHandler;
    public EnemyController enemyController;
    public GameObject targetSelector;

    public GameObject infoBox;
    private TextMeshProUGUI infoText;

    public List<BattleUnit> battleUnits;
    public List<BattleHUD> huds;

    public GameObject combatButtonsParent;
    public GameObject moveButtonsParent;
    [HideInInspector] public List<Button> combatButtons = new List<Button>();
    [HideInInspector] public List<BattleButton> moveButtons = new List<BattleButton>();

    public TurnOrderUI turnOrderUI;
    [HideInInspector] public List<BattleUnit> turnOrder = new List<BattleUnit>();
    private int turnOrderIndex = 0;

    private bool cancelable = false;

    public BattleUtils utils; // TO-DO: change to private
    [HideInInspector] public BattleState state;

    private EventSystem eventSystem;

    public void Awake()
    {
        GameObject temp = GameObject.Find("EventSystem");
        eventSystem = temp.GetComponent<EventSystem>();
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
        infoText.SetText("Besiege alle Gegner!");
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
            StartCoroutine(SelectButton(combatButtons[0].gameObject));
        }
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYER_TURN)   return;
        utils.DisableCombatButtons();
        moveButtonsParent.SetActive(true);
        StartCoroutine(SelectButton(moveButtons[0].gameObject));
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
        infoText.SetText(turnOrder[turnOrderIndex].name + " schützt sich");
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
        yield return new WaitForSeconds(1); // this wait would be replaced by an attack animation

        Animator unitAnimator = turnOrder[turnOrderIndex].gameObject.GetComponent<Animator>();
        if (unitAnimator != null)
        {
            // play the attack animation
            unitAnimator.SetTrigger("Attack");
            turnOrder[turnOrderIndex].isIdeling = false;

            // wait until animation is finished
            yield return new WaitUntil(() => turnOrder[turnOrderIndex].isIdeling);
        }

        utils.UpdateAfterMove();
        infoBox.SetActive(false);
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
            turnOrder[turnOrderIndex].isDefending = false;
            turnOrder[turnOrderIndex].RegenerateAP();
            StartCoroutine(EnemyAttack());
        }
    }

    private IEnumerator EnemyAttack()
    {
        yield return new WaitForSeconds(1f);
        turnOrderUI.HighlightUnit(turnOrderIndex);

        // Move Unit forward
        utils.MoveUnitForward(turnOrder[turnOrderIndex]);

        // Choose Move the unit should use
        Move move = enemyController.ChooseMove(turnOrder[turnOrderIndex]);
        
        if (turnOrder[turnOrderIndex].isDefending)
        {
            infoText.SetText(turnOrder[turnOrderIndex].unitName + " schützt sich");
        }
        else
        {
            // Choose Target
            List<BattleUnit> targets = enemyController.ChooseTargets(move, turnOrder[turnOrderIndex]);

            // Use move on target list
            turnOrder[turnOrderIndex].EnemyUseMove(move, targets);

            infoText.SetText(turnOrder[turnOrderIndex].unitName + " setzt " + move.moveName + " ein.");
        }
        


        infoBox.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        // TO-DO: Resolve attack on target

        infoBox.SetActive(false);
        utils.UpdateAfterMove();
        yield return new WaitForSeconds(1.5f);
        turnOrderUI.UnhighlightUnit(turnOrderIndex);

        // Move Unit back
        utils.MoveUnitBack(turnOrder[turnOrderIndex]);

        NextTurn();
    }

    private void EndBattle()
    {
        if (state == BattleState.WON)
        {
            infoText.SetText("Rocky ist siegreich!");
        }
        else if (state == BattleState.LOST)
        {
            infoText.SetText("Rocky wurde geschlagen!");
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

            StartCoroutine(SelectButton(combatButtons[0].gameObject));
            cancelable = false;
        }
    }

    private IEnumerator SelectButton(GameObject button)
    {
        yield return null;
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(button);
    }
}