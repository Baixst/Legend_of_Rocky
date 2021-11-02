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
    public BattleHUD enemeyHUD;

    // public Transform playerPosition;
    // public Transform enemyPosition;

    public BattleState state;

    void Start()
    {
        state = BattleState.START;
        SetupBattle();
    }

    void SetupBattle()
    {
        // Instantiate(playerPrefab, playerPosition);
        // Instantiate(enemyPrefab, playerPosition);

        GameObject playerGO = Instantiate(playerPrefab);
        playerUnit = playerGO.GetComponent<BattleUnit>();

        GameObject enemyGO = Instantiate(enemyPrefab);
        enemyUnit = enemyGO.GetComponent<BattleUnit>();

        playerHUD.SetHUD(playerUnit);
        enemeyHUD.SetHUD(enemyUnit);
    }
}
