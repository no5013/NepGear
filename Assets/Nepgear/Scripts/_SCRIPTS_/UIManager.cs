using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public GameObject statusUI;
    public GameObject ammoUI;
    public Text gameResultText;
    private Image hitpointBar;
    private Image staminaBar;
    private Image ultimateBar;
    private Text hitpointText;
    private Text staminaText;
    private Text ultimateText;
    private Text bulletText;
    
    // Use this for initialization
	void Start () {
        hitpointText = statusUI.GetComponentsInChildren<Text>()[0];
        staminaText = statusUI.GetComponentsInChildren<Text>()[1];
        ultimateText = statusUI.GetComponentsInChildren<Text>()[2];
        hitpointBar = statusUI.GetComponentsInChildren<Image>()[0];
        staminaBar = statusUI.GetComponentsInChildren<Image>()[1];
        ultimateBar = statusUI.GetComponentsInChildren<Image>()[2];
        bulletText = ammoUI.GetComponentsInChildren<Text>()[0];
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStamina(float stamina, float percentage)
    {
        staminaText.text = stamina + "";
        staminaBar.fillAmount = percentage;
    }
    
    public void SetHitpoint(float health, float percentage)
    {
        hitpointText.text = health + "";
        hitpointBar.fillAmount = percentage;
    }

    public void SetUltimate(float ultimate, float percentage)
    {
        ultimateText.text = ultimate + "";
        ultimateBar.fillAmount = percentage;
    }

    public void SetBullet(int bulletLeft, int bulletMax)
    {
        bulletText.text = bulletLeft + "/" + bulletMax;
    }

    public void SetResult(string winner)
    {
        gameResultText.text = "The winner is " + winner;
    }

    public void ShowResult()
    {
        gameResultText.GetComponent<Text>().enabled = true;
    }

    public void HideResult()
    {
        gameResultText.GetComponent<Text>().enabled = false;
    }
}
