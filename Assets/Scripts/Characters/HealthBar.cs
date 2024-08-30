using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;  
    public Gradient gradient;
    public Image fill;

    // Method to set the maximum health value of the health bar
    public void SetMaxHealth(int _health)
    {
        slider.maxValue = _health;  // Set the maximum value of the slider.
        slider.value = _health;  // Set the current value of the slider to maximum health.

        // fill.color = gradient.Evaluate(1.0f);
    }

    // Method to update the health value of the health bar
    public void SetHealth(int _health)
    {
        slider.value = _health;  // Update the current value of the slider to the specified health value.

        // Update the fill color based on the current health percentage.
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
