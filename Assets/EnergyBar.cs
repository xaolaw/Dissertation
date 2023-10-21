using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    }

    public void SetEnergy(int energy)
    {
        slider.value = energy;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
