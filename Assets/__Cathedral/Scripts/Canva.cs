﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class Canva : MonoBehaviour {

    public static Canva Instance { get; private set; }
    public bool GameIsLaunched = false;

    void Awake()
    {
        Instance = this;
    }

    public void LaunchGame()
    {
        panelMenu.gameObject.SetActive(false);
        panelGame.gameObject.SetActive(true);
        Invoke("ActiveBoolean", 1.0f);
        Cathedral.Instance.Init();
    }

    private void Update()
    {
        if(GameIsLaunched)
        {
            currentTime += Time.deltaTime;
            timer.fillAmount = currentTime / Cathedral.Instance.MaxTime;
            if(currentTime> Cathedral.Instance.MaxTime)
            {
                Cathedral.Instance.SubmitToGod();
            }
        }
    }

    private void ActiveBoolean()
    {
        GameIsLaunched = true;
    }

    public void ResetTime()
    {
        currentTime = 0.0f;
    }

    public void displayInfo()
    {
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(true);
    }

    public void Reload()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Test_Ben");
    }

    [SerializeField]
    private RectTransform panelGame;
    [SerializeField]
    private RectTransform panelMenu;
    [SerializeField]
    private Image timer;

    private float currentTime = 0.0f;
}
