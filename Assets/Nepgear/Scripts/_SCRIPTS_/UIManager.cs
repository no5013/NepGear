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

    public Image staggerImage;
    public Text staggerText;

    public DamageIndicatorBehavior frontIndicator;
    public DamageIndicatorBehavior backIndicator;
    public DamageIndicatorBehavior leftIndicator;
    public DamageIndicatorBehavior rightIndicator;

    public Text gameStateText;

    private float fadeDelay = 3f;

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
        healthText.text = Mathf.Floor(percent*100) + "%";
        healthImage.fillAmount = percent;
    }

    public void SetStamina(float percent)
    {
        staminaText.text = Mathf.Floor(percent*100) + "%";
        staminaImage.fillAmount = percent;
    }

    public void SetStateText(string text)
    {
        if(gameStateText != null)
        {
            gameStateText.text = text;
        }
    }

    public void FadeStateText()
    {
        StartCoroutine(EFadeStateText());
    }

    private IEnumerator EFadeStateText()
    {
        float elapsedTime = 0.0f;
        float wait = fadeDelay - 0.5f;

        yield return null;

        while (elapsedTime < wait)
        {
            Color stateTextColor = gameStateText.color;
            stateTextColor.a = 1.0f - (elapsedTime / wait);
            elapsedTime += Time.deltaTime;

            gameStateText.color = stateTextColor;

            Debug.Log(gameStateText.color.a);

            yield return null;
        }
    }

    public void SetStagger(float percent)
    {
        staggerText.text = (percent * 100) + "%";
        staggerImage.fillAmount = percent;
    }

}
