using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleSystem : MonoBehaviour
{
    public enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

    public GameObject targetSelector;

    public GameObject infoBox;
    private TextMeshProUGUI infoText;

    public GameObject playerPrefab1;
    public GameObject playerPrefab2;
    public GameObject playerPrefab3;
    public GameObject enemyPrefab1;
    public GameObject enemyPrefab2;
    public GameObject enemyPrefab3;

    public BattleHUD playerHUD1;
    public BattleHUD playerHUD2;
    public BattleHUD playerHUD3;
    public BattleHUD enemyHUD1;
    public BattleHUD enemyHUD2;
    public BattleHUD enemyHUD3;

    public GameObject buttonsParent;
    [HideInInspector]
    public List<BattleButton> battleButtons = new List<BattleButton>();

    private List<BattleUnit> turnOrder = new List<BattleUnit>();
    private int turnOrderIndex = 0;

    public BattleUtils utils; // TO-DO: change to private

    public BattleState state;

    void Start()
    {
        infoText = infoBox.GetComponentInChildren<TextMeshProUGUI>();
        infoBox.SetActive(false);
        targetSelector.SetActive(false);
        buttonsParent.SetActive(false);

        // get all children from the battleButtons parent object
        BattleButton[] allChildren = buttonsParent.GetComponentsInChildren<BattleButton>();
        foreach (BattleButton child in allChildren)
        {
            battleButtons.Add(child);
        }

        state = BattleState.START;
        utils.SetupBattle();
        turnOrder = utils.SetupTurnOrder();
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
            nextTurn();
        }
        else
        {   
            utils.MoveUnitForward(turnOrder[turnOrderIndex]);
            utils.updateButtons();
            buttonsParent.SetActive(true);
        }
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
        buttonsParent.SetActive(false);

        // actually use move
        yield return StartCoroutine(turnOrder[turnOrderIndex].useMove(targetSelector, moveIndex));

        // show move name in infobox
        infoText.SetText(turnOrder[turnOrderIndex].moves[moveIndex].moveName);
        infoBox.SetActive(true);
        yield return new WaitForSeconds(2); // this wait would be replaced by an attack animation

        infoBox.SetActive(false);
        utils.UpdateAfterMove();
        yield return new WaitForSeconds(1);
        utils.MoveUnitBack(turnOrder[turnOrderIndex]);
        // yield return new WaitForSeconds(2);
        nextTurn();
    }

    private void nextTurn()
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
            turnOrder = utils.orderByInitiative(turnOrder);
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
            nextTurn();
        }
        else
        {
            StartCoroutine(EnemyAttack());
        }
    }

    private IEnumerator EnemyAttack()
    {
        yield return new WaitForSeconds(1f);
        infoText.SetText(turnOrder[turnOrderIndex].unitName + " is attacking");
        infoBox.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        if (utils.getPlayerUnits()[0].currentHP > 0)
        {
            utils.getPlayerUnits()[0].TakeDamage(turnOrder[turnOrderIndex].phyAtk);
        }
        else if (utils.getPlayerUnits()[1].currentHP > 0)
        {
            utils.getPlayerUnits()[1].TakeDamage(turnOrder[turnOrderIndex].phyAtk);
        }
        else
        {
            utils.getPlayerUnits()[2].TakeDamage(turnOrder[turnOrderIndex].phyAtk);
        }

        infoBox.SetActive(false);
        utils.UpdateAfterMove();
        yield return new WaitForSeconds(1.5f);

        nextTurn();
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

    public BattleUnit getActiveUnit()
    {
        return turnOrder[turnOrderIndex];
    }

    public List<BattleUnit> getPlayerUnits()
    {
        return utils.getPlayerUnits();
    }

    public List<BattleUnit> getEnemyUnits()
    {
        return utils.getEnemyUnits();
    }
}