using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    [SerializeField]
    Text playerHealth;

    [SerializeField]
    Text playerAmmo;

    [SerializeField]
    Text WinLevel;

    [SerializeField]
    Image playerTarget;

    [SerializeField]
    Image overLay;

    Color targetColor = Color.clear;
    bool isPlaying = true;

    public static Vector2 targetScale = Vector2.one * 100;

    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
        playerTarget.enabled = PlayerManager.IsArmed() ? true : false;

        playerTarget.rectTransform.sizeDelta = Vector2.Lerp(playerTarget.rectTransform.sizeDelta, targetScale, Time.deltaTime * 10f);
        targetScale = Vector2.Lerp(targetScale, Vector2.one * 100, Time.deltaTime * 5f);

        if(isPlaying) overLay.color = Color.Lerp(overLay.color, targetColor, Time.deltaTime * 10f);
    }

    void UpdateUI()
    {
        playerHealth.text = "Health - " + PlayerManager.PlayerHealth().ToString();
        if(PlayerManager.IsArmed())
        {
            playerAmmo.text = PlayerManager.GetCurrentAmmo(PlayerManager.Element()) + " - Ammo";
        }
        else
        {
            playerAmmo.text = "UnArmed";
        }
    }

    public void TurnBlack()
    {
        overLay.color = Color.black;
    }

    public void Finish()
    {
        WinLevel.enabled = true;
        isPlaying = false;
        overLay.color = Color.black;
    }
}
