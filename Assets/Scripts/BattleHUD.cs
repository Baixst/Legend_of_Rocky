using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Healthbar hpBar;
    public Healthbar apBar;
    public SpriteRenderer statusEffectDisplay;
    [HideInInspector] public List<Sprite> statusIconList = new List<Sprite>();

    private int currentSprite = 0;

    public void SetHUD(BattleUnit battleUnit)
    {
        nameText.text = battleUnit.unitName;

        if (hpBar != null)
        {
            hpBar.SetMaxValue(battleUnit.maxHP);
            hpBar.SetValue(battleUnit.currentHP);
        }
        if (apBar != null)
        {
            apBar.SetMaxValue(battleUnit.maxAP);
            apBar.SetValue(battleUnit.currentAP);
        }
    }

    public void SetHP(int hp)
    {
        if (hpBar != null)
        {
            hpBar.SetValue(hp);
        }
    }

    public void SetAP(int ap)
    {
        if (apBar != null)
        {
            apBar.SetValue(ap);
        }
    }

    public Sprite GetNextSprite()
    {
        if (statusIconList.Count == 0)
        {
            return null;
        }
        if (currentSprite + 1 >= statusIconList.Count)
        {
            currentSprite = 0;
            return statusIconList[currentSprite];
        }
        else
        {
            currentSprite++;
            return statusIconList[currentSprite];
        }
    }
}
