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

    public Button attackButton;
    public Button healButton;

    // public Transform playerPosition;
    // public Transform enemyPosition;

    public BattleState state;

    void Start()
    {
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

        state = BattleState.PLAYER_TURN;
        PlayerTurn();
    }

    private void PlayerTurn()
    {
        attackButton.gameObject.SetActive(true);
        healButton.gameObject.SetActive(true);
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYER_TURN)
        {
            return;
        }

        StartCoroutine(PlayerAttack());
    }

    IEnumerator PlayerAttack()
    {
        // Damage the enemy
        Debug.Log("Player attacks for " + playerUnit.physicalAttack + " points of damage!");
        Debug.Log("Enemy HP before attack: " + enemyUnit.currentHP);
        enemyUnit.TakeDamage(playerUnit.physicalAttack);
        Debug.Log("Enemy HP after attack: " + enemyUnit.currentHP);
        enemyHUD.SetHP(enemyUnit.currentHP, enemyUnit);
        Debug.Log("Succesfull attack");

        yield return new WaitForSeconds(1f);

        if (enemyUnit.currentHP == 0)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            attackButton.gameObject.SetActive(false);
            healButton.gameObject.SetActive(false);
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
        } else
        {
            state = BattleState.PLAYER_TURN;
            PlayerTurn();
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
