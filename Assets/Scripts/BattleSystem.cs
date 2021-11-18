using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleSystem : MonoBehaviour
{
    public enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

    public GameObject targetSelector;

    public GameObject infoBox;
    private TextMeshProUGUI infoText;
    public GameObject damagePopUpPrefab;

    public GameObject playerPrefab1;
    public GameObject playerPrefab2;
    public GameObject playerPrefab3;
    public GameObject enemyPrefab1;
    public GameObject enemyPrefab2;
    public GameObject enemyPrefab3;

    BattleUnit playerUnit1;
    BattleUnit playerUnit2;
    BattleUnit playerUnit3;
    BattleUnit enemyUnit1;
    BattleUnit enemyUnit2;
    BattleUnit enemyUnit3;

    public BattleHUD playerHUD1;
    public BattleHUD playerHUD2;
    public BattleHUD playerHUD3;
    public BattleHUD enemyHUD1;
    public BattleHUD enemyHUD2;
    public BattleHUD enemyHUD3;

    public GameObject buttonsParent;
    private List<BattleButton> battleButtons = new List<BattleButton>();

    private List<BattleUnit> turnOrder = new List<BattleUnit>();
    private int turnOrderIndex = 0;

    private int totalPlayerHP = 0;
    private int totalEnemyHP = 0;

    // public Transform playerPosition;
    // public Transform enemyPosition;

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
        SetupBattle();
    }

    private void SetupBattle()
    {
        // instantiate all battle units and setup their UI components
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
        if (playerPrefab3 != null)
        {
            GameObject playerGO3 = Instantiate(playerPrefab3);
            playerUnit3 = playerGO3.GetComponent<BattleUnit>();
            playerHUD3.SetHUD(playerUnit3);
            totalPlayerHP += playerUnit3.currentHP;
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
        if (enemyPrefab3 != null)
        {
            GameObject enemyGO3 = Instantiate(enemyPrefab3);
            enemyUnit3 = enemyGO3.GetComponent<BattleUnit>();
            enemyHUD3.SetHUD(enemyUnit3);
            totalEnemyHP += enemyUnit3.currentHP;
        }

        // setup turn order and start battle
        SetupTurnOrder();
        StartCoroutine(StartBattle());
    }

    private void SetupTurnOrder()
    {
        turnOrder.Add(playerUnit1);
        turnOrder.Add(enemyUnit1);

        if (playerUnit2 != null)
        {
            turnOrder.Add(playerUnit2);
        }
        if (playerUnit3 != null)
        {
            turnOrder.Add(playerUnit3);
        }
        if (enemyUnit2 != null)
        {
            turnOrder.Add(enemyUnit2);
        }
        if (enemyUnit3 != null)
        {
            turnOrder.Add(enemyUnit3);
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
            updateButtons();
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
        UpdateAfterMove();
        yield return new WaitForSeconds(2);
        nextTurn();
    }

    private void UpdateAfterMove()
    {
        // Update HP value in UI 
        updateHUDs();
        updateHPTrackers();

        foreach (BattleUnit unit in turnOrder)
        {
            if (unit.currentHP < unit.lastTurnHP) // check if unit has taken damage
            {
                GameObject DamageText = Instantiate(damagePopUpPrefab);
                DamageText.transform.position = unit.transform.position;
                DamageText.transform.GetChild(0).GetComponent<TextMeshPro>().SetText(((unit.lastTurnHP - unit.currentHP).ToString()));
                unit.lastTurnHP = unit.currentHP;
            }
            else if (unit.currentHP > unit.lastTurnHP) // check if unit was healed
            {
                GameObject DamageText = Instantiate(damagePopUpPrefab);
                DamageText.transform.position = unit.transform.position;
                DamageText.transform.GetChild(0).GetComponent<TextMeshPro>().SetText(((unit.currentHP - unit.lastTurnHP).ToString()));
                DamageText.transform.GetChild(0).GetComponent<TextMeshPro>().color = new Color32(32, 193, 51, 255); // set text color to green
                unit.lastTurnHP = unit.currentHP;
            }
        } 
    }

    private void nextTurn()
    {
        // only play next turn when battle is not won or lost
        if (state == BattleState.WON || state == BattleState.LOST) return;
        if (totalEnemyHP == 0)
        {
            state = BattleState.WON;
            EndBattle();
            return;
        }
        else if (totalPlayerHP == 0)
        {
            state = BattleState.LOST;
            EndBattle();
            return;
        }

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
        infoText.SetText(turnOrder[turnOrderIndex].unitName + " is attacking");
        infoBox.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        if (playerUnit1.currentHP > 0)
        {
            playerUnit1.TakeDamage(turnOrder[turnOrderIndex].physicalAttack);
        }
        else if (playerUnit2.currentHP > 0)
        {
            playerUnit2.TakeDamage(turnOrder[turnOrderIndex].physicalAttack);
        }
        else
        {
            playerUnit3.TakeDamage(turnOrder[turnOrderIndex].physicalAttack);
        }

        infoBox.SetActive(false);
        UpdateAfterMove();
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

    private void updateButtons()
    {
        foreach (BattleButton b in battleButtons)
        {
            b.changeTextToActiveUnit();
        }
    }

    private void updateHUDs()
    {
        playerHUD1.SetHP(playerUnit1.currentHP, playerUnit1);
        if (playerPrefab2 != null)
        {
            playerHUD2.SetHP(playerUnit2.currentHP, playerUnit2);
        }
        if (playerPrefab3 != null)
        {
            playerHUD3.SetHP(playerUnit3.currentHP, playerUnit3);
        }

        enemyHUD1.SetHP(enemyUnit1.currentHP, enemyUnit1);
        if (enemyPrefab2 != null)
        {
            enemyHUD2.SetHP(enemyUnit2.currentHP, enemyUnit2);
        }
        if (enemyPrefab3 != null)
        {
            enemyHUD3.SetHP(enemyUnit3.currentHP, enemyUnit3);
        }
    }

    private void updateHPTrackers()
    {
        totalPlayerHP = playerUnit1.currentHP;
        if (playerPrefab2 != null)
        {
            totalPlayerHP += playerUnit2.currentHP;
        }
        if (playerPrefab3 != null)
        {
            totalPlayerHP += playerUnit3.currentHP;
        }

        totalEnemyHP = enemyUnit1.currentHP;
        if (enemyPrefab2 != null)
        {
            totalEnemyHP += enemyUnit2.currentHP;
        }
        if (enemyPrefab3 != null)
        {
            totalEnemyHP += enemyUnit3.currentHP;
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
        if (playerUnit3 != null)
        {
            partyUnits.Add(playerUnit3);
        }
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
        if (enemyUnit3 != null)
        {
            enemyUnits.Add(enemyUnit3);
        }
        return enemyUnits;
    }
}