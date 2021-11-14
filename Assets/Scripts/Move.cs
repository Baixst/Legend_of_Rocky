using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
   public string moveName;
   public string description;
   public int numberOfTargets;
   public string targetTyp;     // can be "enemy", "ally" or "self" 
   public int damage;
   public int healing;
   public string damageTyp;
   public bool canDebuff;
   public int debuffValue;      // how much a stat wil be reduced
   public string debuffStat;    // stat to be reduced, can be "attack", "defense" or "init"
   public bool canBuff;
   public int buffValue;        // how much a stat wil be increased
   public string buffStat;      // stat to be increased

   // Depending on system:

        // public int manaCost;
        // Public int cooldown;
}
