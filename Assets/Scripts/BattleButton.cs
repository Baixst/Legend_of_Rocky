using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleButton : MonoBehaviour
{
    public int moveIndex;
    public BattleSystem battleSystem;
    //public Text buttonText;
    public TextMeshProUGUI buttonText;

    public void changeTextToActiveUnit()
    {
        Move move = battleSystem.getActiveUnit().moves[moveIndex];
        buttonText.SetText(move.moveName);
    }
}
