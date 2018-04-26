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

    public Text leftWeaponText;
    public Text rightWeaponText;

    //Stock ui
    public Image teamStockImage;
    public Text teamStockText;
    public Image enemyStockImage;
    public Text enemyStockText;

    //time ui
    public Text remainingTimeText;

    //Ultimate Slider
    public Slider ultimateSlider;

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
        healthText.text = Mathf.Clamp(Mathf.Floor(percent * 100), 0f, 100f) + "%";
        healthImage.fillAmount = Mathf.Clamp(percent, 0f, 1f);
    }

    public void SetStamina(float percent)
    {
        staminaText.text = Mathf.Clamp(Mathf.Floor(percent*100), 0f, 100f) + "%";
        staminaImage.fillAmount = Mathf.Clamp(percent, 0f, 1f);
    }

    public void SetUltimate(float percent)
    {
        ultimateSlider.value = Mathf.Clamp(Mathf.Floor(percent), 0f, 1f);
    }

    public void SetStateText(string text)
    {
        if(gameStateText != null)
        {
            Color stateTextColor = gameStateText.color;
            stateTextColor.a = 1f;
            gameStateText.text = text;
            gameStateText.color = stateTextColor;
        }
    }

    public void SetLeftWeaponText(string text)
    {
        if(leftWeaponText != null)
        {
            leftWeaponText.text = text;
        }
    }

    public void SetRightWeaponText(string text)
    {
        if (rightWeaponText != null)
        {
            rightWeaponText.text = text;
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

            yield return null;
        }
    }

    public void SetStagger(float percent)
    {
        if(staggerText == null)
        {
            return;
        }
        staggerText.text = (percent * 100) + "%";
        staggerImage.fillAmount = percent;
    }

    public void SetStocks(float teamStock, float enemyStock, float maxStock)
    {
        teamStockText.text = teamStock.ToString();
        teamStockImage.fillAmount = (teamStock/maxStock);

        enemyStockText.text = enemyStock.ToString();
        enemyStockImage.fillAmount = (enemyStock / maxStock);
    }

    public void SetTime(float sec, float dec)
    {
        remainingTimeText.text = sec + "\"" + dec;
    }
}
