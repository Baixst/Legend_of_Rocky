using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public int level;
    public string unitName;
    public int maxHP;
    public int currentHP;
    public int physicalAttack;
    public int init;


    public void TakeDamage(int dmg)
    {
        currentHP = currentHP - dmg;
        if(currentHP < 0) {
            currentHP = 0;
        }
    }
}


