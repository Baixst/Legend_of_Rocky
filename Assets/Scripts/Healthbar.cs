using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Healthbar : MonoBehaviour
{
    public Slider slider;
    public Image fill;
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        slider = gameObject.GetComponent<Slider>();
        textMesh = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetMaxValue(int value)
    {
        slider.maxValue = value;
    }

    public void SetValue(int value)
    {
        slider.value = value;
        fill.fillAmount = slider.normalizedValue;
        if (textMesh != null)
        {
            textMesh.text = value + " / " + slider.maxValue;
        }
    }
}
