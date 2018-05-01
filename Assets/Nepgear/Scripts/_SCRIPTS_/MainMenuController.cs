using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public Text startText;
    public Image darkScreen;
    public RawImage logo;
    private MonoInputHandler ih;
    private bool isLoadingScene;
    public string lobbyScene;

    private float dimTime = 3f;

    private AudioSource source;

    // Use this for initialization
	void Start () {

        isLoadingScene = false;
        ih = GetComponent<MonoInputHandler>();
        startText.enabled = false;
        source = GetComponent<AudioSource>();
        StartCoroutine(LogoFadeIn());
    }
	
	// Update is called once per frame
	void Update () {
        if(ih.any && !isLoadingScene)
        {
            isLoadingScene = true;
            source.Play();
            StartCoroutine(ChangeScene());
        }
        if(isLoadingScene)
        {
            Debug.Log("Dimming Light");
            darkScreen.color = new Color(darkScreen.color.r, darkScreen.color.g, darkScreen.color.b, darkScreen.color.a + Time.deltaTime / dimTime);
        }
	}

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(dimTime);
        SceneManager.LoadScene(lobbyScene);
    }
    IEnumerator LogoFadeIn()
    {
        while (logo.color.a < 1f)
        {
            logo.color = new Color(logo.color.r, logo.color.g, logo.color.b, logo.color.a + Time.deltaTime / dimTime);
            yield return null;
        }
        startText.enabled = true;
    }

}
