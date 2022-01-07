using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUtils : MonoBehaviour
{
    public BattleSystem battleSystem;
    public GameObject damagePopUpPrefab;

    public Color32 criticalDamageColor;
    public Color32 healingNumbersColor;

    private List<BattleUnit> units = new List<BattleUnit>();
    private List<BattleHUD> huds = new List<BattleHUD>();
    private int totalPlayerHP = 0;
    private int totalEnemyHP = 0;

    public void SetupBattle()
    {
        foreach (BattleUnit unit in battleSystem.battleUnits)
        {
            units.Add(unit);
        }

        foreach (BattleHUD hud in battleSystem.huds)
        {
            huds.Add(hud);
        }

        foreach (BattleUnit unit in units)
        {
            // set currentAP to maxAP at start of battle
            unit.currentAP = unit.maxAP;

            // setup HP trackers
            if (unit.playerCharacter)
            {
                totalPlayerHP += unit.currentHP;
            }
            else
            {
                totalEnemyHP += unit.currentHP;
            }
        }

        for (int i = 0; i < huds.Count; i++)
        {
            huds[i].SetHUD(units[i]);
        }
    }

    public List<BattleUnit> SetupTurnOrder()
    {
        List<BattleUnit> turnOrder = new List<BattleUnit>();
        
        foreach (BattleUnit unit in units)
        {
            turnOrder.Add(unit);
        }
        
        turnOrder = OrderByInitiative(turnOrder);
        return turnOrder;
    }

    public List<BattleUnit> OrderByInitiative(List<BattleUnit> list)
    {
        // orders list ascending by init stat -> unit with lowest init is first in list
        list.Sort((x, y) => x.init.CompareTo(y.init));

        // reverse list so that unit with highest init is first in list
        list.Reverse();
        return list;
    }

    public void SetupTurnOrderUI(List<BattleUnit> turnOrder)
    {
        foreach (BattleUnit unit in turnOrder)
        {
            battleSystem.turnOrderUI.AddUnit(unit);
        }
    }

    public void UpdateAfterMove()
    {
        // Update HP value in UI 
        UpdateHUDs();
        UpdateHPTrackers();
        battleSystem.turnOrderUI.UpdateAfterMove(battleSystem.turnOrder);

        foreach (BattleUnit unit in units)
        {
            if (unit.currentHP < unit.lastTurnHP) // check if unit has taken damage
            {
                StartCoroutine(PlayDamageTakeAnimations(unit));
            }
            else if (unit.currentHP > unit.lastTurnHP) // check if unit was healed
            {
                var spawnPosition = unit.transform.position;
                spawnPosition.z -= 5f; // without that, the damage number is behind the unite sprite
                GameObject damageText = Instantiate(damagePopUpPrefab);
                damageText.transform.position = spawnPosition;
                damageText.transform.GetChild(0).GetComponent<TextMeshPro>().SetText(((unit.currentHP - unit.lastTurnHP).ToString()));
                damageText.transform.GetChild(0).GetComponent<TextMeshPro>().color = healingNumbersColor;
                unit.lastTurnHP = unit.currentHP;
            }
        } 
    }

    private IEnumerator PlayDamageTakeAnimations(BattleUnit unit)
    {
        if (unit.gameObject.GetComponent<Animator>() != null)
        {
            unit.gameObject.GetComponent<Animator>().SetTrigger("TakeDamage");
        }

        yield return new WaitForSeconds(0.5f);

        // damage popup
        var spawnPosition = unit.transform.position;
        spawnPosition.z -= 5f; // without that, the damage number is behind the unite sprite
        GameObject damageText = Instantiate(damagePopUpPrefab);
        damageText.transform.position = spawnPosition;
        damageText.transform.GetChild(0).GetComponent<TextMeshPro>().SetText(((unit.lastTurnHP - unit.currentHP).ToString()));
        if (unit.criticalHit)
        {
            damageText.transform.GetChild(0).GetComponent<TextMeshPro>().color = criticalDamageColor;
            unit.criticalHit = false;
        }
        unit.lastTurnHP = unit.currentHP;
    }

    private void UpdateHPTrackers()
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

    public void UpdateHUDs()
    {
        for (int i = 0; i < units.Count; i++)
        {
            huds[i].SetHP(units[i].currentHP, units[i]);
            huds[i].SetAP(units[i].currentAP, units[i]);
        }
        battleSystem.statusEffectHandler.UpdateStatusEffects();
    }

    public void UpdateButtons()
    {
        foreach (BattleButton button in battleSystem.moveButtons)
        {
            button.ChangeTextToActiveUnit();
            
            BattleUnit activeUnit = battleSystem.GetActiveUnit();
            Move move = activeUnit.moves[button.moveIndex];

            if (move.apCost > activeUnit.currentAP)
            {
                button.GetComponent<Button>().interactable = false;
            }
            else
            {
                button.GetComponent<Button>().interactable = true;
            }
        }
    }

    public void DisableCombatButtons()
    {
        foreach (Button b in battleSystem.combatButtons)
        {
            b.GetComponent<Button>().interactable = false;
        }
    }

    public void EnableCombatButtons()
    {
        foreach (Button b in battleSystem.combatButtons)
        {
            b.GetComponent<Button>().interactable = true;
        }
    }

    public List<BattleUnit> GetPlayerUnits()
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

    public List<BattleUnit> GetEnemyUnits()
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
        if (totalEnemyHP == 0)
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
        if (totalPlayerHP == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void MoveUnitForward(BattleUnit activeUnit)
    {
        if (activeUnit.playerCharacter)
        {
            StartCoroutine(MoveOverSeconds(activeUnit.gameObject, new Vector3 (-2f, 0f, 0f), 0.2f));
        }
        else
        {
            StartCoroutine(MoveOverSeconds(activeUnit.gameObject, new Vector3 (2f, 0f, 0f), 0.2f));
        }
    }

    public void MoveUnitBack(BattleUnit activeUnit)
    {
        StartCoroutine(MoveOverSeconds(activeUnit.gameObject, activeUnit.startPosition, 0.2f));
    }

    public IEnumerator MoveOverSeconds (GameObject objectToMove, Vector3 endPosition, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, endPosition, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    objectToMove.transform.position = endPosition;
    }
}