using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicatorBehavior : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private float alphaPercent;
    private float maxPercent = 100f;

	// Use this for initialization
	void Start () {
        alphaPercent = 0f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(255f, 0f, 0f, alphaPercent / maxPercent);
    }

    // Update is called once per frame
    void Update () {
        if(alphaPercent > 0f)
        {
            alphaPercent -= 1f;
            if (alphaPercent < 0f)
            {
                alphaPercent = 0f;
                spriteRenderer.color = new Color(255f, 0f, 0f, alphaPercent / maxPercent);
            }
            else
            {
                spriteRenderer.color = new Color(255f, 0f, 0f, alphaPercent / maxPercent);
            }

        }
	}

    public void Tick()
    {
        alphaPercent = maxPercent;
    }


}
