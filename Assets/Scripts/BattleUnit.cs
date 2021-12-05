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
    public List<string> buffs = new List<string>();
    public List<string> debuffs = new List<string>();

    public List<Move> moves = new List<Move>();

    public GameObject icon;

    [HideInInspector] public int lastTurnHP;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public bool criticalHit = false;
    [HideInInspector] public bool isDefending = false;
    [HideInInspector] public bool hasMight, hasManaRush, hasBarrier, hasArmorBreak = false;

    [HideInInspector] public bool moveCanceled = false;

    private void Start()
    {
        lastTurnHP = currentHP;
        startPosition = gameObject.transform.position;
        foreach (Move move in moves)
        {
            move.WakeUp();
        }
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

            // wait until target is selected by user or selection gets canceled
            yield return new WaitUntil(() => selector.selectedUnit != null || selector.canceled);
            if (selector.canceled)
            {
                selector.canceled = false;  // reset bool
                targetSelector.SetActive(false);
                moveCanceled = true;
                yield break;
            }
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
            if (move.buffs.Count > 0)
            {
                foreach (string buff in move.buffs)
                {
                    targets[i].AddBuff(buff);
                }
            }

            // check if move applies a Debuff
            if (move.debuffs.Count > 0)
            {
                foreach (string debuff in move.debuffs)
                {
                    targets[i].AddDebuff(debuff);
                }
            }

            // check if move removes a Buff
            if (move.removeBuffs > 0)
            {
                targets[i].RemoveBuffs(move.removeBuffs);
            }

            // check if move removes a Debuff
            if (move.removeDebuffs > 0)
            {
                targets[i].RemoveDebuffs(move.removeDebuffs);
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

    public void AddBuff(string effect)
    {
        switch (effect)
        {
            case "Might":
                if (!hasMight)
                {
                    hasMight = true;
                    buffs.Add(effect);
                }
                break;

            case "ManaRush":
                if (!hasManaRush)
                {
                    hasManaRush = true;
                    buffs.Add(effect);
                }
                break;

            case "Barrier":
                if (!hasBarrier)
                {
                    hasBarrier = true;
                    buffs.Add(effect);
                }
                break;

            default:
                Debug.Log("Unable to find buff: " + effect);
                break;
        }
    }

    public void AddDebuff(string effect)
    {
        switch (effect)
        {
            case "ArmorBreak":
                if (!hasArmorBreak)
                {
                    hasArmorBreak = true;
                    debuffs.Add(effect);
                }
                break;

            default:
                Debug.Log("Unable to find stat: " + effect);
                break;
        }
    }

    public void RemoveBuffs(int amount)
    {
        if (buffs.Count == 0)    return;

        for (int i = 0; i < amount; i++)
        {
            if (buffs[0] == "Might")
            {
                hasMight = false;
                buffs.RemoveAt(0);
            }
            else if (buffs[0] == "ManaRush")
            {
                hasManaRush = false;
                buffs.RemoveAt(0);
            }
            else if (buffs[0] == "Barrier")
            {
                hasBarrier = false;
                buffs.RemoveAt(0);
            }
        }
    }

    public void RemoveDebuffs(int amount)
    {
        if (debuffs.Count == 0)    return;

        for (int i = 0; i < amount; i++)
        {
            if (debuffs[0] == "ArmorBreak")
            {
                hasArmorBreak = false;
                debuffs.RemoveAt(0);
            }
        }
    }

    public void RegenerateAP()
    {
        if (hasManaRush)
        {
            currentAP = currentAP + APregenartion + Mathf.RoundToInt(APregenartion * 0.25f);
        }
        else
        {
            currentAP += APregenartion;
        }
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
        float statusEffects = GetStatusEffectsMultiplier(target);

        // damage formular:
        float damage = (movePower * attack / defense + 3) * crit * random * statusEffects;
        if (target.isDefending)     damage = damage / 2;
        
        // mark if hit was critical, so that damage number becomes red
        if (crit > 1f)  target.criticalHit = true;

        return Mathf.RoundToInt(damage);
    }

    private float GetCritMultipier()
    {
        int critChange  = 10;
        float critMultiplier = 1.2f;
        
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

    private float GetStatusEffectsMultiplier(BattleUnit target)
    {
        float retVal = 1.0f;
        if (target.hasArmorBreak)   retVal += 0.2f;
        if (target.hasBarrier)      retVal -= 0.2f;
        if (hasMight)               retVal += 0.2f;
        return retVal;
    }
}