using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public BattleSystem battleSystem;

    public int chanceToDefendAtLowAp;
    public int considerDefendingAtApPercentage;
    private int unitAP;
    private List<BattleUnit> playerUnits = new List<BattleUnit>();
    private List<BattleUnit> enemyUnits = new List<BattleUnit>();
    private Move move1, move2, move3;
    private float movePoints_1, movePoints_2, movePoints_3;

    public Move ChooseMove(BattleUnit attackingUnit)
    {
        unitAP = attackingUnit.currentAP;
        GetUnitsMoves(attackingUnit);

        // first: check if unit can even attack
        if (unitAP < move1.apCost && unitAP < move2.apCost && unitAP < move3.apCost)
        {
            attackingUnit.isDefending = true;
            return attackingUnit.moves[0];  // move will not actually be used, function just needs to return something
        }

        // chance to just defend when units AP is under a threshold
        if (unitAP < attackingUnit.maxAP * (considerDefendingAtApPercentage / 100))
        {
            float randomDefenseValue = Random.Range(0f, 100f);
            if (chanceToDefendAtLowAp >= randomDefenseValue)
            {
                attackingUnit.isDefending = true;
                return attackingUnit.moves[0];  // move will not actually be used, function just needs to return something
            }
        }

        FindLivingPlayerUnits();
        FindLivingEnemyUnits();

        movePoints_1 = CalcPointsNormalMove(move1, attackingUnit);
        movePoints_2 = CalcPointsNormalMove(move2, attackingUnit);
        movePoints_3 = CalcPointsMightyMove(move3, attackingUnit);

        float totalMovePoints = movePoints_1 + movePoints_2 + movePoints_3;

        Debug.Log("Points 1: " + movePoints_1);
        Debug.Log("Points 2: " + movePoints_2);
        Debug.Log("Points 3: " + movePoints_3);
        Debug.Log("totalPoints: " + totalMovePoints);
        
        // percentage = (percent value / base value) * 100
        float chance_move1 = movePoints_1 / totalMovePoints * 100;
        float chance_move2 = movePoints_2 / totalMovePoints * 100 + chance_move1;
        float chance_move3 = movePoints_3 / totalMovePoints * 100 + chance_move1 + chance_move2;

        float randomMoveValue = Random.Range(0f, 100f);

        Debug.Log("Chance 1: " + chance_move1);
        Debug.Log("Chance 2: " + chance_move2);
        Debug.Log("Chance 3: " + chance_move3);
        Debug.Log("RandomMoveValue: " + randomMoveValue);

        if (chance_move1 >= randomMoveValue)
        {
            return move1;
        }
        else if (chance_move2 >= randomMoveValue)
        {
            return move2;
        }
        else
        {
            return move3;
        }
    }

    private void GetUnitsMoves(BattleUnit unit)
    {
        move1 = unit.moves[0];
        move2 = unit.moves[1];
        move3 = unit.moves[2];
        movePoints_1 = 0f;
        movePoints_2 = 0f;
        movePoints_3 = 0f;
    }

    public List<BattleUnit> ChooseTargets(Move move, BattleUnit attackingUnit)
    {
        List<BattleUnit> possibleTargets = FindPossibleTargets(move, attackingUnit);
        if (possibleTargets.Count == 1)     return possibleTargets;
        if (move.numberOfTargets > 1)       return possibleTargets;

        List<float> targetScores = new List<float>();
        foreach (BattleUnit unit in possibleTargets)
        {
            targetScores.Add(0f);
        }

        // Calculate targetScores
        for (int i = 0; i < possibleTargets.Count; i++)
        {
            // when target = enemy:
            if (move.targetTyp == Move.TargetTyp.Enemy)
            {
                //  when move applies debuff:
                if (move.debuffsToApply.Count > 0)
                {
                    // when target has no debuff : add points to targetScore
                    if (possibleTargets[i].debuffs.Count == 0)
                    {
                        targetScores[i] += 20f;
                    }
                }
                // when move removes buff:
                if (move.removeBuffs > 0)
                {
                    // when target has buffs: add points to targetScore
                    if (possibleTargets[i].buffs.Count > 0)
                    {
                        targetScores[i] += 20f;
                    }
                }
                // when move deals damage: add points
                if (move.damage > 0)
                {
                    targetScores[i] += 5f;
                }
            }

            // when target = ally:
            if (move.targetTyp == Move.TargetTyp.Ally)
            {
                // when move heals:
                if (move.healing > 0)
                {
                    // when target is hurt: add points
                    if (possibleTargets[i].currentHP < possibleTargets[i].maxHP)
                    {
                        targetScores[i] += 10f;
                    }
                }

                // when move removes debuff:
                if (move.removeDebuffs > 0)
                {
                    // when target has a debuff: add points
                    if (possibleTargets[i].debuffs.Count > 0)
                    {
                        targetScores[i] += 10f;
                    }
                }

                // when move applies a buff:
                if (move.buffs.Count > 0)
                {
                    // when target has no buffs: add points
                    if (possibleTargets[i].buffs.Count == 0)
                    {
                        targetScores[i] += 10f;
                    }
                }
            }
        }

        // randomly select target weighted by the targetScores
        float totalTargetScore = 0f;
        foreach (float value in targetScores)
        {
            totalTargetScore += value;
        }

        List<BattleUnit> returnList = new List<BattleUnit>();

        // possibleTargets can only contain 2 or 3 units
        if (possibleTargets.Count == 2)
        {
            float chance_target1 = targetScores[0] / totalTargetScore * 100;
            float chance_target2 = targetScores[1] / totalTargetScore * 100 + chance_target1;

            float randomMoveValue = Random.Range(0f, 100f);
            if (chance_target1 >= randomMoveValue)
            {
                returnList.Add(possibleTargets[0]);
            }
            else
            {
                returnList.Add(possibleTargets[1]);
            }
        }
        else
        {
            float chance_target1 = targetScores[0] / totalTargetScore * 100;
            float chance_target2 = targetScores[1] / totalTargetScore * 100 + chance_target1;
            float chance_target3 = targetScores[2] / totalTargetScore * 100 + chance_target1 + chance_target2;

            float randomMoveValue = Random.Range(0f, 100f);
            if (chance_target1 >= randomMoveValue)
            {
                returnList.Add(possibleTargets[0]);
            }
            else if (chance_target2 >= randomMoveValue)
            {
                returnList.Add(possibleTargets[1]);
            }
            else
            {
                returnList.Add(possibleTargets[2]);
            }
        }

        return returnList;
    }

    private void FindLivingPlayerUnits()
    {
        playerUnits.Clear();
        foreach (BattleUnit unit in battleSystem.turnOrder)
        {
            if (unit.currentHP > 0 && unit.playerCharacter)
            {
                playerUnits.Add(unit);
            }
        }
    }

    private void FindLivingEnemyUnits()
    {
        enemyUnits.Clear();
        foreach (BattleUnit unit in battleSystem.turnOrder)
        {
            if (unit.currentHP > 0 && !unit.playerCharacter)
            {
                enemyUnits.Add(unit);
            }
        }
    }

    private float CalcPointsNormalMove(Move move, BattleUnit unit)
    {
        if (unitAP < move.apCost)  return 0;
        
        float sum = 0f;

    // Add points to move when:

        // move deals damage
        if (move.damage > 0)    sum += 10;

        // move applies debuff and there is a player that has no debuff
        if (move.debuffsToApply.Count > 0)
        {
            if (AnyPlayerHasNoDebuff())
            {
                sum += 10f;
            }
        }

        // move removes a buff and there is a player that has a buff
        if (move.removeBuffs > 0)
        {
            if (AnyPlayerHasBuff())
            {
                sum += 10f;
            }
        }

        // move can heal an enemy and at least one enemy is hurt
        if (move.healing > 0 && AnyEnemyIsHurt() && move.targetTyp == Move.TargetTyp.Ally)
        {
            sum += 10f;
        }

        // move heals the user and user is hurt
        if (move.healing > 0 && unit.currentHP < unit.maxHP && move.targetTyp == Move.TargetTyp.Self)
        {
            sum += 10f;
        }

        // move buffs the user and user doesn't have the buff
        if (move.buffsToApply.Count > 0 && move.targetTyp == Move.TargetTyp.Self)
        {
            if (UnitHastAtLeastOneBuffNot(unit, move))
            {
                sum += 5f;
            }
        }

        return sum;
    }

    private float CalcPointsMightyMove(Move move, BattleUnit unit)
    {
        // unit can only use mighty move when it is low on HP
        if (unit.currentHP > unit.maxHP / 2)    return 0f;
        if (unitAP < move.apCost)  return 0f;

        return 25f;
    }

    private bool UnitHastAtLeastOneBuffNot(BattleUnit unit, Move move)
    {
        foreach (string buff in move.buffs)
        {
            if (!UnitHasBuff(unit, buff))
            {
                return true;
            }
        }
        return false;
    }

    private bool UnitHasBuff(BattleUnit unit, string buff)
    {
        foreach (string unitBuff in unit.buffs)
        {
            if (unitBuff.Equals(buff))
            {
                return true;
            }
        }
        return false;
    }

    private bool AnyPlayerHasNoDebuff()
    {
        foreach (BattleUnit unit in playerUnits)
        {
            if (unit.debuffs.Count == 0)     return true;
        }
        return false;
    }

    private bool AnyPlayerHasBuff()
    {
        foreach (BattleUnit unit in playerUnits)
        {
            if (unit.buffs.Count > 0)     return true;
        }
        return false;
    }

    private bool AnyEnemyIsHurt()
    {
        foreach (BattleUnit unit in enemyUnits)
        {
            if (unit.currentHP < unit.maxHP)     return true;
        }
        return false;
    }

    private List<BattleUnit> FindPossibleTargets(Move move, BattleUnit attackingUnit)
    {
        List<BattleUnit> targets = new List<BattleUnit>();

        if (move.targetTyp == Move.TargetTyp.Self)
        {
            targets.Add(attackingUnit);
            return targets;
        }

        else if (move.targetTyp == Move.TargetTyp.Ally)
        {
            foreach (BattleUnit unit in enemyUnits)
            {
                targets.Add(unit);
            }
            return targets;
        }
        else
        {
            foreach (BattleUnit unit in playerUnits)
            {
                targets.Add(unit);
            }
            return targets;
        }
    }
}