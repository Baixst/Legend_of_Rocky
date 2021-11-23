using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text nameText;
    public Text hpText;
    public Text apText;

    public void SetHUD(BattleUnit battleUnit)
    {
        nameText.text = battleUnit.unitName;
        hpText.text = "HP: " + battleUnit.currentHP + " / " + battleUnit.maxHP;
        apText.text = "AP: " + battleUnit.currentAP + " / " + battleUnit.maxAP;
    }

    public void SetHP(int hp, BattleUnit battleUnit)
    {
        hpText.text = "HP: " + hp + " / " + battleUnit.maxHP;
    }

    public void SetAP(int ap, BattleUnit battleUnit)
    {
        apText.text = "AP: " + ap + " / " + battleUnit.maxAP;
    }
}
