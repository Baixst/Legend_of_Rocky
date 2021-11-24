using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderUI : MonoBehaviour
{
    public BattleUnit playerUnit1;
    // private Image unitIcon;

    private void Start()
    {
        // unitIcon = playerUnit1.icon;
        GameObject temp = Instantiate(playerUnit1.icon);
        temp.transform.position = new Vector3(0, 4, 0);
        temp.transform.SetParent(gameObject.transform, false);
        // temp.transform.parent = gameObject.transform;
    }

}