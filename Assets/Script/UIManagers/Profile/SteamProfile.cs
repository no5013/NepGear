using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class SteamProfile : MonoBehaviour
{

    public Text displayName;
    public Text killCount;
    public Image avatar;

    [Header("Player Stats")]
    public string[] statStrings;

    private int killC;

    // Use this for initialization
    void Start()
    {
        if (!SteamManager.Initialized)
        {
            return;
        }

        displayName.text = SteamFriends.GetPersonaName();

        //Set value of stat
        SteamUserStats.SetStat("star5", 5);
        SteamUserStats.StoreStats();


        killC = 0;
        foreach (string s in statStrings)
        {
            int outData;
            SteamUserStats.GetStat("star5", out outData);
            killC += outData;
        }

        killCount.text = "KillCount: " + killC;

        //Set avatar
        StartCoroutine(FetchAvatar());
    }

    int avatarInt;
    uint width, height;
    Texture2D downloadedAvatar;
    Rect rect = new Rect(0, 0, 184, 184);
    Vector2 pivot = new Vector2(0.5f, 0.5f);

    private IEnumerator FetchAvatar()
    {
        avatarInt = SteamFriends.GetLargeFriendAvatar(SteamUser.GetSteamID());
        while (avatarInt == -1)
        {
            yield return null;
        }

        if (avatarInt > 0)
        {
            SteamUtils.GetImageSize(avatarInt, out width, out height);
            if (width > 0 && height > 0)
            {
                byte[] avatarStream = new byte[4 * (int)width * (int)height];

                SteamUtils.GetImageRGBA(avatarInt, avatarStream, 4 * (int)width * (int)height);

                downloadedAvatar = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
                downloadedAvatar.LoadRawTextureData(avatarStream);
                downloadedAvatar.Apply();

                avatar.sprite = Sprite.Create(downloadedAvatar, rect, pivot);
            }
        }
    }
}