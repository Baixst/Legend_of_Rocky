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

        if (move.targetTyp == "ally")
        {
            possibleTargets = battleSystem.getPartyUnits();
        }
        else if (move.targetTyp == "enemy")
        {
            possibleTargets = battleSystem.getEnemyUnits();
        }
        else if (move.targetTyp == "self")
        {
            List<BattleUnit> list = new List<BattleUnit>();
            list.Add(battleSystem.getActiveUnit());
            possibleTargets = list;
            selectedUnit = possibleTargets[0];
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
            Debug.Log("set selectedUnit to a value");
        }
    }
}