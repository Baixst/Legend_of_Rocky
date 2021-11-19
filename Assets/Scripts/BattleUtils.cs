using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleUtils : MonoBehaviour
{
    public BattleSystem battleSystem;
    public GameObject damagePopUpPrefab;

    private List<GameObject> unitPrefabs = new List<GameObject>();
    private List<BattleUnit> units = new List<BattleUnit>();
    private List<BattleHUD> huds = new List<BattleHUD>();
    private int totalPlayerHP = 0;
    private int totalEnemyHP = 0;

    public void SetupBattle()
    {
        if (battleSystem.playerPrefab1 != null)  unitPrefabs.Add(battleSystem.playerPrefab1);
        if (battleSystem.playerPrefab2 != null)   unitPrefabs.Add(battleSystem.playerPrefab2);
        if (battleSystem.playerPrefab3 != null)   unitPrefabs.Add(battleSystem.playerPrefab3);
        if (battleSystem.enemyPrefab1 != null)   unitPrefabs.Add(battleSystem.enemyPrefab1);
        if (battleSystem.enemyPrefab2 != null)   unitPrefabs.Add(battleSystem.enemyPrefab2);
        if (battleSystem.enemyPrefab3 != null)   unitPrefabs.Add(battleSystem.enemyPrefab3);

        if (battleSystem.playerHUD1 != null)   huds.Add(battleSystem.playerHUD1);
        if (battleSystem.playerHUD2 != null)   huds.Add(battleSystem.playerHUD2);
        if (battleSystem.playerHUD3 != null)   huds.Add(battleSystem.playerHUD3);
        if (battleSystem.enemyHUD1 != null)   huds.Add(battleSystem.enemyHUD1);
        if (battleSystem.enemyHUD2 != null)   huds.Add(battleSystem.enemyHUD2);
        if (battleSystem.enemyHUD3 != null)   huds.Add(battleSystem.enemyHUD3);

        foreach (GameObject unitPrefab in unitPrefabs)
        {
            GameObject temp = Instantiate(unitPrefab);
            units.Add(temp.GetComponent<BattleUnit>());
        }

        for (int i = 0; i < huds.Count; i++)
        {
            huds[i].SetHUD(units[i]);
        }

        foreach (BattleUnit unit in units)
        {
            if (unit.playerCharacter)
            {
                totalPlayerHP += unit.currentHP;
            }
            else
            {
                totalEnemyHP += unit.currentHP;
            }
        }
    }

    public List<BattleUnit> SetupTurnOrder()
    {
        List<BattleUnit> turnOrder = new List<BattleUnit>();
        
        foreach (BattleUnit unit in units)
        {
            turnOrder.Add(unit);
        }
        
        turnOrder = orderByInitiative(turnOrder);

        Debug.Log("First in order: " + turnOrder[0].unitName);
        Debug.Log("Speed of first unit: " + turnOrder[0].init);

        return turnOrder;
    }

    public List<BattleUnit> orderByInitiative(List<BattleUnit> list)
    {
        // orders list ascending by init stat -> unit with lowest init is first in list
        list.Sort((x, y) => x.init.CompareTo(y.init));

        // reverse list so that unit with highest init is first in list
        list.Reverse();
        return list;
    }

    public void UpdateAfterMove()
    {
        // Update HP value in UI 
        updateHUDs();
        updateHPTrackers();

        foreach (BattleUnit unit in units)
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

    private void updateHPTrackers()
    {
        totalPlayerHP = 0;
        totalEnemyHP = 0;
        foreach (BattleUnit unit in units)
        {
            if (unit.playerCharacter)
            {
                totalPlayerHP += unit.currentHP;
            }
            else
            {
                totalEnemyHP += unit.currentHP;
            }
        }
    }

    private void updateHUDs()
    {
        for (int i = 0; i < units.Count; i++)
        {
            huds[i].SetHP(units[i].currentHP, units[i]);
        }
    }

    public void updateButtons()
    {
        foreach (BattleButton b in battleSystem.battleButtons)
        {
            b.changeTextToActiveUnit();
        }
    }

    public List<BattleUnit> getPlayerUnits()
    {
        List<BattleUnit> playerUnits = new List<BattleUnit>();
        
        foreach (BattleUnit unit in units)
        {
            if(unit.playerCharacter)
            {
                playerUnits.Add(unit);
            }
        }
        return playerUnits;
    }

    public List<BattleUnit> getEnemyUnits()
    {
        List<BattleUnit> enemyUnits = new List<BattleUnit>();
        
        foreach (BattleUnit unit in units)
        {
            if(!unit.playerCharacter)
            {
                enemyUnits.Add(unit);
            }
        }
        return enemyUnits;
    }

    public bool PlayerWon()
    {
        if (totalPlayerHP == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool EnemyWon()
    {
        if (totalEnemyHP == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}