using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderUI : MonoBehaviour
{
    private List<GameObject> icons = new List<GameObject>();
    private Vector3 SCALE_CHANGE = new Vector3(0.2f, 0.2f, 0f);
    public float scaleChangeX;
    public float scaleChangeY;

    void Start()
    {
        if (scaleChangeX != 0 && scaleChangeY != 0)
        {
            SCALE_CHANGE.x = scaleChangeX;
            SCALE_CHANGE.y = scaleChangeY;
        }
    }

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
            else    // set icon color of alive unit to default
            {
                icons[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }
    }
}