using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoBehaviour
{

    public enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    BattleUnit playerUnit;
    BattleUnit enemyUnit;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public GameObject combatButtons;

    private List<BattleUnit> turnOrder = new List<BattleUnit>();
    private int turnOrderIndex = 0;

    // public Transform playerPosition;
    // public Transform enemyPosition;

    public BattleState state;

    void Start()
    {
        combatButtons.gameObject.SetActive(false);
        state = BattleState.START;
        SetupBattle();
    }

    private void SetupBattle()
    {
        // Instantiate(playerPrefab, playerPosition);
        // Instantiate(enemyPrefab, playerPosition);

        GameObject playerGO = Instantiate(playerPrefab);
        playerUnit = playerGO.GetComponent<BattleUnit>();

        GameObject enemyGO = Instantiate(enemyPrefab);
        enemyUnit = enemyGO.GetComponent<BattleUnit>();

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        SetupTurnOrder();

        if (turnOrder[turnOrderIndex].playerCharacter)
        {
            state = BattleState.PLAYER_TURN;
            PlayerTurn();
        }
        else
        {
            state = BattleState.ENEMY_TURN;
            StartCoroutine(EnemyTurn());
        }
    }

    private void SetupTurnOrder()
    {
        turnOrder.Add(playerUnit);
        turnOrder.Add(enemyUnit);

        turnOrder = orderByInitiative(turnOrder);

        Debug.Log("First in order: " + turnOrder[0].unitName);
        Debug.Log("Speed of first unit: " + turnOrder[0].init);
    }

    private List<BattleUnit> orderByInitiative(List<BattleUnit> list)
    {
        // orders list ascending by init stat -> unit with lowest init is first in list
        list.Sort((x, y) => x.init.CompareTo(y.init));

        // reverse list so that unit with highest init is first in list
        list.Reverse();
        return list;
    }

    private void PlayerTurn()
    {
        combatButtons.gameObject.SetActive(true);
    }

    public void OnMoveButton(int moveIndex)
    {
        if (state != BattleState.PLAYER_TURN)
        {
            return;
        }

        // disable buttons
        combatButtons.gameObject.SetActive(false);

        turnOrder[turnOrderIndex].useMove(enemyUnit, moveIndex);
        PlayerAttack();
    }

    private void PlayerAttack()
    {   
        // Update HP value in UI      
        enemyHUD.SetHP(enemyUnit.currentHP, enemyUnit);

        if (enemyUnit.currentHP == 0)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            nextTurn();
        }
    }

    private void nextTurn()
    {
        // if last unit in turn order acted: start at the beginning of the list
        if (turnOrderIndex + 1 == turnOrder.Count)
        {
            // sort turn order again, a units init could have changed
            turnOrder = orderByInitiative(turnOrder);
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
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy is attacking");

        playerUnit.TakeDamage(enemyUnit.physicalAttack);
        playerHUD.SetHP(playerUnit.currentHP, playerUnit);

        yield return new WaitForSeconds(1f);

        if (playerUnit.currentHP == 0)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            nextTurn();
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            Debug.Log("Rocky won the battle!");
        }
        else if (state == BattleState.LOST)
        {
            Debug.Log("Stone Knight won the battle");
        }
    }
}