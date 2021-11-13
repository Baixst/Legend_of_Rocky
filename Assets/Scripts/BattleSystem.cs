using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoBehaviour
{
    public enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

    public GameObject targetSelector;

    public GameObject playerPrefab1;
    public GameObject playerPrefab2;
    public GameObject enemyPrefab1;
    public GameObject enemyPrefab2;

    BattleUnit playerUnit1;
    BattleUnit playerUnit2;
    BattleUnit enemyUnit1;
    BattleUnit enemyUnit2;

    public BattleHUD playerHUD1;
    public BattleHUD playerHUD2;
    public BattleHUD enemyHUD1;
    public BattleHUD enemyHUD2;

    public GameObject combatButtons;

    private List<BattleUnit> turnOrder = new List<BattleUnit>();
    private int turnOrderIndex = 0;
    
    private int totalEnemyHP = 0;
    private int totalPlayerHP = 0;

    // public Transform playerPosition;
    // public Transform enemyPosition;

    public BattleState state;

    void Start()
    {
        targetSelector.SetActive(false);
        combatButtons.gameObject.SetActive(false);
        state = BattleState.START;
        SetupBattle();
    }

    private void SetupBattle()
    {
        // Instantiate(playerPrefab, playerPosition);
        // Instantiate(enemyPrefab, playerPosition);F

        GameObject playerGO1 = Instantiate(playerPrefab1);
        playerUnit1 = playerGO1.GetComponent<BattleUnit>();
        playerHUD1.SetHUD(playerUnit1);
        totalPlayerHP += playerUnit1.currentHP;

        if (playerPrefab2 != null)
        {
            GameObject playerGO2 = Instantiate(playerPrefab2);
            playerUnit2 = playerGO2.GetComponent<BattleUnit>();
            playerHUD2.SetHUD(playerUnit2);
            totalPlayerHP += playerUnit2.currentHP;
        }

        GameObject enemyGO1 = Instantiate(enemyPrefab1);
        enemyUnit1 = enemyGO1.GetComponent<BattleUnit>();
        enemyHUD1.SetHUD(enemyUnit1);
        totalEnemyHP += enemyUnit1.currentHP;

        if (enemyPrefab2 != null)
        {
            GameObject enemyGO2 = Instantiate(enemyPrefab2);
            enemyUnit2 = enemyGO2.GetComponent<BattleUnit>();
            enemyHUD2.SetHUD(enemyUnit2);
            totalEnemyHP += enemyUnit2.currentHP;
        }

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
        turnOrder.Add(playerUnit1);
        turnOrder.Add(enemyUnit1);

        if (playerUnit2 != null)
        {
            turnOrder.Add(playerUnit2);
        }
        if (enemyUnit2 != null)
        {
            turnOrder.Add(enemyUnit2);
        }

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
        if (turnOrder[turnOrderIndex].currentHP == 0)
        {
            nextTurn();
        }
        else
        {
            combatButtons.gameObject.SetActive(true);
        }
    }

    public void OnMoveButtonWrapper(int moveIndex)
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
        combatButtons.gameObject.SetActive(false);
        
        yield return StartCoroutine(turnOrder[turnOrderIndex].useMove(targetSelector, moveIndex));
        PlayerAttack();
    }

    private void PlayerAttack()
    {
        // Update HP value in UI and logic  
        updateHUDs(); 
        updateHPTrackers();  

        if (totalEnemyHP == 0)
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

        if (playerUnit1.currentHP > 0)
        {
            playerUnit1.TakeDamage(enemyUnit1.physicalAttack);
        }
        else
        {
            playerUnit2.TakeDamage(enemyUnit1.physicalAttack);
        }

        updateHUDs();
        updateHPTrackers();

        yield return new WaitForSeconds(1f);

        if (totalPlayerHP == 0)
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

    private void updateHUDs()
    {
        playerHUD1.SetHP(playerUnit1.currentHP, playerUnit1);
        if (playerPrefab2 != null)
        {
            playerHUD2.SetHP(playerUnit2.currentHP, playerUnit2);
        }

        enemyHUD1.SetHP(enemyUnit1.currentHP, enemyUnit1);
        if (enemyPrefab2 != null)
        {
            enemyHUD2.SetHP(enemyUnit2.currentHP, enemyUnit2);
        }
    }

    private void updateHPTrackers()
    {
        totalPlayerHP = playerUnit1.currentHP;
        if (playerPrefab2 != null)
        {
            totalPlayerHP += playerUnit2.currentHP;
        }

        totalEnemyHP = enemyUnit2.currentHP;
        if (enemyPrefab2 != null)
        {
            totalEnemyHP += enemyUnit2.currentHP;
        }
    }

    public BattleUnit getActiveUnit()
    {
        return turnOrder[turnOrderIndex];
    }

    public List<BattleUnit> getPartyUnits()
    {
        List<BattleUnit> partyUnits = new List<BattleUnit>();
        partyUnits.Add(playerUnit1);
        if (playerUnit2 != null)
        {
            partyUnits.Add(playerUnit2);
        }
        // add later:
        // if(playerUnit3 != null) partyUnits.Add(playerUnit3);
        return partyUnits;
    }

    public List<BattleUnit> getEnemyUnits()
    {
        List<BattleUnit> enemyUnits = new List<BattleUnit>();
        enemyUnits.Add(enemyUnit1);
        if (enemyUnit2 != null)
        {
            enemyUnits.Add(enemyUnit2);
        }
        // add later:
        // if(enemyUnit3 != null) enemyUnits.Add(enemyUnit3);
        return enemyUnits;
    }
}