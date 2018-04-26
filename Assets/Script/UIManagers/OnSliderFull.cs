using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.

public class OnSliderFull : MonoBehaviour
{
    private Slider mainSlider;
    private bool max;
    private bool min;

    [SerializeField] ToggleEvent onToggleMax;
    [SerializeField] ToggleEvent onToggleBetween;
    [SerializeField] ToggleEvent onToggleMin;

    public void Start()
    {
        mainSlider = GetComponent<Slider>();
        ValueChangeCheck();

        //Adds a listener to the main slider and invokes a method when the value changes.
        mainSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    // Invoked when the value of the slider changes.
    public void ValueChangeCheck()
    {
        Debug.Log("FUCK");

        float value = mainSlider.value;

        if (value >= mainSlider.maxValue)
        {
            OnValueMax();
        }
        else if (value <= mainSlider.minValue)
        {
            OnValueMin();
        }
        else
        {
            OnValueChange();
        }
    }

    public void OnValueMax()
    {
        onToggleMax.Invoke(true);

        max = true;
        min = false;
    }

    public void OnValueChange()
    {
        onToggleMax.Invoke(false);

        max = false;
        min = false;
    }

    public void OnValueMin()
    {
        onToggleMax.Invoke(false);

        max = false;
        min = true;
    }
}