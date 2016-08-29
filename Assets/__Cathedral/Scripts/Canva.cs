using UnityEngine;
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

    private void ActiveBoolean()
    {
        GameIsLaunched = true;
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
}
