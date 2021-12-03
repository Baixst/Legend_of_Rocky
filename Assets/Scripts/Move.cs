using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
   public string moveName;
   public string description;
   public int numberOfTargets;

   public enum TargetTyp { Enemy, Ally, Self };
   public TargetTyp targetTyp;
   public enum DamageTyp { Physical, Magical}
   public DamageTyp damageTyp;

   public enum Buff { Might, ManaRush, Barrier }
   public List<Buff> buffsToApply;

   public enum Debuff { ArmorBreak }
   public List<Debuff> debuffsToApply;

   public int damage;
   public int healing;
   public int apCost;          // how much AP does the move use up
   public int removeBuffs;     // how many buffs the moves removes from the target
   public int removeDebuffs;   // how many debuffs the moves removes from the target

   [HideInInspector] public List<string> buffs = new List<string>();
   [HideInInspector] public List<string> debuffs = new List<string>();

   public void WakeUp()
   {
      Debug.Log("Awake called from a move");
      foreach (Buff buff in buffsToApply)
      {
         if (buff == Buff.Might)       buffs.Add("Might");
         if (buff == Buff.ManaRush)    buffs.Add("ManaRush");
         if (buff == Buff.Barrier)     buffs.Add("Barrier");
      }

      foreach (Debuff debuff in debuffsToApply)
      {
         if (debuff == Debuff.ArmorBreak)   debuffs.Add("ArmorBreak");
      }
   }
}
