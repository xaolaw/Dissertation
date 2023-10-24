using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnergyBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetStartEnergy(int startEnergy, int maxEnergy)
    {
        slider.maxValue = maxEnergy;
        slider.value = Mathf.Min(slider.maxValue, startEnergy);

        fill.color = gradient.Evaluate(1f);

        TextMeshProUGUI mText = this.GetComponentInChildren<TextMeshProUGUI>();
        mText.text = startEnergy.ToString();
    }

    public void SetEnergy(int energy)
    {
        slider.value = energy;

        fill.color = gradient.Evaluate(slider.normalizedValue);

        TextMeshProUGUI mText = this.GetComponentInChildren<TextMeshProUGUI>();
        mText.text = energy.ToString();
    }
}
