using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderUI : MonoBehaviour
{
    private List<GameObject> icons = new List<GameObject>();
    private readonly Vector3 SCALE_CHANGE = new Vector3(0.2f, 0.2f, 0f);

    public void AddUnit(BattleUnit unit)
    {
        GameObject temp = Instantiate(unit.icon);
        temp.transform.SetParent(gameObject.transform, false);
        icons.Add(temp);
    }

    public void HighlightUnit(int turnOrderIndex)
    {
        icons[turnOrderIndex].transform.localScale += SCALE_CHANGE;
    }

    public void UnhighlightUnit(int turnOrderIndex)
    {
        icons[turnOrderIndex].transform.localScale -= SCALE_CHANGE;
    }

    public void UpdateAfterMove(List<BattleUnit> units)
    {
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].currentHP == 0) // set icon color of dead unit to grey
            {
                icons[i].GetComponent<Image>().color = new Color32(60, 60, 60, 255);
            }
            else    // set incon color of alive unit to default
            {
                icons[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }
    }
}