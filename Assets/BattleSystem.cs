using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    
    public enum BattleState { START, COMBAT, WON, LOST }

    public BattleState state;

    void Start()
    {
        
    }
}
