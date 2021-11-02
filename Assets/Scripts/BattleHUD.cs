using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text nameText;
    public Text hpText;

    public void SetHUD(BattleUnit battleUnit)
    {
        nameText.text = battleUnit.unitName;
        hpText.text = battleUnit.currentHP + " / " + battleUnit.maxHP;
    }

    public void SetHP(int hp, BattleUnit battleUnit)
    {
        hpText.text = hp + " / " + battleUnit.maxHP;
    }
}
