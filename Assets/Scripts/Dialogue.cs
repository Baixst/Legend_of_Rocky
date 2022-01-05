using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string charName;
    public Color32 charNameFontColor;

    [TextArea(3, 10)]
    public string[] sentences;
}
