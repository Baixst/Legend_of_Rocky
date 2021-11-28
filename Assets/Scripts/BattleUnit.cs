using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public int maxAP;
    [HideInInspector] public int currentAP;
    public int APregenartion;
    public bool playerCharacter;

    public List<Move> moves = new List<Move>();

    public GameObject icon;

    [HideInInspector] public int lastTurnHP;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public bool criticalHit = false;

    private void Start()
    {
        lastTurnHP = currentHP;
        startPosition = gameObject.transform.position;
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
                int damage = calculateDamage(move, targets[i]);
                targets[i].TakeDamage(damage);
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
        
        currentAP -= move.apCost;
        selector.selectedUnit = null;
        targetSelector.SetActive(false);
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

    public void RegenerateAP()
    {
        currentAP += APregenartion;
        if (currentAP > maxAP)
        {
            currentAP = maxAP;
        }
    }

    private int calculateDamage(Move move, BattleUnit target)
    {
        int movePower = move.damage;
        int attack = 0;
        int defense = 0;

        // apply atk and def according to damage type
        if (move.damageTyp == Move.DamageTyp.Physical)
        {
            attack = phyAtk;
            defense = target.phyDef;
        }
        else
        {
            attack = magAtk;
            defense = target.magDef;
        }

        if (defense <= 0)   defense = 1;    // defense can't be 0 (can't divide by 0)
        if (attack <= 0)   attack = 1;    // negativ attack would be weird, let's not do that
        
        float crit = GetCritMultipier();
        float random = GetRandomMultiplier();

        // damage formular:
        float damage = (movePower * attack / defense + 3) * crit * random;
        
        // mark if hit was critical, so that damage number becomes red
        if (crit > 1f)  target.criticalHit = true;

        return Mathf.RoundToInt(damage);
    }

    private float GetCritMultipier()
    {
        int critChange  = 15;
        float critMultiplier = 1.25f;
        
        float randomValue = Random.Range(0f, 100f);
        if (critChange >= randomValue)
        {
            Debug.Log("CRIT!");
            return critMultiplier;
        }
        else
        {
            return 1f;
        }
    }

    private float GetRandomMultiplier()
    {
        float retVal = Random.Range(0.9f, 1.1f);
        Debug.Log("Random Multipier: " + retVal);
        return retVal;
    }
}