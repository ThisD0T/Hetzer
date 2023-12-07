using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class StandardSlider : MonoBehaviour
{
    Slider slider;
    AudioSource clickSound;

    public float soundThreshold;

    float lastValue = 0, valueChange = 0;

    void Start() {
        slider = GetComponent<Slider>();
        clickSound = GetComponent<AudioSource>();
    }

    public void ClickSound() {
        valueChange += Mathf.Abs(slider.value - lastValue);

        if (valueChange > soundThreshold) {
            valueChange = 0;
            clickSound.Play();
        }

        lastValue = slider.value;
    }

}
