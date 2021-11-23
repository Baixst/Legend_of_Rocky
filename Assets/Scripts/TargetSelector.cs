using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelector : MonoBehaviour
{

    public BattleSystem battleSystem;
    public BattleUnit selectedUnit;
    public List<BattleUnit> possibleTargets = new List<BattleUnit>();
    private int possibleTargetsIndex = 0;

    public void findPossibleTargets(Move move)
    {
        Debug.Log("finding possible targets...");
        possibleTargets.Clear();

        if (move.targetTyp == Move.TargetTyp.Ally)
        {
            Debug.Log("target index: " + possibleTargetsIndex);
            
            possibleTargets = battleSystem.GetPlayerUnits();
        }
        else if (move.targetTyp == Move.TargetTyp.Enemy)
        {
            Debug.Log("target index: " + possibleTargetsIndex);

            List<BattleUnit> temp = battleSystem.GetEnemyUnits();       // only add units to list when they are not dead yet
            foreach (BattleUnit unit in temp)
            {
                if (unit.currentHP > 0)
                {
                    possibleTargets.Add(unit);
                }
            }
        }
        else if (move.targetTyp == Move.TargetTyp.Self)
        {
            possibleTargets.Add(battleSystem.GetActiveUnit());
        }
        else
        {
            Debug.Log("could not find possible targets");
        }
    }

    void Update()
    {
        // check for button inputs
        if (Input.GetButtonUp("TargetorUp"))
        {
            if (possibleTargetsIndex == 0)
            {
                possibleTargetsIndex = possibleTargets.Count - 1;
            }
            else
            {
                possibleTargetsIndex--;
            }
            var tmp = possibleTargets[possibleTargetsIndex].transform.position;
            tmp.y += 0.5f;
            transform.position = tmp;
        }

        if (Input.GetButtonUp("TargetorDown"))
        {
            if (possibleTargetsIndex + 1 == possibleTargets.Count)
            {
                possibleTargetsIndex = 0;
            }
            else
            {
                possibleTargetsIndex++;
            }
            var tmp = possibleTargets[possibleTargetsIndex].transform.position;
            tmp.y += 0.5f;
            transform.position = tmp;
        }

        if (Input.GetButtonUp("Submit"))
        {
            selectedUnit = possibleTargets[possibleTargetsIndex];
            possibleTargetsIndex = 0;
        }
    }
}