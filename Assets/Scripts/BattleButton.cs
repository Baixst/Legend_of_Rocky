using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleButton : MonoBehaviour
{
    public int moveIndex;
    public BattleSystem battleSystem;
    public TextMeshProUGUI buttonText;

    public void ChangeTextToActiveUnit()
    {
        Move move = battleSystem.GetActiveUnit().moves[moveIndex];
        buttonText.SetText(move.moveName + " | " + (move.apCost).ToString() + " AP");
    }
}
