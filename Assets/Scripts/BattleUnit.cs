using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public int level;
    public string unitName;
    public int maxHP;
    public int currentHP;
    public int phyAtk;
    public int phyDef;
    public int magAtk;
    public int magDef;
    public int init;
    public bool playerCharacter;

    public List<Move> moves = new List<Move>();

    [HideInInspector]
    public int lastTurnHP;

    private void Start()
    {
        lastTurnHP = currentHP;
    }

    public IEnumerator useMove(GameObject targetSelector, int moveIndex)
    {
        Move move = moves[moveIndex];
        List<BattleUnit> targets = new List<BattleUnit>();
        targetSelector.SetActive(true);
        TargetSelector selector = targetSelector.GetComponent<TargetSelector>();

        // start finding a target
        selector.findPossibleTargets(move);
        if (move.numberOfTargets == 1)
        {
            // set position to first possible target
            var tmp = selector.possibleTargets[0].transform.position;
            tmp.y += 0.5f;
            targetSelector.transform.position = tmp;

            // wait until target is selected by user
            yield return new WaitUntil(() => selector.selectedUnit != null);
        }

        if (move.numberOfTargets > 1)
        {
            targets = selector.possibleTargets;
        }
        else
        {
            targets.Add(selector.selectedUnit);
        }
        
        for (int i = 0; i < targets.Count; i++)
        {
            Debug.Log("Attacking " + targets[i].unitName);

            // check if move heals or deals damage
            if (move.healing > 0) // move heals
            {
                targets[i].HealFor(move.healing);
            }
            else if (move.damage > 0) // move deals damage
            {
                targets[i].TakeDamage(move.damage);
            }

            // check if move applies a buff
            if (move.canBuff)
            {
                targets[i].BuffStat(move.buffStat, move.buffValue);
            }

            // check if move applies a Debuff
            if (move.canDebuff)
            {
                targets[i].DebuffStat(move.debuffStat, move.debuffValue);
            }
        }

        selector.selectedUnit = null;
        targetSelector.SetActive(false);
        Debug.Log(unitName + " is using " + move.moveName);
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP < 0)
        {
            currentHP = 0;
        }
    }

    public void HealFor(int healing)
    {
        currentHP += healing;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    public void BuffStat(string stat, int buffValue)
    {
        switch (stat)
        {
            case "attack":
                phyAtk += buffValue;
                break;

            case "defense":
                phyDef += buffValue;
                break;

            case "init":
                init += buffValue;
                break;

            default:
                Debug.Log("Unable to find stat: " + stat);
                break;
        }
    }

    public void DebuffStat(string stat, int debuffValue)
    {
        switch (stat)
        {
            case "attack":
                phyAtk -= debuffValue;
                break;

            case "defense":
                phyDef -= debuffValue;
                break;

            case "init":
                init -= debuffValue;
                break;

            default:
                Debug.Log("Unable to find stat: " + stat);
                break;
        }
    }
}


