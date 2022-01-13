using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text nameText;
    public Text hpText;
    public Slider hpBar;
    public Image hpBarFill;
    public Text apText;
    public SpriteRenderer statusEffectDisplay;
    [HideInInspector] public List<Sprite> statusIconList = new List<Sprite>();

    private int currentSprite = 0;

    public void SetHUD(BattleUnit battleUnit)
    {
        nameText.text = battleUnit.unitName;
        hpText.text = "HP: " + battleUnit.currentHP + " / " + battleUnit.maxHP;
        apText.text = "AP: " + battleUnit.currentAP + " / " + battleUnit.maxAP;

        if (hpBar != null)
        {
            hpBar.maxValue = battleUnit.maxHP;
            hpBar.value = battleUnit.currentHP;
        }
    }

    public void SetHP(int hp, BattleUnit battleUnit)
    {
        hpText.text = "HP: " + hp + " / " + battleUnit.maxHP;
        if (hpBar != null)
        {
            hpBar.value = battleUnit.currentHP;
            hpBarFill.fillAmount = hpBar.normalizedValue;
        }
    }

    public void SetAP(int ap, BattleUnit battleUnit)
    {
        apText.text = "AP: " + ap + " / " + battleUnit.maxAP;
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
