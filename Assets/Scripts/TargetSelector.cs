using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TargetSelector : MonoBehaviour
{

    public BattleSystem battleSystem;
    [HideInInspector] public BattleUnit selectedUnit;
    [HideInInspector] public List<BattleUnit> possibleTargets = new List<BattleUnit>();
    private int possibleTargetsIndex = 0;

    [HideInInspector] public bool canceled;

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

    // Inputs:
    public void MoveOneUp(InputAction.CallbackContext context)
    {
        if (context.started)
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
    }

    public void MoveOneDown(InputAction.CallbackContext context)
    {
        if (context.started)
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
    }

    public void Submit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            selectedUnit = possibleTargets[possibleTargetsIndex];
            possibleTargetsIndex = 0;
        }
    }

    public void GoBack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            canceled = true;
        }
    }
}