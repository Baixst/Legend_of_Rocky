using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHandler : MonoBehaviour
{
    public BattleSystem battleSystem;
    public Sprite mightIcon;
    public Sprite manaRushIcon;
    public Sprite barrierIcon;
    public Sprite armorBreakIcon;
    public float REFRESH_TIME;

    private List<BattleUnit> units = new List<BattleUnit>();
    private List<BattleHUD> huds = new List<BattleHUD>();

    void Start()
    {
        foreach (BattleUnit unit in battleSystem.battleUnits)
        {
            units.Add(unit);
        }
        foreach (BattleHUD hud in battleSystem.huds)
        {
            huds.Add(hud);
        }

        StartCoroutine(showStatusEffects());
    }

    public void UpdateStatusEffects()
    {
        for (int i = 0; i < huds.Count; i++)
        {
            battleSystem.huds[i].statusIconList.Clear();

            // set icon to null when unit has no status effects
            if (units[i].buffs.Count + units[i].debuffs.Count == 0)
            {
                huds[i].statusEffectDisplay.sprite = null;
                continue;
            }

            foreach (string buff in units[i].buffs)
            {
                switch (buff)
                {
                    case "Might":
                        huds[i].statusIconList.Add(mightIcon);
                        break;
                    case "ManaRush":
                        huds[i].statusIconList.Add(manaRushIcon);
                        break;
                    case "Barrier":
                        huds[i].statusIconList.Add(barrierIcon);
                        break;
                    default:
                        Debug.Log("Unable to find buff: " + buff);
                        break;
                }
            }
            foreach (string debuff in units[i].debuffs)
            {
                switch (debuff)
                {
                    case "ArmorBreak":
                        huds[i].statusIconList.Add(armorBreakIcon);
                        break;
                    default:
                        Debug.Log("Unable to find buff: " + debuff);
                        break;
                }
            }
        }
    }

    IEnumerator showStatusEffects()
    {
        while(true)
        {
            foreach (BattleHUD hud in huds)
            {
                hud.statusEffectDisplay.sprite = hud.GetNextSprite();
            }
            yield return new WaitForSeconds(REFRESH_TIME);
        }
    }
}
