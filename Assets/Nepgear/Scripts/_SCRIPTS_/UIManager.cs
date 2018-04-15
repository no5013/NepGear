using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    //public GameObject healthConsoleUI;
    //public GameObject staminaConsoleUI;
    //public GameObject damageIndicatorUI;
 
    public Image healthImage;
    public Text healthText;

    public Image staminaImage;
    public Text staminaText;

    public DamageIndicatorBehavior frontIndicator;
    public DamageIndicatorBehavior backIndicator;
    public DamageIndicatorBehavior leftIndicator;
    public DamageIndicatorBehavior rightIndicator;

    public 
    // Use this for initialization
    void Start () {
        //healthText = healthConsoleUI.GetComponentInChildren<Text>();

        //staminaText = staminaConsoleUI.GetComponentInChildren<Text>();

        //frontIndicator = damageIndicatorUI.GetComponentsInChildren<DamageIndicatorBehavior>()[0];
        //backIndicator = damageIndicatorUI.GetComponentsInChildren<DamageIndicatorBehavior>()[1];
        //leftIndicator = damageIndicatorUI.GetComponentsInChildren<DamageIndicatorBehavior>()[2];
        //rightIndicator = damageIndicatorUI.GetComponentsInChildren<DamageIndicatorBehavior>()[3];

    }

    public void TickDamage(string dir)
    {
        if(dir.Equals("front"))
        {
            frontIndicator.Tick();
        }
        else if (dir.Equals("back"))
        {
            backIndicator.Tick();
        }
        else if (dir.Equals("left"))
        {
            leftIndicator.Tick();
        }
        else if (dir.Equals("right"))
        {
            rightIndicator.Tick();
        }
    }

    public void SetHealth(float percent)
    {
        healthText.text = (percent*100) + "%";
        healthImage.fillAmount = percent;
    }

    public void SetStamina(float percent)
    {
        staminaText.text = (percent*100) + "%";
        staminaImage.fillAmount = percent;
    }

}
